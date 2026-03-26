using UnityEngine;

public class AudioManager : Singleton<AudioManager>
{
    [Header("Audio Sources")]

    [SerializeField] private AudioSource _bgmSource;

    public AudioSource BgmSource => _bgmSource;

    [SerializeField] private AudioSource _sfxSource;

    public AudioSource SfxSource =>_sfxSource;

    [Header("Audio Clips - SFX")]

    [SerializeField] private AudioClip _pickBlockClip;
    [SerializeField] private AudioClip _placeBlockClip;
    [SerializeField] private AudioClip _clearLineClip;
    [SerializeField] private AudioClip _comboClip;
    [SerializeField] private AudioClip _clickButtonClip;

    public void OnEnable()
    {
        EventBus.Instance.Subscribe<OnAddScore>(OnScoreCalculated);
        EventBus.Instance.Subscribe<OnSelection>(PlayClickSound);
        EventBus.Instance.Subscribe<OnPiecePlaced>(PlayDropBlockSound);
    }

    public void OnDisable()
    {
        EventBus.Instance.UnSubscribe<OnAddScore>(OnScoreCalculated);
        EventBus.Instance.UnSubscribe<OnSelection>(PlayClickSound);
        EventBus.Instance.UnSubscribe<OnPiecePlaced>(PlayDropBlockSound);
    }

    private void OnScoreCalculated(OnAddScore data)
    {
        if (data.AddedScore > 0)
        {
            // Nếu có Combo cao, phát tiếng nổ lớn!
            if (data.CurrentCombo >= 3)
            {
                PlaySFX(_comboClip);
            }
            else 
            {
                // Nếu chỉ ăn 1-2 hàng bình thường
                PlaySFX(_clearLineClip);
            }
        }
    }

    
    

    // --- HÀM LÕI PHÁT ÂM THANH ---

    public void PlaySFX(AudioClip clip, bool randomizePitch = false)
    {
        if (clip == null) return;

        // Random Pitch: Giúp tiếng đặt gạch lúc trầm lúc bổng, não bộ sẽ không bị chán
        if (randomizePitch)
        {
            _sfxSource.pitch = Random.Range(0.85f, 1.15f);
        }
        else
        {
            _sfxSource.pitch = 1f; 
        }
        _sfxSource.PlayOneShot(clip);
    }

    public void PlayBGM()
    {
        if(_bgmSource == null)
        {
            return;
        }
        _bgmSource.Play();
    }

    public void PlayClickSound(OnSelection onSelection)
    {
        PlaySFX(_clickButtonClip);
    }

    public void PlayDropBlockSound(OnPiecePlaced onPiecePlaced)
    {
        PlaySFX(_placeBlockClip, true);
    }

    public void PlayPickBlockSound()
    {
        PlaySFX(_pickBlockClip, true);
    }
}
