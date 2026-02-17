using IdleFrisbeeGolf.Data;
using IdleFrisbeeGolf.Meta;
using UnityEngine;
using UnityEngine.UI;

namespace IdleFrisbeeGolf.UI
{
    public class IAPPopup : MonoBehaviour
    {
        [SerializeField] private IAPCatalog catalog;
        [SerializeField] private MonoBehaviour iapManagerBehaviour;
        [SerializeField] private Transform listRoot;
        [SerializeField] private GameObject itemPrefab;

        private IIAPManager _iapManager;

        private void OnEnable()
        {
            _iapManager ??= iapManagerBehaviour as IIAPManager;
            if (_iapManager == null)
            {
                Debug.LogError("IAP Manager not assigned or does not implement IIAPManager");
                return;
            }

            foreach (Transform child in listRoot)
            {
                Destroy(child.gameObject);
            }

            foreach (var product in catalog.products)
            {
                var go = Instantiate(itemPrefab, listRoot);
                go.GetComponentInChildren<Text>().text = product.id;
                var id = product.id;
                go.GetComponent<Button>().onClick.AddListener(() => Purchase(id));
            }
        }

        private void Purchase(string productId)
        {
            _iapManager.PurchaseProduct(productId);
        }
    }
}
