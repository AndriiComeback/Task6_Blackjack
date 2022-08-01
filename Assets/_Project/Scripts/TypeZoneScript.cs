using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TypeZoneScript : MonoBehaviour
{
    [SerializeField] GameObject hint;
    [SerializeField] GameObject exitHint;
    [SerializeField] PlayerTypeController controller;
    bool didShowExitHint = false;
    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Player")) {
            didShowExitHint = false;
            var playerController = other.GetComponent<PlayerTypeController>();
            playerController.SetIsInTypingZone(true);
            hint.SetActive(true);
        }
    }
    private void OnTriggerExit(Collider other) {
        if (other.CompareTag("Player")) {
            other.GetComponent<PlayerTypeController>().SetIsInTypingZone(false);
            hint.SetActive(false);
        }
    }

    private void OnTriggerStay(Collider other) {
        if (other.CompareTag("Player")) {
            if (controller.isTyping && !didShowExitHint) {
                didShowExitHint = true;
                StartCoroutine(ShowHintForSeconds(exitHint, 3));
                hint.SetActive(false);
            }
        }
    }
    IEnumerator ShowHintForSeconds(GameObject hint, int seconds) {
        hint.SetActive(true);
        yield return new WaitForSeconds(seconds);
        hint.SetActive(false);
    }
}
