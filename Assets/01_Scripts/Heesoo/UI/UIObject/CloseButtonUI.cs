public class CloseButtonUI : ButtonUI
{
    private PopUpUI parentPopUpUI;

    protected override void Awake()
    {
        base.Awake();
        parentPopUpUI = transform.parent.GetComponent<PopUpUI>();
    }

    private void OnEnable()
    {
        SetClickEvent(() => parentPopUpUI.Hide());
    }

    private void OnDisable()
    {
        RemoveAllClickEvent();
    }
}
