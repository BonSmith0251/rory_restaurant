using System;
using System.Collections.Generic;
using UnityEngine;

namespace Rory.Data
{
    /// <summary>A Particle Designer (.pex) emitter, imported from Art/Effects/*Config.xml.</summary>
    [CreateAssetMenu(menuName = "Rory/Particle Config")]
    public class ParticleConfig : ScriptableObject
    {
        public Sprite texture;
        public Vector2 sourcePositionVariance;
        public float speed, speedVariance;
        public float lifespan, lifespanVariance;
        public float angle, angleVariance;
        public Vector2 gravity;
        public float radialAcceleration, radialAccelerationVariance;
        public float tangentialAcceleration, tangentialAccelerationVariance;
        public Color startColor, startColorVariance, endColor, endColorVariance;
        public int maxParticles;
        public float startSize, startSizeVariance, endSize, endSizeVariance;
        [Tooltip("Seconds; negative = endless")] public float duration = -1;
        [Tooltip("0 = gravity, 1 = radial")] public int emitterType;
        public float maxRadius, maxRadiusVariance, minRadius, minRadiusVariance;
        public float rotatePerSecond, rotatePerSecondVariance;
        public float startRotation, startRotationVariance, endRotation, endRotationVariance;
        [Tooltip("Additive blending (blendFuncDestination = ONE)")] public bool additive;
    }
}
