using System.Collections.Generic;
using Rory.Data;
using UnityEngine;
using UnityEngine.UI;

namespace Rory.UI
{
    /// <summary>
    /// Particle Designer emitter drawn as UI quads (the original PDParticleSystem), so particles sort and clip
    /// with the rest of the canvas. The emitter position is this RectTransform's pivot plus <see cref="emitterOffset"/>
    /// (original coordinates: y down).
    /// </summary>
    [RequireComponent(typeof(CanvasRenderer))]
    public class UIParticleEmitter : MaskableGraphic
    {
        public ParticleConfig config;
        [Tooltip("Material for additive configs (Rory/UI Additive). Found by shader name if empty.")]
        public Material additiveMaterial;
        public bool playOnEnable;
        [Tooltip("Emitter position relative to the pivot, y down")]
        public Vector2 emitterOffset;

        class P
        {
            public float x, y, startX, startY, vx, vy, currentTime, totalTime;
            public float emitRadius, emitRadiusDelta, emitRotation, emitRotationDelta;
            public float radialAcc, tangentialAcc, size, sizeDelta, rotation, rotationDelta;
            public Color color, colorDelta;
        }

        readonly List<P> particles = new List<P>();
        int count;
        float emissionTime, frameTime;
        static Material sAdditive;

        public bool IsEmitting => emissionTime > 0;
        public int ParticleCount => count;
        float EmissionRate => config == null ? 0 : config.maxParticles / Mathf.Max(0.01f, config.lifespan);

        public override Texture mainTexture => config != null && config.texture != null ? config.texture.texture : s_WhiteTexture;

        public override Material material
        {
            get
            {
                if (config == null || !config.additive) return base.material;
                if (additiveMaterial != null) return additiveMaterial;
                if (sAdditive == null)
                {
                    var sh = Shader.Find("Rory/UI Additive");
                    if (sh != null) sAdditive = new Material(sh);
                }
                return sAdditive != null ? sAdditive : base.material;
            }
            set => base.material = value;
        }

        protected override void Awake()
        {
            base.Awake();
            raycastTarget = false;
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            if (playOnEnable && Application.isPlaying) Play();
        }

        /// <summary>Starts emitting for <paramref name="duration"/> seconds (default: the config's duration, else forever).</summary>
        public void Play(float duration = -1)
        {
            if (config == null) return;
            if (duration < 0) duration = config.duration > 0 ? config.duration : float.PositiveInfinity;
            if (EmissionRate > 0) emissionTime = duration;
        }

        public void Stop(bool clear = false)
        {
            emissionTime = 0;
            if (clear)
            {
                count = 0;
                SetVerticesDirty();
            }
        }

        static float R() => Random.value * 2 - 1;

        void Init(P p)
        {
            var c = config;
            float life = c.lifespan + c.lifespanVariance * R();
            p.currentTime = 0;
            p.totalTime = Mathf.Max(0, life);
            if (life <= 0) return;
            float ex = emitterOffset.x, ey = emitterOffset.y;
            p.x = ex + c.sourcePositionVariance.x * R();
            p.y = ey + c.sourcePositionVariance.y * R();
            p.startX = ex;
            p.startY = ey;
            float angle = (c.angle + c.angleVariance * R()) * Mathf.Deg2Rad;
            float speed = c.speed + c.speedVariance * R();
            p.vx = speed * Mathf.Cos(angle);
            p.vy = speed * Mathf.Sin(angle);
            float startRadius = c.maxRadius + c.maxRadiusVariance * R();
            float endRadius = c.minRadius + c.minRadiusVariance * R();
            p.emitRadius = startRadius;
            p.emitRadiusDelta = (endRadius - startRadius) / life;
            p.emitRotation = (c.angle + c.angleVariance * R()) * Mathf.Deg2Rad;
            p.emitRotationDelta = (c.rotatePerSecond + c.rotatePerSecondVariance * R()) * Mathf.Deg2Rad;
            p.radialAcc = c.radialAcceleration + c.radialAccelerationVariance * R();
            p.tangentialAcc = c.tangentialAcceleration + c.tangentialAccelerationVariance * R();
            float s0 = Mathf.Max(0.1f, c.startSize + c.startSizeVariance * R());
            float s1 = Mathf.Max(0.1f, c.endSize + c.endSizeVariance * R());
            p.size = s0;
            p.sizeDelta = (s1 - s0) / life;
            var c0 = new Color(c.startColor.r + c.startColorVariance.r * R(), c.startColor.g + c.startColorVariance.g * R(),
                c.startColor.b + c.startColorVariance.b * R(), c.startColor.a + c.startColorVariance.a * R());
            var c1 = new Color(c.endColor.r + c.endColorVariance.r * R(), c.endColor.g + c.endColorVariance.g * R(),
                c.endColor.b + c.endColorVariance.b * R(), c.endColor.a + c.endColorVariance.a * R());
            p.color = c0;
            p.colorDelta = (c1 - c0) / life;
            float r0 = (c.startRotation + c.startRotationVariance * R()) * Mathf.Deg2Rad;
            float r1 = (c.endRotation + c.endRotationVariance * R()) * Mathf.Deg2Rad;
            p.rotation = r0;
            p.rotationDelta = (r1 - r0) / life;
        }

        void Advance(P p, float dt)
        {
            dt = Mathf.Min(dt, p.totalTime - p.currentTime);
            p.currentTime += dt;
            if (config.emitterType == 1)
            {
                p.emitRotation += p.emitRotationDelta * dt;
                p.emitRadius += p.emitRadiusDelta * dt;
                p.x = emitterOffset.x - Mathf.Cos(p.emitRotation) * p.emitRadius;
                p.y = emitterOffset.y - Mathf.Sin(p.emitRotation) * p.emitRadius;
            }
            else
            {
                float dx = p.x - p.startX, dy = p.y - p.startY;
                float dist = Mathf.Max(0.01f, Mathf.Sqrt(dx * dx + dy * dy));
                float radX = dx / dist, radY = dy / dist;
                float tanX = -radY * p.tangentialAcc, tanY = radX * p.tangentialAcc;
                radX *= p.radialAcc;
                radY *= p.radialAcc;
                p.vx += dt * (config.gravity.x + radX + tanX);
                p.vy += dt * (config.gravity.y + radY + tanY);
                p.x += p.vx * dt;
                p.y += p.vy * dt;
            }
            p.size += p.sizeDelta * dt;
            p.rotation += p.rotationDelta * dt;
            p.color += p.colorDelta * dt;
        }

        void Update()
        {
            if (config == null || (count == 0 && emissionTime <= 0)) return;
            float dt = Time.deltaTime;
            int i = 0;
            while (i < count)
            {
                var p = particles[i];
                if (p.currentTime < p.totalTime)
                {
                    Advance(p, dt);
                    i++;
                }
                else
                {
                    particles[i] = particles[count - 1];
                    particles[count - 1] = p;
                    count--;
                }
            }
            if (emissionTime > 0)
            {
                float between = 1f / EmissionRate;
                frameTime += dt;
                while (frameTime > 0)
                {
                    if (count < config.maxParticles)
                    {
                        if (count == particles.Count) particles.Add(new P());
                        var p = particles[count++];
                        Init(p);
                        if (p.totalTime > 0) Advance(p, frameTime);
                    }
                    frameTime -= between;
                }
                if (!float.IsPositiveInfinity(emissionTime)) emissionTime = Mathf.Max(0, emissionTime - dt);
            }
            SetVerticesDirty();
        }

        protected override void OnPopulateMesh(VertexHelper vh)
        {
            vh.Clear();
            if (config == null) return;
            var r = rectTransform.rect;
            // Particle coordinates are relative to the pivot, y down.
            var origin = new Vector2(r.x + rectTransform.pivot.x * r.width, r.y + rectTransform.pivot.y * r.height);
            for (int i = 0; i < count; i++)
            {
                var p = particles[i];
                var col = (Color32)(new Color(p.color.r, p.color.g, p.color.b, Mathf.Clamp01(p.color.a)) * color);
                float h = p.size * 0.5f;
                float cos = Mathf.Cos(-p.rotation), sin = Mathf.Sin(-p.rotation);
                var center = origin + new Vector2(p.x, -p.y);
                int v = vh.currentVertCount;
                vh.AddVert(center + Rot(-h, -h, cos, sin), col, new Vector2(0, 0));
                vh.AddVert(center + Rot(-h, h, cos, sin), col, new Vector2(0, 1));
                vh.AddVert(center + Rot(h, h, cos, sin), col, new Vector2(1, 1));
                vh.AddVert(center + Rot(h, -h, cos, sin), col, new Vector2(1, 0));
                vh.AddTriangle(v, v + 1, v + 2);
                vh.AddTriangle(v + 2, v + 3, v);
            }
        }

        static Vector2 Rot(float x, float y, float cos, float sin) => new Vector2(x * cos - y * sin, x * sin + y * cos);
    }
}
