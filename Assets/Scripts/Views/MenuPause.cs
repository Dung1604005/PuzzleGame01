using UnityEngine;

public class MenuPause : MonoBehaviour
{

    [SerializeField] private GameObject background;
    public void OnActive()
    {
        background.SetActive(true);
        this.gameObject.SetActive(true);
    }
    public void OnDeActive()
    {
        background.SetActive(false);
        this.gameObject.SetActive(false);
    }

    public void ReStartGame()
    {
        EventBus.Instance.Publish(new OnSelection
        {
            
        });
        GameManager.Instance.RestartGame();
        OnDeActive();

    }

    public void Continue()
    {
        EventBus.Instance.Publish(new OnSelection
        {
            
        });
        GameManager.Instance.ContinueGame();
        OnDeActive();
    }
}
