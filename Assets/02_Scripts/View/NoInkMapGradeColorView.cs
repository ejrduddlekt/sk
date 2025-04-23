// NoInkMapGradeColorView.cs
using UnityEngine;
using TMPro;
using System;

public class NoInkMapGradeColorView : MonoBehaviour, IRecordView<Data.NoInkMapGradeColor>
{
    [SerializeField] TMP_Text binCharText, gradeText, passText, remarkText, foreColorText, backColorText;
    private Data.NoInkMapGradeColor _data;
    public event Action<Data.NoInkMapGradeColor> OnClicked;

    void Awake() { /* null checks */ }
    public void Init(Data.NoInkMapGradeColor data)
    {
        if (data == null) return;
        _data = data;
        binCharText.text = data.BINCHAR;
        gradeText.text = data.GRADE;
        passText.text = data.PASS;
        remarkText.text = data.REMARK;
        foreColorText.text = data.FORECOLOR;
        backColorText.text = data.BACKCOLOR;
    }
    void OnMouseDown() { if (_data != null) OnClicked?.Invoke(_data); }
}