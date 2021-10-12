/**
 * Alien Invation
 * 
 * Joshua Verhoeff
 * 
 * 2021-04-18
 * 
 *  Script to handle the enemy AI
 *  Uses NavMeshAgent to move toward player and trys to melee attack
 *  When the enemy is killed score and cash is incremented, and there is 
 *  A chance to drop a pickup
 *  
 */
using UnityEngine;
using UnityEngine.AI;
using System.Collections;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Animator))]
public class EnemyAI : MonoBehaviour {
    [Header("Enemy Stats")]
    [SerializeField] private int health = 10;
    [SerializeField] private float movementSpeed = 5f;
    [SerializeField] private int moneyValue = 10;
    [SerializeField] private int scoreValue = 10;

    [Header("Movement")]
    [SerializeField] private Transform target;
    [SerializeField] private float closeEnough = 1f;

    [Header("Prefabs")]
    [SerializeField] private GameObject healthPackPrefab;
    [SerializeField] private GameObject ammoPackPrefab;

    private NavMeshAgent agent;
    private Animator animator;
    private GameManager manager;
    private float currentHealth;
    private bool isDead = false;

    private void Awake() {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        manager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    // Routine to trigger attack annimation
    // Used to prevent movement during annimation
    IEnumerator AttackPlayer(float delay) {
        animator.SetBool("Attack", true);
        agent.isStopped = true;
        yield return new WaitForSeconds(delay);
        agent.isStopped = false;
        animator.SetBool("Attack", false);
    }

    // Routine triggered when enemy dies
    IEnumerator IsDead(float delay) {
        animator.SetTrigger("IsDead");

        //increment cash and score 
        manager.Cash += moneyValue;
        manager.Score += scoreValue;

        isDead = true;
        agent.isStopped = true;

        // disable collider so player can run overdead enemy
        GetComponent<CapsuleCollider>().enabled = false;

        // Tell wave manager the enemy was killed
        manager.gameObject.GetComponent<WaveManager>().EnemyKilled();

        // Randomly spawn a pickup
        int chance = Random.Range(0, 20); // 5% chance of spawning
        if (chance == 1) {
            int choice = Random.Range(0, 2);
            Vector3 pos = new Vector3(transform.position.x, transform.position.y + 1f, transform.position.z);
            if (choice != 0) {
                Instantiate(healthPackPrefab, pos, transform.rotation);
            } else {
                Instantiate(ammoPackPrefab, pos, transform.rotation);
            }
        }

        yield return new WaitForSeconds(delay);
        // Destroy enemy object
        Destroy(gameObject);
    }

    private void Start() {
        currentHealth = health;
        agent.speed = movementSpeed;
    }

    private void Update() {
        // Check if dead
        if (isDead) {
            return;
        }

        // check if enemy was killed
        if (currentHealth <= 0) {
            StartCoroutine("IsDead", 20f);
            return;
        }

        // Use NavMeshAgent to move towards player
        agent.SetDestination(target.position);
        animator.SetFloat("Speed", agent.velocity.magnitude);

        // Check if close enough to attack player
        float distanceToTarget;
        distanceToTarget = Vector3.Distance(agent.transform.position, agent.destination);
        if (distanceToTarget < closeEnough) {
            StartCoroutine("AttackPlayer", 3f);
        }

    }

    // Method to take damage
    public void TakeDamage(int amountTaken) {
        currentHealth -= amountTaken;
    }

    // Set the target
    public void SetTarget(Transform newTarget) {
        target = newTarget;
    }

    // Adjust enemy health
    // Wave manager increases enemy health as rounds progress
    public void SetHealth(int newHealth) {
        health *= newHealth;
    }
}
