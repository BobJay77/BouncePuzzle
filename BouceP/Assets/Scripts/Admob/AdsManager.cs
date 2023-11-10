using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UIElements;

public class AdsManager : MonoBehaviour
{
#if UNITY_ANDROID
    string appKey = "1c5518545";
#elif UNITY_IPHONE
        string appKey = "8545d445";
#else
    string appKey = "unexpected_platform";
#endif


    [SerializeField] private int adCounter = 0;
    [SerializeField] private bool _noAds = false;
    private const int adThreshold = 8;

    private static AdsManager instance;

    public static AdsManager Instance
    {
        get { return instance; }
    }

    public bool NoAds
    {
        get
        {
            return _noAds;
        }
        set
        {
            _noAds = value;
        }
    }

    private void Start()
    {
        DontDestroyOnLoad(gameObject);

        if (instance == null)
        {
            instance = this;
        }

        else
        {
            Destroy(gameObject);
        }

        IronSource.Agent.validateIntegration();
        IronSource.Agent.init(appKey);
    }

    private void OnEnable()
    {
        IronSourceEvents.onSdkInitializationCompletedEvent  += SdkInitialized;

        //Add AdInfo Banner Events
        IronSourceBannerEvents.onAdLoadedEvent              += BannerOnAdLoadedEvent;
        IronSourceBannerEvents.onAdLoadFailedEvent          += BannerOnAdLoadFailedEvent;
        IronSourceBannerEvents.onAdClickedEvent             += BannerOnAdClickedEvent;
        IronSourceBannerEvents.onAdScreenPresentedEvent     += BannerOnAdScreenPresentedEvent;
        IronSourceBannerEvents.onAdScreenDismissedEvent     += BannerOnAdScreenDismissedEvent;
        IronSourceBannerEvents.onAdLeftApplicationEvent     += BannerOnAdLeftApplicationEvent;

        //Add AdInfo Interstitial Events
        IronSourceInterstitialEvents.onAdReadyEvent         += InterstitialOnAdReadyEvent;
        IronSourceInterstitialEvents.onAdLoadFailedEvent    += InterstitialOnAdLoadFailed;
        IronSourceInterstitialEvents.onAdOpenedEvent        += InterstitialOnAdOpenedEvent;
        IronSourceInterstitialEvents.onAdClickedEvent       += InterstitialOnAdClickedEvent;
        IronSourceInterstitialEvents.onAdShowSucceededEvent += InterstitialOnAdShowSucceededEvent;
        IronSourceInterstitialEvents.onAdShowFailedEvent    += InterstitialOnAdShowFailedEvent;
        IronSourceInterstitialEvents.onAdClosedEvent        += InterstitialOnAdClosedEvent;

        //Add AdInfo Rewarded Video Events
        IronSourceRewardedVideoEvents.onAdOpenedEvent       += RewardedVideoOnAdOpenedEvent;
        IronSourceRewardedVideoEvents.onAdClosedEvent       += RewardedVideoOnAdClosedEvent;
        IronSourceRewardedVideoEvents.onAdAvailableEvent    += RewardedVideoOnAdAvailable;
        IronSourceRewardedVideoEvents.onAdUnavailableEvent  += RewardedVideoOnAdUnavailable;
        IronSourceRewardedVideoEvents.onAdShowFailedEvent   += RewardedVideoOnAdShowFailedEvent;
        IronSourceRewardedVideoEvents.onAdRewardedEvent     += RewardedVideoOnAdRewardedEvent;
        IronSourceRewardedVideoEvents.onAdClickedEvent      += RewardedVideoOnAdClickedEvent;


    }

    public void NoAdsBought()
    {
        _noAds = true;//setting the no ads bool to true for this instance
        GameSystem.instance.AccountSettings.NoAds = _noAds; //updating it in the account settings

        //saving the account settings
        GameSystem.instance.DataService.SaveData<AccountSettings>("/acc.json", GameSystem.instance.AccountSettings, GameSystem.instance.EncryptionEnabled);
    }

    void SdkInitialized()
    {
        print("Sdk in initialized!!");
    }
    void OnApplicationPause(bool isPaused)
    {
        IronSource.Agent.onApplicationPause(isPaused);
    }

    #region banner
    public void LoadBanner()
    {
        IronSource.Agent.loadBanner(IronSourceBannerSize.BANNER, IronSourceBannerPosition.BOTTOM);
    }
    public void DestroyBanner()
    {
        IronSource.Agent.destroyBanner();
    }


    /************* Banner AdInfo Delegates *************/
    //Invoked once the banner has loaded
    void BannerOnAdLoadedEvent(IronSourceAdInfo adInfo)
    {
    }
    //Invoked when the banner loading process has failed.
    void BannerOnAdLoadFailedEvent(IronSourceError ironSourceError)
    {
    }
    // Invoked when end user clicks on the banner ad
    void BannerOnAdClickedEvent(IronSourceAdInfo adInfo)
    {
    }
    //Notifies the presentation of a full screen content following user click
    void BannerOnAdScreenPresentedEvent(IronSourceAdInfo adInfo)
    {
    }
    //Notifies the presented screen has been dismissed
    void BannerOnAdScreenDismissedEvent(IronSourceAdInfo adInfo)
    {
    }
    //Invoked when the user leaves the app
    void BannerOnAdLeftApplicationEvent(IronSourceAdInfo adInfo)
    {
    }

    #endregion

    #region interstitial

    public void LoadInt()
    {
        IronSource.Agent.loadInterstitial();
    }

    public void LoadInterstitial()
    {
        if (_noAds) { return; }

        //Adding 1 to the counter
        adCounter++;

        //Checking if ads threshold met 
        if (adCounter >= adThreshold)
        {
            IronSource.Agent.loadInterstitial();

            StartCoroutine(ShowInterstitial());
            
            adCounter = 0;
        }

    }
    public IEnumerator ShowInterstitial()
    {
        bool isready = false;

        float startTime = Time.time;

        while (!isready)
        {
            if (IronSource.Agent.isInterstitialReady())
            {
                IronSource.Agent.showInterstitial();
                isready = true;
                yield break;
            }
            else if(Time.time - startTime >= 5.0f)
            {
                isready = true;
                yield break;
            }
            else
            {
                //Debug.Log("interstitial not ready!!");
            }

            yield return null;
        }
    }

    /************* Interstitial AdInfo Delegates *************/
    // Invoked when the interstitial ad was loaded succesfully.
    void InterstitialOnAdReadyEvent(IronSourceAdInfo adInfo)
    {
        Debug.Log("unity-script: I got InterstitialOnAdReadyEvent With AdInfo " + adInfo.ToString());
    }
    // Invoked when the initialization process has failed.
    void InterstitialOnAdLoadFailed(IronSourceError ironSourceError)
    {
        Debug.Log("unity-script: I got InterstitialOnAdLoadFailed With Error " + ironSourceError.ToString());
    }
    // Invoked when the Interstitial Ad Unit has opened. This is the impression indication. 
    void InterstitialOnAdOpenedEvent(IronSourceAdInfo adInfo)
    {
        Debug.Log("unity-script: I got InterstitialOnAdOpenedEvent With AdInfo " + adInfo.ToString());
    }

    // Invoked when end user clicked on the interstitial ad
    void InterstitialOnAdClickedEvent(IronSourceAdInfo adInfo)
    {
        Debug.Log("unity-script: I got InterstitialOnAdClickedEvent With AdInfo " + adInfo.ToString());
    }
    // Invoked when the ad failed to show.
    void InterstitialOnAdShowFailedEvent(IronSourceError ironSourceError, IronSourceAdInfo adInfo)
    {
        Debug.Log("unity-script: I got InterstitialOnAdShowFailedEvent With Error " + ironSourceError.ToString() + " And AdInfo " + adInfo.ToString());
    }
    // Invoked when the interstitial ad closed and the user went back to the application screen.
    void InterstitialOnAdClosedEvent(IronSourceAdInfo adInfo)
    {
        Debug.Log("unity-script: I got InterstitialOnAdClosedEvent With AdInfo " + adInfo.ToString());
    }
    // Invoked before the interstitial ad was opened, and before the InterstitialOnAdOpenedEvent is reported.
    // This callback is not supported by all networks, and we recommend using it only if  
    // it's supported by all networks you included in your build. 
    void InterstitialOnAdShowSucceededEvent(IronSourceAdInfo adInfo)
    {
        Debug.Log("unity-script: I got InterstitialOnAdShowSucceededEvent With AdInfo " + adInfo.ToString());
    }

    #endregion

    #region rewarded

    public void LoadRewarded()
    {
        IronSource.Agent.loadRewardedVideo();
    }
    public void ShowRewarded()
    {
        if (IronSource.Agent.isRewardedVideoAvailable())
        {
            IronSource.Agent.showRewardedVideo();
        }
        else
        {
            Debug.Log("rewarded not ready!!");
        }
    }


    /************* RewardedVideo AdInfo Delegates *************/
    // Indicates that there’s an available ad.
    // The adInfo object includes information about the ad that was loaded successfully
    // This replaces the RewardedVideoAvailabilityChangedEvent(true) event
    void RewardedVideoOnAdAvailable(IronSourceAdInfo adInfo)
    {
    }
    // Indicates that no ads are available to be displayed
    // This replaces the RewardedVideoAvailabilityChangedEvent(false) event
    void RewardedVideoOnAdUnavailable()
    {
    }
    // The Rewarded Video ad view has opened. Your activity will loose focus.
    void RewardedVideoOnAdOpenedEvent(IronSourceAdInfo adInfo)
    {
    }
    // The Rewarded Video ad view is about to be closed. Your activity will regain its focus.
    void RewardedVideoOnAdClosedEvent(IronSourceAdInfo adInfo)
    {
    }
    // The user completed to watch the video, and should be rewarded.
    // The placement parameter will include the reward data.
    // When using server-to-server callbacks, you may ignore this event and wait for the ironSource server callback.
    void RewardedVideoOnAdRewardedEvent(IronSourcePlacement placement, IronSourceAdInfo adInfo)
    {
    }
    // The rewarded video ad was failed to show.
    void RewardedVideoOnAdShowFailedEvent(IronSourceError error, IronSourceAdInfo adInfo)
    {
    }
    // Invoked when the video ad was clicked.
    // This callback is not supported by all networks, and we recommend using it only if
    // it’s supported by all networks you included in your build.
    void RewardedVideoOnAdClickedEvent(IronSourcePlacement placement, IronSourceAdInfo adInfo)
    {
    }

    #endregion
}