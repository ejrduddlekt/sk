using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

/// <summary>
/// StackMapView 리스트를 관리하며,
/// - 마우스 오버 시 로컬 X축 기준 오른쪽으로 부드럽게 이동 및 아웃라인 하이라이트
/// - 마우스 아웃 시 원위치 부드럽게 복귀 및 아웃라인 해제
/// - 클릭 시 모두 초기화 후 정보 패널 활성화
/// </summary>
public class StackMapHandler : MonoBehaviour
{
    [Header("스택맵 목록")]
    [SerializeField] private List<StackMapView> stackMapViews;

    [Header("이동 설정")]
    [Tooltip("마우스 오버 시 로컬 X축으로 이동할 거리 (유니티 단위)")]
    [SerializeField] private float hoverMoveDistance = 1f;
    [Tooltip("이동 애니메이션 시간 (초)")]
    [SerializeField] private float moveDuration = 0.3f;
    [Tooltip("이동 애니메이션 이징")]
    [SerializeField] private Ease moveEase = Ease.OutQuad;

    [Header("하이라이트 설정")]
    [SerializeField] private Color highlightColor = Color.yellow;
    [SerializeField] private float highlightWidth = 3f;

    [Header("정보 패널 참조")]
    [SerializeField] private NoInkMapView noInkMapView;

    // 로컬 위치 저장
    private Dictionary<StackMapView, Vector3> originalLocalPositions = new Dictionary<StackMapView, Vector3>();
    private StackMapView currentHighlighted;

    void Awake()
    {
        foreach (var view in stackMapViews)
        {
            if (view == null) continue;

            // 각 View의 초기 로컬 위치 저장
            originalLocalPositions[view] = view.transform.localPosition;

            view.OnMouseEnterView += OnViewEnter;
            view.OnMouseExitView += OnViewExit;
            view.OnClickedView += OnViewClicked;
        }
    }

    private void OnViewEnter(StackMapView view)
    {
        if (noInkMapView != null && noInkMapView.gameObject.activeSelf)
            return;

        // 아웃라인 활성화
        var outline = view.GetComponent<Outline>();
        if (outline != null)
        {
            outline.OutlineColor = highlightColor;
            outline.OutlineWidth = highlightWidth;
            outline.OutlineMode = Outline.Mode.OutlineAll;
            outline.enabled = true;
        }

        // 로컬 X축 기준 오른쪽으로 부드럽게 이동
        if (originalLocalPositions.TryGetValue(view, out var origLocal))
        {
            view.transform.DOKill();
            view.transform.DOLocalMove(origLocal + Vector3.right * hoverMoveDistance, moveDuration)
                          .SetEase(moveEase);
        }
    }

    private void OnViewExit(StackMapView view)
    {
        if (noInkMapView != null && noInkMapView.gameObject.activeSelf)
            return;

        // 아웃라인 비활성화
        var outline = view.GetComponent<Outline>();
        if (outline != null)
            outline.enabled = false;

        // 원위치 로컬 복귀
        if (originalLocalPositions.TryGetValue(view, out var origLocal))
        {
            view.transform.DOKill();
            view.transform.DOLocalMove(origLocal, moveDuration)
                          .SetEase(moveEase);
        }
    }

    private void OnViewClicked(StackMapView clickedView)
    {
        if (noInkMapView != null && noInkMapView.gameObject.activeSelf)
            return;

        // 모든 View 초기화 (아웃라인 해제 및 로컬 위치 복귀)
        foreach (var view in stackMapViews)
        {
            var outl = view.GetComponent<Outline>();
            if (outl != null)
                outl.enabled = false;

            if (originalLocalPositions.TryGetValue(view, out var origLocal))
            {
                view.transform.DOKill();
                view.transform.DOLocalMove(origLocal, moveDuration)
                            .SetEase(moveEase);
            }
        }

        currentHighlighted = clickedView;
        if (currentHighlighted != null && noInkMapView != null)
        {
            noInkMapView.gameObject.SetActive(true);
        }
    }
}
