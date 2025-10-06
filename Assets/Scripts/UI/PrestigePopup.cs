using IdleFrisbeeGolf.Core;
using IdleFrisbeeGolf.Game;
using UnityEngine;
using UnityEngine.UI;

namespace IdleFrisbeeGolf.UI
{
    public class PrestigePopup : MonoBehaviour
    {
        [SerializeField] private PrestigeSystem prestigeSystem;
        [SerializeField] private Text previewText;
        [SerializeField] private Button prestigeButton;

        private void OnEnable()
        {
            Refresh();
            prestigeButton.onClick.AddListener(DoPrestige);
        }

        private void OnDisable()
        {
            prestigeButton.onClick.RemoveListener(DoPrestige);
        }

        private void Refresh()
        {
            var tokens = prestigeSystem.PreviewTokens();
            previewText.text = $"Prestige for {NumberFormatter.Format(tokens)} Spirit";
            prestigeButton.interactable = tokens > 0;
        }

        private void DoPrestige()
        {
            prestigeSystem.ExecutePrestige();
            gameObject.SetActive(false);
        }
    }
}
