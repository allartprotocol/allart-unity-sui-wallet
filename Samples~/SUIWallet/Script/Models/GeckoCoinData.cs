using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeckoCoinData {
    public MarketData market_data;
    public GeckoImage image;
}

[System.Serializable]
public class CoinID
{
    public string id;
    public string symbol;
    public string name;
}

public class GeckoImage {
    public string thumb;
    public string small;
    public string large;
    public Sprite thumbImage;
}

[System.Serializable]
public class SUIMarketData {
    public string id;
    public string symbol;
    public string name;
    public string image;
    public object current_price;
    public object price_change_percentage_24h;

    public double current_price_double {
        get {
            double.TryParse(current_price.ToString(), out double price);
            return price;
        }
    }

    public double price_change_percentage_24h_double {
        get {
            double.TryParse(price_change_percentage_24h.ToString(), out double price);
            return price;
        }
    }
}

public class MarketData {
    public Dictionary<string, float> current_price;
    public float price_change_percentage_24h;
}
