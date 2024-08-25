using UnityEngine;
using UnityEngine.SceneManagement;

public class IntroSceneUI : SceneUI
{
    public void OnClickStart()
    {
        GameManager.Instance.Clear();
        SceneManager.LoadScene((int)GameManager.Instance.GetNextScene());
    }

    public void OnClickQuit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    public void OnClickMenu()
    {
        if (!GameManager.Instance.IsOpenMenu)
        {
            GameManager.Instance.IsOpenMenu = true;

            GameManager.Instance.ShowUI<PopupMenu>(UI.Popup);
        }
        else
        {
            GameManager.Instance.IsOpenMenu = false;

            GameManager.Instance.HideUI<PopupMenu>();
        }
    }   
    
    public void OnClickLoadMenu()
    {
        if (!GameManager.Instance.IsOpenLoadMenu)
        {
            GameManager.Instance.IsOpenLoadMenu = true;

            GameManager.Instance.ShowUI<PopupLoadMenu>(UI.Popup);
        }
        else
        {
            GameManager.Instance.IsOpenLoadMenu = false;

            GameManager.Instance.HideUI<PopupLoadMenu>();
        }
    }
}