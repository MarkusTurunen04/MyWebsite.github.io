using UnityEngine;

namespace IdleFrisbeeGolf.Data
{
    [CreateAssetMenu(fileName = "IAPCatalog", menuName = "IdleFrisbeeGolf/IAPCatalog")]
    public class IAPCatalog : ScriptableObject
    {
        [System.Serializable]
        public class Product
        {
            public string id;
            public ProductType type;
            public int gemReward;
            public string description;
        }

        public enum ProductType
        {
            NonConsumable,
            Consumable,
            Bundle
        }

        public Product[] products =
        {
            new Product { id = "no_ads", type = ProductType.NonConsumable, description = "Remove ads" },
            new Product { id = "gems_small", type = ProductType.Consumable, gemReward = 200 },
            new Product { id = "gems_medium", type = ProductType.Consumable, gemReward = 600 },
            new Product { id = "gems_large", type = ProductType.Consumable, gemReward = 1400 },
            new Product { id = "starter_pack", type = ProductType.Bundle, gemReward = 500 }
        };
    }
}
