using Data;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

/// <summary>
/// 스택맵 및 노잉크맵 선택 상태를 중앙에서 관리합니다.
///  - StackMapLayerView를 선택/해제할 때 DOTween으로 부드럽게 이동
///  - 이전 위치를 저장해 두었다가, null 설정 시 원위치로 복귀
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

    [Header("선택된 레이어 위치")]
    [Tooltip("StackMapLayerView를 선택했을 때 이동시킬 위치")]
    public Transform SelStackLayerPosition;

    [Header("이동 애니메이션 설정")]
    [SerializeField] private float selectMoveDuration = 0.3f;
    [SerializeField] private Ease selectMoveEase = Ease.OutQuad;

    // 각 뷰의 원래 월드 위치를 저장
    private Dictionary<StackMapLayerView, Vector3> _originalPositions = new();

    private StackMapMover _currentHighlighted;
    /// <summary>
    /// 현재 마우스 오버된 StackMapMover
    /// </summary>
    public StackMapMover CurrentHighlighted
    {
        get => _currentHighlighted;
        set
        {
            var gm = GameManager.Instance;
            if (gm.isDragModeActive
                || gm.CurrentStage != SelectionStage.SelectStackMapLayer
                || _currentHighlighted == value)
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
    /// 현재 선택된 StackMapLayerView.
    /// - null이 들어오면 원래 위치로 돌아감
    /// - 값이 들어오면 SelStackLayerPosition으로 부드럽게 이동
    /// </summary>
    public StackMapLayerView CurrentStackMapLayer
    {
        get => _currentStackMapLayer;
        set
        {
            var gm = GameManager.Instance;
            if (gm.isDragModeActive || _currentStackMapLayer == value)
                return;

            // 1) 이전 선택 해제: 원위치로 애니메이션
            if (_currentStackMapLayer != null)
            {
                var prev = _currentStackMapLayer;
                prev.transform.DOKill();
                prev.transform
                    .DOMove(_originalPositions[prev], selectMoveDuration)
                    .SetEase(selectMoveEase)
                    .OnComplete(() =>
                    {
                        // 해제 애니메이션 완료 후 단계 복원
                        if (value == null)
                            gm.CurrentStage = SelectionStage.SelectStackMapLayer;
                    });
            }

            _currentStackMapLayer = value;

            // 2) 새로 선택된 레이어 있으면 SelStackLayerPosition으로 이동
            if (_currentStackMapLayer != null)
            {
                _currentStackMapLayer.transform.DOKill();
                _currentStackMapLayer.transform
                    .DOMove(SelStackLayerPosition.position, selectMoveDuration)
                    .SetEase(selectMoveEase)
                    .OnComplete(() =>
                    {
                        // 선택 애니메이션 완료 후 칩 선택 단계로
                        gm.CurrentStage = SelectionStage.SelectChip;
                    });
            }
            else
            {
                // 값이 null이지만, 이전이 없어서 애니메이션 자체가 없을 때
                // 바로 단계 복원이 필요하다면 여기도 처리할 수 있습니다.
                // gm.CurrentStage = SelectionStage.SelectStackMapLayer;
            }
        }
    }


    private void Awake()
    {
        // Inspector에서 할당된 뷰가 있으면 원위치 저장
        foreach (var view in stackMapViews)
        {
            _originalPositions[view] = view.transform.position;
        }
    }

    /// <summary>
    /// 외부에서 호출: 새로운 레이어 뷰를 추가하고
    /// 칩 프리팹을 그리드에 맞춰 배치합니다.
    /// </summary>
    public void SetStackMapViews(StackMapLayerView view, StackMap data)
    {
        // 1) 리스트에 추가 & 데이터 바인딩
        stackMapViews.Add(view);
        view.Init(data);

        // 2) 해당 뷰의 mover 초기 위치 & 원위치 저장
        var mover = view.GetComponentInParent<StackMapMover>();
        Vector3 startPos = new Vector3(0, stackMapViews.Count, 0);
        mover.transform.position = startPos;
        _originalPositions[view] = startPos;
        mover.originalRotation = mover.transform.localRotation;

        // 3) 칩 생성
        int cols = data.X_AXIS + 1;
        int rows = data.Y_AXIS + 1;
        if (chipPrefab == null || cols <= 0 || rows <= 0) return;

        float chipSize = totalGridSize / Mathf.Max(cols, rows);
        var rend = chipPrefab.GetComponentInChildren<Renderer>();
        float baseSize = rend != null
            ? Mathf.Max(rend.bounds.size.x, rend.bounds.size.z)
            : 1f;
        float scale = chipSize / baseSize;
        float xOffset = -totalGridSize * 0.5f + chipSize * 0.5f;
        float zOffset = -totalGridSize * 0.5f + chipSize * 0.5f;

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

                var clickHandler = chip.GetComponent<StackMapChipHandler>();
                if (clickHandler != null)
                {
                    if (DataManager.Instance.dataStorage
                        .stackMapArgumentList
                        .TryGetArgument(int.Parse(data.STACK_NO),
                                        new Vector2Int(x, y),
                                        out var arg))
                        clickHandler.Initialize(arg);
                    else
                        clickHandler.Initialize(null);
                }
            }
        }
    }
}
