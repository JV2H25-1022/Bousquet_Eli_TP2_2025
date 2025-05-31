#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class TagReorderWindow : EditorWindow
{
    private List<string> tags = new List<string>();
    private Vector2 scrollPos;

    [MenuItem("Tools/Tags/Reorder Tags")]
    public static void OpenWindow()
    {
        GetWindow<TagReorderWindow>("Reorder Tags");
    }

    private void OnEnable()
    {
        LoadTags();
    }

    private void LoadTags()
    {
        tags.Clear();

        Object[] asset = AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset");
        if (asset != null && asset.Length > 0)
        {
            SerializedObject tagManager = new SerializedObject(asset[0]);
            SerializedProperty tagsProp = tagManager.FindProperty("tags");

            for (int i = 0; i < tagsProp.arraySize; i++)
            {
                string tag = tagsProp.GetArrayElementAtIndex(i).stringValue;
                tags.Add(tag);
            }
        }
    }

    private void SaveTags()
    {
        Object[] asset = AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset");
        if (asset != null && asset.Length > 0)
        {
            SerializedObject tagManager = new SerializedObject(asset[0]);
            SerializedProperty tagsProp = tagManager.FindProperty("tags");

            tagsProp.ClearArray();

            for (int i = 0; i < tags.Count; i++)
            {
                tagsProp.InsertArrayElementAtIndex(i);
                tagsProp.GetArrayElementAtIndex(i).stringValue = tags[i];
            }

            tagManager.ApplyModifiedProperties();
            Debug.Log("Tags saved in new order.");
        }
    }

    private void OnGUI()
    {
        EditorGUILayout.LabelField("Drag tags up or down to reorder", EditorStyles.boldLabel);
        EditorGUILayout.Space();

        scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
        for (int i = 0; i < tags.Count; i++)
        {
            EditorGUILayout.BeginHorizontal();

            if (GUILayout.Button("▲", GUILayout.Width(30)) && i > 0)
            {
                (tags[i - 1], tags[i]) = (tags[i], tags[i - 1]);
            }

            if (GUILayout.Button("▼", GUILayout.Width(30)) && i < tags.Count - 1)
            {
                (tags[i + 1], tags[i]) = (tags[i], tags[i + 1]);
            }

            EditorGUILayout.LabelField(tags[i], GUILayout.ExpandWidth(true));
            EditorGUILayout.EndHorizontal();
        }
        EditorGUILayout.EndScrollView();

        EditorGUILayout.Space();
        if (GUILayout.Button("Save Order"))
        {
            SaveTags();
        }

        if (GUILayout.Button("Reload Tags"))
        {
            LoadTags();
        }
    }
}
#endif
