using System;

namespace SharpDX.Core
{
    /// <summary>Thread-Safe Random</summary>
    class RandomDictionary : ThreadDictionary<Random>
    {
        public RandomDictionary(Random global) : base(() => new Random(global.Next())) {}
    }

    class RandomEx
    {
        private static readonly Random _global;
        private static readonly RandomDictionary _random;


        static RandomEx() {
            _global = new Random();
            _random = new RandomDictionary(_global);
        }

        public static int Next(int max) {
            return _random.Get().Next(max);
        }

        public static float Next(float max) {
            return _random.Get().NextFloat(0f, max);
        }

        public static float Next(float min, float max) {
            return _random.Get().NextFloat(min, max);
        }
    }
}
