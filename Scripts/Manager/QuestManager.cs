using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuestManager : Singleton<QuestManager>
{
    [SerializeField] private GameObject questCanvas;
    [SerializeField] private Text questText;

    public List<Quest> ActiveQuests { get; private set; }
    public List<Quest> FinishQuests { get; private set; }

    private Quest newQuest;

    protected override void Awake()
    {
        base.Awake();

        ActiveQuests = new List<Quest>();
        FinishQuests = new List<Quest>();
    }

    public void AddQuest(QuestType type, string[] goalKeys = null)
    {
        switch (type)
        {
            case QuestType.KeyInput:
                newQuest = new InputQuest(type, goalKeys);
                break;
            default:
                newQuest = new Quest(type);
                break;
        }

        ActiveQuests.Add(newQuest);
    }

    public void FinishQuest(Quest quest)
    {
        quest.FinishQuest();

        ActiveQuests.Remove(quest);
        FinishQuests.Add(quest);
    }
}