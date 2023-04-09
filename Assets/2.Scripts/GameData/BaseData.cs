using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// data의 기본 클래스
/// data가 저장될 폴더명을 가지고 있고,
/// 데이터의 갯수과 이름의 목록 리스트를 얻을 수 있다.
/// </summary>
public class BaseData : ScriptableObject
{
    public const string dataDirectory = "/9.Resources/Resources/Data/";
    public List<string> dataNameList = new List<string>();

    public int GetDataCount()
    {
        int dataCount = 0;
        if (this.dataNameList != null)
        {
            dataCount = this.dataNameList.Count;
        }

        return dataCount;
    }
    
    /// <summary>
    /// 툴에 출력하기 위한 이름 목록을 만들어주는 함수
    /// </summary>
    public List<string> GetNameList()
    {
        List<string> retList = new List<string>();
        if (dataNameList != null)
        {
            retList = this.dataNameList.ToList();
        }

        return retList;
    }

    public virtual int AddData()
    {
        return GetDataCount();
    }

    public virtual void RemoveData(int index)
    {
        
    }
}
