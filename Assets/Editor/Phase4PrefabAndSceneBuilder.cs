using System;
using System.IO;
using MagicalTower.Content;
using MagicalTower.Runtime;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

public static class Phase4PrefabAndSceneBuilder
{
    private const string ScenePath = "Assets/Scenes/MagicalTowerPrototype.unity";
    private const string PrefabFolder = "Assets/Prefabs/Gameplay";
    private const string TowerPrefabPath = PrefabFolder + "/Tower.prefab";
    private const string EnemyPrefabPath = PrefabFolder + "/EnemyAgent.prefab";
    private const string FireballPrefabPath = PrefabFolder + "/FireballProjectile.prefab";
    private const string BarragePrefabPath = PrefabFolder + "/BarrageProjectile.prefab";

    [MenuItem("Magical Tower/Phase 4/Create Prefabs And Wire Scene")]
    public static void CreatePrefabsAndWireScene()
    {
        EnsureSceneLoaded();
        EnsureFolder(PrefabFolder);

        var towerDefinition = LoadRequired<TowerDefinition>("Assets/Content/Tower/Tower_Default.asset");
        var poolConfig = LoadRequired<EnemyPoolConfig>("Assets/Content/Pooling/EnemyPool_Default.asset");
        var spawnSchedule = LoadRequired<SpawnScheduleDefinition>("Assets/Content/Spawning/SpawnSchedule_Default.asset");
        var fireballSpell = LoadRequired<SpellDefinition>("Assets/Content/Spells/Spell_Fireball.asset");
        var barrageSpell = LoadRequired<SpellDefinition>("Assets/Content/Spells/Spell_Barrage.asset");
        var fireballProjectile = LoadRequired<ProjectileDefinition>("Assets/Content/Projectiles/Projectile_Fireball.asset");
        var barrageProjectile = LoadRequired<ProjectileDefinition>("Assets/Content/Projectiles/Projectile_Barrage.asset");

        var gameRoot = FindSceneObject("GameRoot");
        var tower = FindSceneObject("GameRoot/GameplayRoot/Tower");
        var enemySpawnRoot = FindSceneObject("GameRoot/GameplayRoot/EnemySpawnRoot");
        var enemyPoolRoot = FindSceneObject("GameRoot/GameplayRoot/EnemyPoolRoot");
        var projectileRoot = FindSceneObject("GameRoot/GameplayRoot/ProjectileRoot");
        var mainCameraObject = FindSceneObject("GameRoot/CameraRoot/Main Camera");

        var serviceRegistry = GetOrAdd<RuntimeServiceRegistry>(gameRoot);
        var messageBus = GetOrAdd<RuntimeMessageBus>(gameRoot);
        var gameSession = GetOrAdd<GameSession>(gameRoot);
        var enemyRegistry = GetOrAdd<ActiveEnemyRegistry>(gameRoot);
        var compositionRoot = GetOrAdd<GameplayCompositionRoot>(gameRoot);
        var towerHealth = GetOrAdd<TowerHealth>(tower);
        var spellScheduler = GetOrAdd<TowerSpellScheduler>(tower);
        var enemyPool = GetOrAdd<EnemyPool>(enemyPoolRoot);
        var enemySpawner = GetOrAdd<EnemySpawner>(enemySpawnRoot);
        var viewCamera = GetRequired<Camera>(mainCameraObject);

        ConfigureTowerSceneObject(tower, towerDefinition, messageBus, gameSession);
        CreateTowerPrefab(tower, towerDefinition, messageBus, gameSession);
        var enemyPrefab = CreateEnemyPrefab();
        var fireballPrefab = CreateLinearProjectilePrefab(FireballPrefabPath, "FireballProjectile", fireballProjectile, 0.35f);
        var barragePrefab = CreateArcProjectilePrefab(BarragePrefabPath, "BarrageProjectile", barrageProjectile, 0.25f);

        SetObject(poolConfig, "enemyPrefab", enemyPrefab.gameObject);
        SetObject(gameSession, "messageBus", messageBus);

        ConfigureSpellBindings(spellScheduler, "spells", fireballSpell, fireballPrefab, barrageSpell, barragePrefab);
        SetObject(spellScheduler, "enemyRegistry", enemyRegistry);
        SetObject(spellScheduler, "projectileRoot", projectileRoot.transform);
        SetObject(spellScheduler, "messageBus", messageBus);
        SetObject(spellScheduler, "viewCamera", viewCamera);

        SetObject(enemyPool, "config", poolConfig);
        SetObject(enemyPool, "enemyPrefab", enemyPrefab);
        SetObject(enemyPool, "poolRoot", enemyPoolRoot.transform);
        SetObject(enemyPool, "registry", enemyRegistry);
        SetObject(enemyPool, "messageBus", messageBus);
        SetObject(enemyPool, "targetTower", towerHealth);

        SetObject(enemySpawner, "schedule", spawnSchedule);
        SetObject(enemySpawner, "enemyPool", enemyPool);
        SetObject(enemySpawner, "gameSession", gameSession);
        SetObject(enemySpawner, "spawnRoot", enemySpawnRoot.transform);
        SetObject(enemySpawner, "targetTower", towerHealth);
        SetObject(enemySpawner, "viewCamera", viewCamera);

        SetObject(compositionRoot, "serviceRegistry", serviceRegistry);
        SetObject(compositionRoot, "messageBus", messageBus);
        SetObject(compositionRoot, "gameSession", gameSession);
        SetObject(compositionRoot, "towerHealth", towerHealth);
        SetObject(compositionRoot, "enemyRegistry", enemyRegistry);
        SetObject(compositionRoot, "enemyPool", enemyPool);
        SetObject(compositionRoot, "enemySpawner", enemySpawner);
        SetObject(compositionRoot, "spellScheduler", spellScheduler);
        SetObject(compositionRoot, "enemySpawnRoot", enemySpawnRoot.transform);
        SetObject(compositionRoot, "enemyPoolRoot", enemyPoolRoot.transform);
        SetObject(compositionRoot, "projectileRoot", projectileRoot.transform);
        SetObject(compositionRoot, "viewCamera", viewCamera);
        SetObject(compositionRoot, "towerDefinition", towerDefinition);
        SetObject(compositionRoot, "enemyPoolConfig", poolConfig);
        SetObject(compositionRoot, "spawnSchedule", spawnSchedule);
        SetObject(compositionRoot, "enemyPrefab", enemyPrefab);
        ConfigureSpellBindings(compositionRoot, "spellBindings", fireballSpell, fireballPrefab, barrageSpell, barragePrefab);

        EditorUtility.SetDirty(poolConfig);
        EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
        EditorSceneManager.SaveScene(SceneManager.GetActiveScene());
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("Phase 4 prefabs created and MagicalTowerPrototype scene wired explicitly.");
    }

    private static void ConfigureTowerSceneObject(
        GameObject tower,
        TowerDefinition definition,
        RuntimeMessageBus messageBus,
        GameSession gameSession)
    {
        GetOrAdd<MeshFilter>(tower);
        GetOrAdd<MeshRenderer>(tower);
        GetOrAdd<CapsuleCollider>(tower);

        var towerHealth = GetOrAdd<TowerHealth>(tower);
        SetObject(towerHealth, "definition", definition);
        SetObject(towerHealth, "messageBus", messageBus);
        SetObject(towerHealth, "gameSession", gameSession);
    }

    private static void CreateTowerPrefab(
        GameObject tower,
        TowerDefinition definition,
        RuntimeMessageBus messageBus,
        GameSession gameSession)
    {
        ConfigureTowerSceneObject(tower, definition, messageBus, gameSession);
        PrefabUtility.SaveAsPrefabAsset(tower, TowerPrefabPath);
    }

    private static EnemyAgent CreateEnemyPrefab()
    {
        var root = GameObject.CreatePrimitive(PrimitiveType.Capsule);
        root.name = "EnemyAgent";
        root.SetActive(false);
        root.transform.localScale = Vector3.one;

        var agent = GetOrAdd<EnemyAgent>(root);
        var movement = GetOrAdd<EnemyMovementController>(root);
        var attack = GetOrAdd<EnemyAttackController>(root);
        var status = GetOrAdd<StatusEffectController>(root);

        SetObject(agent, "movementController", movement);
        SetObject(agent, "attackController", attack);
        SetObject(agent, "statusEffectController", status);
        SetObject(movement, "agent", agent);
        SetObject(attack, "agent", agent);
        SetObject(status, "damageReceiverSource", agent);

        var saved = PrefabUtility.SaveAsPrefabAsset(root, EnemyPrefabPath);
        Object.DestroyImmediate(root);
        return GetRequired<EnemyAgent>(saved);
    }

    private static GameObject CreateLinearProjectilePrefab(
        string path,
        string name,
        ProjectileDefinition definition,
        float scale)
    {
        var root = CreateProjectileRoot(name, scale);
        var projectile = GetOrAdd<LinearExplosiveProjectile>(root);
        SetObject(projectile, "definition", definition);

        var saved = PrefabUtility.SaveAsPrefabAsset(root, path);
        Object.DestroyImmediate(root);
        return saved;
    }

    private static GameObject CreateArcProjectilePrefab(
        string path,
        string name,
        ProjectileDefinition definition,
        float scale)
    {
        var root = CreateProjectileRoot(name, scale);
        var projectile = GetOrAdd<ArcTargetProjectile>(root);
        SetObject(projectile, "definition", definition);

        var saved = PrefabUtility.SaveAsPrefabAsset(root, path);
        Object.DestroyImmediate(root);
        return saved;
    }

    private static GameObject CreateProjectileRoot(string name, float scale)
    {
        var root = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        root.name = name;
        root.transform.localScale = Vector3.one * scale;

        var collider = GetRequired<SphereCollider>(root);
        collider.isTrigger = true;

        var rigidbody = GetOrAdd<Rigidbody>(root);
        rigidbody.isKinematic = true;
        rigidbody.useGravity = false;

        return root;
    }

    private static void ConfigureSpellBindings(
        Object target,
        string propertyName,
        SpellDefinition fireballSpell,
        GameObject fireballProjectilePrefab,
        SpellDefinition barrageSpell,
        GameObject barrageProjectilePrefab)
    {
        var serializedObject = new SerializedObject(target);
        var property = serializedObject.FindProperty(propertyName);
        if (property == null)
        {
            throw new InvalidOperationException($"{target.name} has no serialized property named {propertyName}.");
        }

        property.arraySize = 2;
        ConfigureSpellBinding(property.GetArrayElementAtIndex(0), fireballSpell, fireballProjectilePrefab);
        ConfigureSpellBinding(property.GetArrayElementAtIndex(1), barrageSpell, barrageProjectilePrefab);
        serializedObject.ApplyModifiedPropertiesWithoutUndo();
        EditorUtility.SetDirty(target);
    }

    private static void ConfigureSpellBinding(SerializedProperty property, SpellDefinition spell, GameObject projectilePrefab)
    {
        property.FindPropertyRelative("definition").objectReferenceValue = spell;
        property.FindPropertyRelative("projectilePrefab").objectReferenceValue = projectilePrefab;
    }

    private static void EnsureSceneLoaded()
    {
        if (SceneManager.GetActiveScene().path == ScenePath)
        {
            return;
        }

        EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);
    }

    private static GameObject FindSceneObject(string path)
    {
        var parts = path.Split('/');
        foreach (var root in SceneManager.GetActiveScene().GetRootGameObjects())
        {
            if (root.name != parts[0])
            {
                continue;
            }

            var current = root.transform;
            for (var i = 1; i < parts.Length; i++)
            {
                current = FindDirectChild(current, parts[i]);
                if (current == null)
                {
                    break;
                }
            }

            if (current != null)
            {
                return current.gameObject;
            }
        }

        throw new InvalidOperationException($"Scene object not found: {path}");
    }

    private static Transform FindDirectChild(Transform parent, string childName)
    {
        for (var i = 0; i < parent.childCount; i++)
        {
            var child = parent.GetChild(i);
            if (child.name == childName)
            {
                return child;
            }
        }

        return null;
    }

    private static T LoadRequired<T>(string path) where T : Object
    {
        var asset = AssetDatabase.LoadAssetAtPath<T>(path);
        if (asset == null)
        {
            throw new InvalidOperationException($"Required asset is missing: {path}");
        }

        return asset;
    }

    private static T GetRequired<T>(GameObject gameObject) where T : Component
    {
        var component = gameObject.GetComponent<T>();
        if (component == null)
        {
            throw new InvalidOperationException($"{gameObject.name} is missing required component {typeof(T).Name}.");
        }

        return component;
    }

    private static T GetOrAdd<T>(GameObject gameObject) where T : Component
    {
        var component = gameObject.GetComponent<T>();
        return component != null ? component : gameObject.AddComponent<T>();
    }

    private static void SetObject(Object target, string propertyName, Object value)
    {
        var serializedObject = new SerializedObject(target);
        var property = serializedObject.FindProperty(propertyName);
        if (property == null)
        {
            throw new InvalidOperationException($"{target.name} has no serialized property named {propertyName}.");
        }

        property.objectReferenceValue = value;
        serializedObject.ApplyModifiedPropertiesWithoutUndo();
        EditorUtility.SetDirty(target);
    }

    private static void EnsureFolder(string path)
    {
        if (AssetDatabase.IsValidFolder(path))
        {
            return;
        }

        var parent = Path.GetDirectoryName(path)?.Replace("\\", "/");
        if (!string.IsNullOrEmpty(parent))
        {
            EnsureFolder(parent);
        }

        AssetDatabase.CreateFolder(parent, Path.GetFileName(path));
    }
}
