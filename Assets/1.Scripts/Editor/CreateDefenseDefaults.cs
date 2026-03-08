using UnityEngine;
using UnityEditor;

/// <summary>
/// 디펜스 모드 기본 데이터: EnemyData, WaveData 생성.
/// 메뉴: Tools > Merge Defense > Create Default Defense Data
/// </summary>
public static class CreateDefenseDefaults
{
    const string DataPath = "Assets/Data";

    [MenuItem("Tools/Merge Defense/Create Default Defense Data")]
    public static void CreateAll()
    {
        EnsureFolder(DataPath);
        EnsureFolder(DataPath + "/Enemies");
        EnsureFolder(DataPath + "/Waves");

        var weak = CreateEnemy("Enemy_Weak", "약한 적", 10f, 2f, 3);
        var normal = CreateEnemy("Enemy_Normal", "일반 적", 25f, 1.5f, 8);
        var strong = CreateEnemy("Enemy_Strong", "강한 적", 60f, 1f, 15);

        CreateWave("Wave_1", 1, new[] { (weak, 5) });
        CreateWave("Wave_2", 2, new[] { (weak, 8), (normal, 2) });
        CreateWave("Wave_3", 3, new[] { (weak, 5), (normal, 5), (strong, 1) });

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("Merge Defense default data created under Assets/Data/Enemies, Assets/Data/Waves.");
    }

    static void EnsureFolder(string path)
    {
        if (!AssetDatabase.IsValidFolder("Assets")) return;
        string[] parts = path.Replace("Assets/", "").Split('/');
        string current = "Assets";
        for (int i = 0; i < parts.Length; i++)
        {
            string next = current + "/" + parts[i];
            if (!AssetDatabase.IsValidFolder(next))
                AssetDatabase.CreateFolder(current, parts[i]);
            current = next;
        }
    }

    static EnemyData CreateEnemy(string id, string name, float hp, float speed, int gold)
    {
        string path = DataPath + "/Enemies/" + id + ".asset";
        var e = AssetDatabase.LoadAssetAtPath<EnemyData>(path);
        if (e != null) return e;
        e = ScriptableObject.CreateInstance<EnemyData>();
        e.enemyId = id;
        e.displayName = name;
        e.maxHp = hp;
        e.moveSpeed = speed;
        e.goldReward = gold;
        AssetDatabase.CreateAsset(e, path);
        return e;
    }

    static void CreateWave(string id, int index, (EnemyData enemy, int count)[] entries)
    {
        string path = DataPath + "/Waves/" + id + ".asset";
        if (AssetDatabase.LoadAssetAtPath<WaveData>(path) != null) return;
        var w = ScriptableObject.CreateInstance<WaveData>();
        w.waveId = id;
        w.waveIndex = index;
        w.entries = new WaveData.Entry[entries.Length];
        for (int i = 0; i < entries.Length; i++)
        {
            w.entries[i] = new WaveData.Entry { enemy = entries[i].enemy, count = entries[i].count };
        }
        w.delayBeforeNext = 2f;
        AssetDatabase.CreateAsset(w, path);
    }
}
