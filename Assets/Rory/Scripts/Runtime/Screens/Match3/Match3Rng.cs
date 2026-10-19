using System;

namespace Rory.Screens.Match3
{
    /// <summary>
    /// The screen's random source (AS3 Math.random behind TGlobalClass.RandomRange and maxCrypto.RandomRange, which use
    /// the same formula). One instance drives the board and the customers so a fixed seed replays a day exactly
    /// (tests set <see cref="SeedOverride"/> before loading the scene).
    /// </summary>
    public class Match3Rng
    {
        /// <summary>Seed for the next Match3 scene (tests); null = time based. Consumed when the screen starts.</summary>
        public static int? SeedOverride;

        readonly Random random;

        public Match3Rng(int? seed = null)
        {
            random = seed.HasValue ? new Random(seed.Value) : new Random();
        }

        /// <summary>Math.random(): [0, 1).</summary>
        public double Next() => random.NextDouble();

        /// <summary>RandomRange(min, max) = floor(random * (1 + max - min)) + min (inclusive, like the original).</summary>
        public int Range(int min, int max) => (int)Math.Floor(Next() * (1 + max - min)) + min;
    }
}
