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

    public delegate void EmptyDelegate();
    public event EmptyDelegate GameStarted;

    private CardSettingsSO _cardSettings;

    private Sorter _sorting;

    private bool _dealtPredefinedDeck = false;

    private EGameState _state = EGameState.None;

    private void Awake()
    {
        if (!_settings)
        {
            Debug.LogError("No game settings SO found in GameManager, assign settings SO.");
            return;
        }
        _cardSettings = _settings.cardSettings;
        _sorting = GetComponent<Sorter>();
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
        _state = EGameState.Busy;
        // pass order of suits to the deck
        _deck.SetSuitSettings(_cardSettings);
        // populate cards in deck, shuffle them
        _deck.CreateCards();
        _hand.SetCardListSize(_settings.CardsDealNum);
        // tween in the cards, when finished - deal cards to hand after delay
        _deck.Show(() =>
        {
            StartCoroutine(Utils.DelayAction(() =>
            {
                StartCoroutine(DealCardsToHand(ResetGameState));
            }, 0.75f));
        });
    }
    // return cards to hand, deal randomised deck, check state to prevent pressing multiple times
    public void RedealDefaultHand()
    {
        if (_state != EGameState.Idle)
        {
            return;
        }
        _state = EGameState.Busy;
        _hand.SetCardsInteractable(false);
        bool addCardsToDeck = _dealtPredefinedDeck ? false : true;
        StartCoroutine(ReturnCardsToDeck(addCardsToDeck, () =>
         {
             _deck.Shuffle();
             StartCoroutine(DealCardsToHand(ResetGameState));
         }));
    }
    // Return cards from hand to deck, deal predefined deck
    public void RedealPredefinedHand()
    {
        if (_state != EGameState.Idle)
        {
            return;
        }
        _state = EGameState.Busy;
        _hand.SetCardsInteractable(false);
        bool addCardsToDeck = _dealtPredefinedDeck ? false : true;
        StartCoroutine(ReturnCardsToDeck(addCardsToDeck, () =>
         {
             _deck.ResetDealingPredefinedCards();
             StartCoroutine(DealPredefinedCardsToHand(ResetGameState));
         }));
    }
    // function called from UI button. Check state to prevent pressing multiple times
    public void SubsequentSortOfPlayerHand()
    {
        if (_state != EGameState.Idle)
        {
            return;
        }
        List<int>[] sortedPlayerCards = _sorting.SubsequentSort(_hand.Cards, _settings);
        StartCoroutine(AnimateSortedPlayerCards(sortedPlayerCards, ResetGameState));
    }
    // function called from UI button. Check state to prevent pressing multiple times
    public void SameValueSortOfPlayerHand()
    {
        if (_state != EGameState.Idle)
        {
            return;
        }
        List<int>[] sortedPlayerCards = _sorting.SameValueSort(_hand.Cards, _settings);
        StartCoroutine(AnimateSortedPlayerCards(sortedPlayerCards, ResetGameState));

    }
    // function that is called from UI button. Check state to prevent user from pressing the button right after pressing it once
    public void SmartSortOfPlayerHand()
    {
        if (_state != EGameState.Idle)
        {
            return;
        }
        List<int>[] sortedPlayerCards = _sorting.SmartSorting(_hand.Cards, _settings);
        StartCoroutine(AnimateSortedPlayerCards(sortedPlayerCards, ResetGameState));
    }
    /*
    Animate sorting player cards based on array of lists
    every list in array is a valid combination of cards
    */
    private IEnumerator AnimateSortedPlayerCards(List<int>[] sortedCardIds, Action onComplete)
    {
        List<CardDisplay> cards = _hand.Cards;
        float sortedCardsYOffset = 2f;
        int newSortingOrder = 0;
        for (int i = 0; i < sortedCardIds.Count(); i++)
        {
            if (sortedCardIds[i].Count > 0)
            {
                for (int j = 0; j < sortedCardIds[i].Count; j++)
                {
                    CardDisplay card = cards[sortedCardIds[i][j]];
                    // disable dragging of the card
                    card.Draggable.enabled = false;
                    // set sorting order to be larger than sorting order of previous combination, in case they would overlap by any chance
                    card.SetSortingOrder(newSortingOrder);
                    Vector3 newCardPosition = new Vector3(-2 + i * 1.5f, _hand.transform.position.y + sortedCardsYOffset - j * 0.25f, 0);
                    // wait for the card to reach target destination, only then continue
                    _deck.AnimateCardToPosition(_settings.CardDealingAnimTime, card, i, newCardPosition, Quaternion.Euler(0, 0, 0), null);
                    newSortingOrder++;
                    yield return new WaitForSeconds(_settings.DelayBetweenDealingCards);
                }
            }
        }
        onComplete?.Invoke();
    }
    /*
    Return cards to deck
    keep track of cards that finished tweening to hand (used to call onComplete)
    */
    private IEnumerator ReturnCardsToDeck(bool addCardToDeck, Action onComplete)
    {
        int cardsDealt = 0;
        int cardsInHand = _hand.Cards.Count;
        // for each card- animate it to position, increment cardsDealt to call onComplete once everything is finished
        for (int i = cardsInHand - 1; i >= 0; i--)
        {
            CardDisplay card = _hand.GetCardAt(i);
            card.SetSortingOrder(i);
            _deck.AnimateCardToPosition(_settings.CardReturnToDeckAnimTime, card, i, _deck.CardSpawnRoot.position, _deck.CardSpawnRoot.rotation, cardDisplay =>
            {
                cardsDealt++;
                _hand.RemoveCard(card);
                if (addCardToDeck)
                {
                    _deck.AddCard(card.Card);
                }
                Destroy(card.gameObject);
            });
            card.Turn(false, _settings.CardTurnAnimTime, 0);
            yield return new WaitForSeconds(_settings.DelayBetweenReturningCardsToDeck);
        }
        yield return new WaitUntil(() => cardsDealt == cardsInHand);
        onComplete?.Invoke();
    }

    private void OnCardAnimatedToHand(CardDisplay card)
    {
        if (card != null)
        {
            _hand.AddCard(card);
            card.TurnFaceUp();
        }
        else
        {
            Debug.LogWarning("Dealt NULL from deck, not adding NULL to hand.");
        }
    }

    private void ResetGameState()
    {
        _state = EGameState.Idle;
    }
    /*
    Deal predefined cards to Hand
    save flag _dealtPredefinedDeck to know whether cards from player hand should be added back to deck upon redealing
    keep track of cards that finished tweening to hand (used to call onComplete)
    */
    private IEnumerator DealPredefinedCardsToHand(Action onComplete)
    {
        _dealtPredefinedDeck = true;
        int cardsDealt = 0;
        for (int i = 0; i < 11; i++)
        {
            CardDisplay card;

            card = _deck.DrawPredefinedCard();

            card.SetSortingOrder(_settings.CardsDealNum - i);

            var cardInHandPosition = _hand.GetCardPositionAt(i);
            var cardRotation = _hand.GetCardEulerRotationAt(i);

            _deck.AnimateCardToPosition(_settings.CardDealingAnimTime, card, i, _hand.GetCardPositionAt(i), Quaternion.Euler(cardRotation), (CardDisplay c) =>
            {
                OnCardAnimatedToHand(c);
                cardsDealt++;
            });
            card.Turn(true, _settings.CardTurnAnimTime, 0.3f);
            yield return new WaitForSeconds(_settings.DelayBetweenDealingCards);
        }
        yield return new WaitUntil(() => cardsDealt == 11);
        onComplete?.Invoke();
    }

    private IEnumerator DealCardsToHand(Action onComplete)
    {
        _dealtPredefinedDeck = false;
        // keep track of number of cards that finished tweening to player hand
        int cardsDealt = 0;
        for (int i = 0; i < _settings.CardsDealNum; i++)
        {
            CardDisplay card;

            card = _deck.DrawCard();

            card.SetSortingOrder(_settings.CardsDealNum - i);

            var cardInHandPosition = _hand.GetCardPositionAt(i);
            var cardRotation = _hand.GetCardEulerRotationAt(i);

            _deck.AnimateCardToPosition(_settings.CardDealingAnimTime, card, i, _hand.GetCardPositionAt(i), Quaternion.Euler(cardRotation), (CardDisplay c) =>
            {
                OnCardAnimatedToHand(c);
                cardsDealt++;
            });
            card.Turn(true, _settings.CardTurnAnimTime, 0.3f);
            yield return new WaitForSeconds(_settings.DelayBetweenDealingCards);
        }
        yield return new WaitUntil(() => cardsDealt == _settings.CardsDealNum);
        onComplete?.Invoke();
    }
}