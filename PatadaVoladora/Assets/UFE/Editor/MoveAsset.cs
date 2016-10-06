using UnityEngine;
using UnityEditor;
using System;

public class MoveAsset
{
    [MenuItem("Assets/Create/Move")]
    public static void CreateAsset ()
    {
        ScriptableObjectUtility.CreateAsset<MoveInfo> ();
    }
}
