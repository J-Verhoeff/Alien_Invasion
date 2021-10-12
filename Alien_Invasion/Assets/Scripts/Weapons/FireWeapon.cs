/**
 * Alien Invation
 * 
 * Joshua Verhoeff
 * 
 * 2021-04-18
 * 
 *  Script to handle the weapons
 *  Triggers annimations and hit detection
 *  Some value/methods taken from Modern Guns package
 */
using UnityEngine;
using System.Collections;

public class FireWeapon : MonoBehaviour {
    [Header("Weapon Stats")]
    [SerializeField] private float range = 100f;
    [SerializeField] private float fireRate = 2f;
    [SerializeField] private float reloadTime = 2f;
    [SerializeField] private int damage = 10;
    [SerializeField] private int maxAmmo = 100;
    [SerializeField] private int magazineCapacity = 10;
    [SerializeField] private int startingAmmo = 50;
    [SerializeField] private Sprite weaponSprite;


    [Header("Location References")]
    [SerializeField] private Transform mainCamera = null;
    [SerializeField] private Transform barrelLocation;
    [SerializeField] private Transform casingEjection;
    [SerializeField] private Animator gunAnimator;

    [Header("Layers")]
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private LayerMask wallLayer;
    [SerializeField] private LayerMask enemyLayer;

    [Header("Prefabs")]
    [SerializeField] private GameObject muzzleFlashPrefab;
    [SerializeField] private GameObject casingPrefab;
    [SerializeField] private GameObject bulletHolePrefab;
    [SerializeField] private GameObject bulletPrefab;

    [Header("Settings")]
    [Tooltip("Specify time to destory the casing object")] [SerializeField] private float destroyTimer = 2f;
    [Tooltip("Bullet Speed")] [SerializeField] private float shotPower = 500f;
    [Tooltip("Casing Ejection Speed")] [SerializeField] private float ejectPower = 150f;


    private GameManager manager;
    private int currentTotalAmmo = 0;
    private int currentMagazineAmount = 0;
    private float currentTimer = 0f;
    private bool canFire = false;
    private bool reloading = false;

    private void Awake() {
        manager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    private void Start() {
        // Set starting ammo
        currentMagazineAmount = magazineCapacity;
        currentTotalAmmo = startingAmmo - magazineCapacity;
    }

    private void Update() {
        // if still reloading return
        if (reloading) {
            return;
        }
        // Simple fire rate handling
        if (canFire) {
            // Fire button pressed
            if (Input.GetMouseButtonDown(0)) {
                if (currentMagazineAmount > 0) {
                    Shoot();
                    canFire = false;
                } else if (currentMagazineAmount <= 0 && currentTotalAmmo > 0) {
                    Reload();
                    canFire = false;
                }
            }
        } else {
            if (currentTimer <= 0) {
                currentTimer = fireRate;
                canFire = true;
            } else {
                currentTimer -= Time.deltaTime;
            }
        }

        // Reload button pressed
        if (Input.GetKeyDown(KeyCode.R) && currentMagazineAmount <= magazineCapacity && currentTotalAmmo > 0) {
            Reload();
        }
    }

    // Script to handle firing of the weapon
    private void Shoot() {
        RaycastHit hit;
        //Debug.Log("Shoot!");

        // update ammo count
        currentMagazineAmount--;
        manager.SetAmmoCount(currentMagazineAmount, currentTotalAmmo);

        if (gunAnimator != null) {
            gunAnimator.SetTrigger("Fire");
        } else {
            ShootAnnimation();
            CasingRelease();
        }

        // Detect hitting an enemy
        if (Physics.Raycast(mainCamera.position, mainCamera.forward, out hit, range, enemyLayer)) {
            //Debug.Log("Enemy Hit! " + hit.collider.name);
            if (hit.collider.gameObject.GetComponent<EnemyAI>() != null) {
                hit.collider.gameObject.GetComponent<EnemyAI>().TakeDamage(damage);
            }

        } else if (Physics.Raycast(mainCamera.position, mainCamera.forward, out hit, range, wallLayer)) {
            // Hit a wall
            // Debug.Log("Hit a wall!");
            if (bulletHolePrefab != null) {
                GameObject hole = Instantiate(
                    bulletHolePrefab,
                    hit.point + (0.01f * hit.normal),
                    Quaternion.LookRotation(-1 * hit.normal, hit.transform.up)
                );

                Destroy(hole, 20f); // destroy bullet hole after 20 seconds
            }
        } else if (Physics.Raycast(mainCamera.position, mainCamera.forward, out hit, range, groundLayer)) {
            // Hit the floor
            // Debug.Log("Hit the ground");
            if (bulletHolePrefab != null) {
                GameObject hole = Instantiate(
                    bulletHolePrefab,
                    hit.point + (0.01f * hit.normal),
                    Quaternion.LookRotation(-1 * hit.normal, hit.transform.up)
                );

                Destroy(hole, 20f); // destroy bullet hole after 20 seconds
            }
        }
    }

    // Set the UI image for the weapon
    public void SetImage() {
        manager.SetWeaponImage(weaponSprite);
    }

    // Update UI with ammo count
    public void SetAmmo() {
        manager.SetAmmoCount(currentMagazineAmount, currentTotalAmmo);
    }

    // Upgrade weeapon damage
    public void Upgrade(int newDamage) {
        damage = newDamage;
    }

    // Temp enumerator to have pseudo reload annimation
    // Deops weapon downm, then raises it back up
    IEnumerator TempReloadAnnimation(float delay) {
        reloading = true;
        transform.localPosition = new Vector3(transform.localPosition.x,
                                                transform.localPosition.y - 2f,
                                                transform.localPosition.z);
        yield return new WaitForSeconds(delay);
        transform.localPosition = new Vector3(transform.localPosition.x,
                                                transform.localPosition.y + 2f,
                                                transform.localPosition.z);
        reloading = false;
    }

    // Script to handle reloading
    private void Reload() {
        int remianingAmmo = currentMagazineAmount;
        if (currentTotalAmmo >= magazineCapacity - remianingAmmo) {
            currentMagazineAmount = magazineCapacity;
            currentTotalAmmo -= magazineCapacity - remianingAmmo;
        } else {
            currentMagazineAmount += currentTotalAmmo;
            currentTotalAmmo = 0;
        }
        manager.SetAmmoCount(currentMagazineAmount, currentTotalAmmo);
        // TODO: trigger reload annimation
        StartCoroutine("TempReloadAnnimation", reloadTime);
    }

    // Fill up ammo
    public void FillAmmo() {
        currentTotalAmmo = maxAmmo;
        SetAmmo();
    }

    // Annimation Scripts
    //This function creates the bullet behavior
    void ShootAnnimation() {
        if (muzzleFlashPrefab) {
            //Create the muzzle flash
            GameObject tempFlash;
            tempFlash = Instantiate(muzzleFlashPrefab, barrelLocation.position, barrelLocation.rotation);

            //Destroy the muzzle flash effect
            Destroy(tempFlash, destroyTimer);
        }

        //cancels if there's no bullet prefeb
        if (!bulletPrefab) { return; }

        // Create a bullet and add force on it in direction of the barrel
        Instantiate(bulletPrefab, barrelLocation.position, barrelLocation.rotation).GetComponent<Rigidbody>().AddForce(barrelLocation.forward * shotPower);
    }

    //This function creates a casing at the ejection slot
    void CasingRelease() {
        //Cancels function if ejection slot hasn't been set or there's no casing
        if (!casingEjection || !casingPrefab) { return; }

        //Create the casing
        GameObject tempCasing;
        tempCasing = Instantiate(casingPrefab, casingEjection.position, casingEjection.rotation) as GameObject;
        //Add force on casing to push it out
        tempCasing.GetComponent<Rigidbody>().AddExplosionForce(Random.Range(ejectPower * 0.7f, ejectPower), (casingEjection.position - casingEjection.right * 0.3f - casingEjection.up * 0.6f), 1f);
        //Add torque to make casing spin in random direction
        tempCasing.GetComponent<Rigidbody>().AddTorque(new Vector3(0, Random.Range(100f, 500f), Random.Range(100f, 1000f)), ForceMode.Impulse);

        //Destroy casing after X seconds
        Destroy(tempCasing, destroyTimer);
    }

}
