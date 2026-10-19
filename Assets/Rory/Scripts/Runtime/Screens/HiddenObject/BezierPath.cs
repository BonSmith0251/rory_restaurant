using UnityEngine;

namespace Rory.Screens.HiddenObject
{
    /// <summary>
    /// CubicHermiteInteropolation.BezierPath as the HO screen used it: a cubic Bezier through four points, sampled
    /// at 2000 steps (t = 0, 0.0005, ...); a tween drives positionCount from 0 to 2000 and the follower jumps to
    /// plot point floor(positionCount) (it stays on the last one at the end).
    /// </summary>
    public readonly struct BezierPath
    {
        /// <summary>Number of plot points (the original's t &lt; 1 loop with step 0.0005).</summary>
        public const int PlotPoints = 2000;
        public const float Step = 0.0005f;

        readonly Vector2 p0, p1, p2, p3;

        public BezierPath(Vector2 p0, Vector2 p1, Vector2 p2, Vector2 p3)
        {
            this.p0 = p0;
            this.p1 = p1;
            this.p2 = p2;
            this.p3 = p3;
        }

        /// <summary>plotPoint(t).</summary>
        public Vector2 Plot(float t)
        {
            float u = 1 - t;
            return u * u * u * p0 + 3 * u * u * t * p1 + 3 * u * t * t * p2 + t * t * t * p3;
        }

        /// <summary>
        /// Follower position for the tweened progress <paramref name="progress"/> (0..1, eased): plot point
        /// floor(progress * 2000), or false once positionCount reaches 2000 (draw() then leaves the follower alone).
        /// </summary>
        public bool TryGetFollowerPosition(float progress, out Vector2 position)
        {
            int index = Mathf.FloorToInt(progress * PlotPoints);
            if (index < 0) index = 0;
            if (index >= PlotPoints)
            {
                position = default;
                return false;
            }
            position = Plot(index * Step);
            return true;
        }
    }
}
