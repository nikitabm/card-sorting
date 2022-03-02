using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hand : MonoBehaviour
{
    [SerializeField] private float cardArchMaxDelta = 30;
    private List<CardDisplay> _cards = new List<CardDisplay>();

    public List<CardDisplay> Cards => _cards;
    private int _maxCardsNum = 11;
    void Start()
    {

    }

    void Update()
    {

    }
    public void SetCardListSize(int size)
    {
        _maxCardsNum = size;
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

    public Vector3 GetCardRotationAt(int index)
    {
        return new Vector3(0, 0, ((float)index - _maxCardsNum / 2) * -10 / _maxCardsNum);

    }

    public Vector3 GetCardPositionAt(int index)
    {
        Vector3 cardRotation = GetCardRotationAt(index);
        return transform.position + Vector3.forward * Mathf.Sin(cardRotation.z) * 2 + Vector3.right * Mathf.Cos(cardRotation.z) * 2;
    }

    public void AddCardAt(CardDisplay card)
    {
        if (!_cards.Contains(card))
        {
            _cards.Add(card);
            card.SetSortingOrder(_cards.Count);
        }
    }
}
