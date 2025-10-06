using System;
using IdleFrisbeeGolf.Core;
using IdleFrisbeeGolf.Data;
using UnityEngine;
using UnityEngine.Purchasing;

namespace IdleFrisbeeGolf.Meta
{
    /// <summary>
    /// Unity IAP implementation with catalog ScriptableObject.
    /// </summary>
    public class IAPManager : MonoBehaviour, IIAPManager, IStoreListener
    {
        [SerializeField] private IAPCatalog catalog;
        [SerializeField] private AnalyticsService analyticsService;

        private IStoreController _controller;
        private IExtensionProvider _extensions;
        private Action<string> _onPurchaseCompleted;

        public bool IsInitialized => _controller != null;

        public void Initialize(Action<string> onPurchaseCompleted)
        {
            _onPurchaseCompleted = onPurchaseCompleted;
            var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());
            foreach (var product in catalog.products)
            {
                var type = ProductType.Consumable;
                if (product.type == IAPCatalog.ProductType.NonConsumable)
                {
                    type = ProductType.NonConsumable;
                }
                else if (product.type == IAPCatalog.ProductType.Bundle)
                {
                    type = ProductType.NonConsumable;
                }
                builder.AddProduct(product.id, type);
            }

            UnityPurchasing.Initialize(this, builder);
        }

        public void PurchaseProduct(string productId)
        {
            if (!IsInitialized)
            {
                Debug.LogWarning("IAP not initialized");
                return;
            }

            _controller.InitiatePurchase(productId);
        }

        public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
        {
            _controller = controller;
            _extensions = extensions;
            Debug.Log("IAP initialized");
        }

        public void OnInitializeFailed(InitializationFailureReason error)
        {
            Debug.LogError($"IAP init failed: {error}");
        }

        public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs e)
        {
            analyticsService.LogIapPurchase(e.purchasedProduct.definition.id);
            _onPurchaseCompleted?.Invoke(e.purchasedProduct.definition.id);
            return PurchaseProcessingResult.Complete;
        }

        public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
        {
            Debug.LogWarning($"Purchase failed: {product.definition.id} {failureReason}");
        }
    }
}
