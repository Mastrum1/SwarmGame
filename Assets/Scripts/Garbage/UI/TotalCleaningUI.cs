using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Garbage.UI
{
    public class TotalCleaningUI : MonoBehaviour
    {
        [Header("Percentage")]
        [SerializeField] private TMP_Text _percentageText;
        [SerializeField] private Image _gauge;
        [SerializeField] private string _percentagePreffix;
        [SerializeField] private string _percentageSuffix;
        
        [Header("Total")]
        [SerializeField] private TMP_Text _totalText;
        [SerializeField] private string _totalPreffix;
        [SerializeField] private string _totalSuffix;        


        private void Start()
        {
            var cleaningManager = MainGame.Instance.CleaningManager;
            cleaningManager.OnPercentageChanged += UpdateUI;
            UpdateTotal(0);
            UpdatePercentage(0);
        }

        private void UpdateUI(TotalCleaningManager cleaningManager)
        {
            UpdateTotal(cleaningManager.TotalEntities);
            UpdatePercentage(cleaningManager.CleanedPercentage);
        }

        private void UpdatePercentage(float percentage)
        {
            _percentageText.text = _percentagePreffix + (int)(100*percentage) + _percentageSuffix;
            _gauge.fillAmount = percentage;
        }

        private void UpdateTotal(int cleanedGarbage)
        {
            _totalText.text = _totalPreffix + cleanedGarbage + _totalSuffix;
        }
    }
}