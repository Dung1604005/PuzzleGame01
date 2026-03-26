
using UnityEngine;
using UnityEngine.UI;

public class UIManager : Singleton<UIManager>
{
    [SerializeField] private Image background;

    [SerializeField] private Image buttonIcon;

    [SerializeField] private MenuPause menuPause;


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

    public void OpenMenuPause()
    {
        EventBus.Instance.Publish(new OnSelection
        {
            
        });
        GameManager.Instance.GameStateModel.TransitionTo(GameState.Paused);
        menuPause.OnActive();
    
    }


}


public struct OnSelection: IEvent
{
    
}