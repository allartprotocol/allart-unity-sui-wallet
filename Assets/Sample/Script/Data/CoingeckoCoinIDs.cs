using System.Collections;
using System.Collections.Generic;
using UnityEngine;



[CreateAssetMenu(fileName = "CoingeckoCoinIDs", menuName = "AllArtSUIWallet/ApiData", order = 1)]
public class CoingeckoCoinIDs : ScriptableObject
{
    public List<CoinID> coinIDs;
}
