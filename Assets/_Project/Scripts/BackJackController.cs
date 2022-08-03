using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackJackController : MonoBehaviour
{
    [SerializeField] CanvasManager canvasManager;

    [HideInInspector] public int wins;
    [HideInInspector] public int loses;
    [HideInInspector] public int ties;
    [HideInInspector] public int blackjacks;
    [HideInInspector] public int dealersBlackjacks;

    private List<Card> deck = new List<Card>();
    private List<Card> playerHand = new List<Card>();
    private List<Card> dealerHand = new List<Card>();
    private GameState gameState;

    #region Methods: public

    public void StartNewGame() {
        gameState = GameState.NoGame;
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
                gameState = GameState.Tie;
                ties++;
                blackjacks++;
                dealersBlackjacks++;
            } else {
                canvasManager.Print("You've got a blackjack!");
                canvasManager.ShowHand(dealerHand, false);
                canvasManager.Print("You've won!");
                canvasManager.Print("Type Deal for new game.");
                gameState = GameState.Won;
                wins++;
                blackjacks++;
            }
        } else {
            if (dealerInitResult == SumResult.Blackjack) {
                canvasManager.Print("Dealer has got a blackjack!");
                canvasManager.ShowHand(dealerHand, false);
                canvasManager.Print("You've lost!");
                canvasManager.Print("Type Deal for new game.");
                gameState = GameState.Lost;
                loses++;
                dealersBlackjacks++;
            } else {
                gameState = GameState.PlayerTurn;
            }
        }
    }

    public void LoadGame() {
        SaveData data = SaveSystem.LoadData();
        playerHand = data.playerHand;
        dealerHand = data.dealerHand;
        deck = data.deck;
        gameState = data.gameState;

        if (gameState == GameState.PlayerTurn) {
            canvasManager.ShowHand(playerHand);
            canvasManager.ShowHand(dealerHand, false, true);
            canvasManager.Print("Your turn.", true);
        } else if (gameState == GameState.EnemyTurn) {
            canvasManager.ShowHand(playerHand);
            canvasManager.ShowHand(dealerHand, false);
            DealerTurn();
        } else {
            canvasManager.Print("Type Deal for new game.", true);
        }
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
        if (Equals(command.ToLower(), "stats")) {
            Stats();
            return true;
        }
        if (Equals(command.ToLower(), "help")) {
            canvasManager.Print("Commands:", true);
            canvasManager.Print("Deal - new game.");
            canvasManager.Print("Hit - take a card.");
            canvasManager.Print("Stand - hold the cards.");
            canvasManager.Print("Help - see help.");
            canvasManager.Print("Stats - see stats.");
        }
        return false;
    }

    public void Hit() {
        if (gameState != GameState.PlayerTurn) {
            return;
        }
        gameState = GameState.PassingTurn;
        GiveCardToPlayer(playerHand);
        var result = CountHand(playerHand, out _);
        canvasManager.ShowHand(playerHand);
        if (result == SumResult.Blackjack) {
            gameState = GameState.EnemyTurn;
            canvasManager.Print("You've got a blackjack!");
            blackjacks++;
            DealerTurn();
        } else if (result == SumResult.Bust) {
            gameState = GameState.Lost;
            canvasManager.Print("You're busted! You've lost.");
            canvasManager.Print("Type Deal for new game.");
            loses++;
        } else {
            gameState = GameState.PlayerTurn;
        }
    }

    void DealerTurn() {
        canvasManager.ShowHand(dealerHand, false);
        canvasManager.Print("Dealer takes his turn.", true);
        for (int i = 0; i < 100; i++) {
            var dealerResult = CountHand(dealerHand, out int dealerSum);
            var playerResult = CountHand(playerHand, out int playerSum);
            if (dealerSum < 17 || dealerSum < playerSum) {
                GiveCardToPlayer(dealerHand);
                canvasManager.Print("Dealer takes a card.");
                canvasManager.ShowHand(dealerHand, false);
                dealerResult = CountHand(dealerHand, out dealerSum);
                if (dealerResult == SumResult.Blackjack) {
                    if (playerResult != SumResult.Blackjack) {
                        gameState = GameState.Lost;
                        canvasManager.Print("Dealer has got a blackjack! You've lost!");
                        canvasManager.Print("Type Deal for new game.");
                        dealersBlackjacks++;
                        loses++;
                        break;
                    } else {
                        gameState = GameState.Tie;
                        canvasManager.Print("Dealer has got a blackjack! Tie!");
                        canvasManager.Print("Type Deal for new game.");
                        dealersBlackjacks++;
                        ties++;
                        break;
                    }
                } else if (dealerResult == SumResult.Bust) {
                    if (playerResult != SumResult.Bust) {
                        gameState = GameState.Won;
                        canvasManager.Print("Dealer is busted! You win!");
                        canvasManager.Print("Type Deal for new game.");
                        wins++;
                        break;
                    } else {
                        gameState = GameState.Tie;
                        canvasManager.Print("Dealer is busted! Tie!");
                        canvasManager.Print("Type Deal for new game.");
                        ties++;
                        break;
                    }
                }
            } else {
                if (dealerSum > playerSum) {
                    gameState = GameState.Lost;
                    canvasManager.Print($"Dealers has {dealerSum}. You have {playerSum}. You've lost!");
                    canvasManager.Print("Type Deal for new game.");
                    loses++;
                    break;
                } else if (dealerSum == playerSum) {
                    gameState = GameState.Tie;
                    canvasManager.Print($"Dealer has {dealerSum}! You have {playerSum}. Tie!");
                    canvasManager.Print("Type Deal for new game.");
                    ties++;
                    break;
                } else {
                    gameState = GameState.Won;
                    canvasManager.Print($"Dealer has {dealerSum}! You have {playerSum}. You've won!");
                    canvasManager.Print("Type Deal for new game.");
                    wins++;
                    break;
                }
            }
        }
    }

    public void Stand() {
        if (gameState != GameState.PlayerTurn) {
            return;
        }
        gameState = GameState.EnemyTurn;
        DealerTurn();
    }

    public void Stats() {
        canvasManager.Print($"Wins: {wins}, loses: {loses}, ties: {ties}.", true);
        canvasManager.Print($"Blackjacks: {blackjacks}, dealer's bjs: {dealersBlackjacks}.");
        if (gameState == GameState.PlayerTurn) {
            canvasManager.ShowHand(playerHand);
            canvasManager.ShowHand(dealerHand, false, true);
        }
    }

    public void SaveData() {
        SaveSystem.SaveGame(playerHand, dealerHand, deck, gameState);
    }

    #endregion

    #region Methods: private

    private void Start() {
        List<int> results = FileSaveHelper.ReadStatsFromTextFile();
        if (results.Count == 5) {
            wins = results[0];
            loses = results[1];
            ties = results[2];
            blackjacks = results[3];
            dealersBlackjacks = results[4];
        }
    }

    void GenerateNewDeck() {
        deck.Clear();
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

    #endregion
}

public enum GameState { NoGame, PlayerTurn, PassingTurn, EnemyTurn, Won, Lost, Tie }
public enum SumResult { Default, Bust, Blackjack }