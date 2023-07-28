using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AllArt.SUI.RPC.Filter.Types {


    public class ObjectDataFilter { 

    }

    public class  MatchAllDataFilter: ObjectDataFilter
    {
        public List<ObjectDataFilter> MatchAll;
    }

    public class  MatchAnyDataFilter: ObjectDataFilter
    {
        public List<ObjectDataFilter> MatchAny;
    }

    public class  MatchNoneDataFilter: ObjectDataFilter
    {
        public List<ObjectDataFilter> MatchNone;
    }

    public class  StructTypeDataFilter: ObjectDataFilter
    {
        public string StructType;
    }

    public class OwnerDataFilter: ObjectDataFilter
    {
        public string AddressOwner;
    }

    public class VersionDataFilter : ObjectDataFilter
    {
        public string Version;
    }
}