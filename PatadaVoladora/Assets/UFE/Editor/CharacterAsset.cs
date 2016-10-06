using UnityEngine;
using UnityEditor;
using System;

public class CharacterAsset
{
    [MenuItem("Assets/Create/Character")]
    public static void CreateAsset ()
    {
        ScriptableObjectUtility.CreateAsset<CharacterInfo> ();
    }
}
