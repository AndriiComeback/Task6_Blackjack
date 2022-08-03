using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Card {
    public Rank rank;
    public Suit suit;
}

public enum Suit { Diamonds, Hearts, Clubs, Spades }
public enum Rank { Ace, Two, Three, Four, Five, Six, Seven, Eight, Nine, Ten, Jack, Quenn, King }
