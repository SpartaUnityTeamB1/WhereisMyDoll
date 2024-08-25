public class Quest
{
    public bool IsActive { get; set; }
    public bool IsFinish { get; set; }

    public QuestType QuestType { get; private set; }

    public QuestSO QuestInfo { get; private set; }

    public Quest(QuestType type)
    {
        IsActive = true;
        IsFinish = false;
        QuestType = type;
    }

    public void FinishQuest()
    {
        IsActive = false;
        IsFinish = true;
    }
}
