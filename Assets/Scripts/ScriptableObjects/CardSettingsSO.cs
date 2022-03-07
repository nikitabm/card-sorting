using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "Suits", menuName = "Settings/Create new suits SO")]

public class CardSettingsSO : ScriptableObject
{
    [Tooltip("List of suits used in code to specify the order of suits for creation and suit operations (e.g. creation of cards, shiffling)")]
    [SerializeField] private List<ESuit> _suitOrder = new List<ESuit>();

    [SerializeField] private Sprite _cardBackSprite;

    [SerializeField] private Sprite[] _cardSprites;

    [SerializeField] private List<int> _cardValues = new List<int>();

    public List<ESuit> SuitOrder => _suitOrder;
    public Sprite[] CardSprites => _cardSprites;
    public Sprite CardBackSprite => _cardBackSprite;

    public int GetOrderOfSuit(ESuit suit)
    {
        return _suitOrder.IndexOf(suit);
    }

    public int GetCardValue(int cardRank)
    {
        return _cardValues[cardRank - 1];
    }
}
