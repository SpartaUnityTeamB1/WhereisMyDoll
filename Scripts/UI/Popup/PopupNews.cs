using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopupNews : PopupUI
{
    [SerializeField] private List<GameObject> NewsList;

    private int newsOrder;

    private void OnEnable()
    {
        if(GameManager.Instance.CurrentMap == StageIndex.Stage1)
        {
            newsOrder = (int)NewsType.GoodNews1;
            ShowNews();
        }
        else if (GameManager.Instance.CurrentMap == StageIndex.Stage5)
        {
            newsOrder = (int)NewsType.GoodNews2;
            ShowNews();
        }
        else if (GameManager.Instance.CurrentMap == StageIndex.Stage7)
        {
            newsOrder = (int)NewsType.BadNews1;
            ShowNews();
        }
        else if (GameManager.Instance.CurrentMap == StageIndex.Stage8)
        {
            newsOrder = (int)NewsType.Leaflet;
            ShowNews();
        }
        else if (GameManager.Instance.CurrentMap == StageIndex.Stage10)
        {
            newsOrder = (int)NewsType.BadNews2;
            ShowNews();
        }

        else if(GameManager.Instance.CurrentMap >= StageIndex.Stage11)
        {
            HideNews();
        }
    }

    private void ShowNews()
    {
        for(int i = 0; i< NewsList.Count; i++)
        {
            NewsList[i].SetActive(false);

            if (i == newsOrder)
                NewsList[i].SetActive(true);
        }
    }

    private void HideNews()
    {
        foreach(var news in NewsList)
        {
            news.SetActive(false);
        }
    }
    public override void OnClickExit()
    {
        GameManager.Instance.HideUI<PopupNews>();
    }


}
