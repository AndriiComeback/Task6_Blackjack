using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    [SerializeField] CanvasManager canvasManager;
    void Start()
    {
        canvasManager.Print("Welcome to Super Blackjack 3000!");
        canvasManager.Print("Type Deal to start!");
        canvasManager.Print("Note: you can type Deal at any moment for the new game.");
        canvasManager.Print("Type Help to see available commands.");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
