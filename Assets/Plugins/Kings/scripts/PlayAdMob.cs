//#define USE_PLAY_ADMOB


using System;
using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
#if USE_PLAY_ADMOB
using GoogleMobileAds;
using GoogleMobileAds.Api;
#endif

// Based on https://github.com/googleads/googleads-mobile-unity/tree/master/samples/HelloWorld
public class PlayAdMob : MonoBehaviour
{
    [System.Serializable]
    public class mDescribedEvent
    {
        public UnityEvent mEvent;
        public void Invoke()
        {
            mEvent.Invoke();
        }
    }

    private bool adsSaveEnabled = true; //are the ads enabled from save?


    public bool IsActivated()
    {
#if USE_PLAY_ADMOB
        return true;
#else
    return false;
#endif
    }

    private void Awake()
    {
        adsSaveEnabled = GetSavedEnable();   
    }

#if USE_PLAY_ADMOB
    private static BannerView bannerView;
    private InterstitialAd interstitial;
    private RewardBasedVideoAd rewardBasedVideo;
#endif

    private List<string> outputMessages = new List<string>();

    //public access with:
#if FALSE
    RequestBanner()
    DestroyBanner()
    RequestInterstitial()
    ShowInterstitial()
    DestroyInterstitial()
    RequestRewardBasedVideo()
    ShowRewardBasedVideo()
#endif

    void showNotEnabledWarning()
    {
#if USE_PLAY_ADMOB
//#else
        //Debug.Log(notEnabledMessage);
#endif
    }
    public string notEnabledMessage =
        "PlayAdMob is deactivated.\n" +
        "To enable PlayAdMob import the Google Mobile Ads Unity asset (https://developers.google.com/admob/unity/start). \n" +
        "Afterwards activate PlayAdMob by uncommenting the define '#define USE_PLAY_ADMOB' in the script PlayAdMob.cs. \n" +
        "For more Information about PlayAdMob refer to the manual. \n" +
        "For more Information about Google AdMob refer to https://developers.google.com/admob/unity/start.";


    #region configuration structures
    [System.Serializable]
    public class C_Birthday
    {
        [Tooltip("Enable the transfer of the birthday.")]
        public bool enable = false;
        public int day = 1;
        public int month = 1;
        public int year = 1985;
    }

    [System.Serializable]
    public enum E_max_ad_content_rating
    {
        G,
        PG,
        T,
        MA
    }

    [System.Serializable]
    public class C_max_ad_content_rating
    {
        public bool enable = false;
        public E_max_ad_content_rating max_ad_content_rating;
    }

    [System.Serializable]
    public enum E_YES_NO
    {
        yes,
        no
    }

    [System.Serializable]
    public class C_GenderConfig
    {
#if USE_PLAY_ADMOB
        public bool enable = false;

        public Gender gender;
#endif
    }

    [System.Serializable]
    public class C_EN_YES_NO
    {
        public bool enable;
        public E_YES_NO yesNo;
    }

    [System.Serializable]
    public class C_TargetAudience
    {
        [Tooltip("Enable | Gender\n" +
            "Set the gender with 'SetGender(...)' for the target audience. \n" +
            "To take affect before the first ad is requested, set the birthday in 'Awake()'.")]
        public C_GenderConfig genderConfig;
        [Tooltip("Enable | day.month.year\n" +
            "Set the birthday with 'SetBirthday(...)' for the target audience. \n" +
            "To take affect before the first ad is requested, set the birthday in 'Awake()'.")]
        public C_Birthday birthday;
        [Tooltip("Enable | Tag\n" +
            "Not changeable ingame.")]
        public C_EN_YES_NO TagForChildDirectedTreatment;
        [Tooltip("Enable | Tag\n" +
            "Not changeable ingame.")]
        public C_EN_YES_NO tag_for_under_age_of_consent;
        [Tooltip("Enable | Rating\n" +
            "Not changeable ingame.")]
        public C_max_ad_content_rating max_ad_content_rating;
    }

    [System.Serializable]
    public class C_TestConfiguration
    {
        [Tooltip("Show all debug output messages. Please add a text box with adequate space.")]
        public Text debugOutput;
        [Tooltip("Adds 'AdRequest.TestDeviceSimulator' + 'Test Devices'.\n https://developers.google.com/admob/unity/test-ads#enable_test_devices")]
        public bool testDevicesEnabled = true;
        [Tooltip("https://developers.google.com/admob/unity/test-ads#enable_test_devices")]
        public List<string> testDevices = new List<string>();
    }

    [Tooltip("Configuration for testing.")]
    public C_TestConfiguration testConfiguration;
    [Tooltip("Configure if needed.\n https://developers.google.com/admob/unity/targeting")]
    public C_TargetAudience targetAudience;
    [Tooltip("Configure if needed.\nSorry, Unity directed documentation is missing in https://developers.google.com/admob/unity")]
    public List<string> keyWords = new List<string>();
    public C_IDS ids = new C_IDS();


    [System.Serializable]
    public class C_IDS
    {
        [Tooltip("https://developers.google.com/admob/android/quick-start \nSample AdMob App ID: \n" +
            "ca-app-pub-3940256099942544~3347511713")]
        public string appID_ANDROID = "YOUR_ADMOB_APP_ID";
        [Tooltip("https://developers.google.com/admob/unity/test-ads \nca-app-pub-3940256099942544/6300978111")]
        public string banner_adUnitID_ANDROID = "ca-app-pub-3940256099942544/6300978111";
        [Tooltip("https://developers.google.com/admob/unity/test-ads \nca-app-pub-3940256099942544/1033173712")]
        public string Interstitial_adUnitID_ANDROID = "ca-app-pub-3940256099942544/1033173712";
        [Tooltip("https://developers.google.com/admob/unity/test-ads \nca-app-pub-3940256099942544/5224354917")]
        public string RewardBasedVideo_adUnitID_ANDROID = "ca-app-pub-3940256099942544/5224354917";

        [Tooltip("https://developers.google.com/admob/ios/quick-start \nSample AdMob app ID: \n" +
            "ca-app-pub-3940256099942544~1458002511")]
        public string appID_IPHONE = "YOUR_ADMOB_APP_ID";
        [Tooltip("https://developers.google.com/admob/unity/test-ads \nca-app-pub-3940256099942544/2934735716")]
        public string banner_adUnitID_IPHONE = "ca-app-pub-3940256099942544/2934735716";
        [Tooltip("https://developers.google.com/admob/unity/test-ads \nca-app-pub-3940256099942544/4411468910")]
        public string Interstitial_adUnitID_IPHONE = "ca-app-pub-3940256099942544/4411468910";
        [Tooltip("https://developers.google.com/admob/unity/test-ads \nca-app-pub-3940256099942544/1712485313")]
        public string RewardBasedVideo_adUnitID_IPHONE = "ca-app-pub-3940256099942544/1712485313";

    }

    //General Events
    [System.Serializable]
    public class C_GeneralEvents
    {
        [Tooltip("Called when initialization is done. This is a good place to reset ad dependant values.")]
        public mDescribedEvent OnInitialization;
        [Tooltip("Called when an error occured. More details can be generated with the error events of banner, interstitial or rewardBasedVideos.")]
        public mDescribedEvent OnError;
    }
    public C_GeneralEvents generalEvents;

    //Banner structures and events
    [System.Serializable]
    public enum E_AdSize
    {
        BANNER = 0,
        LARGE_BANNER = 1,
        MEDIUM_RECTANGLE = 2,
        FULL_BANNER = 3,
        LEADERBOARD = 4,
        SMART_BANNER = 5
    }

    [System.Serializable]
    public class C_BannerConfig
    {
#if USE_PLAY_ADMOB
        [Tooltip("https://developers.google.com/admob/unity/banner")]
        public E_AdSize adSize = E_AdSize.SMART_BANNER;
        [Tooltip("https://developers.google.com/admob/unity/banner")]
        public AdPosition adPosition = AdPosition.Top;
#endif
        public bool requestAtStart = false;
        public bool retryOnLoadFail = false;
    }

    public C_BannerConfig bannerConfiguration;

    //Error Events apply to banner, interstitial and RewardBasedVideo
    [System.Serializable]
    public class C_Error_Events
    {
        [Tooltip("Called when an ad request failed to load.")]
        public mDescribedEvent OnAdFailesToLoad;

        [Tooltip("Something happened internally; for instance, an invalid response was received from the ad server.")]
        public mDescribedEvent OnINTERNAL_ERROR;
        [Tooltip("The ad request was invalid; for instance, the ad unit ID was incorrect.")]
        public mDescribedEvent OnINVALID_REQUEST;
        [Tooltip("The ad request was unsuccessful due to network connectivity.")]
        public mDescribedEvent OnNETWORK_ERROR;
        [Tooltip("The ad request was successful, but no ad was returned due to lack of ad inventory.")]
        public mDescribedEvent OnNO_FILL;
    }

    [System.Serializable]
    public class C_Events_Banner
    {
        [Tooltip("Called when an ad request has successfully loaded.")]
        public mDescribedEvent OnAdLoaded;
        [Tooltip("Called when an ad is clicked.")]
        public mDescribedEvent OnAdOpened;
        [Tooltip("This method is invoked after onAdOpened(), when a user click opens another app (such as the Google Play), backgrounding the current app.")]
        public mDescribedEvent OnAdLeftApplication;
        [Tooltip("When a user returns to the app after viewing an ad's destination URL, this method is invoked. Your app can use it to resume suspended activities or perform any other work necessary to make itself ready for interaction.")]
        public mDescribedEvent OnAdClosed;
        [Tooltip("Called when an ad request failed to load.")]
        public C_Error_Events errorEvents;
    }

    [Tooltip("https://developers.google.com/admob/unity/banner")]

    public C_Events_Banner eventsBanner;

    //Interstitial structures and events
    [System.Serializable]
    public class C_InterstitialConfig
    {
        public bool requestAtStart = false;
        public bool retryOnLoadFail = false;
    }
    public C_InterstitialConfig interstitialConfiguration;

    [System.Serializable]
    public class C_Events_Interstitial
    {
        [Tooltip("Called when an ad request has successfully loaded.")]
        public mDescribedEvent OnInterstitialLoaded;
        [Tooltip("This method is invoked when the ad is displayed, covering the device's screen.")]
        public mDescribedEvent OnInterstitialOpened;
        [Tooltip("This method is invoked when a user click opens another app (such as the Google Play), backgrounding the current app.")]
        public mDescribedEvent OnInterstitialLeftApplication;
        [Tooltip("This method is invoked when when the interstitial ad is closed due to the user tapping on the close icon or using the back button. If your app paused its audio output or game loop, this is a great place to resume it.")]
        public mDescribedEvent OnInterstitialClosed;
        [Tooltip("Called when an ad request failed to load.")]
        public C_Error_Events errorEvents;
    }

    [Tooltip("https://developers.google.com/admob/unity/interstitial")]
    public C_Events_Interstitial eventsInterstitial;

    //rewardBasedVideo structures and events
    [System.Serializable]
    public class C_RewardBasedVideoConfig
    {
        public bool requestAtStart = false;
        public bool retryOnLoadFail = false;
        public bool loadNextAfterClosed = false;
    }
    public C_RewardBasedVideoConfig rewardBasedVideoConfiguration;


    [System.Serializable]
    public class C_Events_RewardBasedVideo
    {
        [Tooltip("Called when an ad request has successfully loaded.")]
        public mDescribedEvent HandleRewardBasedVideoLoaded;
        [Tooltip("Called when an ad is shown.")]
        public mDescribedEvent OnRewardBasedVideoOpened;
        [Tooltip("Called when the ad starts to play.")]
        public mDescribedEvent OnRewardBasedVideoStarted;
        [Tooltip("Called when the ad is closed.")]
        public mDescribedEvent OnRewardBasedVideoClosed;
        [Tooltip("Called when the user should be rewarded for watching a video.")]
        public mDescribedEvent OnRewardBasedVideoRewarded;
        [Tooltip("Called when the ad click caused the user to leave the application.")]
        public mDescribedEvent OnRewardBasedVideoLeftApplication;
        [Tooltip("Called when an ad request failed to load.")]
        public C_Error_Events errorEvents;
    }

    [Tooltip("https://developers.google.com/admob/unity/rewarded-video")]
    public C_Events_RewardBasedVideo eventsRewardBasedVideo;

    #endregion


    /// <summary>
    /// Set the birthday with 'SetBirthday(...)' for the target audience. 
    /// To take affect before the first ad is requested, set the birthday in "Awake()".
    /// </summary>
    /// <param name="enable"></param>
    /// <param name="day"></param>
    /// <param name="month"></param>
    /// <param name="year"></param>
    public void SetBirthday(bool enable, int day, int month, int year)
    {
        targetAudience.birthday.enable = enable;
        targetAudience.birthday.day = day;
        targetAudience.birthday.month = month;
        targetAudience.birthday.year = year;
    }



    private string enabledKey = "adsEnabled";

    public void SetEnableAndSave(bool enable)
    {
        adsSaveEnabled = enable;
        SecurePlayerPrefs.SetBool(enabledKey, enable);
    }
    private bool GetSavedEnable()
    {
        if (SecurePlayerPrefs.HasKey(enabledKey))
        {
            return SecurePlayerPrefs.GetBool(enabledKey);
        }
        SetEnableAndSave(true);
        return true;
    }

#if USE_PLAY_ADMOB

    /// <summary>
    /// Set the gender with 'SetGender(...)' for the target audience. 
    /// To take affect before the first ad is requested, set the birthday in 'Awake()'.
    /// </summary>
    /// <param name="enable"></param>
    /// <param name="gender"></param>
    public void SetGender(bool enable, Gender gender)
    {
        targetAudience.genderConfig.enable = enable;
        targetAudience.genderConfig.gender = gender;
    }

    // Returns an ad request with custom ad targeting. Modified to be more dynamic and adjustable from editor.
    private AdRequest CreateAdRequest()
    {

        AdRequest.Builder adRequestBuilder = new AdRequest.Builder();

        if (testConfiguration.testDevicesEnabled == true)
        {
            adRequestBuilder.AddTestDevice(AdRequest.TestDeviceSimulator);
            foreach (string s in testConfiguration.testDevices)
            {
                adRequestBuilder.AddTestDevice(s);
            }
        }

        foreach (string s in keyWords)
        {
            adRequestBuilder.AddKeyword(s);
        }

        if (targetAudience.genderConfig.enable == true)
        {
            adRequestBuilder.SetGender(targetAudience.genderConfig.gender);
        }

        if (targetAudience.birthday.enable == true)
        {
            adRequestBuilder.SetBirthday(new DateTime(targetAudience.birthday.year, targetAudience.birthday.month, targetAudience.birthday.day));
        }


        if (targetAudience.TagForChildDirectedTreatment.enable == true)
        {
            switch (targetAudience.TagForChildDirectedTreatment.yesNo)
            {
                case E_YES_NO.yes:
                    adRequestBuilder.TagForChildDirectedTreatment(true);
                    break;
                case E_YES_NO.no:
                    adRequestBuilder.TagForChildDirectedTreatment(false);
                    break;
                default:
                    //don't add option
                    break;
            }
        }

        if (targetAudience.tag_for_under_age_of_consent.enable == true)
        {
            switch (targetAudience.tag_for_under_age_of_consent.yesNo)
            {
                case E_YES_NO.yes:
                    adRequestBuilder.AddExtra("tag_for_under_age_of_consent", "true");
                    break;
                case E_YES_NO.no:
                    adRequestBuilder.AddExtra("tag_for_under_age_of_consent", "false");
                    break;
                default:
                    //don't add option
                    break;
            }
        }

        if (targetAudience.max_ad_content_rating.enable == true)
        {
            adRequestBuilder.AddExtra("max_ad_content_rating", targetAudience.max_ad_content_rating.ToString());
            mPrint("max_ad_content_rating:'" + targetAudience.max_ad_content_rating.ToString() + "'.");
        }

        return adRequestBuilder.Build();
        /*return new AdRequest.Builder()
            .AddTestDevice(AdRequest.TestDeviceSimulator)
            .AddTestDevice("0123456789ABCDEF0123456789ABCDEF")
            .AddKeyword("game")
            .SetGender(Gender.Male)
            .SetBirthday(new DateTime(1985, 1, 1))
            .TagForChildDirectedTreatment(false)
            .AddExtra("color_bg", "9B30FF")
            .Build();*/
    }
#endif
    public void mPrint(string message)
    {

        outputMessages.Add(message);

        int end = outputMessages.Count;
        if (end > 20)
        {
            outputMessages = outputMessages.GetRange(1, 20);//reduce messages to max 20 entries. Remove old one.
        }

        if (testConfiguration.debugOutput != null)
        {
            testConfiguration.debugOutput.text = string.Join("\n", outputMessages.ToArray());
        }
    }

    public void Start()
    {
#if USE_PLAY_ADMOB
#if UNITY_ANDROID
        string appId = ids.appID_ANDROID;
#elif UNITY_IPHONE
        string appId = ids.appID_IPHONE;
#else
        string appId = "unexpected_platform";
#endif

        if (adsSaveEnabled == true)
        {

            MobileAds.SetiOSAppPauseOnBackground(true);

            // Initialize the Google Mobile Ads SDK.
            MobileAds.Initialize(appId);

            // Get singleton reward based video ad reference.
            this.rewardBasedVideo = RewardBasedVideoAd.Instance;

            // RewardBasedVideoAd is a singleton, so handlers should only be registered once.
            this.rewardBasedVideo.OnAdLoaded += this.HandleRewardBasedVideoLoaded;
            this.rewardBasedVideo.OnAdFailedToLoad += this.HandleRewardBasedVideoFailedToLoad;
            this.rewardBasedVideo.OnAdOpening += this.HandleRewardBasedVideoOpened;
            this.rewardBasedVideo.OnAdStarted += this.HandleRewardBasedVideoStarted;
            this.rewardBasedVideo.OnAdRewarded += this.HandleRewardBasedVideoRewarded;
            this.rewardBasedVideo.OnAdClosed += this.HandleRewardBasedVideoClosed;
            this.rewardBasedVideo.OnAdLeavingApplication += this.HandleRewardBasedVideoLeftApplication;

            generalEvents.OnInitialization.Invoke();

            // enable the load of a banner/interstitial/rewardedVideo at the beginning of the scene
            if (bannerConfiguration.requestAtStart == true)
            {
                RequestBanner();
            }
            if (interstitialConfiguration.requestAtStart == true)
            {
                RequestInterstitial();
            }
            if (rewardBasedVideoConfiguration.requestAtStart == true)
            {
                RequestRewardBasedVideo();
            }
        }

#endif

    }

#if USE_PLAY_ADMOB
    //Some sizes are not predefined in "AdSize.cs". Do it here. 
    private AdSize getAdSize(E_AdSize size)
    {
        switch (size)
        {
            case E_AdSize.BANNER:
                return new AdSize(320, 50);
            case E_AdSize.LARGE_BANNER:
                return new AdSize(320, 100);
            case E_AdSize.MEDIUM_RECTANGLE:
                return new AdSize(300, 250);
            case E_AdSize.FULL_BANNER:
                return new AdSize(468, 60);
            case E_AdSize.LEADERBOARD:
                return new AdSize(728, 90);
            case E_AdSize.SMART_BANNER:
                return AdSize.SmartBanner;
            default:
                return AdSize.SmartBanner;
        }
    }
#endif
    public void RequestBanner()
    {
#if USE_PLAY_ADMOB
        mPrint("RequestBanner");

#if UNITY_EDITOR
        string adUnitId = "unused";
#elif UNITY_ANDROID
        string adUnitId = ids.banner_adUnitID_ANDROID;
#elif UNITY_IPHONE
        string adUnitId = ids.banner_adUnitID_IPHONE;
#else
        string adUnitId = "unexpected_platform";
#endif

        // Clean up banner ad before creating a new one.
        if (bannerView != null)
        {
            bannerView.Destroy();
        }

        // Create a xxx banner at the top of the screen.
        bannerView = new BannerView(adUnitId, getAdSize(bannerConfiguration.adSize), bannerConfiguration.adPosition);

        // Register for ad events.
        bannerView.OnAdLoaded += this.HandleAdLoaded;
        bannerView.OnAdFailedToLoad += this.HandleAdFailedToLoad;
        bannerView.OnAdOpening += this.HandleAdOpened;
        bannerView.OnAdClosed += this.HandleAdClosed;
        bannerView.OnAdLeavingApplication += this.HandleAdLeftApplication;

        if (adsSaveEnabled == true)
        {
            // Load a banner ad.
            bannerView.LoadAd(this.CreateAdRequest());
        }
#else
        showNotEnabledWarning();
#endif
    }

    private void OnDestroy()
    {
        DestroyBanner();
    }

    private void DestroyBanner()
    {
#if USE_PLAY_ADMOB
        
        mPrint("DestroyBanner");
        if (bannerView != null)
        {
            bannerView.Destroy();
        }
#else
        showNotEnabledWarning();
#endif
    }

    public void RequestInterstitial()
    {
#if USE_PLAY_ADMOB
        mPrint("RequestInterstitial");
#if UNITY_EDITOR
        string adUnitId = "unused";
#elif UNITY_ANDROID
        string adUnitId = ids.Interstitial_adUnitID_ANDROID;
#elif UNITY_IPHONE
        string adUnitId = ids.Interstitial_adUnitID_IPHONE;
#else
        string adUnitId = "unexpected_platform";
#endif

        // Clean up interstitial ad before creating a new one.
        if (this.interstitial != null)
        {
            this.interstitial.Destroy();
        }

        // Create an interstitial.
        this.interstitial = new InterstitialAd(adUnitId);

        // Register for ad events.
        this.interstitial.OnAdLoaded += this.HandleInterstitialLoaded;
        this.interstitial.OnAdFailedToLoad += this.HandleInterstitialFailedToLoad;
        this.interstitial.OnAdOpening += this.HandleInterstitialOpened;
        this.interstitial.OnAdClosed += this.HandleInterstitialClosed;
        this.interstitial.OnAdLeavingApplication += this.HandleInterstitialLeftApplication;

        if (adsSaveEnabled == true)
        {
            // Load an interstitial ad.
            this.interstitial.LoadAd(this.CreateAdRequest());
        }
#else
        showNotEnabledWarning();
#endif
    }

    public void RequestRewardBasedVideo()
    {
#if USE_PLAY_ADMOB
        mPrint("RequestRewardBasedVideo");
#if UNITY_EDITOR
        string adUnitId = "unused";
#elif UNITY_ANDROID
        string adUnitId = ids.RewardBasedVideo_adUnitID_ANDROID;
#elif UNITY_IPHONE
        string adUnitId = ids.RewardBasedVideo_adUnitID_IPHONE;
#else
        string adUnitId = "unexpected_platform";
#endif
        if (adsSaveEnabled == true)
        {
            this.rewardBasedVideo.LoadAd(this.CreateAdRequest(), adUnitId);
        }
#else
        showNotEnabledWarning();
#endif
    }

    public void ShowInterstitial()
    {
#if USE_PLAY_ADMOB
        mPrint("ShowInterstitial");
        if (this.interstitial.IsLoaded())
        {
            this.interstitial.Show();
        }
        else
        {
            mPrint("Interstitial is not ready yet");
        }
#else
        showNotEnabledWarning();
#endif
    }

    public void DestroyInterstitial()
    {
#if USE_PLAY_ADMOB
        mPrint("DestroyInterstitial");
        this.interstitial.Destroy();
#else
        showNotEnabledWarning();
#endif
    }

    public void ShowRewardBasedVideo()
    {
#if USE_PLAY_ADMOB
        mPrint("ShowRewardBasedVideo");
        if (this.rewardBasedVideo.IsLoaded())
        {
            this.rewardBasedVideo.Show();
        }
        else
        {
            mPrint("Reward based video ad is not ready yet");
        }
#else
        showNotEnabledWarning();
#endif
    }

    #region Banner callback handlers

    public void HandleAdLoaded(object sender, EventArgs args)
    {
        mPrint("HandleAdLoaded event received");
        eventsBanner.OnAdLoaded.Invoke();
    }
#if USE_PLAY_ADMOB
    public void HandleAdFailedToLoad(object sender, AdFailedToLoadEventArgs args)
    {

        mPrint("HandleFailedToReceiveAd event received with message: " + args.Message);

        generalEvents.OnError.Invoke();
        eventsBanner.errorEvents.OnAdFailesToLoad.Invoke();

        switch (args.Message)
        {
            case "ERROR_CODE_INTERNAL_ERROR":
                eventsBanner.errorEvents.OnINTERNAL_ERROR.Invoke();
                break;
            case "ERROR_CODE_INVALID_REQUEST":
                eventsBanner.errorEvents.OnINVALID_REQUEST.Invoke();
                break;
            case "ERROR_CODE_NETWORK_ERROR":
                eventsBanner.errorEvents.OnNETWORK_ERROR.Invoke();
                break;
            case "ERROR_CODE_NO_FILL":
                eventsBanner.errorEvents.OnNO_FILL.Invoke();
                break;
        }

        //retry to load if loading faileds (configurable)
        if (bannerConfiguration.retryOnLoadFail == true)
        {
            RequestBanner();
        }

    }
#endif
    public void HandleAdOpened(object sender, EventArgs args)
    {
        mPrint("HandleAdOpened event received");
        eventsBanner.OnAdOpened.Invoke();
    }

    public void HandleAdClosed(object sender, EventArgs args)
    {
        mPrint("HandleAdClosed event received");
        eventsBanner.OnAdClosed.Invoke();
    }

    public void HandleAdLeftApplication(object sender, EventArgs args)
    {
        mPrint("HandleAdLeftApplication event received");
        eventsBanner.OnAdLeftApplication.Invoke();
    }

    #endregion

    #region Interstitial callback handlers

    public void HandleInterstitialLoaded(object sender, EventArgs args)
    {
        mPrint("HandleInterstitialLoaded event received");
        eventsInterstitial.OnInterstitialLoaded.Invoke();
    }
#if USE_PLAY_ADMOB
    public void HandleInterstitialFailedToLoad(object sender, AdFailedToLoadEventArgs args)
    {
        mPrint("HandleInterstitialFailedToLoad event received with message: " + args.Message);

        generalEvents.OnError.Invoke();
        eventsInterstitial.errorEvents.OnAdFailesToLoad.Invoke();

        switch (args.Message)
        {
            case "ERROR_CODE_INTERNAL_ERROR":
                eventsInterstitial.errorEvents.OnINTERNAL_ERROR.Invoke();
                break;
            case "ERROR_CODE_INVALID_REQUEST":
                eventsInterstitial.errorEvents.OnINVALID_REQUEST.Invoke();
                break;
            case "ERROR_CODE_NETWORK_ERROR":
                eventsInterstitial.errorEvents.OnNETWORK_ERROR.Invoke();
                break;
            case "ERROR_CODE_NO_FILL":
                eventsInterstitial.errorEvents.OnNO_FILL.Invoke();
                break;
        }

        //retry to load if loading faileds (configurable)
        if (interstitialConfiguration.retryOnLoadFail == true)
        {
            RequestInterstitial();
        }
    }
#endif
    public void HandleInterstitialOpened(object sender, EventArgs args)
    {
        mPrint("HandleInterstitialOpened event received");
        eventsInterstitial.OnInterstitialOpened.Invoke();
    }

    public void HandleInterstitialClosed(object sender, EventArgs args)
    {
        mPrint("HandleInterstitialClosed event received");
        eventsInterstitial.OnInterstitialClosed.Invoke();
    }

    public void HandleInterstitialLeftApplication(object sender, EventArgs args)
    {
        mPrint("HandleInterstitialLeftApplication event received");
        eventsInterstitial.OnInterstitialLeftApplication.Invoke();
    }

    #endregion

    #region RewardBasedVideo callback handlers

    public void HandleRewardBasedVideoLoaded(object sender, EventArgs args)
    {
        mPrint("HandleRewardBasedVideoLoaded event received");
        eventsRewardBasedVideo.HandleRewardBasedVideoLoaded.Invoke();
    }
#if USE_PLAY_ADMOB
    public void HandleRewardBasedVideoFailedToLoad(object sender, AdFailedToLoadEventArgs args)
    {
        mPrint("HandleRewardBasedVideoFailedToLoad event received with message: " + args.Message);

        generalEvents.OnError.Invoke();
        eventsRewardBasedVideo.errorEvents.OnAdFailesToLoad.Invoke();

        switch (args.Message)
        {
            case "ERROR_CODE_INTERNAL_ERROR":
                eventsRewardBasedVideo.errorEvents.OnINTERNAL_ERROR.Invoke();
                break;
            case "ERROR_CODE_INVALID_REQUEST":
                eventsRewardBasedVideo.errorEvents.OnINVALID_REQUEST.Invoke();
                break;
            case "ERROR_CODE_NETWORK_ERROR":
                eventsRewardBasedVideo.errorEvents.OnNETWORK_ERROR.Invoke();
                break;
            case "ERROR_CODE_NO_FILL":
                eventsRewardBasedVideo.errorEvents.OnNO_FILL.Invoke();
                break;
        }

        //retry to load if loading faileds (configurable)
        if (rewardBasedVideoConfiguration.retryOnLoadFail == true)
        {
            RequestRewardBasedVideo();
        }
    }
#endif
    public void HandleRewardBasedVideoOpened(object sender, EventArgs args)
    {
        mPrint("HandleRewardBasedVideoOpened event received");
        eventsRewardBasedVideo.OnRewardBasedVideoOpened.Invoke();
    }

    public void HandleRewardBasedVideoStarted(object sender, EventArgs args)
    {
        mPrint("HandleRewardBasedVideoStarted event received");
        eventsRewardBasedVideo.OnRewardBasedVideoStarted.Invoke();
    }

    public void HandleRewardBasedVideoClosed(object sender, EventArgs args)
    {
        mPrint("HandleRewardBasedVideoClosed event received");
        eventsRewardBasedVideo.OnRewardBasedVideoClosed.Invoke();
        if (rewardBasedVideoConfiguration.loadNextAfterClosed == true)
        {
            RequestRewardBasedVideo();
        }
    }
#if USE_PLAY_ADMOB
    public void HandleRewardBasedVideoRewarded(object sender, Reward args)
    {
        string type = args.Type;
        double amount = args.Amount;
        mPrint("HandleRewardBasedVideoRewarded event received for " + amount.ToString() + " " + type);
        eventsRewardBasedVideo.OnRewardBasedVideoRewarded.Invoke();
    }
#endif
    public void HandleRewardBasedVideoLeftApplication(object sender, EventArgs args)
    {
        mPrint("HandleRewardBasedVideoLeftApplication event received");
        eventsRewardBasedVideo.OnRewardBasedVideoLeftApplication.Invoke();
    }

    #endregion
}

