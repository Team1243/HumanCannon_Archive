using UnityEngine;

public class SoundManager : MonoSingleton<SoundManager>, IDataObserver
{
    private bool isMute = false;
    public bool IsMute => isMute;

    public override void Init()
    {
        if (isMute)
        {
            AudioListener.volume = 0;
        }
        else
        {
            AudioListener.volume = 1;
        }
    }

    public void Play(AudioClip clip, bool isWithVariablePitch = false)
    {
        PoolableSoundObject audioObj = PoolManager.Instance.Pop("SoundObject") as PoolableSoundObject;

        if (isWithVariablePitch)
        {
            audioObj.PlayClipwithVariablePitch(clip);
        }
        else
        {
            audioObj.PlayClip(clip);
        }
    }

    /// <param name="isOn"> : Toggle Object Member Property
    public void VolumeChange(bool isOn)
    {
        if (isOn)
        {
            AudioListener.volume = 1;
        }
        else
        {
            AudioListener.volume = 0;
        }

        isMute = !isOn;
        SaveLoadManager.Instance.SaveData();
    }

    public void WriteData(ref SaveData data)
    {
        data.isMuted = isMute;
    }

    public void ReadData(SaveData data)
    {
        isMute = data.isMuted;
    }
}