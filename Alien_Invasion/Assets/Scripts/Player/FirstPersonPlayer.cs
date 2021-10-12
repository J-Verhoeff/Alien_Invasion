/**
 * Alien Invation
 * 
 * Joshua Verhoeff
 * 
 * 2021-04-18
 * 
 *  Script to handle First Person  Player movement
 */

using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class FirstPersonPlayer : MonoBehaviour {
    [Header("General")]
    [SerializeField] private float movementSpeed = 15f;
    [SerializeField] private float crouchHeight = 1.0f;
    [SerializeField] private float defaultCameraHeight = 2f;

    [Header("Falling")]
    [SerializeField] private float gravityFactor = 1f;
    [SerializeField] private Transform groundPosition;
    [SerializeField] private LayerMask groundLayer;

    [Header("Jumping")]
    [SerializeField] private float jumpSpeed = 7f;
    [SerializeField] private bool canAirControl = true;

    [Header("Looking")]
    [SerializeField] private Transform playerCamera;
    [SerializeField] private float mouseSenitivity;
    [SerializeField] private float verticalClamp = 90f;

    [Header("Weapons")]
    [SerializeField] private GameObject[] weapons;
    [SerializeField] private int maxCarryable = 2;
    [SerializeField] private string defaultWeapon = "pistol";

    private string[] activeWeapons;
    private CharacterController controller;
    private float verticalRotation = 0f;
    private float verticalSpeed = 0f;
    private bool isGrounded = false;
    private bool isCrouched = false;
    private int currentlyEquiped = 0;

    // Function to add a gun to inventory upon purchase
    public void AddWeapon(string weaponTag) {
        int newWeapon = currentlyEquiped + 1;
        // If at max capicity replace the currently equiped weapon
        if (newWeapon >= maxCarryable) {
            activeWeapons[currentlyEquiped] = weaponTag;
            SwapWeapon(weaponTag);
        } else {
            activeWeapons[newWeapon] = weaponTag;
            SwapWeapon(weaponTag);
        }
    }

    // Function to upgrade a weapon
    public void UpgradeWeapon(string weaponTag, int newDamageValue) {
        foreach (GameObject weapon in weapons) {
            if (weapon.tag == weaponTag) {
                weapon.GetComponent<FireWeapon>().Upgrade(newDamageValue);
            }
        }
    }

    // Swap to a weapon
    private void SwapWeapon(string weaponName) {
        foreach (GameObject weapon in weapons) {
            if (weapon.tag == weaponName) {
                weapon.SetActive(true);
                weapon.GetComponentInChildren<FireWeapon>().SetAmmo();
                weapon.GetComponentInChildren<FireWeapon>().SetImage();
            } else {
                weapon.SetActive(false);
            }
        }
    }

    private void Awake() {
        controller = GetComponent<CharacterController>();
    }

    private void Start() {
        Cursor.lockState = CursorLockMode.Locked;

        // Ensure player only has pistol at game start
        activeWeapons = new string[maxCarryable];
        activeWeapons[0] = defaultWeapon;
        SwapWeapon(activeWeapons[0]);
    }

    private void Update() {
        Vector3 x = Vector3.zero;
        Vector3 y = Vector3.zero;
        Vector3 z = Vector3.zero;

        // are we on the ground?
        RaycastHit collision;
        if (Physics.Raycast(groundPosition.position, Vector3.down, out collision, 0.2f, groundLayer)) {
            isGrounded = true;
        } else {
            isGrounded = false;
        }

        // update vertical speed
        if (!isGrounded) {
            verticalSpeed += gravityFactor * -9.81f * Time.deltaTime;
        } else {
            verticalSpeed = 0f;
        }

        // adjust the rotations based on mouse movement
        float mouseX = Input.GetAxis("Mouse X") * mouseSenitivity * Time.deltaTime;
        transform.Rotate(Vector3.up * mouseX);

        float mouseY = Input.GetAxis("Mouse Y") * mouseSenitivity * Time.deltaTime;
        verticalRotation -= mouseY;
        verticalRotation = Mathf.Clamp(verticalRotation, -1f * verticalClamp, verticalClamp);
        playerCamera.localEulerAngles = new Vector3(verticalRotation, 0f, 0f);


        // handle jumping
        if (isGrounded && Input.GetButtonDown("Jump")) {
            verticalSpeed = jumpSpeed;
            isGrounded = false;
            y = transform.up * verticalSpeed;
        } else if (!isGrounded) {
            y = transform.up * verticalSpeed;
        }

        // handle motion
        if (isGrounded || canAirControl) {
            x = transform.right * Input.GetAxis("Horizontal") * movementSpeed;
            z = transform.forward * Input.GetAxis("Vertical") * movementSpeed;
        }

        Vector3 movement = (x + y + z) * Time.deltaTime;
        controller.Move(movement);

        // handle crouching
        if (Input.GetKeyDown(KeyCode.C)) {
            if (!isCrouched) {
                playerCamera.localPosition = new Vector3(playerCamera.localPosition.x, crouchHeight, playerCamera.localPosition.z);
                isCrouched = true;
                movementSpeed /= 2f;
            } else {
                playerCamera.localPosition = new Vector3(playerCamera.localPosition.x, defaultCameraHeight, playerCamera.localPosition.z);
                isCrouched = false;
                movementSpeed *= 2f;
            }
        }

        // Weapon swapping
        if (Input.GetKeyDown(KeyCode.Q) || Input.GetAxis("Mouse ScrollWheel") > 0f) {
            currentlyEquiped += 1;
            if (currentlyEquiped >= maxCarryable || activeWeapons[currentlyEquiped] == null) {
                currentlyEquiped = 0;
            }
            SwapWeapon(activeWeapons[currentlyEquiped]);
        } else if (Input.GetAxis("Mouse ScrollWheel") < 0f) {
            currentlyEquiped -= 1;
            if (currentlyEquiped < 0) {
                currentlyEquiped = maxCarryable - 1;
            }
            SwapWeapon(activeWeapons[currentlyEquiped]);
        }
    }

    // Fill ammo upon ammo pickup
    public void AmmoPickUp() {
        foreach (GameObject weapon in weapons) {
            if (activeWeapons[currentlyEquiped] == weapon.tag) {
                weapon.GetComponentInChildren<FireWeapon>().FillAmmo();
                break;
            }
        }
    }
}
