using UnityEngine;

[CreateAssetMenu(menuName = "Slot/Symbol")]
public class SlotSymbol : ScriptableObject
{
    public string symbolName;
    public int payout;
    public bool isBonus;
    public Sprite icon;
}
