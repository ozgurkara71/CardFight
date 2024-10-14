using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Card", menuName = "Card", order = 0)]
public class Card : ScriptableObject
{
    [SerializeField] List<Color> cardColors = new List<Color>();

    // takes and passes list of colors as parameter. Only one color is passed as list,
    // that's why we add value[0] to the list
    public List<Color> CardColors { get { return cardColors; } set { cardColors.Add(value[0]); } }
}
