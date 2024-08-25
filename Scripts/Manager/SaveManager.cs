using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class SaveData
{
    public List<SavePointData> savePoints = new List<SavePointData>();
}

[System.Serializable]
public class SavePointData
{
    public int saveNum;
    public int sceneIndex;
    public int mapIndex;
    public List<int> collectedItems;
    public int currentDialogueID;
}

public class SaveManager
{
    private string GetKeyForSave()
    {
        return "SaveData";
    }

    public SaveData LoadGame()
    {
        string key = GetKeyForSave();

        if (PlayerPrefs.HasKey(key))
        {
            string json = PlayerPrefs.GetString(key);
            return JsonUtility.FromJson<SaveData>(json);
        }

        return new SaveData();
    }

    public List<int> GetSaveNum()
    {
        SaveData saveData = LoadGame();
        return saveData.savePoints.Select(sp => sp.saveNum).ToList();
    }

    public void SaveGame(int sceneIndex, int mapIndex, List<int> items, int dialogueID)
    {
        SaveData saveData = LoadGame();

        int newSaveNum = 0;

        if(sceneIndex == (int)SceneIndex.Stage && mapIndex == 0)
        {
            newSaveNum = 0;
        }
        else if(sceneIndex == (int)SceneIndex.Stage && mapIndex == 9)
        {
            newSaveNum = 1;
        }
        else
        {
            newSaveNum = 2;
        }
        //if (saveData.savePoints.Count > 0)
        //{
        //    newSaveNum = saveData.savePoints.Max(sp => sp.saveNum) + 1;
        //}

        SavePointData newSavePoint = new SavePointData
        {
            saveNum = newSaveNum,
            sceneIndex = sceneIndex,
            mapIndex = mapIndex,
            collectedItems = items,
            currentDialogueID = dialogueID
        };
        saveData.savePoints.Add(newSavePoint);

        string key = GetKeyForSave();
        string json = JsonUtility.ToJson(saveData);
        PlayerPrefs.SetString(key, json);
        PlayerPrefs.Save();
    }

}
