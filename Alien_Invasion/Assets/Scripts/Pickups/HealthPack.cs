/**
 * Alien Invation
 * 
 * Joshua Verhoeff
 * 
 * 2021-04-18
 * 
 *  Script to handle health pickups
 */
using UnityEngine;

// Script for health pack pickup
public class HealthPack : MonoBehaviour {
    [SerializeField] private int healthGiven = 25;
    [SerializeField] private float rotationSpeed = 5f;

    private GameManager manager;
    private float rotationAngle;

    private void Awake() {
        manager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    private void Update() {
        // Have the health pick up constantly rotating
        rotationAngle += rotationSpeed * Time.deltaTime;

        transform.eulerAngles = new Vector3(0f, rotationAngle, 0f);
    }

    // Heal the player if they run over the health pack
    private void OnTriggerEnter(Collider other) {
        if (other.gameObject.tag == "Player") {
            if (manager.AtMaxHealth()) {
                manager.SetInfoText("Max Health");
            } else {
                manager.Health += healthGiven;
                Destroy(gameObject);
            }
        }
    }

    private void OnTriggerExit(Collider other) {
        if (other.gameObject.tag == "Player") {
            manager.SetInfoText("");
        }
    }
}
