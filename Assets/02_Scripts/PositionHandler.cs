// PositionHandler.cs
// 버튼을 길게 누르면 오브젝트 회전 및 이동 기능을 수행하는 컴포넌트

using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PositionHandler : MonoBehaviour
{
    [Header("Control Buttons (UI GameObjects with Button or EventTrigger)")]
    [Tooltip("왼쪽 회전 버튼 오브젝트(이벤트 트리거 필요)")]
    [SerializeField] private GameObject leftRotateButton;
    [Tooltip("오른쪽 회전 버튼 오브젝트")]
    [SerializeField] private GameObject rightRotateButton;
    [Tooltip("위쪽 이동 버튼 오브젝트")]
    [SerializeField] private GameObject upMoveButton;
    [Tooltip("아래쪽 이동 버튼 오브젝트")]
    [SerializeField] private GameObject downMoveButton;

    [Header("Transformation Settings")]
    [Tooltip("초당 회전 속도 (도 단위)")]
    [SerializeField] private float rotationSpeed = 90f;
    [Tooltip("초당 이동 속도 (단위)")]
    [SerializeField] private float moveSpeed = 2f;

    [Header("Vertical Movement Limits")]
    [Tooltip("최소 Y 위치")]
    [SerializeField] private float minY = 0f;
    [Tooltip("최대 Y 위치")]
    [SerializeField] private float maxY = 5f;

    // 내부 상태 플래그
    private bool isRotatingLeft;
    private bool isRotatingRight;
    private bool isMovingUp;
    private bool isMovingDown;

    void Awake()
    {
        // 각 버튼에 PointerDown/PointerUp 이벤트 바인딩
        BindEvent(leftRotateButton, EventTriggerType.PointerDown, () => isRotatingLeft = true);
        BindEvent(leftRotateButton, EventTriggerType.PointerUp, () => isRotatingLeft = false);

        BindEvent(rightRotateButton, EventTriggerType.PointerDown, () => isRotatingRight = true);
        BindEvent(rightRotateButton, EventTriggerType.PointerUp, () => isRotatingRight = false);

        BindEvent(upMoveButton, EventTriggerType.PointerDown, () => isMovingUp = true);
        BindEvent(upMoveButton, EventTriggerType.PointerUp, () => isMovingUp = false);

        BindEvent(downMoveButton, EventTriggerType.PointerDown, () => isMovingDown = true);
        BindEvent(downMoveButton, EventTriggerType.PointerUp, () => isMovingDown = false);
    }

    void Update()
    {
        float dt = Time.deltaTime;

        if (isRotatingLeft)
            transform.Rotate(Vector3.up, -rotationSpeed * dt, Space.World);
        if (isRotatingRight)
            transform.Rotate(Vector3.up, rotationSpeed * dt, Space.World);

        if (isMovingUp)
            MoveVertical(moveSpeed * dt);
        if (isMovingDown)
            MoveVertical(-moveSpeed * dt);
    }

    /// <summary>
    /// Y축 이동을 수행하고 minY/maxY로 위치를 제한
    /// </summary>
    private void MoveVertical(float delta)
    {
        Vector3 pos = transform.position;
        pos.y = Mathf.Clamp(pos.y + delta, minY, maxY);
        transform.position = pos;
    }

    /// <summary>
    /// 버튼 GameObject에 EventTrigger가 없으면 추가하고, 지정 이벤트에 액션을 바인딩
    /// </summary>
    private void BindEvent(GameObject go, EventTriggerType type, Action callback)
    {
        if (go == null || callback == null) return;

        var trigger = go.GetComponent<EventTrigger>();
        if (trigger == null)
            trigger = go.AddComponent<EventTrigger>();

        // 이벤트 엔트리 생성
        var entry = new EventTrigger.Entry { eventID = type };
        entry.callback.AddListener((data) => callback());
        trigger.triggers.Add(entry);
    }
}
