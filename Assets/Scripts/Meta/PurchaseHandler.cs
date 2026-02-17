using IdleFrisbeeGolf.Data;
using IdleFrisbeeGolf.Game;
using UnityEngine;

namespace IdleFrisbeeGolf.Meta
{
    /// <summary>
    /// Applies purchase rewards and toggles ads removal flag.
    /// </summary>
    public class PurchaseHandler : MonoBehaviour
    {
        [SerializeField] private IdleManager idleManager;
        [SerializeField] private MonoBehaviour iapManagerBehaviour;
        [SerializeField] private IAPCatalog catalog;
        [SerializeField] private AdsManager adsManager;

        private IIAPManager _iapManager;

        private void Start()
        {
            _iapManager = iapManagerBehaviour as IIAPManager;
            if (_iapManager == null)
            {
                Debug.LogError("IAP manager reference must implement IIAPManager");
                enabled = false;
                return;
            }

            _iapManager.Initialize(OnPurchaseCompleted);
            adsManager.Initialize(idleManager.State.adsRemoved);
        }

        private void OnPurchaseCompleted(string productId)
        {
            var state = idleManager.State;
            foreach (var product in catalog.products)
            {
                if (product.id != productId)
                {
                    continue;
                }

                switch (productId)
                {
                    case "no_ads":
                        state.adsRemoved = true;
                        adsManager.Initialize(true);
                        break;
                    case "starter_pack":
                        state.gems += product.gemReward;
                        state.currentScore += 1000;
                        break;
                    default:
                        state.gems += product.gemReward;
                        break;
                }
            }
        }
    }
}
