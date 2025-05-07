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

    private Data.StackMap _data;
    public event Action<Data.StackMap> OnClicked;

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
    public event Action<StackMapLayerView> OnClickedView;
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
        OnClicked?.Invoke(_data);         // 기존 방식 (데이터용)
        OnClickedView?.Invoke(this);      // View 기준 클릭 이벤트 (데이터 없어도 동작)
    }

}

