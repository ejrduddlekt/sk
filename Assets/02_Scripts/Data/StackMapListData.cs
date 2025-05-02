using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using VInspector;
using Data;

/// <summary>
/// StackMapListData: 외부에서 전달된 StackMapList 데이터를
/// 층별·좌표별로 빠른 조회가 가능한 중첩 Dictionary로 저장 관리합니다.
/// </summary>
public class StackMapListData : MonoBehaviour, IData
{
    [ShowInInspector, ReadOnly]
    private SerializedDictionary<int, Dictionary<Vector2Int, StackMap>> _floorDict = new SerializedDictionary<int, Dictionary<Vector2Int, StackMap>>();

    /// <summary>
    /// 층별・좌표별 Dictionary 접근 프로퍼티
    /// </summary>
    public IReadOnlyDictionary<int, Dictionary<Vector2Int, StackMap>> FloorDict => _floorDict;

    /// <summary>
    /// 외부에서 전달된 StackMapList로부터 내부 Dictionary를 재구성합니다.
    /// </summary>
    /// <param name="parsedList">파싱된 StackMapList 데이터</param>
    public void SetData(StackMapList parsedList)
    {
        if (parsedList == null || parsedList.stackmap_list == null)
        {
            Debug.LogWarning("[StackMapListData] 전달된 데이터가 null 입니다.");
            return;
        }

        _floorDict.Clear();

        foreach (var item in parsedList.stackmap_list)
        {
            if (!int.TryParse(item.STACK_NO, out var floor))
                continue;
            if (!int.TryParse(item.X_POSITION, out var x) || !int.TryParse(item.Y_POSITION, out var y))
                continue;

            var pos = new Vector2Int(x, y);

            if (!_floorDict.TryGetValue(floor, out var inner))
            {
                inner = new Dictionary<Vector2Int, StackMap>();
                _floorDict[floor] = inner;
            }
            inner[pos] = item;
        }

        Debug.Log($"[StackMapListData] 데이터 로드 완료: {_floorDict.Count}개 층, {_floorDict.Sum(kv => kv.Value.Count)}개 좌표 매핑.");
    }

    /// <summary>
    /// 특정 층・좌표의 StackMap 데이터를 빠르게 조회합니다.
    /// </summary>
    /// <param name="floor">층 번호</param>
    /// <param name="pos">좌표 (Vector2Int)</param>
    /// <param name="result">조회된 데이터 (없으면 null)</param>
    /// <returns>존재하면 true</returns>
    public bool TryGetMap(int floor, Vector2Int pos, out StackMap result)
    {
        result = null;
        if (_floorDict.TryGetValue(floor, out var inner) && inner.TryGetValue(pos, out var map))
        {
            result = map;
            return true;
        }
        return false;
    }
}
