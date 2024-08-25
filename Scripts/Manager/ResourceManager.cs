using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class MapList
{
    private Dictionary<string, GameObject> maps = new Dictionary<string, GameObject>();

    public GameObject GetMap(string name)
    {
        return maps[name];
    }

    public void Clear()
    {
        maps.Clear();
    }

    public void AddMap(MapType type)
    {
        GameObject[] mapPrefabs = Resources.LoadAll<GameObject>($"Maps/{type.ToString()}");

        for (int i = 0; i < mapPrefabs.Length; ++i)
        {
            GameObject map = GameManager.Instance.InstantiateObject(mapPrefabs[i]);
            map.SetActive(false);

            maps.Add(mapPrefabs[i].name, map);
        }
    }
}

public class ResourceManager
{
    private Dictionary<MapType, MapList> Maps = new Dictionary<MapType, MapList>();

    private Dictionary<string, BaseUI> uiDictionary = new Dictionary<string, BaseUI>();

    private Dictionary<string, Sprite> spriteDicionary = new Dictionary<string, Sprite>();

    private Dictionary<string, AudioClip> soundDictionary = new Dictionary<string, AudioClip>();

    private BaseUI ui;
    private Sprite[] sprite;
    private AudioClip clip;

    public void Clear()
    {
        if (0 != Maps.Count)
        {
            foreach (var map in Maps)
                map.Value.Clear();

            Maps.Clear();
        }

        if (0 != uiDictionary.Count)
        {
            foreach (var ui in uiDictionary.Values)
            {
                ui.gameObject.SetActive(false);
                ui.gameObject.transform.parent = GameManager.Instance.transform;
            }
        }

        if (0 != spriteDicionary.Count)
            spriteDicionary.Clear();

        if (0 != soundDictionary.Count)
            soundDictionary.Clear();
    }

    public BaseUI GetUIInDic(string uiName)
    {
        return uiDictionary[uiName];
    }

    public Sprite GetSpriteInDic(string spriteName)
    {
        return spriteDicionary[spriteName];
    }

    public GameObject GetMap(MapType type, string mapName)
    {
        return Maps[type].GetMap(mapName);
    }

    public AudioClip GetClipInDic(string clipName)
    {
        return soundDictionary[clipName];
    }

    public void AddUIInDic(UI type, string uiName)
    {
        ui = GameManager.Instance.InstantiateObject(Resources.Load<BaseUI>($"UIs/{type.ToString()}/{uiName}"));
        uiDictionary.Add(uiName, ui);

        ui.gameObject.SetActive(false);
    }

    public void AddSpriteInDic(SpriteType type, string spriteName, int index)
    {
        sprite = Resources.LoadAll<Sprite>($"Sprites/{type.ToString()}/{spriteName}");
        spriteDicionary.Add($"{spriteName}_{index}", sprite[index]);
    }

    public void AddMapInDic(MapType type)
    {
        Maps.Add(type, new MapList());

        Maps[type].AddMap(type);
    }

    public void AddClipInDic(AudioClipType type, string clipName)
    {
        clip = Resources.Load<AudioClip>($"Sounds/{type.ToString()}/{clipName}");
        soundDictionary.Add(clipName, clip);
    }

    public bool CheckUIInDic(string uiName)
    {
        return uiDictionary.ContainsKey(uiName);
    }

    public bool CheckSpriteInDic(string spriteName)
    {
        return spriteDicionary.ContainsKey(spriteName);
    }

    public bool CheckMapInDic(MapType type)
    {
        return Maps.ContainsKey(type);
    }

    public bool CheckClipInDic(string clipName)
    {
        return soundDictionary.ContainsKey(clipName);
    }
}