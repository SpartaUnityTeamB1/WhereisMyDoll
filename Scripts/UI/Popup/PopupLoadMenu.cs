using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Linq;

public class PopupLoadMenu : PopupUI
{
    [SerializeField] private Button[] loadButtons;

    private void Start()
    {
        for (int i = 0; i < loadButtons.Length; i++)
        {
            int saveNum = i;
            loadButtons[i].onClick.AddListener(() => OnLoadButtonClicked(saveNum));
            UpdateButtonState(saveNum);
        }
    }

    private void OnLoadButtonClicked(int saveNum)
    {
        GameManager.Instance.LoadGame(saveNum);
    }

    private void UpdateButtonState(int saveNum)
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        int currentMapIndex = (int)GameManager.Instance.CurrentMap;

        SaveData saveData = GameManager.Instance.LoadGame();

        bool hasSaveData = saveData != null && saveData.savePoints != null && saveData.savePoints.Any(sp => sp.saveNum == saveNum);

        loadButtons[saveNum].interactable = hasSaveData;
    }

    public override void OnClickExit()
    {
        GameManager.Instance.IsOpenLoadMenu = false;

        if (SceneIndex.Intro == GameManager.Instance.CurrentScene)
            GameManager.Instance.ChangeCursor(CursorIndex.Menu);
        else
            GameManager.Instance.ChangeCursor(CursorIndex.Camera);

        GameManager.Instance.HideUI<PopupLoadMenu>();
    }

}
