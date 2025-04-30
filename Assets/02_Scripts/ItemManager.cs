// ItemManager.cs
// Prefab 및 부모 Transform 참조만 저장하고,
// 외부에서 데이터 리스트를 받아 화면에 아이템을 생성하며
// 선택된 데이터를 프로퍼티에 보관하는 모듈

using UnityEngine;
using Data;
using System.Collections.Generic;

public class ItemManager : MonoBehaviour
{
    [Header("Prefabs")]
    [SerializeField] private GameObject waferPrefab;
    [SerializeField] private GameObject stackMapPrefab;
    [SerializeField] private GameObject noInkMapPrefab;
    [SerializeField] private GameObject gradeColorPrefab;

    [Header("Parents")]
    [SerializeField] private Transform waferParent;
    [SerializeField] private Transform stackMapParent;
    [SerializeField] private Transform noInkMapParent;
    [SerializeField] private Transform gradeColorParent;

    [Header("Parents")]
    [SerializeField] private NamedPipeManualClient namedPipe;

    /// <summary>선택된 Wafer 데이터</summary>
    public Wafer SelectedWafer { get; private set; }
    /// <summary>선택된 StackMap 데이터</summary>
    public StackMap SelectedStackMap { get; private set; }
    /// <summary>선택된 NoInkMap 데이터</summary>
    public NoInkMap SelectedNoInkMap { get; private set; }
    /// <summary>선택된 GradeColor 데이터</summary>
    public NoInkMapGradeColor SelectedGradeColor { get; private set; }

    #region Item Spawner 아이템을 생성 
    /// <summary>
    /// Wafer 리스트를 받아 화면에 WaferView 인스턴스를 생성
    /// 클릭 시 OnWaferClicked를 호출하여 SelectedWafer에 저장
    /// </summary>
    public void SpawnWafers(List<Wafer> wafers)
    {
        foreach (Transform child in waferParent)
            Destroy(child.gameObject);

        foreach (var wafer in wafers)
        {
            var go = Instantiate(waferPrefab, waferParent);
            var view = go.GetComponent<WaferView>();
            view.Init(wafer);
            view.OnClicked += OnWaferClicked;
        }
    }

    /// <summary>
    /// StackMap 리스트를 받아 화면에 StackMapView 인스턴스를 생성
    /// 클릭 시 OnStackMapClicked를 호출하여 SelectedStackMap에 저장
    /// </summary>
    public void SpawnStackMaps(List<StackMap> maps)
    {
        foreach (Transform child in stackMapParent)
            Destroy(child.gameObject);

        foreach (var map in maps)
        {
            var go = Instantiate(stackMapPrefab, stackMapParent);
            var view = go.GetComponent<StackMapView>();
            view.Init(map);
            view.OnClicked += OnStackMapClicked;
        }
    }

    /// <summary>
    /// NoInkMap 리스트를 받아 화면에 NoInkMapView 인스턴스를 생성
    /// 클릭 시 OnNoInkMapClicked를 호출하여 SelectedNoInkMap에 저장
    /// </summary>
    public void SpawnNoInkMaps(List<NoInkMap> maps)
    {
        foreach (Transform child in noInkMapParent)
            Destroy(child.gameObject);

        foreach (var map in maps)
        {
            var go = Instantiate(noInkMapPrefab, noInkMapParent);
            var view = go.GetComponent<NoInkMapView>();
            view.Init(map);
            view.OnClicked += OnNoInkMapClicked;
        }
    }

    /// <summary>
    /// GradeColor 리스트를 받아 화면에 NoInkMapGradeColorView 인스턴스를 생성
    /// 클릭 시 OnGradeColorClicked를 호출하여 SelectedGradeColor에 저장
    /// </summary>
    public void SpawnGradeColors(List<NoInkMapGradeColor> grades)
    {
        foreach (Transform child in gradeColorParent)
            Destroy(child.gameObject);

        foreach (var grade in grades)
        {
            var go = Instantiate(gradeColorPrefab, gradeColorParent);
            var view = go.GetComponent<NoInkMapGradeColorView>();
            view.Init(grade);
            view.OnClicked += OnGradeColorClicked;
        }
    }
    #endregion

    private CommandWrapper<T> CommandWrapping<T>(string commend, T instance)
    {
        CommandWrapper<T> wrapper = new CommandWrapper<T>
        {
            command = commend,
            payload = instance,
        };

        return wrapper;
    }

    #region 클릭 핸들러 (선택된 데이터를 프로퍼티에 저장)

    /// <summary>
    /// WaferView 클릭 시 호출됩니다.
    /// SelectedWafer에 저장합니다.
    /// </summary>
    protected virtual void OnWaferClicked(Wafer wafer)
    {
        SelectedWafer = wafer;
        Debug.Log($"SelectedWafer: LOT={wafer.LOT_ID}, WF={wafer.WF_ID}");

        var wrapper = CommandWrapping<Wafer>("GetStackMap", wafer);
        string json = JsonUtility.ToJson(wrapper, false);
        namedPipe.Send(json);
    }

    /// <summary>
    /// StackMapView 클릭 시 호출됩니다.
    /// SelectedStackMap에 저장합니다.
    /// </summary>
    protected virtual void OnStackMapClicked(StackMap map)
    {
        SelectedStackMap = map;
        Debug.Log($"SelectedStackMap: LOT={map.LOT_ID}, WF={map.WF_ID}");
    }

    /// <summary>
    /// NoInkMapView 클릭 시 호출됩니다.
    /// SelectedNoInkMap에 저장합니다.
    /// </summary>
    protected virtual void OnNoInkMapClicked(NoInkMap map)
    {
        SelectedNoInkMap = map;
        Debug.Log($"SelectedNoInkMap: LOT={map.LOT_ID}, WF={map.WF_ID}");
    }

    /// <summary>
    /// NoInkMapGradeColorView 클릭 시 호출됩니다.
    /// SelectedGradeColor에 저장합니다.
    /// </summary>
    protected virtual void OnGradeColorClicked(NoInkMapGradeColor grade)
    {
        SelectedGradeColor = grade;
        Debug.Log($"SelectedGradeColor: BIN={grade.BINCHAR}, GRADE={grade.GRADE}");
    }

    #endregion
}