// NoInkMapView.cs
// UIComponent 상속, IRecordView 수행, 스크롤뷰 내 클릭/드래그 분리, 3D 레이 캐스트 및 기즈모 시각화

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

    // 기즈모를 위해 저장할 Ray 및 히트인지 여부
    private Ray _lastRay;
    private bool _hasLastRay;
    private RaycastHit _lastHit;

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

    public void OnPointerDown(PointerEventData eventData)
    {
        _isDragging = false;
        _pointerDownPos = eventData.position;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!_isDragging &&
            Vector2.Distance(eventData.position, _pointerDownPos) > DragThreshold)
        {
            _isDragging = true;
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (!_isDragging)
            HandleClick(eventData);
    }

    /// <summary>
    /// 클릭 시 클릭 지점을 계산해 3D Raycast를 수행하고, 기즈모로 시각화할 Ray를 저장
    /// </summary>
    private void HandleClick(PointerEventData eventData)
    {
        if ( mapImage == null || renderCamera == null)
            return;

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
        _lastRay = ray;
        _hasLastRay = true;

        if (Physics.Raycast(ray, out var hit))
        {
            _lastHit = hit;
            Debug.Log($"Raycast hit {hit.collider.name} at {hit.point}");
        }

        OnClicked?.Invoke(_data);
    }

    /// <summary>
    /// Scene 뷰 및 플레이 중 기즈모로 Ray와 히트 포인트를 시각화
    /// </summary>
    private void OnDrawGizmos()
    {
        if (!_hasLastRay)
            return;

        Gizmos.color = Color.red;
        Gizmos.DrawRay(_lastRay.origin, _lastRay.direction * 100f);

        if (_lastHit.collider != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawSphere(_lastHit.point, 0.05f);
        }
    }
}