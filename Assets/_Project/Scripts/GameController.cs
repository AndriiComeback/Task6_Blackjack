using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    [SerializeField] CanvasManager canvasManager;
    [SerializeField] BackJackController backJackController;
    void Start()
    {
        SaveData data = SaveSystem.LoadData();
        if (data != null) {
            canvasManager.Print("Game was loaded successfully.");
            canvasManager.Print("Type Help to see available commands.");
            backJackController.LoadGame();
        } else {
            canvasManager.Print("Welcome to Super Blackjack 3000!");
            canvasManager.Print("Type Deal to start!");
            canvasManager.Print("Note: you can type Deal at any moment for the new game.");
            canvasManager.Print("Type Help to see available commands.");
        }
        canvasManager.Print("Note: input works only in english");
    }

    private void Update() {
        if (Input.GetKey("escape")) {
            FileSaveHelper.SaveStatsToTextFile(
                backJackController.wins,
                backJackController.loses,
                backJackController.ties,
                backJackController.blackjacks,
                backJackController.dealersBlackjacks
            );

            backJackController.SaveData();

            UnityEngine.Application.Quit();
        }
    }
}
