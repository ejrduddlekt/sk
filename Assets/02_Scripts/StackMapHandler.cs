// StackMapHandler.cs
// StackMapView 리스트를 관리하고 마우스 오버 시 Outline, 클릭 시 NoInkMapView 활성화 처리

using System.Collections.Generic;
using UnityEngine;

public class StackMapHandler : MonoBehaviour
{
    [Header("스택맵 목록")]
    [SerializeField] private List<StackMapView> stackMapViews;

    [Header("하이라이트 설정")]
    [SerializeField] private Color highlightColor = Color.yellow;
    [SerializeField] private float highlightWidth = 3f;

    [Header("정보 패널 참조")]
    [SerializeField] private NoInkMapView noInkMapView;

    private StackMapView currentHighlighted;

    void Awake()
    {
        foreach (var view in stackMapViews)
        {
            if (view == null) continue;
            view.OnMouseEnterView += OnViewEnter;
            view.OnMouseExitView += OnViewExit;
            view.OnClickedView += OnViewClicked;
        }
    }

    /// <summary>
    /// 마우스 오버 시 호출: 아웃라인 활성화
    /// </summary>
    private void OnViewEnter(StackMapView view)
    {
        if (noInkMapView != null && noInkMapView.gameObject.activeSelf)
            return;

        var outline = view.GetComponent<Outline>();
        if (outline != null)
        {
            outline.OutlineColor = highlightColor;
            outline.OutlineWidth = highlightWidth;
            outline.OutlineMode = Outline.Mode.OutlineAll;
            outline.enabled = true;
        }
    }

    /// <summary>
    /// 마우스 아웃 시 호출: 아웃라인 비활성화
    /// </summary>
    private void OnViewExit(StackMapView view)
    {
        if (noInkMapView != null && noInkMapView.gameObject.activeSelf)
            return;

        var outline = view.GetComponent<Outline>();
        if (outline != null)
            outline.enabled = false;
    }

    /// <summary>
    /// 클릭 시 호출: NoInkMapView이 활성화되어 있으면 무시, 아니면 모든 아웃라인 해제 및 패널 활성화
    /// </summary>
    private void OnViewClicked(StackMapView clickedView)
    {
        // 이미 정보 패널이 활성화되어 있으면 처리 중단
        if (noInkMapView != null && noInkMapView.gameObject.activeSelf)
            return;

        // 모든 뷰의 아웃라인 해제
        foreach (var view in stackMapViews)
        {
            var o = view.GetComponent<Outline>();
            if (o != null)
                o.enabled = false;
        }

        // 클릭된 뷰 저장 및 패널 활성화
        currentHighlighted = clickedView;
        if (currentHighlighted != null && noInkMapView != null)
        {
            noInkMapView.gameObject.SetActive(true);
        }
    }
}