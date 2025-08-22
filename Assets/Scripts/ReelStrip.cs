using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "ReelStrip", menuName = "Slots/ReelStrip")]
public class ReelStrip : ScriptableObject
{
    public List<int> symbols = new List<int>();
}
