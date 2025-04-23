using System;
using System.Collections.Generic;

namespace Data
{
    [Serializable]
    public class WaferList { public List<Wafer> wafer_list; }
    [Serializable]
    public class Wafer { public string LOT_ID; public string WF_ID; public string PRODUCT_STATE; public string POSITION; }

    [Serializable]
    public class StackMapList { public List<StackMap> stackmap_list; }
    [Serializable]
    public class StackMap { public string LOT_ID; public string WF_ID; public string STACK_NO; public string X_AXIS; public string Y_AXIS; public string X_POSITION; public string Y_POSITION; public string STACK_DIE_VAL; public string DIE_WF_LOT_ID; public string DIE_WF_ID; public string DIE_X_COORDINATE; public string DIE_Y_COORDINATE; }

    [Serializable]
    public class NoInkMapList { public List<NoInkMap> noinkmap_list; }
    [Serializable]
    public class NoInkMap { public string LOT_ID; public string WF_ID; public string OPER_ID; public string TSV_TYPE; public string PASS_DIE_QTY; public string FLAT_ZONE_TYPE; public string STACK_NO; public string X_AXIS; public string Y_AXIS; public string X_POSITION; public string Y_POSITION; public string DIE_VAL; public string DIE_THICKNESS; public string DIE_X_COORDINATE; public string DIE_Y_COORDINATE; }

    [Serializable]
    public class NoInkMapGradeColorList { public List<NoInkMapGradeColor> noinkmapgradecolor_list; }
    [Serializable]
    public class NoInkMapGradeColor { public string BINCHAR; public string GRADE; public string PASS; public string REMARK; public string FORECOLOR; public string BACKCOLOR; }
}