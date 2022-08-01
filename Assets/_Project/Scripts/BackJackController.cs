using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackJackController : MonoBehaviour
{
    [SerializeField] CanvasManager canvasManager;
    List<Card> deck = new List<Card>();
    List<Card> playerHand = new List<Card>();
    List<Card> dealerHand = new List<Card>();
    GameStates gameState;
    public void StartNewGame() {
        gameState = GameStates.NoGame;
        playerHand.Clear();
        dealerHand.Clear();
        GenerateNewDeck();
        GiveCardToPlayer(playerHand, 2);
        GiveCardToPlayer(dealerHand, 2);
        canvasManager.ShowHand(playerHand, true);
        canvasManager.ShowHand(dealerHand, false, true);
        var playerInitResult = CountHand(playerHand, out _);
        var dealerInitResult = CountHand(dealerHand, out _);
        if (playerInitResult == SumResult.Blackjack) {
            if (dealerInitResult == SumResult.Blackjack) {
                canvasManager.Print("Both sides have got blackjacks!");
                canvasManager.ShowHand(playerHand);
                canvasManager.ShowHand(dealerHand, false);
                canvasManager.Print("Tie!");
                canvasManager.Print("Type Deal for new game.");
                gameState = GameStates.Tie;
            } else {
                canvasManager.Print("You've got a blackjack!");
                canvasManager.ShowHand(dealerHand, false);
                canvasManager.Print("You've won!");
                canvasManager.Print("Type Deal for new game.");
                gameState = GameStates.Won;
            }
        } else {
            if (dealerInitResult == SumResult.Blackjack) {
                canvasManager.Print("Dealer has got a blackjack!");
                canvasManager.ShowHand(dealerHand, false);
                canvasManager.Print("You've lost!");
                canvasManager.Print("Type Deal for new game.");
                gameState = GameStates.Lost;
            } else {
                gameState = GameStates.PlayerTurn;
            }
        }
    }
    public void Hit() {
        if (gameState != GameStates.PlayerTurn) {
            return;
        }
        gameState = GameStates.PassingTurn;
        GiveCardToPlayer(playerHand);
        var result = CountHand(playerHand, out _);
        canvasManager.ShowHand(playerHand);
        if (result == SumResult.Blackjack) {
            gameState = GameStates.EnemyTurn;
            canvasManager.Print("You've got a blackjack!");
            DealerTurn();
        } else if (result == SumResult.Bust) {
            gameState = GameStates.Lost;
            canvasManager.Print("You're busted! You've lost.");
            canvasManager.Print("Type Deal for new game.");
        } else {
            gameState = GameStates.PlayerTurn;
        }
    }
    void DealerTurn() {
        canvasManager.ShowHand(dealerHand, false);
        canvasManager.Print("Dealer takes his turn.", true);
        for (int i = 0; i < 100; i++) {
            var result = CountHand(dealerHand, out int sum);
            if (sum < 17) {
                GiveCardToPlayer(dealerHand);
                canvasManager.Print("Dealer takes a card.");
                canvasManager.ShowHand(dealerHand, false);
                result = CountHand(dealerHand, out sum);
                if (result == SumResult.Blackjack) {
                    if (CountHand(playerHand, out _) != SumResult.Blackjack) {
                        gameState = GameStates.Lost;
                        canvasManager.Print("Dealer has got a blackjack! You've lost!");
                        canvasManager.Print("Type Deal for new game.");
                        break;
                    } else {
                        gameState = GameStates.Tie;
                        canvasManager.Print("Dealer has got a blackjack! Tie!");
                        canvasManager.Print("Type Deal for new game.");
                        break;
                    }
                } else if (result == SumResult.Bust) {
                    if (CountHand(playerHand, out _) != SumResult.Bust) {
                        gameState = GameStates.Won;
                        canvasManager.Print("Dealer is busted! You win!");
                        canvasManager.Print("Type Deal for new game.");
                        break;
                    } else {
                        gameState = GameStates.Tie;
                        canvasManager.Print("Dealer is busted! Tie!");
                        canvasManager.Print("Type Deal for new game.");
                        break;
                    }
                }
            } else {
                CountHand(playerHand, out int playerSum);
                if (sum > playerSum) {
                    gameState = GameStates.Lost;
                    canvasManager.Print($"Dealers has {sum}. You have {playerSum}. You've lost!");
                    canvasManager.Print("Type Deal for new game.");
                    break;
                } else if (sum == playerSum) {
                    gameState = GameStates.Tie;
                    canvasManager.Print($"Dealer has {sum}! You have {playerSum}. Tie!");
                    canvasManager.Print("Type Deal for new game.");
                    break;
                } else {
                    gameState = GameStates.Won;
                    canvasManager.Print($"Dealer has {sum}! You have {playerSum}. You've won!");
                    canvasManager.Print("Type Deal for new game.");
                    break;
                }
            }
        }
    }
    public void Stand() {
        if (gameState != GameStates.PlayerTurn) {
            return;
        }
        gameState = GameStates.EnemyTurn;
        DealerTurn();
    }
    public bool RecognizeCommand(string command) {
        if (Equals(command.ToLower(), "deal")) {
            StartNewGame();
            return true;
        }
        if (Equals(command.ToLower(), "hit")) {
            Hit();
            return true;
        }
        if (Equals(command.ToLower(), "stand")) {
            Stand();
            return true;
        }
        if (Equals(command.ToLower(), "help")) {
            canvasManager.Print("Commands:", true);
            canvasManager.Print("Deal - new game.");
            canvasManager.Print("Hit - take a card.");
            canvasManager.Print("Stand - hold the cards.");
            canvasManager.Print("Help - see help.");
        }
        return false;
    }
    void GenerateNewDeck() {
        for (int i = 0; i < 13; i++) {
            deck.Add(new Card { rank = (Rank)i, suit = Suit.Diamonds });
            deck.Add(new Card { rank = (Rank)i, suit = Suit.Hearts });
            deck.Add(new Card { rank = (Rank)i, suit = Suit.Clubs });
            deck.Add(new Card { rank = (Rank)i, suit = Suit.Spades });
        }
        deck.Shuffle();
    }
    void GiveCardToPlayer(List<Card> hand, int amount = 1) {
        for (int i = 0; i < amount; i++) {
            hand.Add(deck[0]);
            deck.RemoveAt(0);
        }
    }
    SumResult CountHand(List<Card> hand, out int sum) {
        int aceCount = 0;
        sum = 0;

        if (hand.Count == 2) {
            sum = RankToValue(hand[0].rank, true) + RankToValue(hand[1].rank, true);
            if (sum == 21 || sum == 22) {
                return SumResult.Blackjack;
            }
        }

        sum = 0;
        foreach (Card card in hand) {
            if (card.rank != Rank.Ace) {
                sum += RankToValue(card.rank);
            } else {
                aceCount++;
            }
        }
        if (sum > 10) {
            sum += aceCount;
        } else if (sum <= 10 && aceCount > 0) {
            if (sum + 11 + aceCount - 1 <= 21) {
                sum += 11 + aceCount - 1;
            } else {
                sum += aceCount;
            }
        }
        if (sum < 21) {
            return SumResult.Default;
        } else if (sum == 21) {
            return SumResult.Blackjack;
        } else {
            return SumResult.Bust;
        }
    }
    int RankToValue(Rank rank, bool doestTakeMaxValue = false) {
        switch (rank) {
            case Rank.Ace:
                if (!doestTakeMaxValue) {
                    return 1;
                }
                return 11;
            case Rank.Two:
                return 2;
            case Rank.Three:
                return 3;
            case Rank.Four:
                return 4;
            case Rank.Five:
                return 5;
            case Rank.Six:
                return 6;
            case Rank.Seven:
                return 7;
            case Rank.Eight:
                return 8;
            case Rank.Nine:
                return 9;
            case Rank.Ten:
            case Rank.Jack:
            case Rank.Quenn:
            case Rank.King:
                return 10;
            default:
                return 0;
        }
    }
}

public enum GameStates { NoGame, PlayerTurn, PassingTurn, EnemyTurn, Won, Lost, Tie }
public enum SumResult { Default, Bust, Blackjack }

public class Card {
    public Rank rank;
    public Suit suit;
}

public enum Suit { Diamonds, Hearts, Clubs, Spades }
public enum Rank { Ace, Two, Three, Four, Five, Six, Seven, Eight, Nine, Ten, Jack, Quenn, King }

public static class ListHelper {
    public static void Shuffle<T>(this IList<T> list) {
        int n = list.Count;
        while (n > 1) {
            n--;
            int k = Random.Range(0, n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }
}