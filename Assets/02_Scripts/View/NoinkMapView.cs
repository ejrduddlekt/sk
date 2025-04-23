using UnityEngine;
using TMPro;
using System;

public class NoInkMapView : MonoBehaviour, IRecordView<Data.NoInkMap>
{
    [Header("Primary IDs")]
    [SerializeField] TMP_Text lotIdText;
    [SerializeField] TMP_Text wfIdText;

    [Header("Process Info")]
    [SerializeField] TMP_Text operIdText;
    [SerializeField] TMP_Text tsvTypeText;
    [SerializeField] TMP_Text passDieQtyText;
    [SerializeField] TMP_Text flatZoneTypeText;

    [Header("Stack & Position")]
    [SerializeField] TMP_Text stackNoText;
    [SerializeField] TMP_Text xAxisText;
    [SerializeField] TMP_Text yAxisText;
    [SerializeField] TMP_Text xPosText;
    [SerializeField] TMP_Text yPosText;

    [Header("Die Details")]
    [SerializeField] TMP_Text dieValText;
    [SerializeField] TMP_Text dieThicknessText;
    [SerializeField] TMP_Text dieXCoordText;
    [SerializeField] TMP_Text dieYCoordText;

    private Data.NoInkMap _data;
    public event Action<Data.NoInkMap> OnClicked;

    void Awake()
    {
        //if (lotIdText == null) Debug.LogError($"{name}: lotIdText is not assigned!", this);
        //if (wfIdText == null) Debug.LogError($"{name}: wfIdText is not assigned!", this);
        //if (operIdText == null) Debug.LogError($"{name}: operIdText is not assigned!", this);
        //if (tsvTypeText == null) Debug.LogError($"{name}: tsvTypeText is not assigned!", this);
        //if (passDieQtyText == null) Debug.LogError($"{name}: passDieQtyText is not assigned!", this);
        //if (flatZoneTypeText == null) Debug.LogError($"{name}: flatZoneTypeText is not assigned!", this);
        //if (stackNoText == null) Debug.LogError($"{name}: stackNoText is not assigned!", this);
        //if (xAxisText == null) Debug.LogError($"{name}: xAxisText is not assigned!", this);
        //if (yAxisText == null) Debug.LogError($"{name}: yAxisText is not assigned!", this);
        //if (xPosText == null) Debug.LogError($"{name}: xPosText is not assigned!", this);
        //if (yPosText == null) Debug.LogError($"{name}: yPosText is not assigned!", this);
        //if (dieValText == null) Debug.LogError($"{name}: dieValText is not assigned!", this);
        //if (dieThicknessText == null) Debug.LogError($"{name}: dieThicknessText is not assigned!", this);
        //if (dieXCoordText == null) Debug.LogError($"{name}: dieXCoordText is not assigned!", this);
        //if (dieYCoordText == null) Debug.LogError($"{name}: dieYCoordText is not assigned!", this);
    }

    public void Init(Data.NoInkMap data)
    {
        if (data == null)
        {
            Debug.LogWarning($"{name}: Init called with null data", this);
            return;
        }

        _data = data;

        if (lotIdText != null) lotIdText.text = data.LOT_ID;
        if (wfIdText != null) wfIdText.text = data.WF_ID;
        if (operIdText != null) operIdText.text = data.OPER_ID;
        if (tsvTypeText != null) tsvTypeText.text = data.TSV_TYPE;
        if (passDieQtyText != null) passDieQtyText.text = data.PASS_DIE_QTY;
        if (flatZoneTypeText != null) flatZoneTypeText.text = data.FLAT_ZONE_TYPE;
        if (stackNoText != null) stackNoText.text = data.STACK_NO;
        if (xAxisText != null) xAxisText.text = data.X_AXIS;
        if (yAxisText != null) yAxisText.text = data.Y_AXIS;
        if (xPosText != null) xPosText.text = data.X_POSITION;
        if (yPosText != null) yPosText.text = data.Y_POSITION;
        if (dieValText != null) dieValText.text = data.DIE_VAL;
        if (dieThicknessText != null) dieThicknessText.text = data.DIE_THICKNESS;
        if (dieXCoordText != null) dieXCoordText.text = data.DIE_X_COORDINATE;
        if (dieYCoordText != null) dieYCoordText.text = data.DIE_Y_COORDINATE;
    }

    void OnMouseDown()
    {
        if (_data == null)
        {
            Debug.LogWarning($"{name}: Click ignored because data is null", this);
            return;
        }
        OnClicked?.Invoke(_data);
    }
}
