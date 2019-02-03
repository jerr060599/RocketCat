/*
 * This script simply notifies the player script that its touching the checkpoint
 */
using UnityEngine;

public class Checkpoint : MonoBehaviour {
    private void OnTriggerEnter2D(Collider2D collision) {//This is called when the player collider enters the trigger
        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>().OnWin();
    }
}
