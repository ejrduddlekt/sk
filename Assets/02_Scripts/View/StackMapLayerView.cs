using UnityEngine;
using TMPro;
using System;
using Data;

public class StackMapLayerView : MonoBehaviour, IRecordView<Data.StackMap>
{
    [Header("Primary IDs")]
    [SerializeField] TMP_Text lotIdText;
    [SerializeField] TMP_Text wfIdText;

    [Header("Stack Info")]
    [SerializeField] TMP_Text stackNoText;

    [Header("Double-click 설정")]
    [Tooltip("두 번 클릭을 한 번으로 인식할 최대 간격(초)")]
    [SerializeField] private float doubleClickThreshold = 0.3f;

    private float lastClickTime = -1f;   // 직전 클릭 시각

    private Data.StackMap _data;
    public event Action<Data.StackMap> OnClicked;
    private StackMapMover mover;

    public void Init(Data.StackMap data)
    {
        if (data == null)
        {
            Debug.LogWarning($"{name}: Init called with null data", this);
            return;
        }

        _data = data;

        if (lotIdText != null) lotIdText.text = data.LOT_ID;
        if (wfIdText != null) wfIdText.text = data.WF_ID;
        if (stackNoText != null) stackNoText.text = data.STACK_NO;
    }

    public event Action<StackMapLayerView> OnMouseEnterView;
    public event Action<StackMapLayerView> OnMouseExitView;

    void OnMouseEnter()
    {
        OnMouseEnterView?.Invoke(this);
    }

    void OnMouseExit()
    {
        OnMouseExitView?.Invoke(this);
    }

    void OnMouseDown()
    {
        float now = Time.time;

        // 직전 클릭과의 간격이 threshold 이하 → 더블-클릭으로 간주
        if (now - lastClickTime <= doubleClickThreshold)
        {
            lastClickTime = -1f;               // 리셋
            OnClicked?.Invoke(_data);          // 더블-클릭 이벤트 발동
        }
        else
        {
            lastClickTime = now;               // 첫번째 클릭 기록
        }
    }
}

