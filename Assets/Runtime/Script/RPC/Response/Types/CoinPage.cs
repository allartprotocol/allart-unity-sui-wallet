using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinPage {
    public string coinType;
    public string coinObjectId;
    public string version;
    public string digest;
    public string balance;
    public string previousTransaction;
}

public class CoinMetadata
{ 
    public ObjectId id;
    public string decimals;
    public string name;
    public string symbol;
    public string description;
    public string icon_url;
}
