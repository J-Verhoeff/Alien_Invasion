/**
 * Alien Invation
 * 
 * Joshua Verhoeff
 * 
 * 2021-04-18
 * 
 *  Script to handle enemy melee attack 
 */
using UnityEngine;

// Script to deal melee damage to player
public class MeleeAttack : MonoBehaviour {
    [SerializeField] private int damage = 25;
    [SerializeField] private Animator animator;

    private GameManager manager;

    private void Awake() {
        manager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    // If there is a collision, and enemy is attacking, reduce player health
    private void OnTriggerEnter(Collider other) {
        if (animator.GetBool("Attack")) {
            manager.Health -= damage;
        }
    }
}
