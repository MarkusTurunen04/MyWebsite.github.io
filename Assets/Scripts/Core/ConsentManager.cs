using System.Threading.Tasks;
using Unity.Services.Core;
using Unity.Services.Mediation;
using UnityEngine;

namespace IdleFrisbeeGolf.Core
{
    /// <summary>
    /// Handles GDPR consent via Unity UMP.
    /// </summary>
    public class ConsentManager : MonoBehaviour
    {
        [SerializeField] private string privacyPolicyUrl;
        [SerializeField] private string termsUrl;

        private async void Start()
        {
            await InitializeUnityServices();
        }

        private async Task InitializeUnityServices()
        {
            try
            {
                await UnityServices.InitializeAsync();
                await MediationService.Instance.Initialize();
                Debug.Log($"Consent initialized. Privacy: {privacyPolicyUrl} ToS: {termsUrl}");
            }
            catch (System.Exception ex)
            {
                Debug.LogWarning($"Consent init failed: {ex.Message}");
            }
        }
    }
}
