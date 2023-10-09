using UnityEditor;
using UnityEditor.AddressableAssets.Settings;
using UnityEngine;
 
public class ReadonlyRemover : EditorWindow
{
    private AddressableAssetGroup _addressableAssetGroup;
 
    [MenuItem("Window/Readonly Remover")]
    public static void ShowWindow()
    {
        GetWindow<ReadonlyRemover>("Readonly Remover");
    }
 
    private void OnGUI()
    {
        EditorGUILayout.LabelField("AddressableAssetGroup Readonly Remover", EditorStyles.boldLabel);
 
        EditorGUILayout.Space();
 
        _addressableAssetGroup = (AddressableAssetGroup)EditorGUILayout.ObjectField("Addressable Asset Group",
            _addressableAssetGroup, typeof(AddressableAssetGroup), false);
 
        if (GUILayout.Button("Remove Readonly Flag"))
        {
            if (_addressableAssetGroup != null)
            {
                RemoveReadonlyFlag();
            }
            else
            {
                Debug.LogError("AddressableAssetGroup is null. Please assign an AddressableAssetGroup.");
            }
        }
    }
 
    private void RemoveReadonlyFlag()
    {
        foreach (var entry in _addressableAssetGroup.entries)
            entry.ReadOnly = false;
 
        EditorUtility.SetDirty(_addressableAssetGroup);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("Readonly flags have been removed.");
    }
}