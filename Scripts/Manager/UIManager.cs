public class UIManager
{
    private int curSortOrder = 0;
    private string uiName;

    private BaseUI ui;

    public void ShowUI<T>(UI type)
    {
        uiName = typeof(T).Name;

        if (!GameManager.Instance.CheckUIInDic(uiName))
            GameManager.Instance.AddUI(type, uiName);

        ui = GameManager.Instance.GetUIResource(uiName);

        ui.SetOrder(curSortOrder++);

        ui.gameObject.SetActive(true);
    }

    public void HideUI<T>()
    {
        uiName = typeof(T).Name;

        ui = GameManager.Instance.GetUIResource(uiName);

        ui.gameObject.SetActive(false);

        --curSortOrder;
    }

    public BaseUI GetUIByType<T>()
    {
        uiName = typeof(T).Name;

        if (!GameManager.Instance.CheckUIInDic(uiName))
            return null;

        return GameManager.Instance.GetUIResource(uiName);
    }
}