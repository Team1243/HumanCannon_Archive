using UnityEngine;
using DG.Tweening;

[RequireComponent(typeof(CanvasGroup))]
public class PopUpUI : PoolableMono, IUIComponent
{
    protected CanvasGroup canvasGroup;

    private bool isActive;

    protected virtual void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
    }

    public override void Init()
    {
        transform.localPosition = Vector3.zero;
        transform.localScale = Vector3.one * 0.1f;
    }

    public virtual void Show()
    {
        canvasGroup.alpha = 1.0f;

        var sequence = DOTween.Sequence();

        sequence.Append(transform.DOScale(1.1f, 0.2f));
        sequence.Append(transform.DOScale(1f, 0.1f));
        sequence.Play().OnComplete(() =>
        {
            isActive = true;
            canvasGroup.blocksRaycasts = isActive;
            canvasGroup.interactable = isActive;
        });
    }

    [ContextMenu("sdlf")]
    public virtual void Hide()
    {
        var sequence = DOTween.Sequence();

        sequence.Append(transform.DOScale(1.1f, 0.1f));
        sequence.Append(transform.DOScale(0.2f, 0.2f));
        sequence.Play().OnComplete(() =>
        {
            isActive = false;
            canvasGroup.blocksRaycasts = isActive;
            canvasGroup.interactable = isActive;
            canvasGroup.alpha = 0f;
            PoolManager.Instance.Push(this);
        });
    }
}
