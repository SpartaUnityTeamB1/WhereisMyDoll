using UnityEngine;

[CreateAssetMenu(fileName = "QuestSO", menuName = "ScriptableObject/Quest")]
public class QuestSO : ScriptableObject
{
    public string questName;
    public string questDesc;
}