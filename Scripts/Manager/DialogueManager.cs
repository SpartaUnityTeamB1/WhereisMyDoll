using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class DialogueManager : Singleton<DialogueManager>
{
    [field: SerializeField] public GameObject DialogueCanvas { get; set; }
    [SerializeField] private Image characterImg;
    [SerializeField] private TextMeshProUGUI dialogueTxt;

    [SerializeField] private float textAddSpeed = 0.08f;

    public DialogueSO dialogueSO { get; private set; } = null;
    private Dialogue currentDialogue;

    private Vector2 itemSize = new Vector2(256f, 256f);
    private Vector2 characterSize = new Vector2(256f, 512f);

    private Coroutine updateCoroutine;

    private StringBuilder sb;

    public int Id { get; set; }
    private int currentTextIndex;

    private string fullText;
    private string clipName;

    private string spriteName;
    private SpeakerType speakerType;

    public Action DialogueEndEvent;

    protected override void Awake()
    {
        base.Awake();

        sb = new StringBuilder();
    }

    public void ChangeSO()
    {
        dialogueSO = GameManager.Instance.GetDialogueSO();

        Id = 0;

        currentTextIndex = 0;
    }

    public void StartDialogue()
    {
        Cursor.lockState = CursorLockMode.Confined;
        GameManager.Instance.ChangeCursor(CursorIndex.Menu);

        DialogueCanvas.SetActive(true);

        GameManager.Instance.IsOnDialogue = true;

        UpdateDialogue();
    }

    private void UpdateDialogue()
    {
        if (null != updateCoroutine)
            StopCoroutine(updateCoroutine);

        currentDialogue = dialogueSO.dialogue[Id];

        if (-1 == currentDialogue.SpeakerId[currentTextIndex])
        {
            characterImg.gameObject.SetActive(false);

        }
        else
        {
            speakerType = (SpeakerType)currentDialogue.SpeakerType[currentTextIndex];

            spriteName = $"{speakerType.ToString()}_{currentDialogue.SpeakerId[currentTextIndex]}";

            if (SpeakerType.Item == speakerType)
            {
                ((RectTransform)characterImg.transform).sizeDelta = itemSize;

                if (!GameManager.Instance.CheckSpriteInDic(spriteName))
                    GameManager.Instance.AddSprite(SpriteType.Item, speakerType.ToString(), currentDialogue.SpeakerId[currentTextIndex]);
            }
            else
            {
                ((RectTransform)characterImg.transform).sizeDelta = characterSize;

                if (!GameManager.Instance.CheckSpriteInDic(spriteName))
                    GameManager.Instance.AddSprite(SpriteType.Character, speakerType.ToString(), currentDialogue.SpeakerId[currentTextIndex]);
            }

            characterImg.sprite = GameManager.Instance.GetSpriteResource(spriteName);

            if (!characterImg.gameObject.activeInHierarchy)
                characterImg.gameObject.SetActive(true);
        }

        if (-1 != currentDialogue.SFXIndex[currentTextIndex])
        {
            clipName = $"{GameManager.Instance.CurrentScene.ToString()}/{Id}_{currentDialogue.SFXIndex[currentTextIndex]}";

            if (!GameManager.Instance.CheckClipInDic(clipName))
                GameManager.Instance.AddClip(AudioClipType.Dialogue, clipName);

            GameManager.Instance.PlayDialogueSFX(GameManager.Instance.GetClipInDic(clipName));
        }

        dialogueTxt.text = "";
        sb.Clear();

        updateCoroutine = StartCoroutine(PrintText());
    }

    private IEnumerator PrintText()
    {
        fullText = dialogueSO.dialogue[Id].Texts[currentTextIndex];

        for (int i = 0; i < fullText.Length; ++i)
        {
            if ('&' == fullText[i])
                sb.Append($"<sprite={dialogueSO.dialogue[Id].IconIndex[currentTextIndex]}>");
            else
                sb.Append(fullText[i]);

            dialogueTxt.text = sb.ToString();
            yield return YieldInstructionCache.WaitForSeconds(textAddSpeed); //new WaitForSeconds(textAddSpeed);
        }

        updateCoroutine = null;
    }

    public void OnClickNextBtn()
    {
        currentDialogue = dialogueSO.dialogue[Id];

        if (null != updateCoroutine)
        {
            StopCoroutine(updateCoroutine);
            updateCoroutine = null;

            fullText = currentDialogue.Texts[currentTextIndex];

            dialogueTxt.text = fullText.Replace("&", $"<sprite={dialogueSO.dialogue[Id].IconIndex[currentTextIndex]}>");
        }
        else if (currentDialogue.Texts.Count > (currentTextIndex + 1))
        {
            ++currentTextIndex;
            UpdateDialogue();
        }
        else
        {
            DialogueCanvas.SetActive(false);
            GameManager.Instance.IsOnDialogue = false;

            if (SceneIndex.ThirdDimentionStage == GameManager.Instance.CurrentScene)
                Cursor.lockState = CursorLockMode.Locked;
            else
                GameManager.Instance.Player.OnReleaseZoom?.Invoke();

            GameManager.Instance.ChangeCursor(CursorIndex.Camera);

            switch ((EventType)currentDialogue.DialogueEvent)
            {
                case EventType.NoEvent:
                    break;
                case EventType.InputQuest:
                    QuestManager.Instance.AddQuest(QuestType.KeyInput, new string[] { "Q", "q" });
                    break;
                case EventType.FindQuest:
                    QuestManager.Instance.AddQuest(QuestType.Find);
                    break;
                case EventType.NextScene:
                    GameManager.Instance.GoToNextStage();
                    return;
                case EventType.CutScene:
                    QuestManager.Instance.AddQuest(QuestType.Find);
                    return;
            }

            Id = currentDialogue.NextId;
            currentTextIndex = 0;

            DialogueEndEvent?.Invoke();
        }
    }

    public bool CheckEvent(EventType eType)
    {
        return (eType == (EventType)dialogueSO.dialogue[Id].DialogueEvent);
    }

    public void GoToNextID()
    {
        Id = currentDialogue.NextId;
        currentTextIndex = 0;
    }
}
