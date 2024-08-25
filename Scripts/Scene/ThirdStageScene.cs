public class ThirdStageScene : BaseScene
{
    protected override void Start()
    {
        base.Start();

        GameManager.Instance.CurrentScene = SceneIndex.ThirdDimentionStage;

        GameManager.Instance.PlayBGM(bgmClip);

        // SO 교체 함수로 바꿀 예정
        DialogueManager.Instance.ChangeSO();
        DialogueManager.Instance.StartDialogue();
    }
}