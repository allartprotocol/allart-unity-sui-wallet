using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeckoCoinData {
    public MarketData market_data;
    public GeckoImage image;
}

public class GeckoImage {
    public string thumb;
    public string small;
    public string large;
    public Sprite thumbImage;
}

public class MarketData {
    public Dictionary<string, float> current_price;
    public float price_change_percentage_24h;
}
