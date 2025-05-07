// WaferView.cs
using UnityEngine;
using TMPro;
using System;
using UnityEngine.EventSystems;

public class WaferView : UIComponent, IRecordView<Data.Lots>, IPointerClickHandler
{
    [SerializeField] TMP_Text lotIdText;
    [SerializeField] TMP_Text wfIdText;
    private Data.Lots _data;
    public event Action<Data.Lots> OnClicked;

    protected override void Awake()
    {
        base.Awake();
        //if (lotIdText == null) Debug.LogError($"{name}: lotIdText is not assigned!", this);
        //if (wfIdText == null) Debug.LogError($"{name}: wfIdText is not assigned!", this);
    }

    public void Init(Data.Lots data)
    {
        if (data == null) return;
        _data = data;
        if (lotIdText != null) lotIdText.text = data.LOT_ID;
        if (wfIdText != null) wfIdText.text = data.WF_ID;
    }

    // 3D Object Clickw 처리용
    //void OnMouseDown()
    //{
    //    if (_data == null) return;
    //    OnClicked?.Invoke(_data);
    //}

    // UI 클릭 이벤트 처리용
    public void OnPointerClick(PointerEventData eventData)
    {
        if (_data != null)
            OnClicked?.Invoke(_data);
    }
}