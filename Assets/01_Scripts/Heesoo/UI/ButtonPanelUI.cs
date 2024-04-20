using UnityEngine;
using DG.Tweening;

public class ButtonPanelUI : MonoBehaviour, IUIComponent
{
    protected RectTransform rectTrasnform;
    protected CanvasGroup canvasGroup;

    protected bool isActive;

    // Anim
    private Vector2 initPos;
    private Vector2 targetPos;
    [SerializeField] private Vector2 moveOffset;
    [SerializeField] private float moveAnimDuration = 0.5f;

    protected virtual void Awake()
    {
        rectTrasnform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();

        initPos = rectTrasnform.localPosition;
        targetPos = initPos + moveOffset;
    }

    [ContextMenu("Show")]
    public virtual void Show()
    {
        canvasGroup.alpha = 1.0f;

        rectTrasnform.DOAnchorPos(initPos, moveAnimDuration)
            .OnComplete(() =>
            {
                isActive = true;
                canvasGroup.blocksRaycasts = isActive;
                canvasGroup.interactable = isActive;
            });
    }

    [ContextMenu("Hide")]
    public virtual void Hide()
    {
        var sequence = DOTween.Sequence();

        rectTrasnform.DOAnchorPos(targetPos, moveAnimDuration)
             .OnComplete(() =>
             {
                 isActive = false;
                 canvasGroup.blocksRaycasts = isActive;
                 canvasGroup.interactable = isActive;

                 canvasGroup.alpha = 0f;
             });
    }
    
}
