using UnityEngine.UI;
using UnityEngine;
using DG.Tweening;
using TMPro;

public class TopInfoScreen : MonoBehaviour
{
    private TextMeshProUGUI coinCntText;
    private TextMeshProUGUI levelText;

    private CoinPileUI coinPileUI;

    private Image fillImage;

    private RectTransform sliderRectTrasnform;

    [SerializeField] private float textCountingDuration = 1f;
    [SerializeField] private float fillEffectDuration = 0.5f;

    private void Awake()
    {
        coinCntText = transform.GetChild(0).Find("CoinCount_Text").GetComponent<TextMeshProUGUI>();
        levelText = transform.Find("Level_Text").GetComponent<TextMeshProUGUI>();
        coinPileUI = transform.Find("CoinPile").GetComponent<CoinPileUI>(); 
        sliderRectTrasnform = transform.Find("ChargingPoint_Slider").GetComponent<RectTransform>();
        fillImage = transform.Find("ChargingPoint_Slider").GetChild(0).GetChild(0).GetComponent<Image>();
    }

    private void OnEnable()
    {
        GameEventSystem.Instance.Subscribe(this, GameEventType.Start, ShowSlider);
        GameEventSystem.Instance.Subscribe(this, GameEventType.Menu, () =>
        {
            HideSlider();
            BattleSystem.Instance.PlusChargingPoint(100);
        });

        UserDataManager.Instance.OnCoinChanged += SetCoinText;
        LevelManager.Instance.OnStageChanged += SetLevelText;
        BattleSystem.Instance.OnChargingPointChanged += SetFillImage;
    }

    private void OnApplicationQuit()
    {
        GameEventSystem.Instance.Unsubscribe(this, GameEventType.Start, ShowSlider);
        GameEventSystem.Instance.Unsubscribe(this, GameEventType.Menu, () =>
        {
            HideSlider();
            BattleSystem.Instance.PlusChargingPoint(100);
        });

        UserDataManager.Instance.OnCoinChanged -= SetCoinText;
        LevelManager.Instance.OnStageChanged -= SetLevelText;
        BattleSystem.Instance.OnChargingPointChanged -= SetFillImage;
    }

    public void SetCoinText(int coin)
    {
        int beforeCoinCnt = int.Parse(coinCntText.text);
        if (coin == beforeCoinCnt) return;
        int currentCoinCnt = 0;

        float delay = (beforeCoinCnt != 0 && beforeCoinCnt < coin) ? coinPileUI.coinTranslateDelay + coinPileUI.coinTranslateDuration + coinPileUI.ScaleTime : 0;

        // Text Count Effect
        DOVirtual.Float(beforeCoinCnt, coin, textCountingDuration, (num) =>
        {
            currentCoinCnt = (int)num;
            coinCntText.text = currentCoinCnt.ToString();
        }).SetDelay(delay);

        // Image Effect
        if (beforeCoinCnt != 0 && beforeCoinCnt < coin)
        {
            coinPileUI.Show();
        }
    }

    public void SetLevelText(int levelNum)
    {
        levelText.text = $"Level {levelNum}";
    }

    #region ChargingPoint_Slider

    public void SetFillImage(float fillAmount)
    {
        fillAmount *= 0.01f;
        float beforeFillAmount = fillImage.fillAmount;
        float currentFillAmount = 0;

        // fillAmount Lerp
        DOVirtual.Float(beforeFillAmount, fillAmount, textCountingDuration, (num) =>
        {
            currentFillAmount = num;
            fillImage.fillAmount = currentFillAmount;
        }).OnComplete(() => fillImage.fillAmount = currentFillAmount);
    }

    public void ShowSlider()
    {
        BattleSystem.Instance.PlusChargingPoint(30);
        sliderRectTrasnform.DOAnchorPosX(430, 0.5f);
    }

    public void HideSlider()
    {
        sliderRectTrasnform.DOAnchorPosX(650, 0.5f);
    }

    #endregion

}
