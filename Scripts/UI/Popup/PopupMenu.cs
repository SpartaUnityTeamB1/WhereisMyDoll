using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class PopupMenu : PopupUI
{
    [SerializeField] private TMP_Dropdown resolutionsDropdown;
    [SerializeField] private ToggleGroup toggleGroup;
    [SerializeField] private GameObject[] toggleOn;
    [SerializeField] private GameObject[] toggleOff;
    [SerializeField] private GameObject reasoningMenu;
    [SerializeField] private GameObject settingMenu;

    [SerializeField] private TextMeshProUGUI characterTxt;
    [SerializeField] private TextMeshProUGUI itemTxt;

    [SerializeField] private Scrollbar bgmScrollBar;
    [SerializeField] private Scrollbar sfxScrollBar;

    [SerializeField] private AudioClip toggleClip;

    [SerializeField] private GameObject edwardImg;

    [SerializeField] private List<GameObject> itemList;

    //private Dictionary<string, GameObject> itemDic = new Dictionary<string, GameObject>();
    //private Image img;

    private List<Resolution> resolutions;

    private Toggle[] toggles;

    private void Start()
    {
        InitResolutions();

        InitScrollBar();

        //AddItemDic();
        //UpdateItemList();

        toggles = toggleGroup.GetComponentsInChildren<Toggle>();
    }

    private void OnEnable()
    {
        InitMenu();
        //UpdateItemList();
    }

    //private void AddItemDic()
    //{
    //    foreach(var item in itemList)
    //    {
    //        img = item.GetComponent<Image>();
    //        itemDic.Add(img.sprite.name, item);
    //    }
    //}

    //private void UpdateItemList()
    //{
    //    foreach( var item in itemDic)
    //    {
    //        foreach (var item2 in GameManager.Instance.FindItem)
    //        {
    //            if(item.Key == item2)
    //            {
    //                item.Value.transform.GetChild(0).gameObject.SetActive(true);
    //            }
    //        }
    //    }
    //}

    public void FindItem(int index)
    {
        itemList[index].SetActive(true);
    }
    
    public void ClearItem()
    {
        foreach (var item in itemList)
            item.SetActive(false);
    }

    private void InitResolutions()
    {
        resolutionsDropdown.options.Clear();

        resolutions = GameManager.Instance.GetResolutions();
        if (null == resolutions)
            Debug.LogAssertion("Failed load Resolutions");

        int optionNum = 0;

        foreach (Resolution item in resolutions)
        {
            TMP_Dropdown.OptionData option = new TMP_Dropdown.OptionData();
            option.text = $"{item.width} x {item.height} {item.refreshRateRatio} hz";
            resolutionsDropdown.options.Add(option);

            if (item.width == Screen.width && item.height == Screen.height)
                resolutionsDropdown.value = optionNum;

            ++optionNum;
        }

        resolutionsDropdown.RefreshShownValue();
    }

    private void InitMenu()
    {
        int current = (int)GameManager.Instance.CurrentScene;
        switch (current)
        {
            case 0:
                edwardImg.SetActive(false);
                characterTxt.text = "인형 목록";
                itemTxt.text = "아이템 목록";
                break;
            case 1:
                edwardImg.SetActive(true);
                characterTxt.text = "과연 어떤 인형일까?";
                itemTxt.text = "수집한 아이템 목록";
                break;
            case 2:
                edwardImg.SetActive(false);
                characterTxt.text = "과연 어떤 인형일까?";
                itemTxt.text = "수집한 아이템 목록";
                break;
        }
    }

    private void InitScrollBar()
    {
        bgmScrollBar.value = GameManager.Instance.GetBGMVolume();
        sfxScrollBar.value = GameManager.Instance.GetSFXVolume();

        bgmScrollBar.onValueChanged.AddListener(OnChangeBGMVolume);
        sfxScrollBar.onValueChanged.AddListener(OnChangeSFXVolume);
    }

    public override void OnClickExit()
    {
        GameManager.Instance.IsOpenMenu = false;

        if (SceneIndex.Intro == GameManager.Instance.CurrentScene)
            GameManager.Instance.ChangeCursor(CursorIndex.Menu);
        else
            GameManager.Instance.ChangeCursor(CursorIndex.Camera);

        GameManager.Instance.HideUI<PopupMenu>();
    }

    public void OnChangeResolution(int index)
    {
        GameManager.Instance.ChangeResolution(index);
    }

    public void OnClickShaderOn()
    {
        GameManager.Instance.ClickShaderOn();
        GameManager.Instance.ShaderOn();
    }

    public void OnClickShaderOff()
    {
        GameManager.Instance.ClickShaderOff();
        GameManager.Instance.ShaderOff();
    }

    public void OnClickMenuToggle(bool isOn)
    {
        GameManager.Instance.PlaySFX(toggleClip);

        for (int i = 0; i < toggles.Length; ++i)
        {
            if (toggles[i].isOn)
            {
                toggleOff[i].SetActive(false);
                toggleOn[i].SetActive(true);

                if (0 == i)
                {
                    settingMenu.SetActive(false);
                    reasoningMenu.SetActive(true);
                }
                else
                {
                    settingMenu.SetActive(true);
                    reasoningMenu.SetActive(false);
                }
            }
            else
            {
                toggleOff[i].SetActive(true);
                toggleOn[i].SetActive(false);
            }
        }
    }

    public void OnChangeBGMVolume(float volume)
    {
        if (0.0005f > (volume - 0f))
            volume = 0.0001f;

        GameManager.Instance.SetBGMVolume(volume);
    }

    public void OnChangeSFXVolume(float volume)
    {
        if (0.0005f > (volume - 0f))
            volume = 0.0001f;

        GameManager.Instance.SetSFXVolume(volume);
    }
}