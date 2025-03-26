namespace Common.ScreenShake
{
    public class ScreenShakeElement
    {
        public ScreenShakeParams Params;
        public float Timer;

        public ScreenShakeElement(ScreenShakeParams shakeParams)
        {
            Params = shakeParams;
            Timer = 0f;
        }
    }
}