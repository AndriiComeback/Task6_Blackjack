using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SaveData
{
    public List<Card> playerHand;
    public List<Card> dealerHand;
    public List<Card> deck;
    public GameState gameState;

    public SaveData(List<Card> playerHand, List<Card> dealerHand, List<Card> deck, GameState gameState) {
        this.playerHand = playerHand;
        this.dealerHand = dealerHand;
        this.deck = deck;
        this.gameState = gameState;
    }
}
