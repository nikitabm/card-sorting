using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Draggable))]
public class CardDisplay : MonoBehaviour
{
    private SpriteRenderer _spriteRenderer;
    private Draggable _draggable;

    private Card _card;

    public Card Card => _card;
    public delegate void CardDelegate(CardDisplay cardDisplay);

    public event CardDelegate DragStarted;
    public event CardDelegate DragEnded;

    public Draggable Draggable => _draggable;

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _draggable = GetComponent<Draggable>();
        _draggable.enabled = false;
        _draggable.DragStarted += OnDragStarted;
        _draggable.DragEnded += OnDragEnded;
    }

    private void OnDestroy()
    {
        _draggable.DragStarted -= OnDragStarted;
        _draggable.DragEnded -= OnDragEnded;
    }

    private void OnDragStarted()
    {
        DragStarted?.Invoke(this);
    }

    private void OnDragEnded()
    {
        DragEnded?.Invoke(this);
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
    // tween X scale to 0, set sprite to card front sprite or back sprite and tween scale back to initial scale
    public void Turn(bool faceUp, float animationDuration, float animationDelay)
    {
        if (_card == null)
        {
            Debug.LogError("Card is null");
        }
        else
        {
            float initialXScale = transform.localScale.x;
            new Tween()
                .SetEase(Tween.Ease.InQuad)
                .SetTime(animationDuration / 2)
                .SetDelay(animationDelay)
                .SetIgnoreGameSpeed(false)
                .SetStart(initialXScale)
                .SetEnd(0f)
                .SetOnUpdate(delegate (float v, float t)
                {
                    transform.localScale = new Vector3(v, transform.localScale.y, transform.localScale.z);
                })
                .SetOnComplete(delegate ()
                {
                    _spriteRenderer.sprite = faceUp ? _card.FrontSideSprite : _card.BackSideSprite;
                    new Tween()
                        .SetEase(Tween.Ease.OutQuad)
                        .SetTime(animationDuration / 2)
                        .SetIgnoreGameSpeed(false)
                        .SetStart(0f)
                        .SetEnd(initialXScale)
                        .SetOnUpdate(delegate (float v, float t)
                        {
                            transform.localScale = new Vector3(v, transform.localScale.y, transform.localScale.z);
                        });
                });
        }
    }

    public void SetSortingOrder(int newSortingOrder)
    {
        _spriteRenderer.sortingOrder = newSortingOrder;
    }
}
