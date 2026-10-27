#if UNITY_EDITOR
using System.Collections.Generic;
using System.Text;
using UnityEditor;
using UnityEngine;
using System.IO;


public class DialogueImporterWindow : EditorWindow
{
    private TextAsset _csvFile;
    private DialogueDatabase _targetDatabase;
    private SerializedObject _serializedDb;
    private Vector2 _scrollPosition;
    private const string SpriteRoot = "Assets/Art/UI/keyword";

    [MenuItem("Tools/Dialogue Importer Tool")]
    public static void ShowWindow()
    {
        GetWindow<DialogueImporterWindow>("Dialogue Importer");
    }

    private void OnGUI()
    {
        GUILayout.Label("IMPORT DIALOGUE FROM CSV", EditorStyles.boldLabel);
        EditorGUILayout.Space(10);

        EditorGUI.BeginChangeCheck();
        _targetDatabase = (DialogueDatabase)EditorGUILayout.ObjectField(
            "Target Database", _targetDatabase, typeof(DialogueDatabase), false);
        if (EditorGUI.EndChangeCheck())
        {
            _serializedDb = _targetDatabase != null ? new SerializedObject(_targetDatabase) : null;
        }

        _csvFile = (TextAsset)EditorGUILayout.ObjectField("CSV File", _csvFile, typeof(TextAsset), false);

        EditorGUILayout.Space(10);

        if (GUILayout.Button("Import CSV to Database", GUILayout.Height(30)))
        {
            if (_targetDatabase == null || _csvFile == null)
            {
                EditorUtility.DisplayDialog("Lỗi", "Vui lòng chọn đầy đủ Target Database và File CSV!", "OK");
                return;
            }
            ImportCSV();
        }

        EditorGUILayout.Space(20);
        GUILayout.Label("XEM TRƯỚC VÀ CHỈNH SỬA TẠI CỬA SỔ", EditorStyles.boldLabel);

        if (_serializedDb != null)
        {
            _serializedDb.Update();
            SerializedProperty linesProp = _serializedDb.FindProperty("_lines");
            if (linesProp != null)
            {
                _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);
                EditorGUILayout.PropertyField(linesProp, true);
                EditorGUILayout.EndScrollView();
                _serializedDb.ApplyModifiedProperties();
            }
        }
    }

    // Accepts "bed", "Melody/bed" or "Melody/bed.png"
    private Sprite FindSprite(string key, int row)
    {
        key = key.Replace('\\', '/').Trim();
        string name = Path.GetFileNameWithoutExtension(key);
        string folderHint = Path.GetDirectoryName(key)?.Replace('\\', '/');

        string[] guids = AssetDatabase.FindAssets($"{name} t:Sprite", new[] { SpriteRoot });

        var matches = new List<string>();
        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);

            // FindAssets matches substrings, so keep only the exact file name
            if (!string.Equals(Path.GetFileNameWithoutExtension(path), name,
                    System.StringComparison.OrdinalIgnoreCase)) continue;

            // If a folder was given ("Melody/bed"), it must be in that path
            if (!string.IsNullOrEmpty(folderHint) &&
                path.IndexOf("/" + folderHint + "/", System.StringComparison.OrdinalIgnoreCase) < 0) continue;

            matches.Add(path);
        }

        if (matches.Count == 0)
        {
            Debug.LogWarning($"[DialogueImporter] Row {row}: no sprite '{key}' under {SpriteRoot}");
            return null;
        }

        if (matches.Count > 1)
        {
            Debug.LogWarning($"[DialogueImporter] Row {row}: '{key}' matches {matches.Count} sprites, using {matches[0]}. " +
                             $"Add the folder in the CSV (e.g. Melody/{name}) to be specific.");
        }

        // Works for Single and Multiple sprite modes (sprites are sub-assets of the texture)
        Sprite fallback = null;
        foreach (Object asset in AssetDatabase.LoadAllAssetsAtPath(matches[0]))
        {
            if (asset is Sprite s)
            {
                if (s.name.Equals(name, System.StringComparison.OrdinalIgnoreCase)) return s;
                if (fallback == null) fallback = s;
            }
        }
        return fallback;
    }
    private void ImportCSV()
    {
        var db = new SerializedObject(_targetDatabase);
        SerializedProperty linesProp = db.FindProperty("_lines");
        if (linesProp == null)
        {
            EditorUtility.DisplayDialog("Lỗi", "Không tìm thấy field '_lines' trong DialogueDatabase.", "OK");
            return;
        }

        if (!EditorUtility.DisplayDialog("Xác nhận",
                $"Thao tác này sẽ XÓA toàn bộ dòng thoại hiện có trong '{_targetDatabase.name}' và thay bằng nội dung CSV.",
                "Import", "Hủy"))
        {
            return;
        }

        List<List<string>> rows = ParseCsv(_csvFile.text);

        Undo.RecordObject(_targetDatabase, "Import Dialogue CSV");
        db.Update();
        linesProp.ClearArray();

        int imported = 0, skipped = 0;
        bool warnedMissingField = false;

        // i = 1 bỏ qua dòng tiêu đề
        for (int i = 1; i < rows.Count; i++)
        {
            List<string> data = rows[i];
            if (data.Count < 2 || string.IsNullOrWhiteSpace(data[1])) { skipped++; continue; }

            string charName = data[0].Trim();
            string content = data[1].Trim();
            string animTrigger = data.Count >= 3 ? data[2].Trim() : string.Empty;
            string spritePath = data.Count >= 4 ? data[3].Trim() : string.Empty;

            Sprite portrait = null;
            if (!string.IsNullOrEmpty(spritePath))
            {
                portrait = FindSprite(spritePath, i + 1);
            }
            

            linesProp.InsertArrayElementAtIndex(linesProp.arraySize);
            SerializedProperty element = linesProp.GetArrayElementAtIndex(linesProp.arraySize - 1);

            element.FindPropertyRelative("_characterName").stringValue = charName;
            element.FindPropertyRelative("_content").stringValue = content;
            element.FindPropertyRelative("_animationTrigger").stringValue = animTrigger;

            SerializedProperty portraitProp = element.FindPropertyRelative("_characterPortrait");
            if (portraitProp != null) portraitProp.objectReferenceValue = portrait;

            imported++;
        }

        db.ApplyModifiedProperties();
        EditorUtility.SetDirty(_targetDatabase);
        AssetDatabase.SaveAssets();
        _serializedDb = new SerializedObject(_targetDatabase);

        EditorUtility.DisplayDialog("Thành công",
            $"Đã import {imported} dòng thoại. Bỏ qua {skipped} dòng trống/không hợp lệ.", "OK");
    }

    private static void SetString(SerializedProperty element, string field, string value, ref bool warned)
    {
        SerializedProperty prop = element.FindPropertyRelative(field);
        if (prop != null)
        {
            prop.stringValue = value;
        }
        else if (!warned)
        {
            Debug.LogWarning($"[DialogueImporter] DialogueLine has no field '{field}' - that column was ignored.");
            warned = true;
        }
    }

    // Parser CSV xử lý dấu ngoặc kép, dấu phẩy trong nội dung, xuống dòng trong ô, và BOM
    private static List<List<string>> ParseCsv(string text)
    {
        text = text.TrimStart('\uFEFF');

        string header = text.Split('\n')[0];
        char delimiter = header.Contains(";") && !header.Contains(",") ? ';' : ',';

        var rows = new List<List<string>>();
        var row = new List<string>();
        var field = new StringBuilder();
        bool inQuotes = false;

        for (int i = 0; i < text.Length; i++)
        {
            char c = text[i];

            if (inQuotes)
            {
                if (c == '"')
                {
                    if (i + 1 < text.Length && text[i + 1] == '"') { field.Append('"'); i++; }
                    else inQuotes = false;
                }
                else field.Append(c);
            }
            else if (c == '"') inQuotes = true;
            else if (c == delimiter) { row.Add(field.ToString()); field.Clear(); }
            else if (c == '\r') { }
            else if (c == '\n')
            {
                row.Add(field.ToString()); field.Clear();
                rows.Add(row); row = new List<string>();
            }
            else field.Append(c);
        }

        if (field.Length > 0 || row.Count > 0)
        {
            row.Add(field.ToString());
            rows.Add(row);
        }

        return rows;
    }
}
#endif
