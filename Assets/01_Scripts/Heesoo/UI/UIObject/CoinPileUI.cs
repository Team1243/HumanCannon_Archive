using UnityEngine;
using DG.Tweening;

public class CoinPileUI : MonoBehaviour, IUIComponent
{
    private RectTransform[] rectTransform;
    private Vector2[] initialPos;
    private Quaternion[] initialRotation;
    private int coinsAmount;

    [SerializeField] private Vector2 targetImagePosition;
    public float delayPerCoin;
    public float coinTranslateDelay;
    public float coinTranslateDuration;

    [SerializeField] private AudioClip coinEffectClip;

    private void Awake()
    {
        coinsAmount = transform.childCount;
        if (coinsAmount == 0)
        {
            coinsAmount = 10; 
        }

        rectTransform = new RectTransform[coinsAmount];
        initialPos = new Vector2[coinsAmount];
        initialRotation = new Quaternion[coinsAmount];

        for (int i = 0; i < coinsAmount; i++)
        {
            rectTransform[i] = transform.GetChild(i).gameObject.GetComponent<RectTransform>();
            initialPos[i] = rectTransform[i].anchoredPosition;
            initialRotation[i] = rectTransform[i].rotation;
        }

        ImageSetting(false);
        InitPositionAndRotation();
    }

    private void InitPositionAndRotation()
    {
        for (int i = 0; i < coinsAmount; i++)
        {
            rectTransform[i].anchoredPosition = initialPos[i];
            rectTransform[i].rotation = initialRotation[i];
        }
    }

    const float scaleTime = 0.3f;
    public float ScaleTime => scaleTime;
    private void CountCoins()
    {
        SoundManager.Instance.Play(coinEffectClip);

        float coinAppearDelay = delayPerCoin / rectTransform.Length;
        float delay = 0;

        for (int i = 0; i < coinsAmount; i++)
        {
            rectTransform[i].localScale = Vector3.zero;
            rectTransform[i].DOScale(1f, ScaleTime).SetDelay(delay).SetEase(Ease.OutBack);
            rectTransform[i].DOAnchorPos(targetImagePosition, coinTranslateDuration)
                .SetDelay(delay + ScaleTime + coinTranslateDelay).SetEase(Ease.InBack);
            rectTransform[i].DORotate(Vector3.zero, 0.5f).SetDelay(delay + 0.5f).SetEase(Ease.Flash);
            int j = i;
            rectTransform[i].DOScale(0f, ScaleTime)
                .SetDelay(delay + coinTranslateDuration + ScaleTime + coinTranslateDelay).SetEase(Ease.OutBack)
                .OnComplete(() => rectTransform[j].gameObject.SetActive(false));

            delay += delayPerCoin;
        }
    }

    public void Show()
    {
        ImageSetting(true);
        InitPositionAndRotation();
        CountCoins();
    }

    public void Hide()
    {
        ImageSetting(false);
    }

    private void ImageSetting(bool isShow)
    {
        for (int i = 0; i < coinsAmount; i++)
        {
            rectTransform[i].gameObject.SetActive(isShow);
        }
    }
}
