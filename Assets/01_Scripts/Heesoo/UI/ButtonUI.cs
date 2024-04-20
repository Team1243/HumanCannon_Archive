using UnityEngine.EventSystems;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine;
using DG.Tweening;

public class ButtonUI : MonoBehaviour, IUIComponent, IPointerEnterHandler, IPointerExitHandler
{
    protected Button myButtonComponent;

    [SerializeField] private float scaleIncreaseValue;
    [SerializeField] private float hoverAnimDuration;

    private Vector3 initScale;
    private Vector3 targetScale;

    protected virtual void Awake()
    {
        myButtonComponent = GetComponent<Button>();
        initScale = transform.localScale;
        targetScale = initScale * scaleIncreaseValue;
    }

    public void SetClickEvent(UnityAction action)
    {
        myButtonComponent.onClick.AddListener(action);
    }

    public void RemoveAllClickEvent()
    {
        myButtonComponent.onClick.RemoveAllListeners();
    }

    public virtual void Show()
    {
        
    }

    public virtual void Hide()
    {

    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        transform.DOScale(targetScale, hoverAnimDuration);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        transform.DOScale(initScale, hoverAnimDuration);
    }
}
