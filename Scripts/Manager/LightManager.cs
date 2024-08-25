using UnityEngine;

public class LightManager
{
    private int currentStage;
    private int currentStageNum;
    private int tutorialStages;
    private int totalStages; // 전체 스테이지 수
    private bool isDown = true;
    public SceneIndex currentMode { get; set; }

    [Header("Sun")]
    private GameObject sunObject;
    private Light sun;
    private Color sunColor = new Color(0.690f, 0.576f, 0.596f);

    private GameObject obj = null;

    public void LightReset()
    {
        currentStage = 0;
        currentStageNum = 0;
        tutorialStages = 0;
        isDown = true;
        currentMode = SceneIndex.Tutorial;

        if (obj != null)
        {
            Object.Destroy(obj);
            obj = null;
        }

        GameManager.Instance.ResetSkyboxColor();
        InitializeLight();
    }

    public void InitializeLight()
    {
        totalStages = System.Enum.GetValues(typeof(StageIndex)).Length;

        MakeObject();
        CreateLights();
    }

    private void MakeObject()
    {
        if (obj == null)
        {
            obj = new GameObject(typeof(LightManager).Name);
            GameManager.Instance.DontDestroy(obj);
        }
    }

    private void CreateLights()
    {
        sunObject = new GameObject("Sun");
        sun = sunObject.AddComponent<Light>();
        sun.type = LightType.Directional;
        sun.color = sunColor;
        sun.intensity = 5f;
        sun.transform.rotation = Quaternion.Euler(65f, 0f, 0f);

        sunObject.transform.parent = obj.transform;
    }

    public void UpdateStage(StageIndex newStage)
    {
        currentStage = (int)newStage;
        currentStageNum++;
        UpdateLightingForCurrentStage();
        RotateLightDown();
    }

    public void UpdateStage(TutorialIndex newStage)
    {
        currentStage = (int)newStage;
        UpdateLightingForCurrentTutorialStage();
        RotateLightDown();
    }

    private void UpdateLightingForCurrentStage()
    {
        UpdateLighting(sun, sunColor);
    }

    private void UpdateLightingForCurrentTutorialStage()
    {
        if (tutorialStages < 5)
        {
            UpdateLighting(sun, sunColor);
            tutorialStages++;
        }
        else
        {
            currentMode = SceneIndex.Stage;
            currentStage = (int)StageIndex.Stage0;
            currentStageNum = 0;
            UpdateLightingForCurrentStage();
        }
    }

    void UpdateLighting(Light lightSource, Color color)
    {
        if (GameManager.Instance.CurrentScene == SceneIndex.Tutorial)
        {
            lightSource.color = color;
            return;
        }

        float normalizedStage = lightSource.intensity - (float)currentStageNum / totalStages;
        float intensity = Mathf.Max(normalizedStage, 0.3f);

        lightSource.color = color;
        lightSource.intensity = intensity;

        // Adjust the skybox color if needed
        if (GameManager.Instance.SkyboxColor >= 0.05f)
        {
            GameManager.Instance.DecreaseSkyboxColor();
        }
    }

    public void RotateLightUp()
    {
        if (isDown)
        {
            sunObject.transform.rotation = Quaternion.Euler(120f, 0f, 0f);
            isDown = false;
        }
    }

    public void RotateLightDown()
    {
        if (!isDown)
        {
            sunObject.transform.rotation = Quaternion.Euler(65f, 0f, 0f);
            isDown = true;
        }
    }

    public void SetThirdStageLight()
    {
        sun.intensity = 0.3f;
    }
}
