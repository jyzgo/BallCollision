

using GoogleMobileAds.Api;
using MTUnity;
using System;
using UnityEngine;

public static class AdMgr  {
    #region Admob-------------------------

    const string BALLZ_ANDROID_INTER = "ca-app-pub-9160912623449043/2542028436";
    const string BALLZ_ANDROID_BANNER = "ca-app-pub-9160912623449043/6776733015";

    const string BALLZ_IOS_INTER = "ca-app-pub-9160912623449043/2386061869";
    const string BALLZ_IOS_BANNER = "ca-app-pub-9160912623449043/3699143530";
 



    public static void ShowAdmobInterstitial()
    {
        _interstitial.Show();
    }

    public static void PreloadAdmobInterstitial()
    {


#if UNITY_ANDROID
        string adUnitId = BALLZ_ANDROID_INTER;
#elif UNITY_IOS
        string adUnitId = BALLZ_IOS_INTER; 
#else
        string adUnitId = "unexpected_platform";
#endif

        // Initialize an InterstitialAd.
        _interstitial = new InterstitialAd(adUnitId);
        // Create an empty ad request.
        AdRequest request = new AdRequest.Builder().Build();
        _interstitial.OnAdLoaded += HandleOnLoaded;
        _interstitial.OnAdClosed += HandleOnClosed;
        _interstitial.OnAdOpening += HandleOnOpening;
        _interstitial.OnAdFailedToLoad += HandleOnFailedToLoad;
        // Load the interstitial with the request.
        _interstitial.LoadAd(request);

    }

    static void HandleOnOpening(object sender, EventArgs args)
    {
        TrackAdMob("0");
    }

    static void HandleOnClosed(object sender, EventArgs args)
    {
        TrackAdMob("1");
    }

    private static void HandleOnFailedToLoad(object sender, AdFailedToLoadEventArgs e)
    {
        TrackAdMob("2");
    }

    static void HandleOnLoaded(object sender, EventArgs args)
    {
        TrackAdMob("3");
    }

    


    static InterstitialAd _interstitial;

    public static void TrackAdMob(string idStr)
    {
        TrackAd(idStr, "admobMedi");
    }






    public static int downBannerCount = 0;
    public static void ShowDownAdmobBanner()
    {
        if (_downBannerView != null)
        {
            downBannerCount++;
            _downBannerView.Show();
        }
    }

    public static void HideDownAdmobBanner()
    {
        if (_downBannerView != null)
        {
            downBannerCount--;
            if (downBannerCount <= 0)
            {
                _downBannerView.Hide();
                downBannerCount = 0;

            }

        }
    }





    static void  InitBanner()
    {
        //#if UNITY_EDITOR
        //        string adUnitId = "unused";
#if UNITY_ANDROID
        string adUnitId = BALLZ_ANDROID_BANNER;
#elif UNITY_IOS

        string adUnitId = BALLZ_IOS_BANNER;
#else
        string adUnitId = "unexpected_platform";
#endif



        AdRequest downRequest = new AdRequest.Builder().Build();
        _downBannerView = new BannerView(adUnitId, AdSize.SmartBanner, AdPosition.Bottom);
        _downBannerView.LoadAd(downRequest);
        _downBannerView.Hide();





    }


    static BannerView _downBannerView;


    public static bool IsAdmobInterstitialReady()
    {
		if (_interstitial == null) 
		{
			return false;
		}
        return _interstitial.IsLoaded();
    }





#endregion Admob



    static void TrackAd(string idStr, string plat)
    {
       // MTTracker.Instance.Track(SoliTrack.ads, StatisticsMgr.current.WinsCount(), idStr, plat);
    }



    public static void RegisterAllAd()
    {
        InitBanner();
        //PreloadAdmobInterstitial();

    }








}
