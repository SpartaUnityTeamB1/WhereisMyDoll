using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.IO;
using System.Text;

public class DataManager
{
    public Dictionary<SceneIndex, DialogueSO> DialogueSOs { get; private set; }

#if UNITY_EDITOR
    private List<Dictionary<string, object>> csvDatas;

    private StringBuilder sb;
    private string fileName;
    private string tempString;
#endif

#if UNITY_EDITOR
    public void MakeDialogueSO()
    {
        sb = new StringBuilder();

        DirectoryInfo di = new DirectoryInfo("Assets/Resources/Dialogues");

        // int temp;

        foreach (FileInfo file in di.GetFiles("*.csv"))
        {
            DialogueSO dialogueSO = ScriptableObject.CreateInstance<DialogueSO>();

            dialogueSO.dialogue = new List<Dialogue>();

            fileName = Path.GetFileNameWithoutExtension(file.Name);

            sb.Clear();

            sb.Append("Dialogues/");
            sb.Append(fileName);

            csvDatas = CSVImporter.Read(sb.ToString());

            for (int i = 0; i < csvDatas.Count; ++i)
            {
                if ("" != csvDatas[i]["ID"].ToString())
                {
                    Dialogue newDialogue = new Dialogue();

                    newDialogue.SpeakerType = new List<int>();
                    newDialogue.SpeakerId = new List<int>();
                    newDialogue.Texts = new List<string>();
                    newDialogue.SFXIndex = new List<int>();
                    newDialogue.IconIndex = new List<int>();

                    newDialogue.Id = (int)csvDatas[i]["ID"];
                    newDialogue.DialogueEvent = (int)csvDatas[i]["Event"];
                    newDialogue.NextId = (int)csvDatas[i]["NextID"];    

                    tempString = csvDatas[i]["Conversation"].ToString();
                    tempString = tempString.Replace("/", "\"");
                    newDialogue.Texts.Add(tempString);

                    newDialogue.SpeakerType.Add((int)csvDatas[i]["SpeakerType"]);
                    newDialogue.SpeakerId.Add((int)csvDatas[i]["SpeakerID"]);
                    newDialogue.SFXIndex.Add((int)csvDatas[i]["SFX"]);
                    newDialogue.IconIndex.Add((int)csvDatas[i]["IconIndex"]);

                    dialogueSO.dialogue.Add(newDialogue);
                }
                else
                {
                    tempString = csvDatas[i]["Conversation"].ToString();
                    tempString = tempString.Replace("/", "\"");

                    dialogueSO.dialogue[dialogueSO.dialogue.Count - 1].Texts.Add(tempString);

                    dialogueSO.dialogue[dialogueSO.dialogue.Count - 1].IconIndex.Add((int)csvDatas[i]["IconIndex"]);

                    dialogueSO.dialogue[dialogueSO.dialogue.Count - 1].SpeakerType.Add((int)csvDatas[i]["SpeakerType"]);
                    dialogueSO.dialogue[dialogueSO.dialogue.Count - 1].SpeakerId.Add((int)csvDatas[i]["SpeakerID"]);            
                    dialogueSO.dialogue[dialogueSO.dialogue.Count - 1].SFXIndex.Add((int)csvDatas[i]["SFX"]);
                }
            }

            AssetDatabase.CreateAsset(dialogueSO, $"Assets/Resources/{fileName}.asset");
            AssetDatabase.SaveAssets();
        }
    }
#endif
   
    public void InitializeDialogues()
    {
        DialogueSOs = new Dictionary<SceneIndex, DialogueSO>();

        for (var i = SceneIndex.Tutorial; i < SceneIndex.End; ++i)
            DialogueSOs.Add(i, Resources.Load<DialogueSO>($"Dialogues/{i.ToString()}"));
    }
}