using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private static GameManager instance;

    public static GameManager Instance
    {
        get
        {
            if (null == instance)
            {
                instance = (GameManager)FindObjectOfType(typeof(GameManager));

                if (null == instance)
                {
                    GameObject obj = new GameObject(typeof(GameManager).Name, typeof(GameManager));
                    instance = obj.GetComponent<GameManager>();
                }
            }

            return instance;
        }
    }

    public SceneIndex CurrentScene { get; set; }
    public StageIndex CurrentMap { get; set; } = StageIndex.STAGEEND;

    public ObjectPool ObjectPool { get; private set; }

    [SerializeField] private ScriptableRendererData rendererData;
    [SerializeField] private int queueCount = 0;
    [SerializeField] private List<Texture2D> cursorTextures = new List<Texture2D>();
    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private List<AudioMixerGroup> audioMixerGroups = new List<AudioMixerGroup>();
    [SerializeField] private Material pixelEffect;
    [SerializeField] private Material skybox;

    private float current;
    private float lerpTime = 1f;
    private float pixelRate;
    private float skyboxColor = 1;
    private List<int> itemList = new List<int>();
    private List<int> SaveitemList = new List<int>();


    private GameSceneManager sceneMgr;
    private ResourceManager resourceMgr;
    private GraphicManager graphicMgr;
    private SoundManager soundMgr;
    private DataManager dataMgr;
    private UIManager uiMgr;
    private LightManager lightMgr;
    private SaveManager saveMgr;

    public bool IsOpenMenu { get; set; } = false;
    public bool IsOnDialogue { get; set; } = false;
    public bool IsOnAnime { get; set; } = false;
    public bool TakeKnife { get; set; } = false;
    public bool IsShaderOn { get; private set; } = true;
    public bool IsOpenLoadMenu { get; set; } = false;
    public float SkyboxColor { get { return skyboxColor; } }
    public int DialogueSaveId { get; private set; } = -1;
    public bool IsLoadGame { get; set; } = false;

    public PlayerInput Player { get; set; }

    private void Awake()
    {
        if (null == instance)
            DontDestroyOnLoad(transform.root.gameObject);
        else
        {
            Destroy(transform.root.gameObject);
            return;
        }

        resourceMgr = new ResourceManager();
        graphicMgr = new GraphicManager();
        sceneMgr = new GameSceneManager();
        soundMgr = new SoundManager();
        dataMgr = new DataManager();
        uiMgr = new UIManager();
        lightMgr = new LightManager();
        saveMgr = new SaveManager();

        graphicMgr.InitializeGraphics(rendererData);
        soundMgr.InitializeSound(audioMixer, audioMixerGroups, queueCount);
        lightMgr.InitializeLight();

        ObjectPool = GetComponent<ObjectPool>();

        CurrentScene = SceneIndex.SCENEEND;

        pixelEffect.SetFloat("_PixelRate", 280);
        skybox.SetFloat("_Color", 1);
    }

    private void Start()
    {
        //dataMgr.MakeDialogueSO();
        dataMgr.InitializeDialogues();
        soundMgr.SettingVolumes();
        //PlayerPrefs.DeleteAll();
    }
    
    public T InstantiateObject<T>(T obj) where T : Object
    {
        return Instantiate(obj);
    }

    public Coroutine CoroutineStart(IEnumerator coroutine)
    {
        return StartCoroutine(coroutine);
    }

    public void CoroutineStop(Coroutine coroutine)
    {
        StopCoroutine(coroutine);
    }

    public bool CheckUIInDic(string uiName)
    {
        return resourceMgr.CheckUIInDic(uiName);
    }

    public bool CheckSpriteInDic(string spriteName)
    {
        return resourceMgr.CheckSpriteInDic(spriteName);
    }

    public bool CheckMapInDic(MapType type)
    {
        return resourceMgr.CheckMapInDic(type);
    }

    public bool CheckClipInDic(string clipName)
    {
        return resourceMgr.CheckClipInDic(clipName);
    }

    public void AddUI(UI type, string name)
    {
        resourceMgr.AddUIInDic(type, name);
    }

    public void AddSprite(SpriteType type, string name, int index = 0)
    {
        resourceMgr.AddSpriteInDic(type, name, index);
    }

    public void AddMap(MapType type)
    {
        resourceMgr.AddMapInDic(type);
    }

    public void AddClip(AudioClipType type, string name)
    {
        resourceMgr.AddClipInDic(type, name);
    }

    public BaseUI GetUIResource(string name)
    {
        return resourceMgr.GetUIInDic(name);
    }

    public Sprite GetSpriteResource(string name)
    {
        return resourceMgr.GetSpriteInDic(name);
    }

    public GameObject GetMapResource(MapType type, string name)
    {
        return resourceMgr.GetMap(type, name);
    }

    public AudioClip GetClipInDic(string name)
    {
        return resourceMgr.GetClipInDic(name);
    }

    public void ShowUI<T>(UI type)
    {
        uiMgr.ShowUI<T>(type);
    }

    public void HideUI<T>()
    {
        uiMgr.HideUI<T>();
    }

    public BaseUI GetUIByType<T>()
    {
        return uiMgr.GetUIByType<T>();
    }

    public void SetScene(BaseScene scene)
    {
        sceneMgr.SetScene(scene);
    }

    public SceneIndex GetNextScene()
    {
        return sceneMgr.CurrentScene.NextScene;
    }

    public void GoToNextStage()
    {
        StartCoroutine(LoadNextMap());
        Invoke(nameof(GoNextStage),2f);
        //sceneMgr.CurrentScene.GoToNextScene();
    }

    public void GoToPrevStage()
    {
        sceneMgr.CurrentScene.GoToPrevScene();
    }

    private void GoNextStage()
    {
        sceneMgr.CurrentScene.GoToNextScene();
    }

    public List<Resolution> GetResolutions()
    {
        return graphicMgr.Resolutions;
    }

    public void ChangeResolution(int index)
    {
        graphicMgr.ChangeResolution(index);
    }

    public void ShaderOn()
    {
        graphicMgr.ShaderOn();
    }

    public void ShaderOff()
    {
        graphicMgr.ShaderOff();
    }

    public void ShaderOffWithoutSave()
    {
        graphicMgr.ShaderOffWithoutSave();
    }

    public void ClickShaderOn()
    {
        IsShaderOn = true;
    }

    public void ClickShaderOff()
    {
        IsShaderOn = false;
    }

    public void Clear()
    {
        if (CheckUIInDic(typeof(PopupMenu).Name) && !IsLoadGame)
            ((PopupMenu)GetUIByType<PopupMenu>()).ClearItem();

        ObjectPool.Clear();

        resourceMgr.Clear();
    }

    public void DontDestroy(GameObject obj)
    {
        DontDestroyOnLoad(obj);
    }

    public void PlayBGM(AudioClip bgmClip)
    {
        soundMgr.PlayBGM(bgmClip);
    }

    public void StopBGM()
    {
        soundMgr.StopBGM();
    }

    public void PlaySFX(AudioClip sfxClip)
    {
        soundMgr.PlaySFX(sfxClip);
    }

    public void PlayPlusSFX(AudioClip plusSfxClip)
    {
        soundMgr.PlayPlusSFX(plusSfxClip);
    }

    public void PlayDialogueSFX(AudioClip dialogueClip)
    {
        soundMgr.PlayDialogueSFX(dialogueClip);
    }

    public float GetBGMVolume()
    {
        return soundMgr.GetBGMVolume();
    }

    public float GetSFXVolume()
    {
        return soundMgr.GetSFXVolume();
    }

    public void SetBGMVolume(float volume)
    {
        soundMgr.SetBGMVolume(volume);
    }

    public void SetSFXVolume(float volume)
    {
        soundMgr.SetSFXVolume(volume);
    }

    public void ChangeCursor(CursorIndex index)
    {
        Cursor.SetCursor(cursorTextures[(int)index], Vector2.zero, CursorMode.ForceSoftware);
    }

    public void RotateLightUp()
    {
        lightMgr.RotateLightUp();
    }

    public void RotateLightDown()
    {
        lightMgr.RotateLightDown();
    }
    public void UpdateStage(StageIndex newStage)
    {
        lightMgr.UpdateStage(newStage);
    }

    public void UpdateStage(TutorialIndex newStage)
    {
        lightMgr.UpdateStage(newStage);
    }

    public void LightReset()
    {
        lightMgr.LightReset();
    }

    public DialogueSO GetDialogueSO()
    {
        return dataMgr.DialogueSOs[CurrentScene];
    }

    private IEnumerator LoadNextMap()
    {
        current = 0;
        while (current < lerpTime)
        {
            current += Time.deltaTime;
            pixelRate = Mathf.Lerp(280, 10, current / lerpTime);
            pixelEffect.SetFloat("_PixelRate", pixelRate);
            yield return null;
        }

        yield return YieldInstructionCache.WaitForSeconds(lerpTime);

        current = 0;
        while (current < lerpTime)
        {
            current += Time.deltaTime;
            pixelRate = Mathf.Lerp(10, 280, current / lerpTime);
            pixelEffect.SetFloat("_PixelRate", pixelRate);
            yield return null;
        }
    }

    public void DecreaseSkyboxColor()
    {
        skyboxColor -= 0.12f;
        skybox.SetFloat("_Color", skyboxColor);
    }

    public void ResetSkyboxColor()
    {
        skyboxColor = 1.0f;
        skybox.SetFloat("_Color", skyboxColor);
    }

    public void FindItem()
    {
        int index = sceneMgr.CurrentScene.GetItemIndex();

        if (-1 != index)
        {
            if (!CheckUIInDic(typeof(PopupMenu).Name))
                AddUI(UI.Popup, typeof(PopupMenu).Name);

            ((PopupMenu)GetUIByType<PopupMenu>()).FindItem(index);
        }
    }
    public void LoadGame(int saveNum)
    {
        SaveData saveData = saveMgr.LoadGame();
        SavePointData savePoint = saveData.savePoints.Find(sp => sp.saveNum == saveNum);

        if (savePoint != null)
        {
            IsLoadGame = true;
            CurrentMap = (StageIndex)savePoint.mapIndex;
            
            itemList = savePoint.collectedItems;

            if (!CheckUIInDic(typeof(PopupMenu).Name))
                AddUI(UI.Popup, typeof(PopupMenu).Name);

            foreach (int item in itemList)
                ((PopupMenu)GetUIByType<PopupMenu>()).FindItem(item);

            DialogueSaveId = savePoint.currentDialogueID;

            Clear();
            SceneManager.LoadScene(savePoint.sceneIndex);
        }
    }

    public SaveData LoadGame()
    {
        return saveMgr.LoadGame();
    }

    public void SaveGame(int mapIndex, List<int> items, int dialogueID)
    {
        saveMgr.SaveGame((int)CurrentScene, mapIndex, items, dialogueID);
    }

    public List<int> GetSaveNum()
    {
        return saveMgr.GetSaveNum();
    }

    public void SetThirdStageLight()
    {
        lightMgr.SetThirdStageLight();
    }

    public void SetPlusSFXVolume(float volume)
    {
        soundMgr.SetPlusSFXVolume(volume);
    }

    public void StopPlusSFX()
    {
        soundMgr.StopPlusSFX();
    }
}