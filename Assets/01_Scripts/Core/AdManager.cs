using System.Net.NetworkInformation;
using GoogleMobileAds.Api;
using UnityEngine;
using System;

public class AdManager : MonoSingleton<AdManager>
{
    private AppOpenAd openAd;
    private BannerView bannerAd;
    private InterstitialAd frontAd;

    // private readonly string appId = "ca-app-pub-5714181718235393~2504844371;
    private readonly string openAdId = "ca-app-pub-5714181718235393/6859587007";
    private readonly string frontAdId = "ca-app-pub-5714181718235393/3690498187";
    private readonly string bannerAdId = "ca-app-pub-5714181718235393/9057997637";

    private DateTime loadTime;

    public override void Init()
    {
        if (IsNetworkAvailable())
        {
            // 모바일 광고 SDK를 초기화함. + 각 광고단위들 로드
            MobileAds.Initialize(initStatus => { });

            // LoadOpenAd();
            LoadFrontAd();
            LoadBannerAd();
        }
        else
        {
            Debug.Log("인터넷이 연결되지 않았습니다.");
        }

        // 게임 오버 -> 전면광고 재생 이벤트 구독
        GameEventSystem.Instance.Subscribe(this, GameEventType.Over, ShowFrontAd);
    }

    private void OnDisable()
    {
        // 게임 오버 -> 전면광고 재생 이벤트 구독 해제
        GameEventSystem.Instance.Unsubscribe(this, GameEventType.Over, ShowFrontAd);
    }

    private AdRequest GetAdRequest()
    {
        return new AdRequest.Builder().Build();
    }

    #region OpenAd

    private void LoadOpenAd()
    {
        AdRequest adRequest = GetAdRequest();

        AppOpenAd.LoadAd(openAdId, ScreenOrientation.Portrait, adRequest, (ad, error) =>
        {
            if (error != null)
            {
                Debug.LogFormat("Failed to load the ad. (reason: {0})", error.LoadAdError.GetMessage());
                return;
            }

            // 로드된 광고를 넣어주고, 시간을 저장한다.
            openAd = ad;
            loadTime = DateTime.Now;
        });
    }

    private void ShowOpenAd()
    {
        // 광고를 로드한지 4시간이 지나 렌더링된 광고는 더 이상 유효하지 않게 된다. 
        // 따라서 광고를 로드한 시점과 렌더링하려는 시점까지의 경과 시간이 4시간 이하인지 확인해야 한다.
        float passedHour = DateTime.Now.Hour - loadTime.Hour;
        if (openAd != null && passedHour < 4)
        {
            openAd.Show();
            LoadOpenAd();
        }
    }

    #endregion

    #region FrontAd

    private void LoadFrontAd()
    {
        AdRequest adRequst = GetAdRequest();

        frontAd = new InterstitialAd(frontAdId);
        frontAd.LoadAd(adRequst);

        frontAd.OnAdClosed += OnAdClosedHandle;
    }
    
    public void ShowFrontAd()
    {
        if (frontAd.IsLoaded())
        {
            frontAd.Show();
            LoadFrontAd();
        }
    }

    public void OnAdClosedHandle(object sender, EventArgs args)
    {
        GameManager.Instance.ChangeGameState(GameEventType.Menu);
    }

    #endregion

    #region BannerAd

    private void LoadBannerAd()
    {
        if (bannerAd != null)
        {
            bannerAd.Destroy();
        }

        AdRequest adRequst = GetAdRequest();
        AdSize adSize = AdSize.GetCurrentOrientationAnchoredAdaptiveBannerAdSizeWithWidth(AdSize.FullWidth);

        bannerAd = new BannerView(bannerAdId, adSize, AdPosition.Bottom);
        bannerAd.LoadAd(adRequst);
    }

    public void ShowBannerAd()
    {
        bannerAd?.Show();
    }

    public void HideBannerAd()
    {
        bannerAd.Hide();
    }

    #endregion

    private bool IsNetworkAvailable()
    {
        return NetworkInterface.GetIsNetworkAvailable();
    }
}
