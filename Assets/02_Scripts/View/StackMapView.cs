using UnityEngine;
using TMPro;
using System;
using Data;

public class StackMapView : UIComponent, IRecordView<Data.StackMap>
{
    [Header("Primary IDs")]
    [SerializeField] TMP_Text lotIdText;
    [SerializeField] TMP_Text wfIdText;

    [Header("Stack Info")]
    [SerializeField] TMP_Text stackNoText;
    [SerializeField] TMP_Text xAxisText;
    [SerializeField] TMP_Text yAxisText;
    [SerializeField] TMP_Text xPosText;
    [SerializeField] TMP_Text yPosText;

    [Header("Die Details")]
    [SerializeField] TMP_Text dieValText;
    [SerializeField] TMP_Text dieThicknessText;
    [SerializeField] TMP_Text dieWfLotIdText;
    [SerializeField] TMP_Text dieWfIdText;
    [SerializeField] TMP_Text dieXCoordText;
    [SerializeField] TMP_Text dieYCoordText;

    private Data.StackMap _data;
    public event Action<Data.StackMap> OnClicked;

    protected override void Awake()
    {
        base.Awake();
        //// 필수 참조 누락 체크
        //if (lotIdText == null) Debug.LogError($"{name}: lotIdText is not assigned!", this);
        //if (wfIdText == null) Debug.LogError($"{name}: wfIdText is not assigned!", this);
        //if (stackNoText == null) Debug.LogError($"{name}: stackNoText is not assigned!", this);
        //if (xAxisText == null) Debug.LogError($"{name}: xAxisText is not assigned!", this);
        //if (yAxisText == null) Debug.LogError($"{name}: yAxisText is not assigned!", this);
        //if (xPosText == null) Debug.LogError($"{name}: xPosText is not assigned!", this);
        //if (yPosText == null) Debug.LogError($"{name}: yPosText is not assigned!", this);
        //if (dieValText == null) Debug.LogError($"{name}: dieValText is not assigned!", this);
        //if (dieThicknessText == null) Debug.LogError($"{name}: dieThicknessText is not assigned!", this);
        //if (dieWfLotIdText == null) Debug.LogError($"{name}: dieWfLotIdText is not assigned!", this);
        //if (dieWfIdText == null) Debug.LogError($"{name}: dieWfIdText is not assigned!", this);
        //if (dieXCoordText == null) Debug.LogError($"{name}: dieXCoordText is not assigned!", this);
        //if (dieYCoordText == null) Debug.LogError($"{name}: dieYCoordText is not assigned!", this);
    }

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
        if (xAxisText != null) xAxisText.text = data.X_AXIS;
        if (yAxisText != null) yAxisText.text = data.Y_AXIS;
        if (xPosText != null) xPosText.text = data.X_POSITION;
        if (yPosText != null) yPosText.text = data.Y_POSITION;
        if (dieValText != null) dieValText.text = data.STACK_DIE_VAL;
        if (dieWfLotIdText != null) dieWfLotIdText.text = data.DIE_WF_LOT_ID;
        if (dieWfIdText != null) dieWfIdText.text = data.DIE_WF_ID;
        if (dieXCoordText != null) dieXCoordText.text = data.DIE_X_COORDINATE;
        if (dieYCoordText != null) dieYCoordText.text = data.DIE_Y_COORDINATE;
    }
    public event Action<StackMapView> OnClickedView;
    public event Action<StackMapView> OnMouseEnterView;
    public event Action<StackMapView> OnMouseExitView;

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
        //if (_data == null)
        //{
        //    Debug.LogWarning($"{name}: Click ignored because data is null", this);

        //    return;
        //}
        OnClicked?.Invoke(_data);         // 기존 방식 (데이터용)
        OnClickedView?.Invoke(this);      // View 기준 클릭 이벤트 (데이터 없어도 동작)

    }

}

