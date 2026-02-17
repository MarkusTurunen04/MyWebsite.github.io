namespace IdleFrisbeeGolf.Meta
{
    public interface IAdsManager
    {
        bool IsRewardedReady(string placement);
        void ShowRewarded(string placement);
        bool CanShowInterstitial();
        void ShowInterstitial(string placement);
        void RequestInterstitial();
        void Tick(float deltaTime);
    }
}
