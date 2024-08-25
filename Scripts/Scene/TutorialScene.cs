using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TutorialScene : BaseScene
{
    [SerializeField] private GameObject playerMap;
    [SerializeField] private List<string> mapNames;
    [SerializeField] private List<int> itemNames;

    private CameraController cam;
    private GameObject map;
    private GameObject target;
    private GameObject character;
    private SpriteRenderer objSprite;
    private SpriteRenderer characterSprite;
    private BoxCollider targetCollider;
    private List<GameObject> characters = new List<GameObject>();

    private TutorialIndex currentMap;

    private Vector3 targetPos;
    private Vector3 randomPos = Vector3.zero;
    private string strTarget;
    private string strCharacter;

    protected override void Start()
    {
        base.Start();

        ZoomUI zoomUI = (ZoomUI)GameManager.Instance.GetUIByType<ZoomUI>();
        if(zoomUI == null )
        {
            GameManager.Instance.AddUI(UI.Zoom, typeof(ZoomUI).Name);
            zoomUI = (ZoomUI)GameManager.Instance.GetUIByType<ZoomUI>();
        }
        GameManager.Instance.Player.OnZoom += zoomUI.Zoom;
        GameManager.Instance.Player.OnReleaseZoom += zoomUI.ReleaseZoom;


        GameManager.Instance.CurrentScene = SceneIndex.Tutorial;

        currentMap = TutorialIndex.Tutorial0;

        cam = Camera.main.GetComponent<CameraController>();

        if (!GameManager.Instance.CheckMapInDic(MapType.Tutorial))
            GameManager.Instance.AddMap(MapType.Tutorial);

        map = GameManager.Instance.GetMapResource(MapType.Tutorial, mapNames[(int)currentMap]);
        map.transform.SetParent(playerMap.transform);
        map.SetActive(true);

        SettingTarget();

        GameManager.Instance.PlayBGM(bgmClip);
        
        if (GameManager.Instance.IsShaderOn)
            GameManager.Instance.ShaderOn();

        DialogueManager.Instance.ChangeSO();
        DialogueManager.Instance.StartDialogue();
    }

    public override void GoToNextScene()
    {
        if (TutorialIndex.Tutorial3 == currentMap)
        {
            target.GetComponent<CharacterObject>().DespawnCharacter();

            target.transform.SetParent(GameManager.Instance.transform);
            target.SetActive(false);

            foreach (var character in characters)
            {
                character.GetComponent<CharacterObject>().DespawnCharacter();

                character.transform.SetParent(GameManager.Instance.transform);
                character.SetActive(false);
            }

            GameManager.Instance.Clear();
            SceneManager.LoadScene((int)NextScene);
        }
        else
        {
            map.SetActive(false);

            ++currentMap;

            map = GameManager.Instance.GetMapResource(MapType.Tutorial, mapNames[(int)currentMap]);
            map.transform.SetParent(playerMap.transform);
            map.SetActive(true);

            SettingTarget();

            cam.ResetCamera();
            UpdateDayNightCycleStage(currentMap);

            DialogueManager.Instance.GoToNextID();
            DialogueManager.Instance.StartDialogue();
        }
    }

    public void SettingTarget()
    {
        target = GameManager.Instance.ObjectPool.SpawnFromPool("Target");
        objSprite = target.GetComponentInChildren<SpriteRenderer>();
        targetCollider = target.GetComponent<BoxCollider>();

        if (TutorialIndex.Tutorial3 != currentMap)
        {
            targetPos = Vector3.zero;
            targetPos.y = 1.1f;

            strTarget = $"Item_{itemNames[(int)currentMap]}";

            if (!GameManager.Instance.CheckSpriteInDic(strTarget))
                GameManager.Instance.AddSprite(SpriteType.Item, "Item", itemNames[(int)currentMap]);

            targetCollider.center = Vector3.zero;
            targetCollider.size = new Vector3(0.6f, 0.6f, 0.1f);

            objSprite.gameObject.transform.localPosition = Vector3.zero;
        }
        else
        {
            randomPos.x = Random.Range(-12f, 12f);
            randomPos.y = 0.95f;
            randomPos.z = Random.Range(-12f, 12f);

            targetPos = randomPos;

            strTarget = $"Char_{(int)CharacterCode.Edward}";

            if (!GameManager.Instance.CheckSpriteInDic(strTarget))
                GameManager.Instance.AddSprite(SpriteType.Character, "Char", (int)CharacterCode.Edward);

            targetCollider.center = new Vector3(0f, 1.15f, 0f);
            targetCollider.size = new Vector3(1.2f, 2.25f, 0.2f);

            objSprite.gameObject.transform.localPosition = new Vector3(0f, 1.15f, 0f);

            for (int i = 1; i < (int)CharacterCode.CHARACTEREND; ++i)
            {
                character = GameManager.Instance.ObjectPool.SpawnFromPool("Character");
                characterSprite = character.GetComponentInChildren<SpriteRenderer>();

                randomPos.x = Random.Range(-12f, 12f);
                randomPos.y = 0.95f;
                randomPos.z = Random.Range(-12f, 12f);

                character.transform.position = randomPos;

                strCharacter = $"Char_{i}";

                if (!GameManager.Instance.CheckSpriteInDic(strCharacter))
                    GameManager.Instance.AddSprite(SpriteType.Character, "Char", i);

                characterSprite.sprite = GameManager.Instance.GetSpriteResource(strCharacter);
                character.transform.SetParent(playerMap.transform);

                character.GetComponent<CharacterObject>().SpawnCharacter();

                characters.Add(character);
            }
        }

        target.transform.position = targetPos;
        target.transform.SetParent(playerMap.transform);
        objSprite.sprite = GameManager.Instance.GetSpriteResource(strTarget);
        target.GetComponent<CharacterObject>().SpawnCharacter();
    }

    private void UpdateDayNightCycleStage(TutorialIndex stage)
    {
        GameManager.Instance.UpdateStage(stage);
    }

    public override int GetItemIndex()
    {
        if (TutorialIndex.Tutorial3 != currentMap)
            return itemNames[(int)currentMap];

        return -1;
    }
}