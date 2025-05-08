using UnityEngine;
using DG.Tweening;

/// <summary>
/// 스택맵 오브젝트의 마우스 인터랙션 및 이동/회전 처리
/// </summary>
public class StackMapMover : MonoBehaviour
{
    [SerializeField] private Transform moveTarget; // 움직일 자식 오브젝트
    [SerializeField] private float hoverDistance = 1f;
    [SerializeField] private float moveDuration = 0.3f;
    [SerializeField] private Ease moveEase = Ease.OutQuad;
    [SerializeField] private Color highlightColor = Color.yellow;
    [SerializeField] private float highlightWidth = 3f;
    private StackMapHandler handler;

    private Outline outline;
    public Quaternion originalRotation;

    private bool _isSelect = false;

    private bool StageActive =>
    GameManager.Instance != null &&
    GameManager.Instance.CurrentStage == SelectionStage.SelectStackMapLayer;

    /// <summary>
    /// 선택 여부에 따라 회전 및 이동
    /// </summary>
    public bool IsSelect
    {
        get => _isSelect;
        set
        {
            if (_isSelect == value) return;
            _isSelect = value;

            if (_isSelect)
            {
                MoveRight();
            }
            else
            {
                MoveBack();
                ResetRotation();
            }
        }
    }



    private void Awake()
    {
        if (moveTarget == null)
            Debug.LogWarning("moveTarget이 설정되지 않았습니다.", this);

        outline = moveTarget.GetComponent<Outline>();
        handler = GetComponentInParent<StackMapHandler>();
    }

    private void Update()
    {
        if (_isSelect && Camera.main != null)
        {
            Vector3 camPos = Camera.main.transform.position;
            Vector3 lookDir = camPos - transform.position;

            // 부모 기준으로 방향 벡터 변환 (로컬 회전 유지 목적)
            lookDir = transform.parent != null ?
                transform.parent.InverseTransformDirection(lookDir) :
                lookDir;

            lookDir.y = 0f;

            if (lookDir.sqrMagnitude > 0.001f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(lookDir.normalized, Vector3.up);
                Vector3 targetEuler = targetRotation.eulerAngles;

                // Y축만 회전 적용
                transform.localRotation = Quaternion.Euler(0f, targetEuler.y, 0f);
            }
        }
    }




    public void MoveRight()
    {
        if (moveTarget == null || !StageActive) return;

        moveTarget.DOKill();
        moveTarget.DOLocalMove(Vector3.right * hoverDistance, moveDuration)
                  .SetEase(moveEase);
        HighlightOn();
    }

    public void MoveBack()
    {
        if (moveTarget == null || !StageActive) return;

        moveTarget.DOKill();
        moveTarget.DOLocalMove(Vector3.zero, moveDuration)
                  .SetEase(moveEase);
        HighlightOff();
    }

    public void ResetRotation()
    {
        if (moveTarget == null || !StageActive) return;

        moveTarget.DORotateQuaternion(originalRotation, moveDuration).SetEase(moveEase);
    }

    public void HighlightOn()
    {
        if (outline == null || !StageActive) return;
        outline.OutlineColor = highlightColor;
        outline.OutlineWidth = highlightWidth;
        outline.OutlineMode = Outline.Mode.OutlineAll;
        outline.enabled = true;
    }

    public void HighlightOff()
    {
        if (outline == null || !StageActive) return;
        outline.enabled = false;
    }

    private void OnMouseEnter()
    {
        if (handler != null || !StageActive)
            handler.CurrentHighlighted = this;
    }
}
