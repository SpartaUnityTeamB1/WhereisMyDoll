using System;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.ProBuilder.MeshOperations;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StageScene: BaseScene
{
    [SerializeField] private GameObject playerMap;
    [SerializeField] private List<string> mapNames;
    [SerializeField] private List<int> targetIndex;
    // 추후 데이터 매니저 쪽으로 옮기고 싶음
    [SerializeField] private List<float> itemSizes;
    [SerializeField] private List<float> characterSizes;
    // 진짜 짜친다 고쳐야지
    [SerializeField] private Material cutScene1;
    [SerializeField] private Material cutScene2;
    [SerializeField] private Material cutScene3;

    private List<GameObject> characters = new List<GameObject>();
    private CameraController cam;
    private GameObject map;
    private GameObject target;
    private GameObject character;
    private SpriteRenderer objSprite;
    private SpriteRenderer characterSprite;
    private BoxCollider targetCollider;

    private NavMeshSurface mapNav;
    private NavMeshHit navHit;

    private StageIndex currentMap;
    private CharacterCode characterIndex;

    private Vector3 targetPos;
    private Vector3 randomPos = Vector3.zero;
    private string strTarget;
    private string strCharacter;
    
    public Action OnStageClear;

    protected override void Start()
    {
        base.Start();

        ZoomUI zoomUI = (ZoomUI)GameManager.Instance.GetUIByType<ZoomUI>();
        if (zoomUI == null)
        {
            GameManager.Instance.AddUI(UI.Zoom, typeof(ZoomUI).Name);
            zoomUI = (ZoomUI)GameManager.Instance.GetUIByType<ZoomUI>();
        }
        GameManager.Instance.Player.OnZoom += zoomUI.Zoom;
        GameManager.Instance.Player.OnReleaseZoom += zoomUI.ReleaseZoom;

        GameManager.Instance.CurrentScene = SceneIndex.Stage;
        GameManager.Instance.RotateLightDown();

        if (!GameManager.Instance.IsLoadGame)
        {
            currentMap = StageIndex.Stage0;
            GameManager.Instance.CurrentMap = currentMap;
        }
        else
        {
            currentMap = GameManager.Instance.CurrentMap;
            GameManager.Instance.PlayBGM(bgmClip);
            
            if (StageIndex.Stage2 == currentMap)
                GameManager.Instance.Player.CutScene.GetComponent<Image>().material = cutScene1;
            else if (StageIndex.Stage5 == currentMap)
                GameManager.Instance.Player.CutScene.GetComponent<Image>().material = cutScene2;
            else if (StageIndex.Stage9 == currentMap)
            {
                GameManager.Instance.Player.CutScene.GetComponent<Image>().material = cutScene3;
                GameManager.Instance.SetPlusSFXVolume(0.27f);
            }
        }

        GameManager.Instance.PlayPlusSFX(plusSfxClip);

        cam = Camera.main.GetComponent<CameraController>();

        if (!GameManager.Instance.CheckMapInDic(MapType.Stage))
            GameManager.Instance.AddMap(MapType.Stage);

        map = GameManager.Instance.GetMapResource(MapType.Stage, mapNames[(int)currentMap]);
        map.transform.SetParent(playerMap.transform);
        map.SetActive(true);

        mapNav = playerMap.GetComponent<NavMeshSurface>();
        mapNav.BuildNavMesh();

        SettingTarget();
        SettingCharacters();

        if (GameManager.Instance.IsShaderOn)
            GameManager.Instance.ShaderOn();

        DialogueManager.Instance.ChangeSO();
        
        if (-1 != GameManager.Instance.DialogueSaveId)
            DialogueManager.Instance.Id = GameManager.Instance.DialogueSaveId;

        DialogueManager.Instance.StartDialogue();
    }

    private void SettingTarget()
    {
        if (StageIndex.Stage10 == currentMap)
        {
            for (CharacterCode i = CharacterCode.Suspect0; i < CharacterCode.CHARACTEREND; ++i)
            {
                if (CharacterCode.Suspect0 == i)
                {
                    target = GameManager.Instance.ObjectPool.SpawnFromPool("Target");
                    targetCollider = target.GetComponent<BoxCollider>();

                    targetCollider.center = new Vector3(0f, 1.15f, 0f);
                    targetCollider.size = new Vector3(1.4f * characterSizes[(int)currentMap], 2.45f * characterSizes[(int)currentMap], 0.2f);
                }
                else
                {
                    target = GameManager.Instance.ObjectPool.SpawnFromPool("FakeTarget");
                    targetCollider = target.GetComponent<BoxCollider>();

                    targetCollider.center = new Vector3(0f, 1.15f, 0f);
                    targetCollider.size = new Vector3(1.2f * characterSizes[(int)currentMap], 2.25f * characterSizes[(int)currentMap], 0.2f);

                    characters.Add(target);
                }

                objSprite = target.GetComponentInChildren<SpriteRenderer>();
                

                NavMesh.SamplePosition(playerMap.transform.position + (UnityEngine.Random.onUnitSphere * UnityEngine.Random.Range(1f, 15f)),
                            out navHit, 15f, NavMesh.AllAreas);

                strTarget = $"Char_{(int)i}";

                if (!GameManager.Instance.CheckSpriteInDic(strTarget))
                    GameManager.Instance.AddSprite(SpriteType.Character, "Char", (int)i);

                targetPos = navHit.position;                

                objSprite.gameObject.transform.localPosition = new Vector3(0f, 1.15f, 0f);

                target.transform.localScale = Vector3.one * characterSizes[(int)currentMap];

                target.transform.position = targetPos;
                target.transform.SetParent(playerMap.transform);
                objSprite.sprite = GameManager.Instance.GetSpriteResource(strTarget);
                target.GetComponent<CharacterObject>().SpawnCharacter();
            }
        }
        else
        {
            target = GameManager.Instance.ObjectPool.SpawnFromPool("Target");
            objSprite = target.GetComponentInChildren<SpriteRenderer>();
            targetCollider = target.GetComponent<BoxCollider>();

            NavMesh.SamplePosition(playerMap.transform.position + (UnityEngine.Random.onUnitSphere * UnityEngine.Random.Range(1f, 15f)),
                        out navHit, 15f, NavMesh.AllAreas);

            if (StageIndex.Stage10 > currentMap)
            {
                strTarget = $"Item_{targetIndex[(int)currentMap]}";

                if (!GameManager.Instance.CheckSpriteInDic(strTarget))
                    GameManager.Instance.AddSprite(SpriteType.Item, "Item", targetIndex[(int)currentMap]);

                targetCollider.size = new Vector3(0.6f, 0.6f, 0.1f);

                targetPos = navHit.position;
                targetPos.y += 0.25f;

                target.transform.localScale = Vector3.one * itemSizes[(int)currentMap];

                objSprite.gameObject.transform.localPosition = Vector3.zero;

                targetCollider.center = Vector3.zero;
                targetCollider.size = new Vector3(0.6f * itemSizes[(int)currentMap], 0.6f * itemSizes[(int)currentMap], 0.1f);
            }
            else
            {
                strTarget = $"Char_{(int)CharacterCode.Suspect0}";

                if (!GameManager.Instance.CheckSpriteInDic(strTarget))
                    GameManager.Instance.AddSprite(SpriteType.Character, "Char", (int)CharacterCode.Suspect0);

                targetCollider.size = new Vector3(1.2f, 2.25f, 0.2f);

                if (StageIndex.Stage19 == currentMap)
                    targetPos = new Vector3(-6.7f, 2f, -8.2f);
                else
                    targetPos = navHit.position;

                targetCollider.center = new Vector3(0f, 1.15f, 0f);
                targetCollider.size = new Vector3(1.4f * characterSizes[(int)currentMap], 2.45f * characterSizes[(int)currentMap], 0.2f);

                objSprite.gameObject.transform.localPosition = new Vector3(0f, 1.15f, 0f);

                target.transform.localScale = Vector3.one * characterSizes[(int)currentMap];
            }

            target.transform.position = targetPos;
            target.transform.SetParent(playerMap.transform);
            objSprite.sprite = GameManager.Instance.GetSpriteResource(strTarget);
            target.GetComponent<CharacterObject>().SpawnCharacter();
        }
    }

    private void SettingCharacters()
    {
        if (StageIndex.Stage10 > currentMap)
        {
            for (int i = 1; i <= (int)currentMap; ++i)
            {
                character = GameManager.Instance.ObjectPool.SpawnFromPool("Character");
                characterSprite = character.GetComponentInChildren<SpriteRenderer>();

                NavMesh.SamplePosition(playerMap.transform.position + (UnityEngine.Random.onUnitSphere * UnityEngine.Random.Range(1f, 15f)),
                        out navHit, 15f, NavMesh.AllAreas);

                randomPos = navHit.position;

                character.transform.position = randomPos;
                character.transform.localScale = Vector3.one * characterSizes[(int)currentMap];

                strCharacter = $"Char_{i}";

                if (!GameManager.Instance.CheckSpriteInDic(strCharacter))
                    GameManager.Instance.AddSprite(SpriteType.Character, "Char", i);

                characterSprite.sprite = GameManager.Instance.GetSpriteResource(strCharacter);
                character.transform.SetParent(playerMap.transform);

                character.GetComponent<CharacterObject>().SpawnCharacter();

                characters.Add(character);
            }
        }
        else
        {
            for (int i = 2 + (currentMap - StageIndex.Stage10); i < (int)CharacterCode.CHARACTEREND; ++i)
            {
                character = GameManager.Instance.ObjectPool.SpawnFromPool("Character");
                characterSprite = character.GetComponentInChildren<SpriteRenderer>();

                NavMesh.SamplePosition(playerMap.transform.position + (UnityEngine.Random.onUnitSphere * UnityEngine.Random.Range(1f, 15f)),
                        out navHit, 15f, NavMesh.AllAreas);

                randomPos = navHit.position;

                character.transform.position = randomPos;
                character.transform.localScale = Vector3.one * characterSizes[(int)currentMap];

                strCharacter = $"Char_{i}";

                if (!GameManager.Instance.CheckSpriteInDic(strCharacter))
                    GameManager.Instance.AddSprite(SpriteType.Character, "Char", i);

                characterSprite.sprite = GameManager.Instance.GetSpriteResource(strCharacter);
                character.transform.SetParent(playerMap.transform);

                character.GetComponent<CharacterObject>().SpawnCharacter();

                characters.Add(character);
            }
        }
    }

    public override void GoToNextScene()
    {
        if (StageIndex.STAGEEND == (currentMap + 1))
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
            foreach (var character in characters)
            {
                character.GetComponent<CharacterObject>().DespawnCharacter();

                character.transform.parent = GameManager.Instance.transform;
                character.SetActive(false);
            }

            map.SetActive(false);

            ++currentMap;
            GameManager.Instance.CurrentMap = currentMap;

            map = GameManager.Instance.GetMapResource(MapType.Stage, mapNames[(int)currentMap]);
            map.transform.SetParent(playerMap.transform);
            map.SetActive(true);

            mapNav.BuildNavMesh();

            SettingTarget();
            if (StageIndex.Stage10 != currentMap)
                SettingCharacters();

            cam.ResetCamera();

            UpdateDayNightCycleStage((int)currentMap);

            if (StageIndex.Stage2 == currentMap)
                GameManager.Instance.Player.CutScene.GetComponent<Image>().material = cutScene1;
            else if (StageIndex.Stage5 == currentMap)
                GameManager.Instance.Player.CutScene.GetComponent<Image>().material = cutScene2;
            else if (StageIndex.Stage9 == currentMap)
                GameManager.Instance.Player.CutScene.GetComponent<Image>().material = cutScene3;

            if (StageIndex.Stage10 > currentMap)
                GameManager.Instance.SetPlusSFXVolume(0.03f);
            else if (StageIndex.Stage10 == currentMap)
                GameManager.Instance.StopPlusSFX();

            DialogueManager.Instance.GoToNextID();
            DialogueManager.Instance.StartDialogue();
        }
    }

    private void UpdateDayNightCycleStage(int stage)
    {
        GameManager.Instance.UpdateStage((StageIndex)stage);
    }

    public override int GetItemIndex()
    {
        if (StageIndex.Stage10 > currentMap)
            return targetIndex[(int)currentMap];

        return -1;
    }
}