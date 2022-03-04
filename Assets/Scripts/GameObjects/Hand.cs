using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hand : MonoBehaviour
{
    [SerializeField] private float _cardSpacing = 0.3f;
    [SerializeField] private float _maxCardRotationDiff = 30;
    [SerializeField] private float _cardYPositionFromRotationScaleFactor = 0.01f;

    [SerializeField] private List<CardDisplay> _cards = new List<CardDisplay>();

    public List<CardDisplay> Cards => _cards;

    private int _maxCardsNum = 11;

    [SerializeField] private CardDisplay _draggedCard = null;
    private int _draggedCardIndex = 0;
    private void Start()
    {

    }

    private void Update()
    {
        if (_draggedCard)
        {
            _draggedCardIndex = _cards.IndexOf(_draggedCard);
            CardDisplay leftNeighbour = null;
            CardDisplay rightNeightbour = null;
            if (_draggedCardIndex > 0)
            {
                leftNeighbour = _cards[_draggedCardIndex - 1];

                if (_draggedCard.transform.position.x < leftNeighbour.transform.position.x)
                {
                    // move neighbour t
                    _cards.Remove(_draggedCard);
                    _cards.Insert(_draggedCardIndex - 1, _draggedCard);
                    _cards[_draggedCardIndex].SetSortingOrder(_draggedCardIndex + 1);
                    _draggedCard.SetSortingOrder(_draggedCardIndex);
                }
            }
            if (_draggedCardIndex < _maxCardsNum - 1)
            {
                rightNeightbour = _cards[_draggedCardIndex + 1];
                if (_draggedCard.transform.position.x > rightNeightbour.transform.position.x)
                {
                    // move neighbour t
                    _cards.Remove(_draggedCard);
                    _cards.Insert(_draggedCardIndex + 1, _draggedCard);
                    _cards[_draggedCardIndex].SetSortingOrder(_draggedCardIndex);
                    _draggedCard.SetSortingOrder(_draggedCardIndex + 1);
                }
            }

            for (int i = 0; i < _cards.Count; i++)
            {
                if (_cards[i] != _draggedCard)
                {
                    _cards[i].transform.position = Vector3.Lerp(_cards[i].transform.position, GetCardPositionAt(i), 0.3f);
                }
                _cards[i].transform.rotation = Quaternion.Lerp(_cards[i].transform.rotation, Quaternion.Euler(GetCardEulerRotationAt(i)), 0.3f);
            }
        }
    }

    private void OnCardDragStarted(CardDisplay cardDisplay)
    {
        print("drag started");
        _draggedCard = cardDisplay;
    }

    private void OnCardDragEnded(CardDisplay cardDisplay)
    {
        print("drag ended");
        int cardIndex = _cards.IndexOf(cardDisplay);
        _draggedCard = null;

        Vector3 cardStartPos = cardDisplay.transform.position;
        Vector3 cardEndPos = GetCardPositionAt(cardIndex);

        Quaternion startRotation = cardDisplay.transform.rotation;
        Quaternion endRotation = Quaternion.Euler(GetCardEulerRotationAt(cardIndex));

        cardDisplay.transform.position = cardEndPos;
        cardDisplay.Draggable.enabled = false;

        new Tween()
           .SetEase(Tween.Ease.InQuad)
           .SetTime(0.5f)
           .SetIgnoreGameSpeed(false)
           .SetStart(0f)
           .SetEnd(1)
           .SetOnUpdate(delegate (float v, float t)
           {
               cardDisplay.transform.position = Vector3.Lerp(cardStartPos, cardEndPos, v);
               cardDisplay.transform.rotation = Quaternion.Lerp(startRotation, endRotation, v);
           })
           .SetOnComplete(delegate ()
           {
               cardDisplay.Draggable.enabled = true;
           });
    }

    public void SetCardsInteractable(bool newInteractable)
    {
        for (int i = 0; i < _cards.Count; i++)
        {
            CardDisplay card = _cards[i];
            if (card)
            {
                card.Draggable.enabled = newInteractable;
            }
        }
    }
    public void SetCardListSize(int size)
    {
        _maxCardsNum = size;
    }

    public void AddCard(CardDisplay card)
    {
        if (!_cards.Contains(card))
        {
            card.Draggable.enabled = true;
            _cards.Add(card);
            card.transform.parent = transform;
            card.SetSortingOrder(_cards.Count - 1);

            card.DragStarted += OnCardDragStarted;
            card.DragEnded += OnCardDragEnded;
        }
    }

    public void RemoveCardAt(int index)
    {
        try
        {
            _cards.RemoveAt(index);
        }
        catch (System.Exception e)
        {
            Debug.LogWarning(e);
        }
    }

    public void RemoveCard(CardDisplay card)
    {
        _cards.Remove(card);
    }

    public CardDisplay GetCardAt(int index)
    {
        if (_cards.Count > index)
        {

            return _cards[index];
        }
        else
        {
            return null;
        }
    }

    // leftmost card will have rotation _maxCardRotationDiff/2, rightmost will be -_maxCardRotationDiff/2
    // make _maxCardRotationDiff larger- distance rotation diff between cards will get bigger
    public Vector3 GetCardEulerRotationAt(int index)
    {
        return new Vector3(0, 0, ((float)index - _maxCardsNum / 2) * -_maxCardRotationDiff / _maxCardsNum);
    }

    public Vector3 GetCardPositionAt(int index)
    {
        Vector3 cardRotation = GetCardEulerRotationAt(index);
        return transform.position + new Vector3(-_maxCardsNum * _cardSpacing / 2 + index * _cardSpacing, -Mathf.Abs(GetCardEulerRotationAt(index).z) * _cardYPositionFromRotationScaleFactor, 0);
    }
}
