using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Common.ScreenShake
{
    public class ScreenShake : MonoBehaviour
    {
        private static ScreenShake _instance;

        [SerializeField] private ScreenShakeType _orderingType = ScreenShakeType.MAXIMUM;
        
        private Dictionary<ScreenShakeType, Func<Vector3>> _orderingActions = new Dictionary<ScreenShakeType, Func<Vector3>>();

        private Vector3 _startPosition;
        private List<ScreenShakeElement> _shakeElements = new List<ScreenShakeElement>();

        private void Awake()
        {
            _instance = this;
        }

        private void Start()
        {
            _startPosition = transform.localPosition;
            _orderingActions.Add(ScreenShakeType.MAXIMUM, GetMaximumOffset);
            _orderingActions.Add(ScreenShakeType.OVERRIDE, GetOverrideOffset);
            _orderingActions.Add(ScreenShakeType.ADDITIVE, GetAdditiveOffset);
        }

        public static void Shake(ScreenShakeParams shakeParams)
        {
            if (_instance != null) _instance._shakeElements.Add(new ScreenShakeElement(shakeParams));
            else Debug.LogWarning("ScreenShake Instance does not exist!");
        }
        
        private void Update()
        {
            if (_shakeElements.Count < 1) return;
            
            transform.localPosition = _startPosition;

            transform.localPosition += GetShakeOffset();
        }

        private Vector3 UpdateShakeElement(ScreenShakeElement shakeElement)
        {
            shakeElement.Timer += UnityEngine.Time.deltaTime;
            if (shakeElement.Timer > shakeElement.Params.Duration)
            {
                _shakeElements.Remove(shakeElement);
                return Vector3.zero;
            }

            float curveStrength = shakeElement.Params.Curve.Evaluate(shakeElement.Timer / shakeElement.Params.Duration);
            var targetDirection = curveStrength * shakeElement.Params.Strength * Random.insideUnitSphere;
            targetDirection.z = _startPosition.z;

            return targetDirection;
        }

        private Vector3 GetShakeOffset()
        {
            return _orderingActions[_orderingType].Invoke();
        }
        
        private Vector3 GetOverrideOffset()
        {
            var targetOffset = Vector3.zero;
            
            for (int i = _shakeElements.Count - 1; i >= 0; i--)
            {
                if (i == _shakeElements.Count - 1) targetOffset = UpdateShakeElement(_shakeElements[i]);
                else UpdateShakeElement(_shakeElements[i]);
            }

            return targetOffset;
        }

        private Vector3 GetMaximumOffset()
        {
            var targetOffset = Vector3.zero;
            float max = 0;
            
            for (int i = _shakeElements.Count - 1; i >= 0; i--)
            {
                var strength = _shakeElements[i].Params.Strength;
                if (strength > max)
                {
                    targetOffset = UpdateShakeElement(_shakeElements[i]);
                    max = strength;
                }
                else
                {
                    UpdateShakeElement(_shakeElements[i]);
                }
            }

            return targetOffset;
        }
        
        private Vector3 GetAdditiveOffset()
        {
            var targetOffset = Vector3.zero;
            
            for (int i = _shakeElements.Count - 1; i >= 0; i--)
            {
                targetOffset += UpdateShakeElement(_shakeElements[i]);
            }

            return targetOffset;
        }
    }
}