using UnityEngine;
[CreateAssetMenu(fileName = "Settings", menuName = "Settings/Create new settings SO")]

public class SettingsSO : ScriptableObject
{
    [SerializeField] private int _cardsDealNum = 11;
    [SerializeField] private int _minLegalCardCombNum = 3;
    [Space(10)]
    [Tooltip("List of suits used in code to specify the order of suits for creation and suit operations (e.g. creation of cards, shiffling)")]
    public SuitSO suitSettings;
    [Header("Dealing Animation Settings")]
    [SerializeField] private float _cardDealingAnimTime = 1f;
    [SerializeField] private float _delayBetweenDealingCards = 0.2f;

    [Header("Card Animation Settings")]
    [SerializeField] private float _cardTurnAnimTime = 0.35f;

    [Header("Returning Cards To Deck Animation Settings")]
    [SerializeField] private float _cardReturnToDeckAnimTime = 0.2f;
    [SerializeField] private float _delayBetweenReturningCardsToDeck = 0.1f;

    public int CardsDealNum => _cardsDealNum;
    public int MinLegalCardsCombNum => _minLegalCardCombNum;

    public float CardDealingAnimTime => _cardDealingAnimTime;
    public float DelayBetweenDealingCards => _delayBetweenDealingCards;

    public float CardTurnAnimTime => _cardTurnAnimTime;

    public float CardReturnToDeckAnimTime => _cardReturnToDeckAnimTime;
    public float DelayBetweenReturningCardsToDeck => _delayBetweenReturningCardsToDeck;
}
