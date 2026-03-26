using UnityEngine;
namespace Solo.MOST_IN_ONE
{
    public class HapticManager : MonoBehaviour
    {
        private const float SelectionCooldown = 0.06f;
        private const float LightImpactCooldown = 0.08f;
        private const float MediumImpactCooldown = 0.12f;
        private const float HeavyImpactCooldown = 0.18f;

        public void OnEnable()
        {
            EventBus.Instance.Subscribe<OnAddScore>(OnLineCleared);
            EventBus.Instance.Subscribe<OnSelection>(OnSelection);

        }
            
        public void OnDisable()
        {
            EventBus.Instance.UnSubscribe<OnAddScore>(OnLineCleared);
            EventBus.Instance.UnSubscribe<OnSelection>(OnSelection);
        }


        private void OnLineCleared(OnAddScore onAddScore)
        {
            if (onAddScore.AddedScore == 0)
            {
                MOST_HapticFeedback.GenerateWithCooldown(
                    MOST_HapticFeedback.HapticTypes.LightImpact,
                    LightImpactCooldown);
                return;
            }

            if (onAddScore.AddedScore <= 300 && onAddScore.CurrentCombo < 3)
            {
                MOST_HapticFeedback.GenerateWithCooldown(
                    MOST_HapticFeedback.HapticTypes.MediumImpact,
                    MediumImpactCooldown);
                return;
            }

            if (onAddScore.AddedScore >= 600 &&onAddScore.CurrentCombo >= 3 )
            {
                MOST_HapticFeedback.GenerateWithCooldown(
                    MOST_HapticFeedback.HapticTypes.HeavyImpact,
                    HeavyImpactCooldown);
                return;
            }
        }

        private void OnSelection(OnSelection onSelection)
        {
            MOST_HapticFeedback.GenerateWithCooldown(
                MOST_HapticFeedback.HapticTypes.Selection,
                SelectionCooldown);
        }




    }
}