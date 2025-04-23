// WaferView.cs
using UnityEngine;
using TMPro;
using System;

public class WaferView : UIComponent, IRecordView<Data.Wafer>
{
    [SerializeField] TMP_Text lotIdText;
    [SerializeField] TMP_Text wfIdText;
    private Data.Wafer _data;
    public event Action<Data.Wafer> OnClicked;

    protected override void Awake()
    {
        base.Awake();
        //if (lotIdText == null) Debug.LogError($"{name}: lotIdText is not assigned!", this);
        //if (wfIdText == null) Debug.LogError($"{name}: wfIdText is not assigned!", this);
    }

    public void Init(Data.Wafer data)
    {
        if (data == null) return;
        _data = data;
        if (lotIdText != null) lotIdText.text = data.LOT_ID;
        if (wfIdText != null) wfIdText.text = data.WF_ID;
    }

    void OnMouseDown()
    {
        if (_data == null) return;
        OnClicked?.Invoke(_data);
    }
}