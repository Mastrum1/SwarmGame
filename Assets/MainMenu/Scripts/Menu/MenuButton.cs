using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class MenuButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private MenuManager _menuManager;

    [Header("Button Parameters")]
    [SerializeField] private string _targetScene;
    [SerializeField] private float _originalScale = 1;
    [SerializeField] private float _hoverScale = 1.1f;
    [SerializeField] private float _hoverScaleDuration = 0.3f;
    [SerializeField] private float _notHoverScaleDuration = 0.5f;

    private void Start()
    {
        _menuManager = MenuManager.MenuManagerInstance;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (_menuManager.CanClickButtons)
        {
            transform.DOKill();
            transform.DOScale(new Vector3(_hoverScale, _hoverScale), _hoverScaleDuration);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        transform.DOKill();
        transform.DOScale(new Vector3(_originalScale, _originalScale), _notHoverScaleDuration);
    }

    public void GoToScene()
    {
        if (_menuManager.CanClickButtons)
        {
            _menuManager.SetButtonsUnclickable();
            _menuManager.Transition.SetTransition(() => SceneManager.LoadScene(_targetScene));

        }
    }

    public void QuitGame()
    {
        if (_menuManager.CanClickButtons)
        {
            _menuManager.SetButtonsUnclickable();
            _menuManager.Transition.SetTransition(() => Application.Quit());

        }
    }
}
