using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class CardDisplay : MonoBehaviour
{
    private SpriteRenderer _spriteRenderer;
    private Card _card;
    public Card Card => _card;

    void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void SetCard(Card card)
    {
        _card = card;
        _spriteRenderer.sprite = _card.BackSideSprite;
    }

    public void TurnFaceUp()
    {
        if (_card == null)
        {
            Debug.LogError("Card is null");
        }
        else
        {
            _spriteRenderer.sprite = _card.FrontSideSprite;
        }
    }
}
