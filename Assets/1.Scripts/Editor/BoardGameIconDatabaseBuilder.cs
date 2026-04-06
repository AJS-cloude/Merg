#if UNITY_EDITOR
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

/// <summary>
/// <c>kenney_board-game-icons/PNG/Double (128px)</c> 아래 PNG 스프라이트를 모두 수집해
/// <see cref="BoardGameIconDatabase"/>를 채웁니다.
/// </summary>
public static class BoardGameIconDatabaseBuilder
{
    const string KenneyFolder = "Assets/3.UI/Icon/kenney_board-game-icons/PNG/Double (128px)";

    const string DefaultAssetPath = "Assets/99.ETC/BoardGameIconDatabase.asset";

    [MenuItem("MergIdel/Rebuild Board Game Icon Database")]
    public static void Rebuild()
    {
        if (!AssetDatabase.IsValidFolder("Assets/3.UI/Icon/kenney_board-game-icons"))
        {
            Debug.LogError($"Kenney 폴더를 찾을 수 없습니다: {KenneyFolder}");
            return;
        }

        var guids = AssetDatabase.FindAssets("t:Sprite", new[] { KenneyFolder });
        var list = new List<BoardGameIconDatabase.Entry>();

        foreach (var guid in guids)
        {
            var path = AssetDatabase.GUIDToAssetPath(guid);
            if (string.IsNullOrEmpty(path) || Path.GetExtension(path).ToLowerInvariant() != ".png")
                continue;

            var sprite = AssetDatabase.LoadAssetAtPath<Sprite>(path);
            if (sprite == null)
                continue;

            var id = Path.GetFileNameWithoutExtension(path);
            list.Add(new BoardGameIconDatabase.Entry { id = id, sprite = sprite });
        }

        list = list.OrderBy(e => e.id, System.StringComparer.OrdinalIgnoreCase).ToList();

        var db = AssetDatabase.LoadAssetAtPath<BoardGameIconDatabase>(DefaultAssetPath);
        if (db == null)
        {
            db = ScriptableObject.CreateInstance<BoardGameIconDatabase>();
            AssetDatabase.CreateAsset(db, DefaultAssetPath);
        }

        var so = new SerializedObject(db);
        var prop = so.FindProperty("_entries");
        prop.ClearArray();
        for (int i = 0; i < list.Count; i++)
        {
            prop.InsertArrayElementAtIndex(i);
            var el = prop.GetArrayElementAtIndex(i);
            el.FindPropertyRelative("id").stringValue = list[i].id;
            el.FindPropertyRelative("sprite").objectReferenceValue = list[i].sprite;
        }

        so.ApplyModifiedProperties();
        db.RefreshLookup();
        EditorUtility.SetDirty(db);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log($"BoardGameIconDatabase: {list.Count}개 스프라이트 수집 → {DefaultAssetPath}");
    }
}
#endif
