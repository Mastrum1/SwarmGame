using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuManager : MonoBehaviour
{
    [field:SerializeField] public Transition Transition { get;  private set; }
    public static MenuManager MenuManagerInstance { get; private set; }
    public bool CanClickButtons {get; private set;}

    private void Awake()
    {
        MenuManagerInstance = this;
        CanClickButtons = true;
    }

    public void SetButtonsUnclickable()
    {
        CanClickButtons = false;
    }
}
