using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CurrencyController
{
    public int CoinsOwned {  get; private set; }

    public CurrencyController(int startingCoins)
    {
        CoinsOwned = startingCoins;
    }

    public void UpdateCoins(int amount) => CoinsOwned += amount;
}
