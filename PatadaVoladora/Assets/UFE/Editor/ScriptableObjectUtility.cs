using UnityEngine;
using UnityEditor;
using System.IO;

public static class ScriptableObjectUtility
{
    public static void CreateAsset<T> () where T : ScriptableObject
    {
        T asset = ScriptableObject.CreateInstance<T> ();
        
        string path = AssetDatabase.GetAssetPath (Selection.activeObject);
        if (path == "") 
        {
            path = "Assets";
        } 
        else if (Path.GetExtension (path) != "") 
        {
            path = path.Replace (Path.GetFileName (AssetDatabase.GetAssetPath (Selection.activeObject)), "");
        }
		
        string fileName;
		if (asset is MoveInfo) {
			fileName = "New Move";
		}else if (asset is CharacterInfo) {
			fileName = "New Character";
		}else if (asset is GlobalInfo) {
			fileName = "UFE_Config";
		}else{
			fileName = typeof(T).ToString();
		}
        string assetPathAndName = AssetDatabase.GenerateUniqueAssetPath (path + "/" + fileName + ".asset");
        
        AssetDatabase.CreateAsset (asset, assetPathAndName);
        
        AssetDatabase.SaveAssets ();
        EditorUtility.FocusProjectWindow ();
        Selection.activeObject = asset;
		
		if (asset is MoveInfo) {
			//MoveEditorWindow.Init();
		}else if (asset is GlobalInfo) {
			//GlobalEditorWindow.Init();
		}else if (asset is CharacterInfo) {
			CharacterEditorWindow.Init();
		}
		
    }
}
