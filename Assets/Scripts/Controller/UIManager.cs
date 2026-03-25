
using UnityEngine;
using UnityEngine.UI;

public class UIManager : Singleton<UIManager>
{
    [SerializeField] private Image background;

    [SerializeField] private Image buttonIcon;

    


    public void OnEnable()
    {
        EventBus.Instance.Subscribe<OnChangeTheme>(OnUpdateUITheme);
    }
    public void OnDisable()
    {
        EventBus.Instance.UnSubscribe<OnChangeTheme>(OnUpdateUITheme);
    }

    public void OnUpdateUITheme(OnChangeTheme onChangeTheme)
    {
        background.color = GameManager.Instance.ThemeData.colorBackground;
        buttonIcon.sprite = GameManager.Instance.ThemeData.buttonIcon;
    }
}
