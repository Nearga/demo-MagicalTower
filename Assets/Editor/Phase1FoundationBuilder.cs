using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public static class Phase1FoundationBuilder
{
    public static void CreateFoundation()
    {
        CreateFolders();

        var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

        var gameRoot = CreateRoot("GameRoot");
        var gameplayRoot = CreateRoot("GameplayRoot", gameRoot.transform);
        var uiRoot = CreateRoot("UIRoot", gameRoot.transform);
        var cameraRoot = CreateRoot("CameraRoot", gameRoot.transform);
        var lightingRoot = CreateRoot("LightingRoot", gameRoot.transform);

        CreatePrimitiveRoot("Tower", gameplayRoot.transform, PrimitiveType.Cylinder, new Vector3(0f, 1f, 0f), new Vector3(1.6f, 2f, 1.6f));
        CreateRoot("EnemySpawnRoot", gameplayRoot.transform);
        CreateRoot("EnemyPoolRoot", gameplayRoot.transform);
        CreateRoot("ProjectileRoot", gameplayRoot.transform);
        CreateRoot("VfxRoot", gameplayRoot.transform);
        CreateWorldCanvas(uiRoot.transform);
        CreateHudCanvas(uiRoot.transform);
        CreateCamera(cameraRoot.transform);
        CreateLight("Directional Light", lightingRoot.transform, LightType.Directional, new Vector3(50f, -35f, 0f), 1.2f);
        CreateLight("Fill Light", lightingRoot.transform, LightType.Point, new Vector3(-4f, 5f, -4f), 0.55f);

        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene, "Assets/Scenes/MagicalTowerPrototype.unity");
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    private static void CreateFolders()
    {
        EnsureFolder("Assets/Scenes");
        EnsureFolder("Assets/Scripts");
        EnsureFolder("Assets/Scripts/Runtime");
        EnsureFolder("Assets/Scripts/Runtime/Pooling");
        EnsureFolder("Assets/Scripts/UI");
        EnsureFolder("Assets/Content");
        EnsureFolder("Assets/Prefabs");
        EnsureFolder("Assets/Materials");
        EnsureFolder("Assets/Art");
        EnsureFolder("Assets/Art/Generated");
        EnsureFolder("Assets/Art/Generated/Textures");
        EnsureFolder("Assets/Editor");
    }

    private static void EnsureFolder(string path)
    {
        if (!AssetDatabase.IsValidFolder(path))
        {
            Directory.CreateDirectory(path);
        }
    }

    private static GameObject CreateRoot(string name, Transform parent = null)
    {
        var go = new GameObject(name);
        go.transform.SetParent(parent, false);
        Undo.RegisterCreatedObjectUndo(go, $"Create {name}");
        return go;
    }

    private static GameObject CreatePrimitiveRoot(string name, Transform parent, PrimitiveType primitiveType, Vector3 position, Vector3 scale)
    {
        var go = GameObject.CreatePrimitive(primitiveType);
        go.name = name;
        go.transform.SetParent(parent, false);
        go.transform.position = position;
        go.transform.localScale = scale;
        Undo.RegisterCreatedObjectUndo(go, $"Create {name}");
        return go;
    }

    private static void CreateWorldCanvas(Transform parent)
    {
        var canvasGo = CreateRoot("WorldCanvas", parent);
        var canvas = canvasGo.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.WorldSpace;
        canvasGo.AddComponent<CanvasScaler>();
        canvasGo.AddComponent<GraphicRaycaster>();
        canvasGo.transform.position = new Vector3(0f, 2.8f, 0f);
        canvasGo.transform.localScale = Vector3.one * 0.01f;

        var rect = canvasGo.GetComponent<RectTransform>();
        rect.sizeDelta = new Vector2(800f, 450f);
    }

    private static void CreateHudCanvas(Transform parent)
    {
        var canvasGo = CreateRoot("HudCanvas", parent);
        var canvas = canvasGo.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;

        var scaler = canvasGo.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920f, 1080f);
        scaler.matchWidthOrHeight = 0.5f;

        canvasGo.AddComponent<GraphicRaycaster>();
    }

    private static void CreateCamera(Transform parent)
    {
        var go = CreateRoot("Main Camera", parent);
        go.tag = "MainCamera";
        go.transform.position = new Vector3(0f, 12f, -10f);
        go.transform.rotation = Quaternion.Euler(50f, 0f, 0f);

        var camera = go.AddComponent<Camera>();
        camera.orthographic = true;
        camera.orthographicSize = 8f;
        camera.clearFlags = CameraClearFlags.SolidColor;
        camera.backgroundColor = new Color(0.05f, 0.055f, 0.07f, 1f);
    }

    private static void CreateLight(string name, Transform parent, LightType type, Vector3 positionOrEuler, float intensity)
    {
        var go = CreateRoot(name, parent);
        var light = go.AddComponent<Light>();
        light.type = type;
        light.intensity = intensity;

        if (type == LightType.Directional)
        {
            go.transform.rotation = Quaternion.Euler(positionOrEuler);
        }
        else
        {
            go.transform.position = positionOrEuler;
            light.range = 12f;
        }
    }
}
