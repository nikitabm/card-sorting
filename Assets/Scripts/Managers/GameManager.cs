using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField] private Deck _deck;
    [SerializeField] private Hand _hand;
    [SerializeField] private UIManager _uiManager;

    public delegate void EmptyDelegate();
    public event EmptyDelegate GameStarted;


    private void Start()
    {
        SubscribeToEvents();

        GameStarted.Invoke();
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
        _deck.CreateCards();
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
        for (int i = 0; i < 11; i++)
        {
            CardDisplay card = _deck.DrawCard();
            _deck.AnimateCardToPosition(card, _hand.transform.position + new Vector3(-1.5f + i * 0.3f, 0, 0), card.TurnFaceUp);
            yield return new WaitForSeconds(0.2f);
        }
    }
}
