using MagicalTower.Content;
using UnityEditor;
using UnityEngine;

public static class Phase2ContentBuilder
{
    [MenuItem("Magical Tower/Phase 2/Create Content Assets")]
    public static void CreateContentAssets()
    {
        EnsureFolder("Assets/Content/Tower");
        EnsureFolder("Assets/Content/Enemies");
        EnsureFolder("Assets/Content/Spells");
        EnsureFolder("Assets/Content/Projectiles");
        EnsureFolder("Assets/Content/StatusEffects");
        EnsureFolder("Assets/Content/Spawning");
        EnsureFolder("Assets/Content/Pooling");

        var tower = LoadOrCreate<TowerDefinition>("Assets/Content/Tower/Tower_Default.asset");
        SetInt(tower, "maxHealth", 100);

        var defaultEnemy = LoadOrCreate<EnemyDefinition>("Assets/Content/Enemies/Enemy_Default.asset");
        ConfigureEnemy(defaultEnemy, "Default Enemy", 30, 2f, 5, 1f, 1f);

        var fastEnemy = LoadOrCreate<EnemyDefinition>("Assets/Content/Enemies/Enemy_Fast.asset");
        ConfigureEnemy(fastEnemy, "Fast Enemy", 20, 3.5f, 3, 0.8f, 0.8f);

        var bigSlowEnemy = LoadOrCreate<EnemyDefinition>("Assets/Content/Enemies/Enemy_BigSlow.asset");
        ConfigureEnemy(bigSlowEnemy, "Big Slow Enemy", 80, 1.1f, 10, 1.4f, 1.6f);

        var burning = LoadOrCreate<BurningStatusEffectDefinition>("Assets/Content/StatusEffects/Status_Burning.asset");
        SetInt(burning, "damagePerTick", 4);
        SetFloat(burning, "duration", 3f);
        SetFloat(burning, "tickInterval", 1f);
        SetEnum(burning, "stackPolicy", StatusStackPolicy.RefreshDuration);

        var fireballProjectile = LoadOrCreate<ProjectileDefinition>("Assets/Content/Projectiles/Projectile_Fireball.asset");
        ConfigureProjectile(fireballProjectile, 25, 8f, 2f, 0f, burning);

        var barrageProjectile = LoadOrCreate<ProjectileDefinition>("Assets/Content/Projectiles/Projectile_Barrage.asset");
        ConfigureProjectile(barrageProjectile, 10, 7f, 0f, 3f, null);

        var fireballSpell = LoadOrCreate<SpellDefinition>("Assets/Content/Spells/Spell_Fireball.asset");
        ConfigureSpell(fireballSpell, "Fireball Spell", 2.5f, fireballProjectile, SpellTargetMode.RandomEnemyDirection);

        var barrageSpell = LoadOrCreate<SpellDefinition>("Assets/Content/Spells/Spell_Barrage.asset");
        ConfigureSpell(barrageSpell, "Barrage Spell", 5f, barrageProjectile, SpellTargetMode.VisibleEnemies);

        var spawnSchedule = LoadOrCreate<SpawnScheduleDefinition>("Assets/Content/Spawning/SpawnSchedule_Default.asset");
        ConfigureSpawnSchedule(spawnSchedule, defaultEnemy, fastEnemy, bigSlowEnemy);

        var poolConfig = LoadOrCreate<EnemyPoolConfig>("Assets/Content/Pooling/EnemyPool_Default.asset");
        SetObject(poolConfig, "enemyPrefab", null);
        SetInt(poolConfig, "initialCapacity", 12);
        SetInt(poolConfig, "maxCapacity", 60);

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    private static void ConfigureEnemy(
        EnemyDefinition asset,
        string displayName,
        int maxHealth,
        float movementSpeed,
        int contactDamage,
        float attackInterval,
        float visualScale)
    {
        SetString(asset, "displayName", displayName);
        SetInt(asset, "maxHealth", maxHealth);
        SetFloat(asset, "movementSpeed", movementSpeed);
        SetInt(asset, "contactDamage", contactDamage);
        SetFloat(asset, "attackInterval", attackInterval);
        SetFloat(asset, "visualScale", visualScale);
    }

    private static void ConfigureProjectile(
        ProjectileDefinition asset,
        int damage,
        float speed,
        float impactRadius,
        float arcHeight,
        BurningStatusEffectDefinition burningStatusEffect)
    {
        SetInt(asset, "damage", damage);
        SetFloat(asset, "speed", speed);
        SetFloat(asset, "impactRadius", impactRadius);
        SetFloat(asset, "arcHeight", arcHeight);
        SetObject(asset, "burningStatusEffect", burningStatusEffect);
    }

    private static void ConfigureSpell(
        SpellDefinition asset,
        string displayName,
        float cooldown,
        ProjectileDefinition projectileDefinition,
        SpellTargetMode targetMode)
    {
        SetString(asset, "displayName", displayName);
        SetFloat(asset, "cooldown", cooldown);
        SetObject(asset, "projectileDefinition", projectileDefinition);
        SetEnum(asset, "targetMode", targetMode);
    }

    private static void ConfigureSpawnSchedule(
        SpawnScheduleDefinition asset,
        EnemyDefinition defaultEnemy,
        EnemyDefinition fastEnemy,
        EnemyDefinition bigSlowEnemy)
    {
        var serializedObject = new SerializedObject(asset);
        var timeBands = serializedObject.FindProperty("timeBands");
        timeBands.arraySize = 3;

        ConfigureBand(timeBands.GetArrayElementAtIndex(0), 0f, 2.2f, defaultEnemy, 8, fastEnemy, 2, null, 0);
        ConfigureBand(timeBands.GetArrayElementAtIndex(1), 30f, 1.5f, defaultEnemy, 5, fastEnemy, 5, null, 0);
        ConfigureBand(timeBands.GetArrayElementAtIndex(2), 60f, 1.1f, defaultEnemy, 4, fastEnemy, 4, bigSlowEnemy, 2);

        serializedObject.ApplyModifiedPropertiesWithoutUndo();
        EditorUtility.SetDirty(asset);
    }

    private static void ConfigureBand(
        SerializedProperty band,
        float startTime,
        float spawnInterval,
        EnemyDefinition firstEnemy,
        int firstWeight,
        EnemyDefinition secondEnemy,
        int secondWeight,
        EnemyDefinition thirdEnemy,
        int thirdWeight)
    {
        band.FindPropertyRelative("startTime").floatValue = startTime;
        band.FindPropertyRelative("spawnInterval").floatValue = spawnInterval;

        var enemies = band.FindPropertyRelative("enemies");
        enemies.arraySize = thirdEnemy == null ? 2 : 3;
        ConfigureEnemyEntry(enemies.GetArrayElementAtIndex(0), firstEnemy, firstWeight);
        ConfigureEnemyEntry(enemies.GetArrayElementAtIndex(1), secondEnemy, secondWeight);

        if (thirdEnemy != null)
        {
            ConfigureEnemyEntry(enemies.GetArrayElementAtIndex(2), thirdEnemy, thirdWeight);
        }
    }

    private static void ConfigureEnemyEntry(SerializedProperty entry, EnemyDefinition enemy, int weight)
    {
        entry.FindPropertyRelative("enemyDefinition").objectReferenceValue = enemy;
        entry.FindPropertyRelative("weight").intValue = weight;
    }

    private static T LoadOrCreate<T>(string path) where T : ScriptableObject
    {
        var asset = AssetDatabase.LoadAssetAtPath<T>(path);
        if (asset != null)
        {
            return asset;
        }

        asset = ScriptableObject.CreateInstance<T>();
        AssetDatabase.CreateAsset(asset, path);
        return asset;
    }

    private static void EnsureFolder(string path)
    {
        if (AssetDatabase.IsValidFolder(path))
        {
            return;
        }

        var parent = System.IO.Path.GetDirectoryName(path)?.Replace("\\", "/");
        if (!string.IsNullOrEmpty(parent))
        {
            EnsureFolder(parent);
        }

        AssetDatabase.CreateFolder(parent, System.IO.Path.GetFileName(path));
    }

    private static void SetString(Object asset, string propertyName, string value)
    {
        var property = GetProperty(asset, propertyName);
        property.stringValue = value;
        property.serializedObject.ApplyModifiedPropertiesWithoutUndo();
        EditorUtility.SetDirty(asset);
    }

    private static void SetInt(Object asset, string propertyName, int value)
    {
        var property = GetProperty(asset, propertyName);
        property.intValue = value;
        property.serializedObject.ApplyModifiedPropertiesWithoutUndo();
        EditorUtility.SetDirty(asset);
    }

    private static void SetFloat(Object asset, string propertyName, float value)
    {
        var property = GetProperty(asset, propertyName);
        property.floatValue = value;
        property.serializedObject.ApplyModifiedPropertiesWithoutUndo();
        EditorUtility.SetDirty(asset);
    }

    private static void SetEnum<T>(Object asset, string propertyName, T value) where T : System.Enum
    {
        var property = GetProperty(asset, propertyName);
        property.enumValueIndex = System.Convert.ToInt32(value);
        property.serializedObject.ApplyModifiedPropertiesWithoutUndo();
        EditorUtility.SetDirty(asset);
    }

    private static void SetObject(Object asset, string propertyName, Object value)
    {
        var property = GetProperty(asset, propertyName);
        property.objectReferenceValue = value;
        property.serializedObject.ApplyModifiedPropertiesWithoutUndo();
        EditorUtility.SetDirty(asset);
    }

    private static SerializedProperty GetProperty(Object asset, string propertyName)
    {
        var serializedObject = new SerializedObject(asset);
        return serializedObject.FindProperty(propertyName);
    }
}
