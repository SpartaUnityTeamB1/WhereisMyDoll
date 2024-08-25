using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System;


public class PlayerInput : MonoBehaviour
{
    [SerializeField] private LayerMask targetLayer;
    [SerializeField] private LayerMask itemLayer;
    [SerializeField] private LayerMask newsLayer;
    [SerializeField] private LayerMask filmLayer;
    [SerializeField] private AudioClip cameraSFX;

    private float rotateDir = 90f;
    private bool isCoroutineRuniing;

    private float current = 0f;
    private float percent = 0f;
    private float targetRotation = 0f;
    // private int saveNum = 0;

    private Camera _camera;
    private Ray ray;
    private RaycastHit hit;

    // 0 : end, 1 : cor, 2 : start
    public int IsZoom { get; set; } = 0;
    public bool IsTop { get; set; } = false;

    public Action OnMove;
    public Action OnChangeView;
    public Action OnZoom;
    public Action OnReleaseZoom;
    public Action OnSave;

    private string clipName = "SFXNoise";

    private SpriteRenderer img;

    [field: SerializeField] public GameObject CutScene { get; private set; }
    [field: SerializeField] public GameObject FakeEndingScene { get; private set; }

    private void Awake()
    {
        GameManager.Instance.Player = this;
    }

    private void Start()
    {
        GameManager.Instance.AddClip(AudioClipType.SFX, clipName);
        _camera = Camera.main;
    }

    // 메뉴(사운드, 그래픽, 추리 등) 키는 Q
    public void OnMenu(InputAction.CallbackContext context)
    {
        if ((0 == IsZoom) && !GameManager.Instance.IsOnDialogue && (InputActionPhase.Started == context.phase) && !GameManager.Instance.IsOnAnime)
        {
            if (!GameManager.Instance.IsOpenMenu)
            {
                GameManager.Instance.IsOpenMenu = true;

                GameManager.Instance.ShowUI<PopupMenu>(UI.Popup);
                GameManager.Instance.ChangeCursor(CursorIndex.Menu);
            }
            else
            {
                GameManager.Instance.IsOpenMenu = false;

                GameManager.Instance.HideUI<PopupMenu>();
                GameManager.Instance.ChangeCursor(CursorIndex.Camera);
            }

            foreach (Quest quest in QuestManager.Instance.ActiveQuests)
            {
                if (QuestType.KeyInput == quest.QuestType)
                {
                    if (quest is InputQuest)
                    {
                        if (((InputQuest)quest).CheckClear("Q"))
                        {
                            QuestManager.Instance.FinishQuest(quest);



                            DialogueManager.Instance.StartDialogue();

                            break;
                        }
                    }
                }
            }
        }
    }

    // 스페이스바 눌러서 넘기기
    public void OnSpacebarClick(InputAction.CallbackContext context)
    {
        if(context.phase == InputActionPhase.Started && GameManager.Instance.IsOnDialogue && !GameManager.Instance.IsOnAnime)
        {
            DialogueManager.Instance.OnClickNextBtn();
        }
    }

    public void OnMouseRightClick(InputAction.CallbackContext context)
    {
        if(context.phase == InputActionPhase.Started && !GameManager.Instance.IsOnDialogue && !GameManager.Instance.IsOpenMenu && !GameManager.Instance.IsOnAnime)
        {
            OnMove?.Invoke(); 
        }
        if(context.phase == InputActionPhase.Performed && !GameManager.Instance.IsOnDialogue && !GameManager.Instance.IsOpenMenu && (0 == IsZoom) && !GameManager.Instance.IsOnAnime)
        {
            IsZoom = 2;

            GameManager.Instance.ShowUI<ZoomUI>(UI.Zoom);

            OnZoom?.Invoke();
        }
        if(context.phase == InputActionPhase.Canceled && !GameManager.Instance.IsOpenMenu && (0 != IsZoom))
        {
            OnReleaseZoom?.Invoke();
        }
    }

    public void OnPicking(InputAction.CallbackContext context)
    {
        if ((InputActionPhase.Started == context.phase) && !GameManager.Instance.IsOnDialogue && (2 == IsZoom) && !GameManager.Instance.IsOnAnime)
            Picking();
    }

    public void OnLook(InputAction.CallbackContext context)
    {
        if (InputActionPhase.Performed == context.phase && !GameManager.Instance.IsOnDialogue && (2 == IsZoom))
        {
            OnMove?.Invoke();
        }
    }
    public void OnRotateRight(InputAction.CallbackContext context)
    {
        // 오른쪽 회전
        if (context.phase == InputActionPhase.Started && !isCoroutineRuniing && !GameManager.Instance.IsOnDialogue && !GameManager.Instance.IsOpenMenu)
        {
            StartCoroutine(RotateGround(-rotateDir));
        }
    }

    public void OnRotateLeft(InputAction.CallbackContext context)
    {
        // 왼쪽 회전
        if (context.phase == InputActionPhase.Started && !isCoroutineRuniing && !GameManager.Instance.IsOnDialogue && !GameManager.Instance.IsOpenMenu)
        {
            StartCoroutine(RotateGround(rotateDir));
        }
    }

    public void OnRotateUp(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started && !GameManager.Instance.IsOnDialogue && !GameManager.Instance.IsOpenMenu && (0 == IsZoom) && !IsTop)
        {
            OnChangeView?.Invoke();
            IsTop = true;
            GameManager.Instance.RotateLightUp();
        }
    }

    public void OnRotateDown(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started && !GameManager.Instance.IsOnDialogue && !GameManager.Instance.IsOpenMenu && (0 == IsZoom) && IsTop)
        {
            OnChangeView?.Invoke();
            IsTop = false;
            GameManager.Instance.RotateLightDown();
        }
    }

    IEnumerator RotateGround(float Dir)
    {
        // 지면 회전 코루틴
        isCoroutineRuniing = true;

        current = 0f;
        percent = 0f;

        targetRotation = transform.rotation.eulerAngles.y + Dir;

        while (percent < 1)
        {
            current += Time.deltaTime;
            percent = current / 0.5f;
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0, targetRotation, 0), percent);
            yield return null;
        }

        isCoroutineRuniing = false;
    }

    public void Picking()
    {
        ray = _camera.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit, _camera.farClipPlane))
        {
            if (hit.collider.gameObject.layer == 7)
            {
                foreach (Quest quest in QuestManager.Instance.ActiveQuests)
                {
                    if (QuestType.Find == quest.QuestType)
                    {
                        StartCoroutine(PickingEffect());
                        GameManager.Instance.PlaySFX(cameraSFX);
                        GameManager.Instance.FindItem();

                        hit.collider.gameObject.GetComponent<CharacterObject>().DespawnCharacter();

                        hit.collider.gameObject.transform.SetParent(GameManager.Instance.transform);

                        hit.collider.gameObject.SetActive(false);

                        QuestManager.Instance.FinishQuest(quest);

                        if (DialogueManager.Instance.CheckEvent(EventType.CutScene))
                        {
                            GameManager.Instance.IsOnAnime = true;
                            OnReleaseZoom?.Invoke();
                            CutScene.SetActive(true);
                            GameManager.Instance.PlaySFX(GameManager.Instance.GetClipInDic(clipName));
                            // 애니메이션 이벤트나 재생이 끝나면
                            StartCoroutine(PlayCutScene(3f));
                        }
                        else
                            DialogueManager.Instance.StartDialogue();

                        break;
                    }
                }
            }  
            // 캐릭터 잘못 클릭했을 때
            else if (hit.collider.gameObject.layer == 11)
            {
                StartCoroutine(PlayFakeEnding());
            }
            // 저장
            else if(hit.collider.gameObject.layer == 12)
            {
                OnSave?.Invoke();

                Destroy(hit.collider.gameObject);
            }
            else if (hit.collider.gameObject.layer == 9)
            {
                GameManager.Instance.ShowUI<PopupNews>(UI.Popup);
            }
            // 2D에서 칼 먹는 거
            else if (hit.collider.gameObject.layer == 10)
            {
                GameManager.Instance.TakeKnife = true;
                hit.collider.gameObject.SetActive(false);
            }
        }
    }

    private bool IsLayerMatched(int value, int layer)
    {
        return value == (value | 1 << layer);
    }

    private IEnumerator PlayCutScene(float delay)
    {
        yield return YieldInstructionCache.WaitForSeconds(delay); //new WaitForSeconds(delay);

        CutScene.SetActive(false);
        GameManager.Instance.IsOnAnime = false;
        DialogueManager.Instance.GoToNextID();
        DialogueManager.Instance.StartDialogue();
    }

    private IEnumerator PickingEffect()
    {
        GameManager.Instance.ShowUI<CameraClickEffect>(UI.Popup);
        yield return YieldInstructionCache.WaitForSeconds(1.5f); //new WaitForSeconds(1.5f);
        GameManager.Instance.HideUI<CameraClickEffect>();
    }

    private IEnumerator PlayFakeEnding()
    {
        GameManager.Instance.IsOnAnime = true;
        FakeEndingScene.SetActive(true);

        yield return YieldInstructionCache.WaitForSeconds(10f);

        FakeEndingScene.SetActive(false);
        GameManager.Instance.GoToPrevStage();
    }
}