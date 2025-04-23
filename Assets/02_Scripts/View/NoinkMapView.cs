// NoInkMapView.cs
// UIComponent 상속, IRecordView 수행, 스크롤뷰 내 클릭/드래그 분리, 3D 레이 캐스트

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using Data;
using System;

public class NoInkMapView : UIComponent,
    IRecordView<NoInkMap>,
    IPointerDownHandler,
    IPointerUpHandler,
    IDragHandler
{
    [Header("Primary IDs")]
    [SerializeField] private TMP_Text lotIdText;
    [SerializeField] private TMP_Text wfIdText;

    [Header("Process Info")]
    [SerializeField] private TMP_Text operIdText;
    [SerializeField] private TMP_Text tsvTypeText;
    [SerializeField] private TMP_Text passDieQtyText;
    [SerializeField] private TMP_Text flatZoneTypeText;

    [Header("Stack & Position")]
    [SerializeField] private TMP_Text stackNoText;
    [SerializeField] private TMP_Text xAxisText;
    [SerializeField] private TMP_Text yAxisText;
    [SerializeField] private TMP_Text xPosText;
    [SerializeField] private TMP_Text yPosText;

    [Header("Die Details")]
    [SerializeField] private TMP_Text dieValText;
    [SerializeField] private TMP_Text dieThicknessText;
    [SerializeField] private TMP_Text dieXCoordText;
    [SerializeField] private TMP_Text dieYCoordText;

    [Header("Map Interaction")]
    [Tooltip("RawImage displaying the RenderTexture from the 3D camera")]
    [SerializeField] private RawImage mapImage;
    [Tooltip("Camera rendering to the mapImage's RenderTexture")]
    [SerializeField] private Camera renderCamera;

    private NoInkMap _data;
    public event Action<NoInkMap> OnClicked;

    // 드래그 판정을 위한 필드
    private bool _isDragging;
    private Vector2 _pointerDownPos;
    private const float DragThreshold = 10f;

    /// <summary>
    /// 데이터로 UI를 바인딩
    /// </summary>
    public void Init(NoInkMap data)
    {
        _data = data;
        lotIdText.text = data.LOT_ID;
        wfIdText.text = data.WF_ID;
        operIdText.text = data.OPER_ID;
        tsvTypeText.text = data.TSV_TYPE;
        passDieQtyText.text = data.PASS_DIE_QTY;
        flatZoneTypeText.text = data.FLAT_ZONE_TYPE;
        stackNoText.text = data.STACK_NO;
        xAxisText.text = data.X_AXIS;
        yAxisText.text = data.Y_AXIS;
        xPosText.text = data.X_POSITION;
        yPosText.text = data.Y_POSITION;
        dieValText.text = data.DIE_VAL;
        dieThicknessText.text = data.DIE_THICKNESS;
        dieXCoordText.text = data.DIE_X_COORDINATE;
        dieYCoordText.text = data.DIE_Y_COORDINATE;
    }

    /// <summary>
    /// 포인터를 다운한 시점에 호출
    /// 스크롤 vs 클릭 구분을 위해 초기 위치 저장
    /// </summary>
    public void OnPointerDown(PointerEventData eventData)
    {
        _isDragging = false;
        _pointerDownPos = eventData.position;
    }

    /// <summary>
    /// 드래그 중 호출
    /// 일정 거리 이상 이동하면 드래그로 간주
    /// </summary>
    public void OnDrag(PointerEventData eventData)
    {
        if (!_isDragging &&
            Vector2.Distance(eventData.position, _pointerDownPos) > DragThreshold)
        {
            _isDragging = true;
        }
    }

    /// <summary>
    /// 포인터를 올린 시점에 호출
    /// 드래그가 아니라면 클릭으로 처리
    /// </summary>
    public void OnPointerUp(PointerEventData eventData)
    {
        if (!_isDragging)
            HandleClick(eventData);
    }

    /// <summary>
    /// 실제 클릭 처리: 맵 상 좌표 → 3D Raycast → 이벤트 호출
    /// </summary>
    private void HandleClick(PointerEventData eventData)
    {
        if (_data == null || mapImage == null || renderCamera == null)
            return;

        // RawImage 로컬 좌표 변환
        RectTransform rt = mapImage.rectTransform;
        if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(
            rt, eventData.position, eventData.pressEventCamera, out var localPos))
            return;

        Rect rect = rt.rect;
        Vector2 uv = new Vector2(
            (localPos.x - rect.x) / rect.width,
            (localPos.y - rect.y) / rect.height
        );

        Ray ray = renderCamera.ViewportPointToRay(uv);
        if (Physics.Raycast(ray, out var hit))
        {
            Debug.Log($"Raycast hit {hit.collider.name} at {hit.point}");
        }

        OnClicked?.Invoke(_data);
    }
}
