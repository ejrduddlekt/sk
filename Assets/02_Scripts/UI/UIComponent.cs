// IUIComponent.cs
// 공통 UI 패널이 구현해야 할 메서드를 정의하는 인터페이스
using UnityEngine;
using UnityEngine.UI;
public interface IUIComponent
{
    /// <summary>초기화 (데이터 바인딩, 이벤트 연결 등)</summary>
    void Initialize();

    /// <summary>패널 열기</summary>
    void Show();

    /// <summary>패널 닫기</summary>
    void Close();

    /// <summary>현재 보이는 상태</summary>
    bool IsVisible { get; }
}


// UIComponent.cs
// MonoBehaviour와 IUIComponent를 결합한 추상 베이스 클래스
// 모든 UI 패널이 상속받아 공통 기능을 사용할 수 있습니다.

public abstract class UIComponent : MonoBehaviour, IUIComponent
{
    /// <summary>닫기 버튼 (Inspector에서 할당)</summary>
    [SerializeField] protected Button closeButton;

    protected virtual void Awake()
    {
        // closeButton이 있으면 Close 메서드를 바인딩
        if (closeButton != null)
        {
            closeButton.onClick.AddListener(Close);
        }
    }

    /// <summary>데이터 바인딩, 버튼 이벤트 연결 등을 구현</summary>
    public virtual void Initialize()
    {
        // 빈 구현: 자식 클래스에서 오버라이드
    }

    /// <summary>패널을 보이도록 설정</summary>
    public virtual void Show()
    {
        gameObject.SetActive(true);
    }

    /// <summary>패널을 숨기도록 설정</summary>
    public virtual void Close()
    {
        gameObject.SetActive(false);
    }

    /// <summary>현재 보이는지 여부</summary>
    public bool IsVisible => gameObject.activeSelf;
}
