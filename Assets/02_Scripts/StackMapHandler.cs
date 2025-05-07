using System.Collections.Generic;
using UnityEngine;

public class StackMapHandler : MonoBehaviour
{
    [Header("스택맵 목록")]
    [SerializeField] public List<StackMapLayerView> stackMapViews;

    [Header("정보 패널 참조")]
    [SerializeField] private NoInkMapView noInkMapView;

    private StackMapMover _currentHighlighted;

    public StackMapMover CurrentHighlighted
    {
        get => _currentHighlighted;
        set
        {
            if (_currentHighlighted == value) return;

            // 이전 오브젝트 비활성화
            if (_currentHighlighted != null)
                _currentHighlighted.IsSelect = false;

            _currentHighlighted = value;

            // 새 오브젝트 활성화
            if (_currentHighlighted != null)
                _currentHighlighted.IsSelect = true;
        }
    }
}
