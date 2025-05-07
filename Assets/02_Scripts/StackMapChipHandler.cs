using System;
using UnityEngine;

public class StackMapChipHandler : MonoBehaviour
{
    public Vector2Int GridPosition { get; private set; }
    public Data.StackMapArgument Argument { get; private set; }

    /// <summary>칩 클릭 시 발생하는 이벤트</summary>
    public event Action<Data.StackMapArgument> OnChipClicked;

    /// <summary>
    /// 초기화: 좌표와 데이터를 꼭 전달하세요.
    /// </summary>
    public void Initialize(Data.StackMapArgument arg)
    {
        GridPosition = new Vector2Int(int.Parse(arg.X_POSITION), int.Parse(arg.Y_POSITION));
        Argument = arg;
    }

    private void OnMouseDown()
    {
        OnChipClicked?.Invoke(Argument);
    }
}
