/*
 * There is a script that allows the player to pass through the platfor on one way
 */
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class OneWayPlatform : MonoBehaviour {
    public float offset = 1.1f;//How much higher the player has to be for hte colider to be activated
    PlayerController c = null;
    BoxCollider2D box;
    public void Awake() {
        c = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        box = GetComponent<BoxCollider2D>();
    }

    void FixedUpdate() {
        box.enabled = c.transform.position.y - transform.position.y > offset;//Enable or disable the collider
    }
}
