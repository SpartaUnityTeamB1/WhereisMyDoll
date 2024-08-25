public class InputQuest : Quest
{
    public string[] GoalKeys { get; private set; }

    public InputQuest(QuestType type, string[] goalKeys) : base(type)
    {
        GoalKeys = goalKeys;
    }

    public bool CheckClear(string key)
    {
        for (int i = 0; i < GoalKeys.Length; ++i)
        {
            if (key == GoalKeys[i])
                return true;
        }

        return false;
    }
}