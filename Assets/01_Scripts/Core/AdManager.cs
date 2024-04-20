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
            // ����� ���� SDK�� �ʱ�ȭ��. + �� ��������� �ε�
            MobileAds.Initialize(initStatus => { });

            // LoadOpenAd();
            LoadFrontAd();
            LoadBannerAd();
        }
        else
        {
            Debug.Log("���ͳ��� ������� �ʾҽ��ϴ�.");
        }

        // ���� ���� -> ���鱤�� ��� �̺�Ʈ ����
        GameEventSystem.Instance.Subscribe(this, GameEventType.Over, ShowFrontAd);
    }

    private void OnDisable()
    {
        // ���� ���� -> ���鱤�� ��� �̺�Ʈ ���� ����
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

            // �ε�� ���� �־��ְ�, �ð��� �����Ѵ�.
            openAd = ad;
            loadTime = DateTime.Now;
        });
    }

    private void ShowOpenAd()
    {
        // ���� �ε����� 4�ð��� ���� �������� ����� �� �̻� ��ȿ���� �ʰ� �ȴ�. 
        // ���� ���� �ε��� ������ �������Ϸ��� ���������� ��� �ð��� 4�ð� �������� Ȯ���ؾ� �Ѵ�.
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
