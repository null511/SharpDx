namespace SharpDX.Data
{
    class FrameCounter
    {
        public delegate void UpdateEvent(int value);
        public event UpdateEvent OnUpdate;

        private int count, value;
        private float duration;

        public int Value => value;


        public void Update(float elapsed) {
            duration += elapsed;
            count++;

            if (duration > 1000f) {
                value = count;
                duration -= 1000f;
                count = 0;

                OnUpdate?.Invoke(value);
            }
        }
    }
}
