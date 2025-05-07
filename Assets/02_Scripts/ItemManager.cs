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
    [SerializeField] private GameObject lotsPrefab;
    [SerializeField] private GameObject stackMapLayerPrefab;
    [SerializeField] private GameObject stackMapPrefab;
    [SerializeField] private GameObject noInkMapPrefab;
    [SerializeField] private GameObject gradeColorPrefab;

    [Header("Parents")]
    [SerializeField] private Transform lotsParent;
    [SerializeField] private Transform stackMapLayerParent;
    [SerializeField] private Transform noInkMapParent;
    [SerializeField] private Transform gradeColorParent;

    [Header("Parents")]
    [SerializeField] private NamedPipeManualClient namedPipe;

    /// <summary>선택된 Wafer 데이터</summary>
    public Lots SelectedWafer { get; private set; }
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
    public void SpawnLots()
    {
        foreach (Transform child in lotsParent)
            Destroy(child.gameObject);

        foreach (var item in DataStorage.Instance.lotsList.wafer_list)
        {
            var go = Instantiate(lotsPrefab, lotsParent);
            var view = go.GetComponent<WaferView>();
            view.Init(item);
            view.OnClicked += OnWaferClicked;
        }
    }

    /// <summary>
    /// StackMap 리스트를 받아 화면에 StackMapView 인스턴스를 생성
    /// 클릭 시 OnStackMapClicked를 호출하여 SelectedStackMap에 저장
    /// </summary>
    public void SpawnStackMapLayer()
    {
        foreach (Transform child in stackMapLayerParent)
            Destroy(child.gameObject);

        foreach (var item in DataStorage.Instance.stackMapList.stackmap_list)
        {
            var go = Instantiate(stackMapLayerPrefab, stackMapLayerParent);
            var view = go.GetComponent<StackMapLayerView>();
            view.Init(item);
            view.OnClicked += OnStackMapClicked;
        }
    }

    /// <summary>
    /// NoInkMap 리스트를 받아 화면에 NoInkMapView 인스턴스를 생성
    /// 클릭 시 OnNoInkMapClicked를 호출하여 SelectedNoInkMap에 저장
    /// </summary>
    public void SpawnNoInkMaps(List<NoInkMap> items)
    {
        foreach (Transform child in noInkMapParent)
            Destroy(child.gameObject);

        foreach (var item in items)
        {
            var go = Instantiate(noInkMapPrefab, noInkMapParent);
            var view = go.GetComponent<NoInkMapView>();
            view.Init(item);
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
    protected virtual void OnWaferClicked(Lots wafer)
    {
        SelectedWafer = wafer;
        Debug.Log($"SelectedWafer: LOT={wafer.LOT_ID}, WF={wafer.WF_ID}");

        var wrapper = CommandWrapping<Lots>("GetStackMap", wafer);
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