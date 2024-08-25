using System.Collections.Generic;
using UnityEngine;

public class SaveItem : MonoBehaviour
{
    private void Start()
    {
        List<int> list = GameManager.Instance.GetSaveNum();

        if(GameManager.Instance.CurrentScene == SceneIndex.Stage)
        {
            GameManager.Instance.Player.OnSave += SaveGame;

            if (GameManager.Instance.CurrentMap == StageIndex.Stage0 && list.Contains(0))
            {
                gameObject.SetActive(false);

            }

            if (GameManager.Instance.CurrentMap == StageIndex.Stage9 && list.Contains(1))
            {
                gameObject.SetActive(false);
            }
        }
        else
        {
            if (list.Contains(2))
            {
                gameObject.SetActive(false);
            }
        } 
    }

    public void SaveGame()
    {
        int currentMapIndex = (int)GameManager.Instance.CurrentMap;
        int currentSceneIndex = (int)GameManager.Instance.CurrentScene;
        List<int> collectedItems = new List<int>();

        if(currentSceneIndex < 3)
        {
            if (currentMapIndex == 9)
            {
                collectedItems = new List<int> { 6, 4, 1, 9, 0, 7, 3, 13, 14 };
            }

            int currentDialogueID;
            if ((int)EventType.CutScene == DialogueManager.Instance.dialogueSO.dialogue[DialogueManager.Instance.Id].DialogueEvent)
                currentDialogueID = DialogueManager.Instance.Id;
            else
                currentDialogueID = DialogueManager.Instance.Id - 1;

            GameManager.Instance.SaveGame(currentMapIndex, collectedItems, currentDialogueID);
        }
        else
        {
            GameManager.Instance.SaveGame(currentMapIndex, collectedItems, 0);
        }

        GameManager.Instance.Player.OnSave -= SaveGame;
    }
}