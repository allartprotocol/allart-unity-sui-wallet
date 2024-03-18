using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace AllArt.SUI.RPC.Response {
    public class Page_for_SuiObjectResponse_and_ObjectID<ObjectType> { 
        public List<SUIObjectResponse<ObjectType>> data { get; set; }
        public string nextCursor { get; set; }
        public bool hasNextPage { get; set; }
    }

}
