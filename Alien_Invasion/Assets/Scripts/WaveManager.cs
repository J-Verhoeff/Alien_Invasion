/**
 * Alien Invation
 * 
 * Joshua Verhoeff
 * 
 * 2021-04-18
 * 
 *  Script to manage enemy spaws each round
 *  spawns regular enemies, whith increasing health values
 *  spawns boss enemies every 5 rounds
 */
using UnityEngine;
using System.Collections;

public class WaveManager : MonoBehaviour {
    [Header("General")]
    [SerializeField] private Transform player;

    [Header("Wave Stats")]
    [SerializeField] private int maxEnemiesSpawned = 10;
    [SerializeField] private float timeBetweenRounds = 5f;
    [SerializeField] private float timeBetweenSpawns = 1f;

    [Header("Prefabs")]
    [SerializeField] private GameObject basicEnemyPrefab;
    [SerializeField] private GameObject bossEnemyPrefab;

    [Header("Spawners")]
    [SerializeField] private Transform[] spawnLocations;

    private bool spawn = false;
    private bool isBossRound = false;
    private GameManager manager;
    private float lastSpawned = 1f;
    private int spawnedThisRound;
    private int currentlySpawned;
    private int spawnThisRound;

    private void Awake() {
        manager = GetComponent<GameManager>();
        // deactivate the portals
        foreach (Transform location in spawnLocations) {
            location.Find("Portal").gameObject.SetActive(false);
        }
    }

    // Routine to go to next round
    IEnumerator WaitForRound(float delay) {
        // Cash and Score bonus at end of round
        manager.Score += 50;
        manager.Cash += 50;

        manager.Round += 1;
        SetRoundParameters();
        spawn = false;

        // Trigger a boss round every 5 rounds
        if (manager.Round % 5 == 0) {
            isBossRound = true;
            spawnThisRound = manager.Round / 5;
        }

        yield return new WaitForSeconds(delay);
        spawn = true;
    }

    private void Start() {
        StartCoroutine("WaitForRound", timeBetweenRounds);
    }

    private void Update() {
        // check if able to spawn
        if (!spawn) {
            return;
        }

        // Check for round end
        if (currentlySpawned <= 0 && spawnedThisRound >= spawnThisRound) {
            StartCoroutine("WaitForRound", timeBetweenRounds);
            return;
        }

        // Timer for spawning enemies
        if (lastSpawned <= 0f) {
            // Spawn an enemy if capable
            if (spawnedThisRound < spawnThisRound && currentlySpawned < maxEnemiesSpawned) {
                lastSpawned = timeBetweenSpawns;
                int spawnIndex = Random.Range(0, spawnLocations.Length);
                spawnLocations[spawnIndex].Find("Portal").gameObject.SetActive(true);

                if (isBossRound) {
                    // Boss enemies
                    GameObject newBoss = Instantiate(bossEnemyPrefab, spawnLocations[spawnIndex].position, spawnLocations[spawnIndex].rotation);
                    newBoss.GetComponent<EnemyAI>().SetTarget(player);
                    newBoss.GetComponent<EnemyAI>().SetHealth(manager.Round / 5);
                    spawnedThisRound += 1;
                    currentlySpawned += 1;

                } else {
                    // Normal enemies
                    GameObject newEnemy = Instantiate(basicEnemyPrefab, spawnLocations[spawnIndex].position, spawnLocations[spawnIndex].rotation);
                    newEnemy.GetComponent<EnemyAI>().SetTarget(player);
                    // Increase Enemy health every 5 rounds after round 10
                    if (manager.Round > 5) {
                        newEnemy.GetComponent<EnemyAI>().SetHealth((manager.Round / 5));
                    }
                    spawnedThisRound += 1;
                    currentlySpawned += 1;
                }
            }

        } else {
            lastSpawned -= Time.deltaTime;
        }
    }

    // Reset the parameters for the round
    private void SetRoundParameters() {
        isBossRound = false;
        spawnedThisRound = 0;
        currentlySpawned = 0;
        spawnThisRound = manager.Round * 5;
        foreach (Transform location in spawnLocations) {
            location.Find("Portal").gameObject.SetActive(false);
        }
    }

    // Decrement currently spawned upon enemy death
    public void EnemyKilled() {
        currentlySpawned -= 1;
    }

    // Add to spawn location list
    public void UpgradeSpawnLocations(Transform[] newLocations) {
        Transform[] temp = new Transform[newLocations.Length + spawnLocations.Length];
        spawnLocations.CopyTo(temp, 0);
        newLocations.CopyTo(temp, spawnLocations.Length);
        spawnLocations = temp;
    }
}
