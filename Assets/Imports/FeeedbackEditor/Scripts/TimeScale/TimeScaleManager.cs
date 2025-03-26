using System.Collections;
using UnityEngine;

namespace Common.TimeScale
{
    public class TimeScaleManager : MonoBehaviour
    {
        private static TimeScaleManager _instance;
        private static Coroutine _currentCoroutine;

        private void Awake()
        {
            _instance = this;
        }

        public static void ChangeTimeScale(TimeScaleParams timeScaleParams)
        {
            if (_instance != null)
            {
                if (_currentCoroutine != null) _instance.StopCoroutine(_currentCoroutine);
                _currentCoroutine = _instance.StartCoroutine(_instance.LerpTimeScale(timeScaleParams));
            }
            else
            {
                Debug.LogWarning("TimeScaleManager Instance does not exist!");
            }
        }

        private IEnumerator LerpTimeScale(TimeScaleParams scaleParams)
        {
            float elapsedTime = 0f;

            while (elapsedTime < scaleParams.Duration)
            {
                elapsedTime += UnityEngine.Time.unscaledDeltaTime;
                float curveStrength = scaleParams.Curve.Evaluate(elapsedTime / scaleParams.Duration);
                Time.timeScale = curveStrength;
                yield return null;
            }
        }
    }
}