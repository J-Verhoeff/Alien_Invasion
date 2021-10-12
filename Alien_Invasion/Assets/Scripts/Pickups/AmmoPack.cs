/**
 * Alien Invation
 * 
 * Joshua Verhoeff
 * 
 * 2021-04-18
 * 
 *  Script to use for ammo pickups
 *  Picking up an ammo pack will fill ammo on currently equiped weapon
 */
using UnityEngine;

public class AmmoPack : MonoBehaviour {
    [SerializeField] private float rotationSpeed = 100f;

    private GameManager manager;
    private float rotationAngle;

    private void Awake() {
        manager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    private void Update() {
        // Have the ammo pick up constantly rotating
        rotationAngle += rotationSpeed * Time.deltaTime;

        transform.eulerAngles = new Vector3(0f, rotationAngle, 0f);
    }

    private void OnTriggerEnter(Collider other) {
        if (other.gameObject.tag == "Player") {
            other.gameObject.GetComponent<FirstPersonPlayer>().AmmoPickUp();
            Destroy(gameObject);
        }
    }
}
