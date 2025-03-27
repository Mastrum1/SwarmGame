using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class Transition : MonoBehaviour
{
    [Tooltip("The visual representation type of the transition")] [SerializeField] private AnimationMode _transitionMode;
    [Tooltip("Fades in at the beginning")] [SerializeField] private bool _fadeIn;
    [Header("Fade")]
    [Tooltip("The Image used for the Fade transition")] [SerializeField] private Image _fadeImage;
    [Tooltip("The time the fade takes to complete in seconds")] [SerializeField] private float _fadeTime = 1f;
    [Header("Animations")]
    [Tooltip("The Animator used for the Animation transition")] [SerializeField] private Animator _animator;
    

    enum AnimationMode
    {
        Fade,
        Animation
    }

    private void Start()
    {
        if (_fadeIn)
        {
            switch (_transitionMode)
            {
                case AnimationMode.Fade:
                    _fadeImage.DOFade(1, 0);
                    _fadeImage.DOFade(0, _fadeTime);
                    break;
                case AnimationMode.Animation:
                    AnimationTransition("TransitionIn");
                    break;
            }
        }
    }

    public void SetTransition(Action endAction)
    {
        StartCoroutine(_startTransition(endAction));
    }

    private IEnumerator _startTransition(Action endAction)
    {
        switch (_transitionMode)
        {
            case AnimationMode.Fade:
                _fadeImage.DOFade(0, 0);
                _fadeImage.DOFade(1, _fadeTime);
                yield return new WaitForSeconds(_fadeTime);
                break;
            case AnimationMode.Animation:
                float _animationDuration = AnimationTransition("TransitionOut");
                yield return new WaitForSeconds(_animationDuration);
                break;
            default:
                yield return new WaitForSeconds(0f);
                break;
        }
        
        endAction?.Invoke();
    }

    private float AnimationTransition(string transitionName)
    {
        _animator.Play(transitionName);
        var _stateInfo = _animator.GetCurrentAnimatorStateInfo(0);
        float _animationLength = (1 / _stateInfo.length);
        float _framePerSecondsInAnimation = 24;
        float _targetFPS = 60;
        float _animationDuration = (1-_animationLength / _framePerSecondsInAnimation * _targetFPS * Time.deltaTime) + 1;
        return _animationDuration;
    }
}
