using UnityEngine;
using UnityEngine.UI;

public class AudioToggleUI : MonoBehaviour
{
    [SerializeField] private Image imageButtonSound;

    [SerializeField] private Sprite _iconSoundOff;

    [SerializeField] private Sprite _iconSoundOn;
    private bool isSoundOn;


    void Start()
    {
        isSoundOn = PlayerPrefs.GetInt("SOUND_STATE", 1) == 1;
    }
    public void ToggleSound()
    {
        isSoundOn = !isSoundOn;
        ApplySoundState();

        PlayerPrefs.SetInt("SOUND_STATE", isSoundOn ? 1:0);



    }

    public void ApplySoundState()
    {
        AudioManager.Instance.BgmSource.mute = !isSoundOn;

        AudioManager.Instance.SfxSource.mute = !isSoundOn;

        imageButtonSound.sprite = isSoundOn ? _iconSoundOn : _iconSoundOff;
    }
}
