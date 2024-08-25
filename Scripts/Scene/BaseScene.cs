using UnityEngine;
using UnityEngine.SceneManagement;

public class BaseScene : MonoBehaviour
{
    [field: SerializeField] public SceneIndex NextScene { get; private set; }
    [field: SerializeField] public SceneIndex PrevScene { get; private set; }

    [field: SerializeField] protected AudioClip bgmClip;
    [field: SerializeField] protected AudioClip sfxClip;
    [field: SerializeField] protected AudioClip plusSfxClip;

    [field: SerializeField] protected CursorIndex cursorIndex;

    protected virtual void Start()
    {
        GameManager.Instance.SetScene(this);

        Cursor.lockState = CursorLockMode.Confined;
        GameManager.Instance.ChangeCursor(cursorIndex);
    }

    public virtual void GoToNextScene()
    {
        GameManager.Instance.Clear();
        SceneManager.LoadScene((int)NextScene);
    }

    public virtual void GoToPrevScene()
    {
        GameManager.Instance.Clear();
        SceneManager.LoadScene((int)PrevScene);
        GameManager.Instance.IsOnAnime = false;
    }

    public virtual int GetItemIndex()
    {
        return -1;
    }
}