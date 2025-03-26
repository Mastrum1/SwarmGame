using TMPro;
using UnityEngine;

namespace Garbage.UI
{
    public class TotalCleaningUI : MonoBehaviour
    {
        [SerializeField] private TMP_Text _text;
        [SerializeField] private string _preffix;
        [SerializeField] private string _suffix;

        private void Start()
        {
            var cleaningManager = MainGame.Instance.CleaningManager;
            cleaningManager.OnPercentageChanged += UpdatePercentage;
            UpdatePercentage(0);
        }

        private void UpdatePercentage(float percentage)
        {
            _text.text = _preffix + (int)(100*percentage) + _suffix;
        }
    }
}