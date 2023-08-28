using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IsoMovementController : MonoBehaviour
{
    [SerializeField] private float horizontalMovementSpeed = 1f;
    [SerializeField] private float verticalMovementSpeed = 1f;
    [SerializeField] private float rotationSpeed = 1f;

    public Transform isoCameraTransform;

    private Rigidbody rigidBody;
    private PlayerInputController playerInput;
    private float isoAngle;

    // Input Values
    private float _inputHorizontal;
    private float _inputVertical;
    private bool _inputAttackPressed;

    // Movement values
    Vector3 velocity;

    private void Awake()
    {
        rigidBody = GetComponent<Rigidbody>();
        if (!rigidBody)
        {
            Debug.LogError("Cannot find player rigidBody! Please add a rigidBody component to the player!");
        }

        playerInput = GetComponent<PlayerInputController>();
        if (!playerInput)
        {
            Debug.LogError("Cannot find player input controller! Please add a player input controller component to the player!");
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        // Hide cursor from game window (Not used yet)
        //Cursor.lockState = CursorLockMode.Locked;
        //Cursor.visible = false;

        // Grab camera rotation
        isoAngle = isoCameraTransform.rotation.eulerAngles.y;
    }

    // Update is called once per frame
    void Update()
    {
        // Save inputs every frame
        _inputHorizontal = playerInput.Horizontal;
        _inputVertical = playerInput.Vertical;

        // Calculate velocity
        velocity = (Vector3.right * _inputHorizontal * horizontalMovementSpeed) + 
                   (Vector3.forward * _inputVertical * verticalMovementSpeed);

        // Rotate to isometric world coordinates
        velocity = Quaternion.Euler(0, isoAngle, 0) * velocity;
    }

    // Physics movements within FixedUpdate
    private void FixedUpdate()
    {        
        // Act on inputs every physics frame
        rigidBody.MovePosition(transform.position + velocity * Time.deltaTime);

        // Rotate player to face velocity direction with slerp
        transform.forward = Vector3.Slerp(transform.forward, velocity.normalized, rotationSpeed * Time.deltaTime);
    }
}
