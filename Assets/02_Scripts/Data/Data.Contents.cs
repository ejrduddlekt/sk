using System;
using System.Collections.Generic;
using System.Linq;
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
    public class LotsList { public List<Lots> wafer_list; }
    [Serializable]
    public class Lots { public string LOT_ID; public string WF_ID; public string PRODUCT_STATE; public string POSITION; }

    [Serializable]
    public class StackMapArgumentList
    {
        /// <summary>
        /// [층 번호] → [좌표(Vector2Int) → 인자]
        /// </summary>
        public SerializedDictionary<int, SerializedDictionary<Vector2Int, StackMapArgument>> stackMapArgumentDict;

        /// <summary>
        /// Raw 데이터를 받아서 중첩 딕셔너리를 초기화합니다.
        /// </summary>
        public void InitFromRaw(RawStackMapList raw)
        {
            var outer = new SerializedDictionary<int, SerializedDictionary<Vector2Int, StackMapArgument>>();

            foreach (var r in raw.stackmap_list)
            {
                if (!int.TryParse(r.STACK_NO, out var floor))
                    continue;

                // 좌표 파싱
                if (!int.TryParse(r.X_POSITION, out var x)) x = 0;
                if (!int.TryParse(r.Y_POSITION, out var y)) y = 0;
                var pos = new Vector2Int(x, y);

                // 층별 딕셔너리 가져오기/생성
                if (!outer.TryGetValue(floor, out var inner))
                {
                    inner = new SerializedDictionary<Vector2Int, StackMapArgument>();
                    outer[floor] = inner;
                }

                // 중복 방지(원하는 정책에 따릅니다)
                if (inner.ContainsKey(pos))
                    continue;

                // 인자 생성 및 저장
                inner[pos] = new StackMapArgument
                {
                    X_AXIS = r.X_AXIS,
                    Y_AXIS = r.Y_AXIS,
                    X_POSITION = r.X_POSITION,
                    Y_POSITION = r.Y_POSITION,
                    STACK_DIE_VAL = r.STACK_DIE_VAL,
                    DIE_WF_LOT_ID = r.DIE_WF_LOT_ID,
                    DIE_WF_ID = r.DIE_WF_ID,
                    DIE_X_COORDINATE = r.DIE_X_COORDINATE,
                    DIE_Y_COORDINATE = r.DIE_Y_COORDINATE
                };
            }

            stackMapArgumentDict = outer;
        }

        /// <summary>
        /// 특정 층과 좌표에 해당하는 인자를 꺼내봅니다.
        /// </summary>
        public bool TryGetArgument(int floor, Vector2Int pos, out StackMapArgument arg)
        {
            arg = null;

            if (stackMapArgumentDict != null
                && stackMapArgumentDict.TryGetValue(floor, out var inner)
                && inner.TryGetValue(pos, out arg))
            {
                return true;
            }

            return false;
        }
    }


    [Serializable]
    public class StackMapArgument
    {
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


    [Serializable]
    public class StackMapList { public List<StackMap> stackmap_list; }
    [Serializable]
    public class StackMap
    {
        public string LOT_ID;
        public string WF_ID;
        public string STACK_NO;
    }

    [Serializable]
    public class NoInkMapList { public List<NoInkMap> noinkmap_list; }
    [Serializable]
    public class NoInkMap { public string LOT_ID; public string WF_ID; public string OPER_ID; public string TSV_TYPE; public string PASS_DIE_QTY; public string FLAT_ZONE_TYPE; public string STACK_NO; public string X_AXIS; public string Y_AXIS; public string X_POSITION; public string Y_POSITION; public string DIE_VAL; public string DIE_THICKNESS; public string DIE_X_COORDINATE; public string DIE_Y_COORDINATE; }

    [Serializable]
    public class NoInkMapGradeColorList { public List<NoInkMapGradeColor> noinkmapgradecolor_list; }
    [Serializable]
    public class NoInkMapGradeColor { public string BINCHAR; public string GRADE; public string PASS; public string REMARK; public string FORECOLOR; public string BACKCOLOR; }

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
                // StackMapList 중복 제거 키
                string key = $"{r.LOT_ID}|{r.WF_ID}|{r.STACK_NO}";
                if (!uniqueSet.Contains(key))
                {
                    uniqueSet.Add(key);
                    resultList.Add(new StackMap
                    {
                        LOT_ID = r.LOT_ID,
                        WF_ID = r.WF_ID,
                        STACK_NO = r.STACK_NO
                    });
                }

                // 좌표 파싱 및 StackMapArgumentDict 구성
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
                        X_AXIS = r.X_AXIS,
                        Y_AXIS = r.Y_AXIS,
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

