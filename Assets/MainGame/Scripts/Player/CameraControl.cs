//Lets player interact using camera

using UnityEngine;
using UnityEngine.InputSystem;
using StarMaps;

public class CameraControl : MonoBehaviour
{
    //Scripts & references
    PathFinder _pathfinder;
    Transform camTrans;

    //Player actions
    InputAction inputMove;
    Vector3 movement;
    InputAction inputBoost;
    InputAction inputLook;
    Vector2 look;
    float lookX;
    float lookY;
    float rotX;

    [Header("Settings")]
    [SerializeField] float adjustableSensitivity;
    [SerializeField] float speedBoostAmount;
    [HideInInspector] public GameObject UIPauseMenu;

    [Space]
    [Header("Audio")]
    [SerializeField] AudioSource escapeButtonNoise;

    bool isUIMenuActive=false;

    private void Start() {
        UIPauseMenu.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        camTrans = gameObject.transform;
        _pathfinder = GameObject.Find("PathFinder").GetComponent<PathFinder>();
        inputMove = GetComponent<PlayerInput>().actions["CamMove"];
        inputBoost = GetComponent<PlayerInput>().actions["CamBoost"];
        inputLook = GetComponent<PlayerInput>().actions["CamLook"];
        rotX = camTrans.rotation.eulerAngles.x;
    }

    private void Update() {
        if (!isUIMenuActive) {
            CamMove(inputBoost.ReadValue<float>() > 0);
            CamLook();
        }
    }

    //Move camera around
    public void CamMove(bool boost) {
        movement = inputMove.ReadValue<Vector3>() * Time.deltaTime * 60;
        if (boost) {
            camTrans.position += ((camTrans.forward * movement.z * 0.05f) + (camTrans.right * movement.x * 0.05f) + (camTrans.up * movement.y * 0.05f)) * speedBoostAmount;
        }
        else {
            camTrans.position += (camTrans.forward * movement.z * 0.05f) + (camTrans.right * movement.x * 0.05f) + (camTrans.up * movement.y * 0.05f);
        }
    }

    //Rotate the camera around
    public void CamLook() {
        look = inputLook.ReadValue<Vector2>();
        lookX = look.x * adjustableSensitivity * Time.deltaTime;
        lookY = look.y * adjustableSensitivity * Time.deltaTime;
        rotX -= lookY;
        rotX = Mathf.Clamp(rotX, -70, 70);

        transform.localRotation = Quaternion.Euler(rotX, camTrans.localEulerAngles.y, 0);
        camTrans.Rotate(Vector3.up * lookX);
    }

    //Pause the game
    public void OnPauseGame() {
        escapeButtonNoise.Play();
        if (!isUIMenuActive) {
            UIPauseMenu.SetActive(true);
            Cursor.lockState = CursorLockMode.None;
            isUIMenuActive = true;
        }
        else if (isUIMenuActive) {
            UIPauseMenu.SetActive(false);
            Cursor.lockState = CursorLockMode.Locked;
            isUIMenuActive = false;
        }
    }

    //Activates pathfinder checker
    public void OnSelectDestination() {
        if (!isUIMenuActive) {

            Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit)) {
                hit.transform.GetComponent<IInteract>().Interact();
            }
        }
    }
}
