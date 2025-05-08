// ChipLayoutCalculator.cs
using UnityEngine;

public struct ChipLayout
{
    public float ChipSize;   // 한 칩의 한 변 길이
    public float Scale;      // 프리팹 원래 크기에 대한 스케일
    public Vector3 Offset;   // 첫 칩 위치 오프셋 (x, 0, z)
}

public static class ChipLayoutCalculator
{
    /// <summary>
    /// totalGridSize, cols, rows, prefabRenderer 기반으로
    /// ChipLayout(크기·스케일·오프셋)을 계산합니다.
    /// </summary>
    public static ChipLayout Calculate(float totalGridSize, int cols, int rows, Renderer prefabRenderer)
    {
        // 1) 칩 당 크기
        float chipSize = totalGridSize / Mathf.Max(cols, rows);

        // 2) 프리팹 베이스 사이즈 (x/z 중 큰 값)
        float baseSize = prefabRenderer != null
                       ? Mathf.Max(prefabRenderer.bounds.size.x, prefabRenderer.bounds.size.z)
                       : 1f;

        // 3) 스케일
        float scale = chipSize / baseSize;

        // 4) 그리드 전체 중앙을 (0,0) 으로 맞추기 위한 오프셋
        float half = totalGridSize * 0.5f;
        float xOffset = -half + chipSize * 0.5f;
        float zOffset = -half + chipSize * 0.5f;

        return new ChipLayout
        {
            ChipSize = chipSize,
            Scale = scale,
            Offset = new Vector3(xOffset, 0f, zOffset)
        };
    }
}
