using UnityEngine;
using Data;

public class DataStorage : MonoBehaviour
{
    public static DataStorage Instance { get; private set; }

    [Header("Wafer 데이터")]
    public LotsList lotsList;

    [Header("스택맵 데이터")]
    public RawStackMapList rawStackMapList;
    public StackMapList stackMapList;
    public StackMapArgumentList stackMapArgumentList;

    [Header("노잉크맵 데이터")]
    public NoInkMapList noInkMapList;

    [Header("노잉크맵 컬러 등급 데이터")]
    public NoInkMapGradeColorList noInkMapGradeColorList;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject); // 필요 시 생존 유지
    }

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
