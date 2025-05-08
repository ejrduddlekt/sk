using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System;

public class ZoomHandler : MonoBehaviour, IScrollHandler
{
    [Header("Zoom Settings")]
    [Tooltip("최소 확대 배율")]
    [SerializeField] private float minZoom = 0.5f;
    [Tooltip("최대 확대 배율")]
    [SerializeField] private float maxZoom = 3f;
    [Tooltip("휠 스크롤/버튼 클릭 시 증감량")]
    [SerializeField] private float zoomSpeed = 0.1f;

    [Header("UI Buttons (Optional)")]
    [Tooltip("Zoom In 버튼 할당 (UI) 또는 원하는 호출 지점에 연결")]
    [SerializeField] private Button zoomInButton;
    [Tooltip("Zoom Out 버튼 할당")]
    [SerializeField] private Button zoomOutButton;

    [Header("Scroll Rect Settings (Optional)")]
    [Tooltip("ScrollRect Content RectTransform. 줌 시 레이아웃을 강제 리빌드할 영역")]
    [SerializeField] private RectTransform scrollContent;

    /// <summary>현재 줌 배율</summary>
    public float Zoom { get; private set; } = 1f;
    /// <summary>줌 변경 시 호출되는 이벤트</summary>
    public event Action<float> OnZoomChanged;

    // 확대 축소 - 마우스 휠
    private float finalZoomFactor = 0;
    private float scrollScale = 10f;
    private float zoomFactorMin = -10;
    private float zoomFactorMax = 10;
    private float quadFuncA, quadFuncB, quadFuncC;

    // 회전 - 마우스 우측버튼
    private bool draggingForRotate = false;
    private Vector3 posClick;
    private float rotateX = 0f;
    private float rotateY = 0f;
    private float dragX, dragY;

    // 확대/축소를 적용할 대상 Transform (UI면 RectTransform, 3D면 Transform)
    [Tooltip("확대/축소를 적용할 대상 Transform. 비어있으면 이 GameObject의 transform 사용")]
    [SerializeField] private Transform targetTransform;

    // ① 현재 Stage 가 StackMapLayer 인지 판정
    private bool StageActive =>
        GameManager.Instance != null &&
        GameManager.Instance.CurrentStage == SelectionStage.SelectStackMapLayer;

    void Awake()
    {
        // targetTransform이 미설정된 경우에만 기본 대상 할당
        if (targetTransform == null)
            targetTransform = transform;
        ApplyZoom();

        if (zoomInButton != null)
            zoomInButton.onClick.AddListener(ZoomIn);
        if (zoomOutButton != null)
            zoomOutButton.onClick.AddListener(ZoomOut);

        // calculate quadratic function coefficient
        float temp = (zoomFactorMax);
        quadFuncA = (maxZoom + minZoom - 2.0f) / (2.0f * temp * temp);
        quadFuncB = (maxZoom - minZoom) / (2.0f * temp);
        quadFuncC = 1.0f;
    }

    /// <summary>
    /// 스크롤 휠/터치 스크롤 발생 시 확대/축소 처리
    /// </summary>
    public void OnScroll(PointerEventData eventData)
    {
        ChangeZoom(eventData.scrollDelta.y * zoomSpeed);
    }

    /// <summary>
    /// Zoom In 버튼용 메서드
    /// </summary>
    public void ZoomIn()
    {
        ChangeZoom(zoomSpeed);
    }

    /// <summary>
    /// Zoom Out 버튼용 메서드
    /// </summary>
    public void ZoomOut()
    {
        ChangeZoom(-zoomSpeed);
    }

    /// <summary>
    /// 줌 배율을 delta만큼 변경 후 clamped, 적용 및 이벤트 호출
    /// </summary>
    private void ChangeZoom(float delta)
    {
        float newZoom = Zoom + delta;
        SetZoom(newZoom);
    }

    private void SetZoom(float newZoom)
    {
        newZoom = Mathf.Clamp(newZoom, minZoom, maxZoom);
        if (!Mathf.Approximately(newZoom, Zoom))
        {
            Zoom = newZoom;
            ApplyZoom();
            OnZoomChanged?.Invoke(Zoom);

            // ScrollRect ContentSizeFitter가 있으면 강제 레이아웃 리빌드
            if (scrollContent != null)
                LayoutRebuilder.ForceRebuildLayoutImmediate(scrollContent);
        }
    }

    /// <summary>
    /// 대상 Transform의 스케일에 현재 줌 값 적용
    /// </summary>
    private void ApplyZoom()
    {
        if (targetTransform != null)
            targetTransform.localScale = Vector3.one * Zoom;
    }

    void Update()
    {
        // StackMapLayer 단계가 아니면 입력 무시
        if (!StageActive || targetTransform == null) return;

        /* ── 1) 줌 인/아웃 ─────────────────────────────── */
        float deltaScroll = Input.GetAxisRaw("Mouse ScrollWheel") * scrollScale;
        if (deltaScroll != 0)
        {
            finalZoomFactor = Mathf.Round(finalZoomFactor + deltaScroll);
            finalZoomFactor = Mathf.Clamp(finalZoomFactor, zoomFactorMin, zoomFactorMax);

            float newZoom = quadFuncA * finalZoomFactor * finalZoomFactor +
                            quadFuncB * finalZoomFactor + quadFuncC;
            SetZoom(newZoom);
        }

        /* ── 2) 우클릭 드래그 회전 ─────────────────────── */
        if (Input.GetMouseButtonDown(1))
        {
            draggingForRotate = true;
            GameManager.Instance.isDragModeActive = true;
            posClick = Input.mousePosition;
            dragX = dragY = 0;
        }
        if (Input.GetMouseButtonUp(1))
        {
            draggingForRotate = false;
            GameManager.Instance.isDragModeActive = false;
            rotateX += dragX;
            rotateY -= dragY;
        }
        if (draggingForRotate)
        {
            Vector3 diff = Input.mousePosition - posClick;
            float rotY = 360f * diff.x / Screen.width;
            float rotX = 360f * diff.y / Screen.height;

            dragX = rotX;
            dragY = rotY;
            Quaternion rot = Quaternion.AngleAxis(rotateY - dragY, Vector3.up);
            rot = Quaternion.AngleAxis(rotateX + dragX, Camera.main.transform.right) * rot;
            targetTransform.rotation = rot;
        }
    }
}