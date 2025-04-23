// ItemManager.cs
// Prefab 및 부모 Transform 참조만 저장하고,
// 외부에서 데이터 리스트를 받아 화면에 아이템을 생성하는 모듈
// 클릭 처리 메서드를 빈 메서드로 두어, 원하는 로직을 오버라이드하거나 구현하도록 설계

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

    /// <summary>
    /// Wafer 리스트를 받아 화면에 WaferView 인스턴스를 생성합니다.
    /// 클릭 시 OnWaferClicked 메서드를 호출합니다.
    /// </summary>
    public void SpawnWafers(List<Wafer> wafers)
    {
        // 기존 오브젝트 정리
        foreach (Transform child in waferParent)
            Destroy(child.gameObject);

        // 새로 생성
        foreach (var wafer in wafers)
        {
            var go = Instantiate(waferPrefab, waferParent);
            var view = go.GetComponent<WaferView>();
            view.Init(wafer);
            view.OnClicked += OnWaferClicked;
        }
    }

    /// <summary>
    /// StackMap 리스트를 받아 화면에 StackMapView 인스턴스를 생성합니다.
    /// 클릭 시 OnStackMapClicked 메서드를 호출합니다.
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
    /// NoInkMap 리스트를 받아 화면에 NoInkMapView 인스턴스를 생성합니다.
    /// 클릭 시 OnNoInkMapClicked 메서드를 호출합니다.
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
    /// GradeColor 리스트를 받아 화면에 NoInkMapGradeColorView 인스턴스를 생성합니다.
    /// 클릭 시 OnGradeColorClicked 메서드를 호출합니다.
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

    #region 클릭 핸들러 (빈 메서드, 원하는 로직으로 구현 가능)

    /// <summary>
    /// WaferView 클릭 시 호출됩니다.
    /// 다른 로직이 필요하면 이 메서드를 오버라이드하거나 수정하세요.
    /// </summary>
    protected virtual void OnWaferClicked(Wafer wafer)
    {
        // 빈 메서드: 클릭 로직 구현
    }

    /// <summary>
    /// StackMapView 클릭 시 호출됩니다.
    /// </summary>
    protected virtual void OnStackMapClicked(StackMap map)
    {
        // 빈 메서드: 클릭 로직 구현
    }

    /// <summary>
    /// NoInkMapView 클릭 시 호출됩니다.
    /// </summary>
    protected virtual void OnNoInkMapClicked(NoInkMap map)
    {
        // 빈 메서드: 클릭 로직 구현
    }

    /// <summary>
    /// NoInkMapGradeColorView 클릭 시 호출됩니다.
    /// </summary>
    protected virtual void OnGradeColorClicked(NoInkMapGradeColor grade)
    {
        // 빈 메서드: 클릭 로직 구현
    }

    #endregion
}
