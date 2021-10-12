/**
 * Alien Invation
 * 
 * Joshua Verhoeff
 * 
 * 2021-04-18
 * 
 *  Script to handle clearing an obstacle
 *  Purchasing and removing the obstacle, as well as 
 *  adding any new enemy spawn points
 */
using UnityEngine;

// Script to remove an obstacle and access new area
public class BuyArea : MonoBehaviour {
    [Header("General")]
    [SerializeField] private int cost = 100;
    [SerializeField] private string objectName;
    [SerializeField] private Transform[] newSpawnLocations;

    [Header("Door Fields")]
    [SerializeField] private bool isDoor;
    [SerializeField] private float doorRotate = 90f;

    [Header("Player Fields")]
    [SerializeField] private Transform playerPosition;

    private GameManager manager;
    private bool purchased = false;

    private void Awake() {
        manager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    private void Start() {
        if (newSpawnLocations.Length > 0) {
            foreach (Transform location in newSpawnLocations) {
                location.Find("Portal").gameObject.SetActive(false);
            }
        }
    }

    private void OnMouseOver() {
        // If the player is in a min distance, let them purchase obstacle
        //Debug.Log("MouseOver");
        Vector3 dist = transform.position - playerPosition.position;
        if (dist.magnitude < 5f && !purchased) {
            manager.SetInfoText("Press 'E' to Purchase " + objectName + "\nCost: " + cost);

            // Handle Purchasing of the obstacle use E as interact
            if (Input.GetKeyDown(KeyCode.E) && manager.Cash >= cost) {
                purchased = true;
                manager.Cash -= cost;
                if (isDoor) {
                    transform.eulerAngles = new Vector3(transform.rotation.x,
                                                        transform.rotation.y + doorRotate,
                                                        transform.rotation.z);
                } else {
                    manager.SetInfoText("");
                    Destroy(gameObject);
                }

                // Upgrade spawn locations if applicable
                if (newSpawnLocations.Length > 0) {
                    manager.gameObject.GetComponent<WaveManager>().UpgradeSpawnLocations(newSpawnLocations);
                }
            }
        }
    }

    private void OnMouseExit() {
        manager.SetInfoText("");
    }
}
