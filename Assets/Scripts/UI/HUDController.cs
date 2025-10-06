using IdleFrisbeeGolf.Core;
using IdleFrisbeeGolf.Game;
using IdleFrisbeeGolf.Meta;
using UnityEngine;
using UnityEngine.UI;

namespace IdleFrisbeeGolf.UI
{
    /// <summary>
    /// Updates HUD values and handles manual throws.
    /// </summary>
    public class HUDController : MonoBehaviour
    {
        [SerializeField] private Text scoreText;
        [SerializeField] private Text perSecondText;
        [SerializeField] private Text autoThrowersText;
        [SerializeField] private Button throwButton;
        [SerializeField] private Button doubleGainButton;
        [SerializeField] private IdleManager idleManager;
        [SerializeField] private ThrowSystem throwSystem;
        [SerializeField] private AdsManager adsManager;

        private void Awake()
        {
            throwButton.onClick.AddListener(() => idleManager.ManualThrow());
            doubleGainButton.onClick.AddListener(OnDoubleGainClicked);
            EventBus.Subscribe<ScoreChangedMessage>(OnScoreChanged);
        }

        private void OnDestroy()
        {
            EventBus.Unsubscribe<ScoreChangedMessage>(OnScoreChanged);
        }

        private void Update()
        {
            adsManager.Tick(Time.deltaTime);
            doubleGainButton.interactable = adsManager.IsRewardedReady("double_gain");
            autoThrowersText.text = $"Auto: {NumberFormatter.Format(throwSystem.AutoThrowers)} ({NumberFormatter.Format(throwSystem.PointsPerSecond)}/s)";
        }

        private void OnScoreChanged(ScoreChangedMessage message)
        {
            scoreText.text = NumberFormatter.Format(message.Current);
            perSecondText.text = $"{NumberFormatter.Format(message.PerSecond)}/s";
        }

        private void OnDoubleGainClicked()
        {
            if (!adsManager.IsRewardedReady("double_gain"))
            {
                return;
            }

            adsManager.ShowRewarded("double_gain");
            // Would trigger temporary multiplier buff in production.
        }
    }
}
