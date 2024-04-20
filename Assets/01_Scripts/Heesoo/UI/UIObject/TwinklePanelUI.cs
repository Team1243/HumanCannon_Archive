using DG.Tweening;
using UnityEngine;

public class TwinklePanelUI : MonoBehaviour, IUIComponent
{
    private Vector3 initScale;
    [SerializeField] private float scaleIncreaseValue = 1.1f;
    [SerializeField] private float roopDuration = 0.8f;

    private void Awake()
    {
        initScale = transform.localScale;
    }

    public virtual void Show()
    {
        gameObject.SetActive(true);

        var sequence = DOTween.Sequence();
        sequence.Append(transform.DOScale(initScale * scaleIncreaseValue, roopDuration)).SetLoops(-1, LoopType.Yoyo);
    }

    public virtual void Hide()
    {
        gameObject.SetActive(false);
        transform.localScale = initScale;
    }
    
}
