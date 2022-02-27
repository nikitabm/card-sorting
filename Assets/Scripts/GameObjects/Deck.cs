using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Deck : MonoBehaviour
{

    [SerializeField] private CardDisplay _cardPrefab;

    [SerializeField] private Sprite _cardBackSprite;

    [SerializeField] private Sprite[] _cardSprites;

    [SerializeField] private Transform _visualsRoot;

    private Stack<Card> _cards = new Stack<Card>();

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
                tempCard = new Card((ESuit)i, j, _cardSprites[cardInDeckIndex], _cardBackSprite);
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
            .SetDestroyOnLoad(true)
            .SetOnUpdate(delegate (float v, float t)
            {
                _visualsRoot.localScale = Vector3.one * v;

            })
            .SetOnComplete(delegate ()
            {
                onComplete?.Invoke();
            });
    }

    // pop card from _cards stack, create CardDisplay, assign card data to it, return it
    public CardDisplay DrawCard()
    {
        if (_cards.Count > 0)
        {
            CardDisplay drawnCard = Instantiate(_cardPrefab, transform.position, Quaternion.Euler(0, 0, 180));
            drawnCard.SetCard(_cards.Pop());
            return drawnCard;
        }
        else
        {
            return null;
        }
    }

    public void AnimateCardToPosition(CardDisplay card, Vector3 targetPosition, Action onComplete)
    {
        Vector3 cardStartPosition = card.transform.position;
        Quaternion cardStartRotation = card.transform.rotation;
        float animationDuration = 0.7f;
        new Tween()
           .SetEase(Tween.Ease.InExpo)
           .SetTime(animationDuration)
           .SetIgnoreGameSpeed(false)
           .SetStart(0f)
           .SetEnd(1)
           .SetDestroyOnLoad(false)
           .SetOnUpdate(delegate (float v, float t)
           {
               card.transform.position = Vector3.Lerp(cardStartPosition, targetPosition, v);
           })
           .SetOnComplete(delegate ()
           {
               onComplete?.Invoke();
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
