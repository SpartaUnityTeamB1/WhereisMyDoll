using UnityEngine;

public class BaseUI : MonoBehaviour
{
    private int sortOrder;

    public void SetOrder(int order)
    {
        sortOrder = order;
        GetComponent<Canvas>().sortingOrder = sortOrder;
    }
}