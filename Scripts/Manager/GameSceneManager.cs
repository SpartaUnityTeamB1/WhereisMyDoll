public class GameSceneManager
{
    public BaseScene CurrentScene { get; private set; }

    public void SetScene(BaseScene scene)
    {
        CurrentScene = scene;
    }
}