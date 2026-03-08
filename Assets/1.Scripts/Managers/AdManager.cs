using System;
using UnityEngine;

/// <summary>
/// GDD: 광고 - 생산속도 2배, 무료 아이템, 오프라인 보상 2배.
/// 인앱 결제: Starter Pack, Gem Pack, Remove Ads.
/// 실제 SDK 연동 전 스텁.
/// </summary>
public class AdManager : MonoBehaviour
{
    public static AdManager Instance { get; private set; }

    [SerializeField] float productionMultiplier = 1f;
    [SerializeField] bool adsRemoved;

    public float ProductionMultiplier => productionMultiplier;
    public bool AdsRemoved => adsRemoved;

    public event Action OnRewardedAdWatched;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void RequestDoubleProductionAd(Action onSuccess, Action onFail)
    {
        // TODO: 광고 SDK 연동 후 콜백
        onFail?.Invoke();
    }

    public void RequestFreeItemAd(Action onSuccess, Action onFail)
    {
        onFail?.Invoke();
    }

    public void RequestDoubleOfflineRewardAd(Action onSuccess, Action onFail)
    {
        onFail?.Invoke();
    }

    public void SetProductionMultiplier(float value) => productionMultiplier = Mathf.Max(1f, value);
    public void SetAdsRemoved(bool value) => adsRemoved = value;
}
