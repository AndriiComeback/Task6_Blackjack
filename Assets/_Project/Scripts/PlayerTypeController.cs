using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTypeController : MonoBehaviour
{
    [SerializeField] FirstPersonController controller;
    [HideInInspector] public bool isInTypingZone = false;
    [HideInInspector] public bool isTyping = false;
    [SerializeField] CanvasManager canvasManager;
    [SerializeField] GameObject pointer;
    public void SetIsInTypingZone(bool value) {
        isInTypingZone = value;
    }
    public void Update() {
        if (!isTyping && isInTypingZone && Input.GetKeyDown(KeyCode.E)) {
            isTyping = true;
            controller.MoveSpeed = 0f;
            controller.SprintSpeed = 0f;
            controller.JumpHeight = 0f;
            pointer.SetActive(false);
        } else if (isTyping && Input.GetKeyDown(KeyCode.Tab)) {
            isTyping = false;
            controller.MoveSpeed = 4f;
            controller.SprintSpeed = 6f;
            controller.JumpHeight = 1.2f;
            pointer.SetActive(true);
        } else if (isTyping) {
            HandleInput();
        }
    }
    void HandleInput() {
        foreach (char c in Input.inputString) {
            canvasManager.PrintInput(c);
        }
    }
}
