using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Page_for_SuiObjectResponse_and_ObjectID { 
    public List<SUIObjectResponse> data { get; set; }
    public string nextCursor { get; set; }
    public bool hasNextPage { get; set; }
}
