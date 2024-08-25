using System.Collections;
using UnityEngine;
using DG.Tweening;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class StoryController : MonoBehaviour
{
    [SerializeField] private GameObject attackSprite; // 반격하는 스프라이트
    [SerializeField] private GameObject deathSprite;  // 죽는 스프라이트
    [SerializeField] private GameObject knifeObject;
    [SerializeField] private GameObject spiderObject;

    [SerializeField] private AudioClip bgmClip;

    private bool knifeCollected = false; // 칼을 수집했는지 여부

    void Start()
    {
        GameManager.Instance.CurrentScene = SceneIndex.ThirdDimentionStage;

        GameManager.Instance.SetThirdStageLight();

        GameManager.Instance.PlayBGM(bgmClip);

        GameManager.Instance.ShaderOffWithoutSave();

        DialogueManager.Instance.ChangeSO();
        DialogueManager.Instance.StartDialogue();

        attackSprite.SetActive(false);
        deathSprite.SetActive(false);
        if (GameManager.Instance.TakeKnife)
        {
            knifeObject.SetActive(false);
        }

        DialogueManager.Instance.DialogueEndEvent += EndEvent;
    }

    // 인형이 칼 수집O
    public void CollectKnife()
    {
        knifeCollected = true;
        GameManager.Instance.IsOnAnime = true;
        // 죽는 코루틴 중지
        StopCoroutine("DieAfterDelay");
        attackSprite.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        StartCoroutine(IntroScene(5.0f));
    }

    IEnumerator DieAfterDelay(float delay)
    {
        yield return YieldInstructionCache.WaitForSeconds(delay); //new WaitForSeconds(delay);

        // 인형이 칼 수집X
        if (!knifeCollected)
        {
            GameManager.Instance.IsOnAnime = true;
            deathSprite.SetActive(true);
            Cursor.lockState = CursorLockMode.None;
            StartCoroutine(IntroScene(12.0f));
        }
    }

    IEnumerator IntroScene(float delay)
    {
        yield return YieldInstructionCache.WaitForSeconds(delay); //new WaitForSeconds(delay);

        DialogueManager.Instance.DialogueEndEvent -= EndEvent;
        GameManager.Instance.StopBGM();
        GameManager.Instance.IsOnAnime = false;

        SceneManager.LoadScene("IntroScene");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Door"))
        {
            spiderObject.SetActive(true);

            Sequence spiderSequence = DOTween.Sequence();
            spiderSequence.Append(spiderObject.transform.DOLocalMoveY(0, 1).SetEase(Ease.OutBack));
            spiderSequence.Append(spiderObject.transform.DOLocalMoveY(1, 1).SetEase(Ease.InBack));

            spiderSequence.OnComplete(() => spiderObject.SetActive(false));
        }
    }

    private void EndEvent()
    {
        StartCoroutine(DieAfterDelay(60.0f));
    }
}
