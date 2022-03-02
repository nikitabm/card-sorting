using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using System;

public class GameManager : MonoBehaviour
{
    [SerializeField] private SettingsSO _settings;
    [Space(10)]

    [SerializeField] private Deck _deck;
    [SerializeField] private Hand _hand;
    [SerializeField] private UIManager _uiManager;


    public delegate void EmptyDelegate();
    public event EmptyDelegate GameStarted;

    private SuitSO _suitSettings;

    private void Awake()
    {
        if (!_settings)
        {
            Debug.LogError("No game settings SO found in GameManager, assign settings SO.");
            return;
        }
        _suitSettings = _settings.suitSettings;

    }

    private void Start()
    {
        if (_settings)
        {
            SubscribeToEvents();

            GameStarted.Invoke();
        }
    }

    private void OnDestroy()
    {
        UnsubscribeFromEvents();
    }

    private void SubscribeToEvents()
    {
        GameStarted += OnGameStarted;
    }

    private void UnsubscribeFromEvents()
    {
        GameStarted -= OnGameStarted;
    }

    private void OnGameStarted()
    {
        // populate cards in deck, shuffle them
        _deck.CreateCards(_suitSettings);
        _hand.SetCardListSize(_settings.CardsDealNum);
        // tween in the cards, when finished - deal cards to hand after delay
        _deck.Show(() =>
        {
            StartCoroutine(Utils.DelayAction(() =>
            {
                StartCoroutine(DealCardsToHand());
            }, 0.75f));
        });
    }

    private IEnumerator DealCardsToHand()
    {
        for (int i = 0; i < _settings.CardsDealNum; i++)
        {
            CardDisplay card = _deck.DrawCard();
            card.SetSortingOrder(_settings.CardsDealNum - i);
            var cardInHandPosition = _hand.GetCardPositionAt(i);
            var cardRotation = _hand.GetCardRotationAt(i);
            _deck.AnimateCardToPosition(_settings.CardDealingAnimTime, card, i, _hand.transform.position + new Vector3(-2 + i * 0.3f, 0, 0), Quaternion.Euler(cardRotation), OnCardAnimatedToHand);
            card.Turn(true, _settings.CardTurnAnimTime, 0.3f);
            yield return new WaitForSeconds(_settings.DelayBetweenDealingCards);
        }
    }

    private IEnumerator ReturnCardsToDeck(Action onComplete)
    {
        int cardsDealt = 0;
        int cardsInHand = _hand.Cards.Count;
        for (int i = cardsInHand - 1; i >= 0; i--)
        {
            CardDisplay card = _hand.GetCardAt(i);
            card.SetSortingOrder(i);
            _deck.AnimateCardToPosition(_settings.CardReturnToDeckAnimTime, card, i, _deck.CardSpawnRoot.position, _deck.CardSpawnRoot.rotation, cardDisplay =>
            {
                cardsDealt++;
                _hand.RemoveCard(card);
                _deck.AddCard(card.Card);
                Destroy(card.gameObject);
            });
            card.Turn(false, _settings.CardTurnAnimTime, 0);
            yield return new WaitForSeconds(_settings.DelayBetweenReturningCardsToDeck);
        }
        yield return new WaitWhile(() => cardsDealt != cardsInHand);
        onComplete?.Invoke();
    }

    private void OnCardAnimatedToHand(CardDisplay card)
    {
        _hand.AddCardAt(card);
        card.TurnFaceUp();
    }

    // return cards from hand to deck, shuffle, deal new hand
    public void RedealHand()
    {
        StartCoroutine(ReturnCardsToDeck(() =>
        {
            _deck.Shuffle();
            StartCoroutine(DealCardsToHand());
        }));
    }

    public void SubsequentSort()
    {
        // get cards in hand
        List<CardDisplay> cardDisplays = _hand.Cards;
        // calculate max possible number of successful card combinations;
        var maxSortedCardGroups = _settings.CardsDealNum / _settings.MinLegalCardsCombNum;
        // array of lists that will store valid card combinations + LINQ initialization
        var sortedCardsIds = new List<int>[maxSortedCardGroups].Select(item => new List<int>()).ToArray();
        // array of lists that will store sorted by suit cards + LINQ initialization
        var cardsInSuits = new List<CardDisplay>[_suitSettings.SuitOrder.Count].Select(item => new List<CardDisplay>()).ToArray();

        int numOfCompletedCombinations = 0;
        for (int i = 0; i < cardsInSuits.Length; i++)
        {
            cardsInSuits[i] = cardDisplays.Where(cardDisplay => _suitSettings.SuitOrder.IndexOf(cardDisplay.Card.Suit) == i).ToList();
            if (cardsInSuits[i].Count == 0)
            {
                continue;
            }
            List<CardDisplay> sortedCardDisplays = cardsInSuits[i].OrderBy(CardDisplay => CardDisplay.Card.Rank).ToList();
            var currentCombination = new List<int>();
            currentCombination.Add(cardDisplays.IndexOf(sortedCardDisplays[0]));
            for (int j = 1; j < sortedCardDisplays.Count; j++)
            {
                if (cardDisplays[currentCombination[currentCombination.Count - 1]].Card.Rank + 1 == sortedCardDisplays[j].Card.Rank)
                {
                    currentCombination.Add(cardDisplays.IndexOf(sortedCardDisplays[j]));
                }
                else
                {
                    if (currentCombination.Count >= _settings.MinLegalCardsCombNum)
                    {
                        sortedCardsIds[numOfCompletedCombinations] = new List<int>(currentCombination);
                        numOfCompletedCombinations++;
                    }
                    currentCombination.Clear();
                    currentCombination.Add(cardDisplays.IndexOf(sortedCardDisplays[j]));
                    continue;
                }
            }
            if (currentCombination.Count >= _settings.MinLegalCardsCombNum)
            {
                sortedCardsIds[numOfCompletedCombinations] = new List<int>(currentCombination);
                numOfCompletedCombinations++;
            }
            currentCombination.Clear();
        }

        for (int i = 0; i < sortedCardsIds.Count(); i++)
        {
            if (sortedCardsIds[i].Count > 0)
            {
                print("===| " + cardDisplays[sortedCardsIds[i][0]].Card.Suit + " |===");
                foreach (var cardId in sortedCardsIds[i])
                {
                    print(cardDisplays[cardId].Card.Rank);
                }
            }
            else
            {
                print("No 123 combinations in suit " + i);
            }
        }
    }
}
