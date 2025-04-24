// PanelSwitcher.cs
// 여러 버튼에 매핑된 패널(GameObject)을 활성/비활성하여 화면을 전환하는 컴포넌트

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public struct PanelEntry
{
    [Tooltip("이 버튼 클릭 시 해당 패널을 활성화 합니다.")]
    public Button button;
    [Tooltip("매핑된 패널(GameObject)")]
    public GameObject panel;
}

public class PanelSwitcher : MonoBehaviour
{
    [Header("버튼 ↔ 패널 매핑 리스트")]
    [Tooltip("버튼과 패널을 짝으로 추가하세요.")]
    [SerializeField] private List<PanelEntry> entries = new List<PanelEntry>();

    [Header("기본으로 활성화할 패널 인덱스")]
    [SerializeField] private int defaultIndex = 0;

    private void Awake()
    {
        // 모든 버튼에 클릭 리스너 등록
        for (int i = 0; i < entries.Count; i++)
        {
            int index = i; // 클로저 캡쳐 방지
            if (entries[i].button != null)
            {
                entries[i].button.onClick.AddListener(() => SwitchTo(index));
            }
        }

        // 초기 패널 설정
        SwitchTo(defaultIndex);
    }

    /// <summary>
    /// 지정한 인덱스의 패널만 활성화하고 나머지는 비활성화합니다.
    /// </summary>
    /// <param name="index">entries 리스트 내 활성화할 패널 인덱스</param>
    public void SwitchTo(int index)
    {
        if (entries == null || entries.Count == 0)
            return;

        // 인덱스 범위 클램핑
        index = Mathf.Clamp(index, 0, entries.Count - 1);

        for (int i = 0; i < entries.Count; i++)
        {
            var panel = entries[i].panel;
            if (panel != null)
                panel.SetActive(i == index);
        }
    }

    /// <summary>
    /// 외부 코드에서 직접 버튼-패널 전환 호출
    /// </summary>
    public void SwitchToPanel(GameObject panel)
    {
        int idx = entries.FindIndex(e => e.panel == panel);
        if (idx >= 0)
            SwitchTo(idx);
    }
}
