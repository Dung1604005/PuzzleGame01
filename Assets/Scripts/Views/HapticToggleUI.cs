using UnityEngine;
using UnityEngine.UI;
using Solo.MOST_IN_ONE;

public class HapticToggleUI : MonoBehaviour
{
    [SerializeField] private Image imageButtonHaptic;
    [SerializeField] private Sprite iconHapticOff;
    [SerializeField] private Sprite iconHapticOn;

    private bool isHapticOn;

    private void Start()
    {
        isHapticOn = MOST_HapticFeedback.HapticsEnabled;
        ApplyHapticState();
    }

    public void ToggleHaptic()
    {
        isHapticOn = !isHapticOn;
        ApplyHapticState();

        if (isHapticOn)
        {
            MOST_HapticFeedback.GenerateWithCooldown(
                MOST_HapticFeedback.HapticTypes.Selection,
                0.05f);
        }
    }

    public void ApplyHapticState()
    {
        MOST_HapticFeedback.HapticsEnabled = isHapticOn;

        if (imageButtonHaptic != null)
        {
            imageButtonHaptic.sprite = isHapticOn ? iconHapticOn : iconHapticOff;
        }
    }
}
