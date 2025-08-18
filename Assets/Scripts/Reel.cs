using System.Collections.Generic;
using UnityEngine;

public class Reel : MonoBehaviour
{
    public List<SlotSymbol> symbols;
    [HideInInspector]
    public SlotSymbol[] currentSymbols;

    public void Spin()
    {
        if (symbols == null || symbols.Count == 0)
        {
            Debug.LogWarning("Reel has no symbols defined");
            return;
        }

        currentSymbols = new SlotSymbol[3];
        for (int i = 0; i < 3; i++)
        {
            currentSymbols[i] = symbols[Random.Range(0, symbols.Count)];
        }
    }
}
