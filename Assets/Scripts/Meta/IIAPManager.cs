using System;

namespace IdleFrisbeeGolf.Meta
{
    public interface IIAPManager
    {
        void Initialize(Action<string> onPurchaseCompleted);
        void PurchaseProduct(string productId);
        bool IsInitialized { get; }
    }
}
