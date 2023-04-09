using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class EditorHelper 
{
    public static string GetPath(UnityEngine.Object clip)
    {
        string dataPath = string.Empty;
        dataPath = AssetDatabase.GetAssetPath(clip);
        string[] pathNode = dataPath.Split('/'); //Assets/9.ResourcesData/Resources/Sound/BGM.wav
        bool findResource = false;
        for (int i = 0; i < pathNode.Length - 1; i++)
        {
            if (findResource == false)
            {
                if (pathNode[i] == "Resources")
                {
                    findResource = true;
                    dataPath = string.Empty;
                }
            }
            else
            {
                dataPath += pathNode[i] + "/";
            }
        }

        return dataPath;
    }
}
