using UnityEngine;
using UnityEditor;
using System;

public class GlobalAsset
{
    [MenuItem("Assets/Create/UFE Config")]
    public static void CreateAsset ()
    {
        ScriptableObjectUtility.CreateAsset<GlobalInfo> ();
    }
}
