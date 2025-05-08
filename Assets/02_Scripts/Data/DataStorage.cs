using UnityEngine;
using Data;
using VInspector;
[System.Serializable]
public class DataStorage
{
    [Tab("Lots")]
    [Header("Wafer 데이터")]
    public LotsList lotsList;
    [Tab("Stack Map")]
    [Header("스택맵 데이터")]
    public RawStackMapList rawStackMapList;
    public StackMapList stackMapList;
    public StackMapArgumentList stackMapArgumentList;
    [Tab("NoInkMap")]
    [Header("노잉크맵 데이터")]
    public NoInkMapList noInkMapList;
    [Header("노잉크맵 컬러 등급 데이터")]
    public NoInkMapGradeColorList noInkMapGradeColorList;

    public void SetLots(LotsList lots)
    {
        lotsList = lots;
    }

    public void SetStackMap(RawStackMapList raw)
    {
        rawStackMapList = raw;
        stackMapList = new StackMapList();
        stackMapArgumentList = new StackMapArgumentList();
        raw.Populate(stackMapList, stackMapArgumentList);
    }

    public void SetNoInkMap(NoInkMapList list)
    {
        noInkMapList = list;
    }

    public void SetNoInkMapGradeColor(NoInkMapGradeColorList list)
    {
        noInkMapGradeColorList = list;
    }
}
