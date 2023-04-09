using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Resources.Load를 래필하는 클래스
/// 추 후, 어드레서블 에셋으로 변경할 예정.
/// </summary>
public class ResourceManager 
{
    public static UnityEngine.Object Load(string path)
    {
        return Resources.Load(path);
    }

    public static GameObject Instantiate(string path)
    {
        UnityEngine.Object source = Load(path);
        if (source == null)
        {
            return null;
        }
        return GameObject.Instantiate(source) as GameObject;
    }
}
