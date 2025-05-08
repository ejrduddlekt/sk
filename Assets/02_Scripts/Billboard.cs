// Assets/02_Scripts/Billboard.cs
using UnityEngine;

/// <summary>
/// 카메라를 바라보도록 회전시키는 빌보드 스크립트.
/// </summary>
[ExecuteAlways]      // 에디터 씬 뷰에서도 동작
public class Billboard : MonoBehaviour
{
    public enum UpdatePhase { Update, LateUpdate }

    [Header("카메라 지정 (비워두면 Camera.main)")]
    public Camera targetCamera;

    [Header("업데이트 위치")]
    public UpdatePhase updatePhase = UpdatePhase.LateUpdate;

    [Header("축 고정")]
    public bool lockX = false;
    public bool lockY = false;
    public bool lockZ = false;

    private void Awake()
    {
        if(targetCamera == null)
            targetCamera = Camera.main;
    }

    void Update()
    {
        if (updatePhase == UpdatePhase.Update) FaceCamera();
    }

    void LateUpdate()
    {
        if (updatePhase == UpdatePhase.LateUpdate) FaceCamera();
    }

    void FaceCamera()
    {
        if (targetCamera == null) targetCamera = Camera.main;
        if (targetCamera == null) return;

        // 카메라→오브젝트 방향
        Vector3 dir = transform.position - targetCamera.transform.position;

        // 선택된 축을 고정하기 위해 성분 제거
        if (lockX) dir.x = 0f;
        if (lockY) dir.y = 0f;
        if (lockZ) dir.z = 0f;

        // 방향 벡터가 0이면 회전 건너뜀
        if (dir.sqrMagnitude < 0.0001f) return;

        transform.rotation = Quaternion.LookRotation(dir);
    }
}
