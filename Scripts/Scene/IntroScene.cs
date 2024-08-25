using UnityEngine;

public class IntroScene : BaseScene
{
    protected override void Start()
    {
        base.Start();

        GameManager.Instance.CurrentScene = SceneIndex.Intro;

        GameManager.Instance.IsLoadGame = false;

        GameManager.Instance.PlayBGM(bgmClip);

        GameManager.Instance.ShowUI<IntroSceneUI>(UI.Scene);

        GameManager.Instance.LightReset();
    }
}