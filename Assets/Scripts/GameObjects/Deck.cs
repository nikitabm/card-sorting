using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Deck : MonoBehaviour
{
    [SerializeField] private CardDisplay _cardPrefab;

    [SerializeField] private Transform _visualsRoot;

    [SerializeField] private Transform _cardSpawnRoot;

    [Space(10)]
    [SerializeField] private List<CardData> _predefinedCards;

    private Stack<Card> _cards = new Stack<Card>();

    private CardSettingsSO _cardSettings;

    public Transform CardSpawnRoot => _cardSpawnRoot;
    private int _currentPredefinedCardId = 0;

    public void SetSuitSettings(CardSettingsSO settings)
    {
        _cardSettings = settings;
    }
    /*
        Create ordered array of cards, assign them with sprites ranks and suits
        Shuffle array and then populate _cards stack
    */
    public void CreateCards()
    {
        Card[] tempCardArr = new Card[52];
        Card tempCard;
        int cardInDeckIndex = 0;
        for (int i = 0; i < 4; i++)
        {
            for (int j = 0; j < 13; j++)
            {
                cardInDeckIndex = i * 13 + j;
                tempCard = new Card(_cardSettings.SuitOrder[i], j + 1, _cardSettings.CardSprites[cardInDeckIndex], _cardSettings.CardBackSprite);
                tempCardArr[cardInDeckIndex] = tempCard;
            }
        }
        ShuffleCardArr(tempCardArr);

        _cards.Clear();
        foreach (Card card in tempCardArr)
        {
            _cards.Push(card);
        }
    }

    public void Shuffle()
    {
        _currentPredefinedCardId = 0;
        Card[] tempCardArr = _cards.ToArray();
        ShuffleCardArr(tempCardArr);

        _cards.Clear();
        foreach (Card card in tempCardArr)
        {
            _cards.Push(card);
        }
    }

    public void ResetDealingPredefinedCards()
    {
        _currentPredefinedCardId = 0;
    }

    // Tween in the deck of cards
    public void Show(Action onComplete)
    {
        if (!_visualsRoot)
        {
            Debug.LogError("Deck's visualsRoot is null.");
            return;
        }
        _visualsRoot.localScale = Vector3.zero;

        new Tween()
            .SetEase(Tween.Ease.Elastic)
            .SetTime(0.4f)
            .SetDelay(0.25f)
            .SetIgnoreGameSpeed(false)
            .SetStart(0f)
            .SetEnd(1)
            .SetDamping(3f)
            .SetOscillations(1f)
            .SetOnUpdate(delegate (float v, float t)
            {
                _visualsRoot.localScale = Vector3.one * v;

            })
            .SetOnComplete(delegate ()
            {
                onComplete?.Invoke();
            });
    }

    public CardDisplay DrawPredefinedCard()
    {
        if (_predefinedCards.Count > _currentPredefinedCardId && _predefinedCards.Count > 0 && _cardSpawnRoot != null)
        {
            var cardsArr = _cards.ToArray();
            CardData cardData = _predefinedCards[_currentPredefinedCardId];
            CardDisplay drawnCard = Instantiate(_cardPrefab, _cardSpawnRoot.transform.position, Quaternion.identity);
            // get correct sprite Id (using rank-1 because card ranks are defined from 1 in inspector, not from 0)
            int spriteId = _cardSettings.GetOrderOfSuit(cardData.suit) * 13 + (cardData.rank - 1);
            _currentPredefinedCardId++;

            drawnCard.SetCard(new Card(cardData.suit, cardData.rank, _cardSettings.CardSprites[spriteId], _cardSettings.CardBackSprite));
            drawnCard.name = $"{drawnCard.Card.Suit}_{drawnCard.Card.Rank}";
            return drawnCard;
        }
        else
        {
            return null;
        }
    }

    // pop card from _cards stack, create CardDisplay, assign card data to it, return it
    public CardDisplay DrawCard()
    {
        if (_cards.Count > 0 && _cardSpawnRoot != null)
        {
            CardDisplay drawnCard = Instantiate(_cardPrefab, _cardSpawnRoot.transform.position, Quaternion.identity);
            drawnCard.SetCard(_cards.Pop());
            drawnCard.name = $"{drawnCard.Card.Suit}_{drawnCard.Card.Rank}";
            return drawnCard;
        }
        else
        {
            return null;
        }
    }

    public void AddCard(Card card)
    {
        _cards.Push(card);
    }

    public void AnimateCardToPosition(float animationDuration, CardDisplay card, int i, Vector3 targetPosition, Quaternion targetRotation, Action<CardDisplay> onComplete)
    {
        Vector3 cardStartPosition = card.transform.position;
        Quaternion cardStartRotation = card.transform.rotation;
        new Tween()
           .SetEase(Tween.Ease.InQuad)
           .SetTime(animationDuration)
           .SetIgnoreGameSpeed(false)
           .SetStart(0f)
           .SetEnd(1)
           .SetOnUpdate(delegate (float v, float t)
           {
               card.transform.position = Vector3.Lerp(cardStartPosition, targetPosition, v);
               card.transform.rotation = Quaternion.Lerp(cardStartRotation, targetRotation, v);
           })
           .SetOnComplete(delegate ()
           {
               onComplete?.Invoke(card);
           });
    }
    /*
        Convert stack of cards to array of cards
        Shuffle them using Fisher-Yates shuffle
        Add them back to stack
    */
    private void ShuffleCardArr(Card[] cardArr)
    {
        int arrLength = cardArr.Length;
        System.Random rnd = new System.Random();
        Card tempCard;
        for (int i = 0; i < arrLength - 2; i++)
        {
            int k = rnd.Next(i, arrLength);
            tempCard = cardArr[k];
            cardArr[k] = cardArr[i];
            cardArr[i] = tempCard;
        }
    }
}
