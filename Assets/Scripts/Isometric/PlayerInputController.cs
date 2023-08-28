using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInputController : MonoBehaviour
{
    // Can do some fine tuning later
    private float filteredVerticalInput = 0f;
    private float filteredHorizontalInput = 0f;
    private float forwardSpeedLimit = 1f;

    public bool InputMapToCircular = true;

    public float forwardInputFilter = 5f;
    public float turnInputFilter = 5f;
    public float Vertical { get; private set; }

    public float Horizontal { get; private set; }

    public bool AttackPressed { get; private set; }

    public bool InteractPressed { get; private set; }

    public bool JumpPressed { get; private set; }

    public bool SprintHeld { get; private set; }

    public bool UtilityPressed { get; private set; }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //GetAxisRaw() so we can do filtering here instead of the InputManager
        float h = Input.GetAxisRaw("Horizontal");// setup h variable as our horizontal input axis
        float v = Input.GetAxisRaw("Vertical"); // setup v variables as our vertical input axis

        if (InputMapToCircular)
        {
            // make coordinates circular
            h *= Mathf.Sqrt(1f - 0.5f * v * v);
            v *= Mathf.Sqrt(1f - 0.5f * h * h);
        }

        filteredVerticalInput = Mathf.Clamp(Mathf.Lerp(filteredVerticalInput, v,
            Time.deltaTime * forwardInputFilter), -forwardSpeedLimit, forwardSpeedLimit);

        filteredHorizontalInput = Mathf.Lerp(filteredHorizontalInput, h,
            Time.deltaTime * turnInputFilter);

        Vertical = filteredVerticalInput;
        Horizontal = filteredHorizontalInput;

        // Grab values from input manager
        JumpPressed = Input.GetButtonDown("Jump");
        InteractPressed = Input.GetButtonDown("Interact");
        AttackPressed = Input.GetButtonDown("Attack");
        SprintHeld = Input.GetButton("Sprint");
        UtilityPressed = Input.GetButtonDown("Utility");
    }
}
