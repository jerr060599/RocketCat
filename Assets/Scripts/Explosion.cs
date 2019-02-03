/*
 * This script applies damage to the player and automatically despawns after animation finish
 * 
 */
using UnityEngine;

[RequireComponent(typeof(Animator))]    //This script requires an animator
public class Explosion : MonoBehaviour {
    public float radius = 5f;   //The blast radius
    public float damage = 30f;  //The blast damage
    Animator animator;          //The animator component for the explosion

    void Start() {
        GameObject player = GameObject.FindGameObjectWithTag("Player");                 //Find and retrieves the first object tagged with "player"
        if (Vector3.Distance(player.transform.position, transform.position) <= radius)  //Check in the sole player is in range
            player.GetComponent<PlayerController>().Damage(damage);                     //Apply damage if player is in blast range

        animator = GetComponent<Animator>();    //Find the animator
    }

    void Update() {
        if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 1 && !animator.IsInTransition(0))
            Destroy(gameObject);    //Destory object once the explosion plays through
    }
}
