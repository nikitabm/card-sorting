using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Card
{
    [SerializeField] private ESuit _suit;
    [SerializeField] private int _rank;
    [SerializeField] private Sprite _faceSideSprite;
    [SerializeField] private Sprite _backSideSprite;

    public ESuit Suit => _suit;
    public int Rank => _rank;
    public Sprite FrontSideSprite => _faceSideSprite;
    public Sprite BackSideSprite => _backSideSprite;

    public Card(ESuit suit, int rank, Sprite frontSideSprite, Sprite backsideSprite)
    {
        _suit = suit;
        _rank = rank;
        _faceSideSprite = frontSideSprite;
        _backSideSprite = backsideSprite;
    }
}
