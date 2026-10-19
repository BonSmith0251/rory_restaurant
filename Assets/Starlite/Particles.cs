// Port of Starling's PDParticleSystem (Particle Designer .pex) rendered with pooled UI quads.
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using URandom = System.Random;

namespace Starlite
{
    public class ColorArgb
    {
        public float red, green, blue, alpha;
        public ColorArgb(float r = 0, float g = 0, float b = 0, float a = 0) { red = r; green = g; blue = b; alpha = a; }
        public void fromRgb(uint rgb) { red = ((rgb >> 16) & 0xff) / 255f; green = ((rgb >> 8) & 0xff) / 255f; blue = (rgb & 0xff) / 255f; }
        public void fromArgb(uint argb) { alpha = ((argb >> 24) & 0xff) / 255f; fromRgb(argb); }
        public uint toRgb() => ((uint)(Mathf.Clamp01(red) * 255) << 16) | ((uint)(Mathf.Clamp01(green) * 255) << 8) | (uint)(Mathf.Clamp01(blue) * 255);
    }

    public static class Context3DBlendFactor
    {
        public const string ONE = "one", ZERO = "zero", SOURCE_ALPHA = "sourceAlpha", ONE_MINUS_SOURCE_ALPHA = "oneMinusSourceAlpha",
            DESTINATION_ALPHA = "destinationAlpha", SOURCE_COLOR = "sourceColor";
    }

    public class PDParticleSystem : DisplayObject, IAnimatable
    {
        class P
        {
            public float x, y, scale = 1, rotation, alpha = 1, currentTime, totalTime;
            public float startX, startY, velocityX, velocityY, radialAcceleration, tangentialAcceleration;
            public float emitRadius, emitRadiusDelta, emitRotation, emitRotationDelta, rotationDelta, scaleDelta;
            public ColorArgb color = new ColorArgb(), colorDelta = new ColorArgb();
            public RawImage img;
        }

        static readonly URandom Rng = new URandom();
        static float Rand() => (float)Rng.NextDouble();

        readonly Texture mTexture;
        readonly List<P> mParticles = new List<P>();
        int mNumParticles;
        float mFrameTime, mEmissionTime, mEmissionRate;

        // config (public like Starling for runtime tweaks)
        public int emitterType;
        public float emitterXVariance, emitterYVariance, defaultDuration;
        public int maxNumParticles;
        public float lifespan, lifespanVariance, startSize, startSizeVariance, endSize, endSizeVariance;
        public float emitAngle, emitAngleVariance, startRotation, startRotationVariance, endRotation, endRotationVariance;
        public float speed, speedVariance, gravityX, gravityY, radialAcceleration, radialAccelerationVariance,
            tangentialAcceleration, tangentialAccelerationVariance;
        public float maxRadius, maxRadiusVariance, minRadius, minRadiusVariance, rotatePerSecond, rotatePerSecondVariance;
        public ColorArgb startColor = new ColorArgb(), startColorVariance = new ColorArgb(), endColor = new ColorArgb(), endColorVariance = new ColorArgb();
        public float emitterX, emitterY;
        public string blendFactorSource = Context3DBlendFactor.ONE, blendFactorDestination = Context3DBlendFactor.ONE_MINUS_SOURCE_ALPHA;
        bool mAdditive;

        public PDParticleSystem(XML config, Texture texture)
        {
            mTexture = texture;
            Parse(config);
            mEmissionRate = maxNumParticles / Math.Max(0.01f, lifespan);
            mEmissionTime = 0;
            touchable = false;
        }

        static float F(XML x, string node, string attr = "value") => AS3.toNumber(x[node][0]?.attribute(attr));
        static ColorArgb C(XML x, string node) => new ColorArgb(F(x, node, "red"), F(x, node, "green"), F(x, node, "blue"), F(x, node, "alpha"));

        void Parse(XML x)
        {
            emitterXVariance = F(x, "sourcePositionVariance", "x");
            emitterYVariance = F(x, "sourcePositionVariance", "y");
            gravityX = F(x, "gravity", "x");
            gravityY = F(x, "gravity", "y");
            emitterType = (int)F(x, "emitterType");
            maxNumParticles = (int)F(x, "maxParticles");
            lifespan = Math.Max(0.01f, F(x, "particleLifeSpan"));
            lifespanVariance = F(x, "particleLifespanVariance");
            startSize = F(x, "startParticleSize");
            startSizeVariance = F(x, "startParticleSizeVariance");
            endSize = F(x, "finishParticleSize");
            endSizeVariance = F(x, "FinishParticleSizeVariance");
            emitAngle = F(x, "angle") * Mathf.Deg2Rad;
            emitAngleVariance = F(x, "angleVariance") * Mathf.Deg2Rad;
            startRotation = F(x, "rotationStart") * Mathf.Deg2Rad;
            startRotationVariance = F(x, "rotationStartVariance") * Mathf.Deg2Rad;
            endRotation = F(x, "rotationEnd") * Mathf.Deg2Rad;
            endRotationVariance = F(x, "rotationEndVariance") * Mathf.Deg2Rad;
            speed = F(x, "speed");
            speedVariance = F(x, "speedVariance");
            radialAcceleration = F(x, "radialAcceleration");
            radialAccelerationVariance = F(x, "radialAccelVariance");
            tangentialAcceleration = F(x, "tangentialAcceleration");
            tangentialAccelerationVariance = F(x, "tangentialAccelVariance");
            maxRadius = F(x, "maxRadius");
            maxRadiusVariance = F(x, "maxRadiusVariance");
            minRadius = F(x, "minRadius");
            rotatePerSecond = F(x, "rotatePerSecond") * Mathf.Deg2Rad;
            rotatePerSecondVariance = F(x, "rotatePerSecondVariance") * Mathf.Deg2Rad;
            startColor = C(x, "startColor");
            startColorVariance = C(x, "startColorVariance");
            endColor = C(x, "finishColor");
            endColorVariance = C(x, "finishColorVariance");
            defaultDuration = F(x, "duration");
            if (defaultDuration <= 0) defaultDuration = float.PositiveInfinity;
            mAdditive = (int)F(x, "blendFuncDestination") == 1;
        }

        public int numParticles => mNumParticles;
        public int capacity => maxNumParticles;
        public bool isEmitting => mEmissionTime > 0 && mEmissionRate > 0;
        public float emissionRate { get => mEmissionRate; set => mEmissionRate = value; }
        public Texture texture => mTexture;

        public void start(float duration = float.PositiveInfinity)
        {
            if (mEmissionRate != 0) mEmissionTime = duration;
            if (!Starling.juggler.contains(this)) Starling.juggler.add(this);
        }

        public void stop(bool clearParticles = false)
        {
            mEmissionTime = 0;
            if (clearParticles) { mNumParticles = 0; Render(); }
        }

        public void pause() => mEmissionTime = 0;
        public void clear() => stop(true);

        P NewParticle()
        {
            var p = new P();
            var g = new GameObject("p", typeof(RectTransform));
            var r = (RectTransform)g.transform;
            r.SetParent(inner, false);
            r.anchorMin = r.anchorMax = new Vector2(0, 1);
            r.pivot = new Vector2(0.5f, 0.5f);
            p.img = g.AddComponent<RawImage>();
            p.img.raycastTarget = false;
            p.img.texture = mTexture.Native;
            p.img.uvRect = mTexture.UvRect;
            if (mAdditive || blendFactorDestination == Context3DBlendFactor.ONE) p.img.material = Starling.AdditiveMaterial;
            return p;
        }

        void InitParticle(P p)
        {
            float lifespanV = lifespan + lifespanVariance * (Rand() * 2 - 1);
            p.currentTime = 0;
            p.totalTime = lifespanV > 0 ? lifespanV : 0;
            if (lifespanV <= 0) return;
            p.x = emitterX + emitterXVariance * (Rand() * 2 - 1);
            p.y = emitterY + emitterYVariance * (Rand() * 2 - 1);
            p.startX = emitterX;
            p.startY = emitterY;
            float angle = emitAngle + emitAngleVariance * (Rand() * 2 - 1);
            float spd = speed + speedVariance * (Rand() * 2 - 1);
            p.velocityX = spd * Mathf.Cos(angle);
            p.velocityY = spd * Mathf.Sin(angle);
            float startRadius = maxRadius + maxRadiusVariance * (Rand() * 2 - 1);
            float endRadius = minRadius + minRadiusVariance * (Rand() * 2 - 1);
            p.emitRadius = startRadius;
            p.emitRadiusDelta = (endRadius - startRadius) / lifespanV;
            p.emitRotation = emitAngle + emitAngleVariance * (Rand() * 2 - 1);
            p.emitRotationDelta = rotatePerSecond + rotatePerSecondVariance * (Rand() * 2 - 1);
            p.radialAcceleration = radialAcceleration + radialAccelerationVariance * (Rand() * 2 - 1);
            p.tangentialAcceleration = tangentialAcceleration + tangentialAccelerationVariance * (Rand() * 2 - 1);
            float startSz = Math.Max(0.1f, startSize + startSizeVariance * (Rand() * 2 - 1));
            float endSz = Math.Max(0.1f, endSize + endSizeVariance * (Rand() * 2 - 1));
            p.scale = startSz / mTexture.width;
            p.scaleDelta = (endSz - startSz) / lifespanV / mTexture.width;
            var sc = p.color; var dc = p.colorDelta;
            sc.red = startColor.red + startColorVariance.red * (Rand() * 2 - 1);
            sc.green = startColor.green + startColorVariance.green * (Rand() * 2 - 1);
            sc.blue = startColor.blue + startColorVariance.blue * (Rand() * 2 - 1);
            sc.alpha = startColor.alpha + startColorVariance.alpha * (Rand() * 2 - 1);
            float er = endColor.red + endColorVariance.red * (Rand() * 2 - 1);
            float eg = endColor.green + endColorVariance.green * (Rand() * 2 - 1);
            float eb = endColor.blue + endColorVariance.blue * (Rand() * 2 - 1);
            float ea = endColor.alpha + endColorVariance.alpha * (Rand() * 2 - 1);
            dc.red = (er - sc.red) / lifespanV; dc.green = (eg - sc.green) / lifespanV;
            dc.blue = (eb - sc.blue) / lifespanV; dc.alpha = (ea - sc.alpha) / lifespanV;
            float startRot = startRotation + startRotationVariance * (Rand() * 2 - 1);
            float endRot = endRotation + endRotationVariance * (Rand() * 2 - 1);
            p.rotation = startRot;
            p.rotationDelta = (endRot - startRot) / lifespanV;
        }

        void AdvanceParticle(P p, float passedTime)
        {
            float restTime = p.totalTime - p.currentTime;
            passedTime = restTime > passedTime ? passedTime : restTime;
            p.currentTime += passedTime;
            if (emitterType == 1) // radial
            {
                p.emitRotation += p.emitRotationDelta * passedTime;
                p.emitRadius += p.emitRadiusDelta * passedTime;
                p.x = emitterX - Mathf.Cos(p.emitRotation) * p.emitRadius;
                p.y = emitterY - Mathf.Sin(p.emitRotation) * p.emitRadius;
            }
            else
            {
                float distX = p.x - p.startX, distY = p.y - p.startY;
                float distScalar = Mathf.Sqrt(distX * distX + distY * distY);
                if (distScalar < 0.01f) distScalar = 0.01f;
                float radX = distX / distScalar, radY = distY / distScalar;
                float tanX = radX, tanY = radY;
                radX *= p.radialAcceleration; radY *= p.radialAcceleration;
                float newY = tanX;
                tanX = -tanY * p.tangentialAcceleration;
                tanY = newY * p.tangentialAcceleration;
                p.velocityX += passedTime * (gravityX + radX + tanX);
                p.velocityY += passedTime * (gravityY + radY + tanY);
                p.x += p.velocityX * passedTime;
                p.y += p.velocityY * passedTime;
            }
            p.scale += p.scaleDelta * passedTime;
            p.rotation += p.rotationDelta * passedTime;
            p.color.red += p.colorDelta.red * passedTime;
            p.color.green += p.colorDelta.green * passedTime;
            p.color.blue += p.colorDelta.blue * passedTime;
            p.color.alpha += p.colorDelta.alpha * passedTime;
        }

        public void advanceTime(float passedTime)
        {
            if (isDisposed) { Starling.juggler.remove(this); return; }
            int i = 0;
            while (i < mNumParticles)
            {
                var p = mParticles[i];
                if (p.currentTime < p.totalTime) { AdvanceParticle(p, passedTime); i++; }
                else
                {
                    if (i != mNumParticles - 1) { mParticles[i] = mParticles[mNumParticles - 1]; mParticles[mNumParticles - 1] = p; }
                    mNumParticles--;
                }
            }
            if (mEmissionTime > 0)
            {
                float timeBetween = 1f / mEmissionRate;
                mFrameTime += passedTime;
                while (mFrameTime > 0)
                {
                    if (mNumParticles < maxNumParticles)
                    {
                        if (mNumParticles == mParticles.Count) mParticles.Add(NewParticle());
                        var p = mParticles[mNumParticles++];
                        InitParticle(p);
                        if (p.totalTime > 0) AdvanceParticle(p, mFrameTime);
                    }
                    mFrameTime -= timeBetween;
                }
                if (!float.IsPositiveInfinity(mEmissionTime)) mEmissionTime = Math.Max(0, mEmissionTime - passedTime);
                if (mNumParticles == 0 && mEmissionTime == 0) dispatchEventWith(Event.COMPLETE);
            }
            Render();
        }

        void Render()
        {
            if (isDisposed) return;
            float tw = mTexture.width, th = mTexture.height;
            for (int i = 0; i < mParticles.Count; i++)
            {
                var p = mParticles[i];
                bool on = i < mNumParticles;
                if (p.img.enabled != on) p.img.enabled = on;
                if (!on) continue;
                var r = p.img.rectTransform;
                r.anchoredPosition = new Vector2(p.x, -p.y);
                r.sizeDelta = new Vector2(tw, th);
                r.localScale = new Vector3(p.scale, p.scale, 1);
                r.localEulerAngles = new Vector3(0, 0, -p.rotation * Mathf.Rad2Deg);
                p.img.color = new UnityEngine.Color(p.color.red, p.color.green, p.color.blue, Mathf.Clamp01(p.color.alpha));
            }
        }

        internal override Rectangle LocalBounds => new Rectangle(0, 0, 0, 0);

        public override void dispose()
        {
            Starling.juggler.remove(this);
            base.dispose();
        }
    }
}
