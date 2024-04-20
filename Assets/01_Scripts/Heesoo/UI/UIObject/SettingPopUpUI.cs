using UnityEngine.UI;

public class SettingPopUpUI : PopUpUI
{
    private Toggle onOffToggle;

    protected override void Awake()
    {
        base.Awake();
        onOffToggle = transform.Find("Sound_Toggle").Find("OnOff_Toggle").GetComponent<Toggle>();
    }

	private void Start()
	{
        onOffToggle.isOn = !SoundManager.Instance.IsMute;
	}

	private void OnEnable()
    {
        onOffToggle.onValueChanged.AddListener(SoundManager.Instance.VolumeChange);
    }

    private void OnDisable()
    {
        onOffToggle.onValueChanged.RemoveListener(SoundManager.Instance.VolumeChange);
    }
}
