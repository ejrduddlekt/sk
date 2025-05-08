using Data;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Progress;

/// <summary>
/// 스택맵 및 노잉크맵 선택 상태를 중앙에서 관리합니다.
/// </summary>
public class StackMapHandler : MonoBehaviour
{
    [Header("스택맵 목록")]
    [SerializeField] public List<StackMapLayerView> stackMapViews;

    [Header("정보 패널 참조")]
    [SerializeField] private NoInkMapView noInkMapView;

    [Header("칩 프리팹 (Grid에 맞춰 스케일)")]
    [SerializeField] private GameObject chipPrefab;
    [Tooltip("그리드 전체가 차지할 로컬 폭 (유닛)")]
    [SerializeField] private float totalGridSize = 1f;

    private StackMapMover _currentHighlighted;
    /// <summary>
    /// 현재 선택된 StackMapMover를 가져오거나 설정합니다.
    /// 설정 시 canChangeHighlight가 true여야 변경됩니다.
    /// </summary>
    public StackMapMover CurrentHighlighted
    {
        get => _currentHighlighted;
        set
        {
            var gameManager = GameManager.Instance;

            if (gameManager.isDragModeActive == true || gameManager.CurrentStage != SelectionStage.StackMapLayer || _currentHighlighted == value)
                return;

            // 이전 오브젝트 비활성화
            if (_currentHighlighted != null)
                _currentHighlighted.IsSelect = false;

            _currentHighlighted = value;

            // 새 오브젝트 활성화
            if (_currentHighlighted != null)
                _currentHighlighted.IsSelect = true;
        }
    }

    private StackMapLayerView _currentStackMapLayer;
    /// <summary>
    /// 현재 선택된 StackMapLayerView를 가져오거나 설정합니다.
    /// 설정 시 자동으로 CurrentHighlighted도 갱신됩니다.
    /// </summary>
    public StackMapLayerView CurrentStackMapLayer
    {
        get => _currentStackMapLayer;
        set
        {
            var gameManager = GameManager.Instance;

            if (gameManager.isDragModeActive == true || gameManager.CurrentStage != SelectionStage.StackMapLayer || _currentStackMapLayer == value)
                return;

            _currentStackMapLayer = value;
            _currentStackMapLayer.transform.position = new Vector3(0, stackMapViews.Count, 0);
        }
    }

    /// <summary>
    /// 뷰 하나와 해당 레이어 데이터를 받아
    /// - 리스트에 추가
    /// - 데이터 초기화
    /// - CHIP 프리팹을 그리드 크기에 맞춰 배치
    /// </summary>
    public void SetStackMapViews(StackMapLayerView view, StackMap data)
    {
        // 1) 리스트에 추가 & 데이터 바인딩
        stackMapViews.Add(view);
        view.Init(data);
        var mover = view.GetComponentInParent<StackMapMover>();

        mover.transform.position = new Vector3(0, stackMapViews.Count, 0);
        mover.originalRotation = mover.transform.localRotation;

        // 2) 컬럼/로우 개수
        int cols = data.X_AXIS + 1;
        int rows = data.Y_AXIS + 1;
        if (chipPrefab == null || cols <= 0 || rows <= 0)
            return;

        // 3) 셀 하나당 크기, 4) 스케일 계산, 5) 오프셋 계산 생략…

        float chipSize = totalGridSize / Mathf.Max(cols, rows);
        var rend = chipPrefab.GetComponentInChildren<Renderer>();
        float baseSize = rend != null ? Mathf.Max(rend.bounds.size.x, rend.bounds.size.z) : 1f;
        float scale = chipSize / baseSize;
        float xOffset = -totalGridSize * 0.5f + chipSize * 0.5f;
        float zOffset = -totalGridSize * 0.5f + chipSize * 0.5f;

        // 6) 칩 생성 및 ChipClickHandler 바인딩
        for (int x = 0; x < cols; x++)
        {
            for (int y = 0; y < rows; y++)
            {
                var chip = Instantiate(chipPrefab, view.transform);
                chip.transform.localScale = Vector3.one * scale;
                chip.transform.localPosition = new Vector3(
                    xOffset + x * chipSize,
                    0f,
                    zOffset + y * chipSize
                );

                // --- 여기서 ChipClickHandler 를 초기화 & 이벤트 연결 ---
                var clickHandler = chip.GetComponent<StackMapChipHandler>();
                if (clickHandler != null)
                {
                    // 해당 칩의 인자를 가져와 Initialize
                    if (DataStorage.Instance
                        .stackMapArgumentList
                        .TryGetArgument(int.Parse(data.STACK_NO),
                                        new Vector2Int(x, y),
                                        out var arg))
                    {
                        clickHandler.Initialize(arg);
                    }
                    else
                    {
                        clickHandler.Initialize(null);
                    }
                }
            }
        }
    }

}
