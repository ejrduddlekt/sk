// Assets/02_Scripts/Data/RawStackMapJsonGenerator.cs

using System.IO;
using System.Collections.Generic;
using UnityEngine;
using Data;  // RawStackMapList, RawStackMap 정의된 네임스페이스

#if UNITY_EDITOR
using UnityEditor;
using VInspector;  // [Button] 어트리뷰트
#endif

public class RawStackMapJsonGenerator : MonoBehaviour
{
    [Header("직접 입력할 값")]
    public string LOT_ID = "LOT_ID";
    public string WF_ID = "12";
    [Tooltip("1부터 이 숫자까지 층을 생성")]
    public int maxStackLayer = 1;
    [Tooltip("0부터 이 숫자까지 X_POSITION/Y_POSITION 생성")]
    public int X_AXIS = 26;
    public int Y_AXIS = 25;
    [Tooltip("STACK_DIE_VAL 에서 랜덤 추출할 값들")]
    public List<string> stackDieVals = new List<string> { "VAL_A", "VAL_B", "VAL_C" };
    public string DIE_WF_LOT_ID = "DIE_WF_LOT_ID";
    public string DIE_WF_ID = "15";
    public string DIE_X_COORDINATE = "14";
    public string DIE_Y_COORDINATE = "29";

    [Header("출력")]
    [Tooltip("Assets/02_Scripts/Data 폴더 내에 이 이름으로 저장")]
    public string outputFileName = "stackmap.json";

#if UNITY_EDITOR
    [Button("Generate JSON")]
#endif
    public void GenerateJson()
    {
        // 1) RawStackMapList 생성
        var rawList = new RawStackMapList
        {
            stackmap_list = new List<RawStackMap>()
        };

        // 2) 모든 층(layer)과 좌표(x,y) 조합에 대해 엔트리 추가
        for (int layer = 1; layer <= maxStackLayer; layer++)
        {
            string sNo = layer.ToString();
            for (int x = 0; x <= X_AXIS; x++)
            {
                for (int y = 0; y <= Y_AXIS; y++)
                {
                    rawList.stackmap_list.Add(new RawStackMap
                    {
                        LOT_ID = LOT_ID,
                        WF_ID = WF_ID,
                        STACK_NO = sNo,
                        X_AXIS = X_AXIS.ToString(),
                        Y_AXIS = Y_AXIS.ToString(),
                        X_POSITION = x.ToString(),
                        Y_POSITION = y.ToString(),
                        STACK_DIE_VAL = stackDieVals[Random.Range(0, stackDieVals.Count)],
                        DIE_WF_LOT_ID = DIE_WF_LOT_ID,
                        DIE_WF_ID = DIE_WF_ID,
                        DIE_X_COORDINATE = DIE_X_COORDINATE,
                        DIE_Y_COORDINATE = DIE_Y_COORDINATE
                    });
                }
            }
        }

        // 3) JSON 직렬화
        string json = JsonUtility.ToJson(rawList, true);

        // 4) 파일 쓰기 (Assets/02_Scripts/Data 폴더)
        string dataFolder = Path.Combine(Application.dataPath, "02_Scripts", "Data");
        if (!Directory.Exists(dataFolder))
            Directory.CreateDirectory(dataFolder);

        string fullPath = Path.Combine(dataFolder, outputFileName);
        File.WriteAllText(fullPath, json);

        // 5) 에디터 프로젝트 창 갱신
#if UNITY_EDITOR
        AssetDatabase.Refresh();
#endif

        Debug.Log($"[RawStackMapJsonGenerator] JSON saved to: {fullPath} (entries: {rawList.stackmap_list.Count})");
    }
}
