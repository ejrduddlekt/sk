using System;
using System.Collections.Generic;
using UnityEngine;
using VInspector;

namespace Data
{
    [Serializable]
    public class CommandWrapper<T>
    {
        public string command;
        public T payload;
    }

    [Serializable]
    public class LotsList
    {
        public List<Lots> wafer_list;
    }

    [Serializable]
    public class Lots
    {
        public string LOT_ID;
        public string WF_ID;
        public string PRODUCT_STATE;
        public string POSITION;
    }

    [Serializable]
    public class StackMapArgumentList
    {
        /// <summary>
        /// [층 번호] → [좌표(Vector2Int) → 인자]
        /// </summary>
        public SerializedDictionary<int, SerializedDictionary<Vector2Int, StackMapArgument>> stackMapArgumentDict;

        /// <summary>
        /// 특정 층과 좌표에 해당하는 인자를 꺼내봅니다.
        /// </summary>
        public bool TryGetArgument(int floor, Vector2Int pos, out StackMapArgument arg)
        {
            arg = null;
            return stackMapArgumentDict != null
                && stackMapArgumentDict.TryGetValue(floor, out var inner)
                && inner.TryGetValue(pos, out arg);
        }
    }

    [Serializable]
    public class StackMapArgument
    {
        public string X_POSITION;
        public string Y_POSITION;
        public string STACK_DIE_VAL;
        public string DIE_WF_LOT_ID;
        public string DIE_WF_ID;
        public string DIE_X_COORDINATE;
        public string DIE_Y_COORDINATE;
    }

    [Serializable]
    public class StackMapList
    {
        public List<StackMap> stackmap_list;
    }

    [Serializable]
    public class StackMap
    {
        public string LOT_ID;
        public string WF_ID;
        public string STACK_NO;

        // 해당 층의 그리드 최대 크기
        public int X_AXIS;
        public int Y_AXIS;
    }

    [Serializable]
    public class NoInkMapList
    {
        public List<NoInkMap> noinkmap_list;
    }

    [Serializable]
    public class NoInkMap
    {
        public string LOT_ID;
        public string WF_ID;
        public string OPER_ID;
        public string TSV_TYPE;
        public string PASS_DIE_QTY;
        public string FLAT_ZONE_TYPE;
        public string STACK_NO;
        public string X_AXIS;
        public string Y_AXIS;
        public string X_POSITION;
        public string Y_POSITION;
        public string DIE_VAL;
        public string DIE_THICKNESS;
        public string DIE_X_COORDINATE;
        public string DIE_Y_COORDINATE;
    }

    [Serializable]
    public class NoInkMapGradeColorList
    {
        public List<NoInkMapGradeColor> noinkmapgradecolor_list;
    }

    [Serializable]
    public class NoInkMapGradeColor
    {
        public string BINCHAR;
        public string GRADE;
        public string PASS;
        public string REMARK;
        public string FORECOLOR;
        public string BACKCOLOR;
    }

    /// <summary>
    /// 파이프를 통해 들어온 원본 JSON 구조를 그대로 담기 위한 모델
    /// </summary>
    [Serializable]
    public class RawStackMapList
    {
        public List<RawStackMap> stackmap_list;

        /// <summary>
        /// Raw 데이터를 주어진 StackMapList와 StackMapArgumentList에 직접 채워넣습니다.
        /// </summary>
        public void Populate(StackMapList mapList, StackMapArgumentList argList)
        {
            var uniqueSet = new HashSet<string>();
            var resultList = new List<StackMap>();
            var outer = new SerializedDictionary<int, SerializedDictionary<Vector2Int, StackMapArgument>>();

            foreach (var r in stackmap_list)
            {
                // 1) StackMapList 중복 제거 및 X_AXIS/Y_AXIS 파싱
                string key = $"{r.LOT_ID}|{r.WF_ID}|{r.STACK_NO}";
                if (!uniqueSet.Contains(key))
                {
                    uniqueSet.Add(key);

                    int xAxis = int.TryParse(r.X_AXIS, out var xa) ? xa : 0;
                    int yAxis = int.TryParse(r.Y_AXIS, out var ya) ? ya : 0;

                    resultList.Add(new StackMap
                    {
                        LOT_ID = r.LOT_ID,
                        WF_ID = r.WF_ID,
                        STACK_NO = r.STACK_NO,
                        X_AXIS = xAxis,
                        Y_AXIS = yAxis
                    });
                }

                // 2) StackMapArgumentList 구성 (좌표별 인자)
                if (!int.TryParse(r.STACK_NO, out var floor)) continue;
                if (!int.TryParse(r.X_POSITION, out var x)) x = 0;
                if (!int.TryParse(r.Y_POSITION, out var y)) y = 0;
                var pos = new Vector2Int(x, y);

                if (!outer.TryGetValue(floor, out var inner))
                {
                    inner = new SerializedDictionary<Vector2Int, StackMapArgument>();
                    outer[floor] = inner;
                }

                if (!inner.ContainsKey(pos))
                {
                    inner[pos] = new StackMapArgument
                    {
                        X_POSITION = r.X_POSITION,
                        Y_POSITION = r.Y_POSITION,
                        STACK_DIE_VAL = r.STACK_DIE_VAL,
                        DIE_WF_LOT_ID = r.DIE_WF_LOT_ID,
                        DIE_WF_ID = r.DIE_WF_ID,
                        DIE_X_COORDINATE = r.DIE_X_COORDINATE,
                        DIE_Y_COORDINATE = r.DIE_Y_COORDINATE
                    };
                }
            }

            mapList.stackmap_list = resultList;
            argList.stackMapArgumentDict = outer;
        }
    }

    /// <summary>
    /// StackMapList 안의 개별 레코드.
    /// JsonUtility.FromJson 로 바로 파싱하기 위한 구조체입니다.
    /// </summary>
    [Serializable]
    public class RawStackMap
    {
        public string LOT_ID;
        public string WF_ID;
        public string STACK_NO;
        public string X_AXIS;
        public string Y_AXIS;
        public string X_POSITION;
        public string Y_POSITION;
        public string STACK_DIE_VAL;
        public string DIE_WF_LOT_ID;
        public string DIE_WF_ID;
        public string DIE_X_COORDINATE;
        public string DIE_Y_COORDINATE;
    }
}
