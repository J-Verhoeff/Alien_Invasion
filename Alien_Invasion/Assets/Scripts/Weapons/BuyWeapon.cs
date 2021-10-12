/**
 * Alien Invation
 * 
 * Joshua Verhoeff
 * 
 * 2021-04-18
 * 
 * Script to handle buying weapons off the all racks
 * Also handles weapon upgrades
 */
using UnityEngine;

// Script to buy a new weapon off the wall
public class BuyWeapon : MonoBehaviour {
    [Header("General")]
    [SerializeField] private GameObject attachedWeapon;
    [SerializeField] private int cost = 100;
    [SerializeField] private string weaponName;

    [Header("Upgrades")]
    [SerializeField] private bool isUpgrade = false;
    [SerializeField] private int newDamageValue = 20;

    [Header("Player Fields")]
    [SerializeField] private Transform playerPosition;

    private GameManager manager;
    private FirstPersonPlayer player;
    private bool purchased = false;

    private void Awake() {
        manager = GameObject.Find("GameManager").GetComponent<GameManager>();
        player = GameObject.Find("FirstPersonPlayer").GetComponent<FirstPersonPlayer>();
    }

    private void Start() {
        attachedWeapon.SetActive(false);
    }

    private void OnMouseOver() {
        // If the player is in a min distance, let them purchase weapon
        //Debug.Log("MouseOver");
        Vector3 dist = transform.position - playerPosition.position;
        if (dist.magnitude < 5f && !purchased) {
            manager.SetInfoText("Press 'E' to Purchase " + weaponName + "\nCost: " + cost);

            // If the player has enough money purchase the weapon by pressing E
            if (Input.GetKeyDown(KeyCode.E) && manager.Cash >= cost) {
                manager.Cash -= cost;
                purchased = true;
                attachedWeapon.SetActive(true);
                gameObject.transform.Find("WeaponImage").gameObject.SetActive(false);

                if (!isUpgrade) {
                    player.AddWeapon(attachedWeapon.tag);
                } else {
                    player.UpgradeWeapon(attachedWeapon.tag, newDamageValue);
                }
            }
        }
    }

    private void OnMouseExit() {
        manager.SetInfoText("");
    }
}
