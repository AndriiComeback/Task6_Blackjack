using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Text.RegularExpressions;
using System.Linq;

public class CanvasManager : MonoBehaviour
{
    [SerializeField] TMP_Text screenText;
    [SerializeField] BackJackController backJackController;
    [SerializeField] int maxScreenLines = 16;
    List<string> queue = new List<string>();
    bool isTyping = false;
    public void Print(string text, bool isNewLineNeeded = false) {
        if (!isTyping) {
            StartCoroutine(PrintByLetter($"{text}", isNewLineNeeded));
        } else {
            queue.Add(text);
        }
    }
    IEnumerator PrintByLetter(string text, bool isNewLineNeeded = false) {
        var linesCount = screenText.text.Split('\n').Length;
        if (linesCount > maxScreenLines) {
            RemoveFirstLines(linesCount - maxScreenLines);
        }

        isTyping = true;
        if (isNewLineNeeded) {
            screenText.text += "\n\n";
        }
        screenText.text += "> ";
        foreach (char c in text){
            screenText.text += c;
            yield return new WaitForSeconds(0.03f);
        }
        screenText.text += "\n\n";
        isTyping = false;
        if (queue != null && queue.Count > 0) {
            var txt = queue[0];
            queue.RemoveAt(0);
            StartCoroutine(PrintByLetter(txt));
        }
    }

    public void PrintInput(char c) {
        if (isTyping) {
            return;
        }
        var lastLine = screenText.text.Split('\n').Last();
        if (!lastLine.Contains("> ")) {
            screenText.text += "> ";
            lastLine = screenText.text.Split('\n').Last();
        }
        if (c == '\b') {
            if (lastLine.Length > 2) {
                screenText.text = screenText.text.Substring(0, screenText.text.Length - 1);
            }
        } else if ((c == '\n') || (c == '\r')) {
            if (InputCommand()) {
                RemoveFirstLines(2);
            }
        } else {
            bool isAlphaBet = Regex.IsMatch(c.ToString(), "[a-z]", RegexOptions.IgnoreCase);
            if (isAlphaBet) {
                screenText.text += c;
            }
        }
    }
    void RemoveFirstLines(int n) {
        var lines = screenText.text.Split('\n');
        if (lines.Length <= n) {
            return;
        }
        var newLines = new List<string>();
        for (int i = n; i < lines.Length; i++) {
            newLines.Add(lines[i]);
        }
        screenText.text = string.Join("\n", newLines);
    }
    bool InputCommand() {
        var lastLine = screenText.text.Split('\n').Last();
        lastLine = lastLine.Replace("> ", "");

        return backJackController.RecognizeCommand(lastLine);
    }
    public void ShowHand(List<Card> cards, bool isPlayerHand = true, bool isDealerHandClosed = false) {
        var linesCount = screenText.text.Split('\n').Length;
        if (linesCount > maxScreenLines) {
            RemoveFirstLines(linesCount - maxScreenLines);
        }

        string result = "";
        if (isPlayerHand) {
            result += "Your hand: ";
        } else {
            result += "Dealer hand: ";
        }
        for (int i = 0; i < cards.Count; i++) {
            if (i == 0 && isDealerHandClosed) {
                result += $"?? ";
            } else {
                result += $"{GetRankShortName(cards[i].rank)}{GetSuitImage(cards[i].suit)} ";
            }
        }
        Print(result, true);
    }
    string GetRankShortName(Rank rank) {
        switch (rank) {
            case Rank.Ace:
                return "A";
            case Rank.Two:
                return "2";
            case Rank.Three:
                return "3";
            case Rank.Four:
                return "4";
            case Rank.Five:
                return "5";
            case Rank.Six:
                return "6";
            case Rank.Seven:
                return "7";
            case Rank.Eight:
                return "8";
            case Rank.Nine:
                return "9";
            case Rank.Ten:
                return "10";
            case Rank.Jack:
                return "J";
            case Rank.Quenn:
                return "Q";
            case Rank.King:
                return "K";
            default:
                return "0";
        }
    }
    char GetSuitImage(Suit suit) {
        switch (suit) {
            case Suit.Diamonds:
                return '\u2666';
            case Suit.Spades:
                return '\u2660';
            case Suit.Hearts:
                return '\u2665';
            case Suit.Clubs:
                return '\u2663';
            default:
                return 'x';
        }
    }
}
