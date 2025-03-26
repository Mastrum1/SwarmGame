using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Common.Haptic
{
    public class HapticManager : MonoBehaviour
    {
        private static HapticManager _instance;

        private void Awake()
        {
            _instance = this;
        }
        
        /// <summary>
        /// Start vibrating a gamepad for a set time in seconds
        /// </summary>
        /// <param name="lowFrequency"></param>
        /// <param name="highFrequency"></param>
        /// <param name="time"></param>
        public static void VibrateForTime(float lowFrequency, float highFrequency, float time)
        {
            if (_instance == null)
            {
                Debug.LogWarning("HapticManager Instance does not exist!");
                return;
            }
                
            if (Gamepad.current == null) return;
            Gamepad.current.SetMotorSpeeds(lowFrequency, highFrequency);
            _instance.StartCoroutine(_instance.VibrateController(time));
        }

        /// <summary>
        /// Start vibrating a gamepad for a set time in seconds
        /// </summary>
        /// <param name="feedback"></param>
        public static void VibrateForTime(HapticFeedback feedback)
        {
            VibrateForTime(feedback.LowFrequency, feedback.HighFrequency, feedback.Time);
        }

        private IEnumerator VibrateController(float time)
        {
            yield return new WaitForSeconds(time);
            Gamepad.current.SetMotorSpeeds(0, 0);
        }
    }

    [Serializable]
    public struct HapticFeedback
    {
        public float LowFrequency;
        public float HighFrequency;
        public float Time;

        public HapticFeedback(float low, float high, float time)
        {
            LowFrequency = low;
            HighFrequency = high;
            Time = time;
        }
    }
}