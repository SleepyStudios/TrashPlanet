using UnityEngine;
using UnityEngine.InputSystem;
using System.Linq;

public class Player : PhysicsEntityBase
{
    private Collider col;
    public float speed = 8f, jumpForce = 10f, jumpArcForce = 10f;
    private Controls controls;

    private PlayerSide[] sides;
    private PlayerBottom bottom;

    private bool grabbing, canPlayGrabSound;

    private AudioSource sfx;

    new void Start()
    {
        base.Start();
        rb = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();
        sides = GetComponentsInChildren<PlayerSide>();
        bottom = GetComponentInChildren<PlayerBottom>();
        sfx = GetComponent<AudioSource>();
    }

    private new void OnEnable()
    {
        base.OnEnable();
        controls = new Controls();
        controls.Player.Enable();
        LevelManager.OnPlayerDeath += OnDeath;
    }

    private new void OnDisable()
    {
        base.OnDisable();
        controls.Player.Disable();
        LevelManager.OnPlayerDeath -= OnDeath;
    }

    public void HandleMove(Vector2 axis)
    {
        var xVel = axis.x * speed;
        if (xVel < 0 && !CanMoveLeft()) return; 
        if (xVel > 0 && !CanMoveRight()) return;

        if (axis.x != 0)
        {
            transform.Find("Model").localRotation = Quaternion.Euler(new Vector3(-90f, -90f, xVel > 0 ? 0f : 180f));
        }

        switch (worldRotationState)
        {
            case 0: // 12 o'clock
                rb.velocity = new Vector3(xVel, rb.velocity.y, rb.velocity.z);
                break;
            case 1: // 3 o'clock
                rb.velocity = new Vector3(rb.velocity.x, xVel, rb.velocity.z);
                break;
            case 2: // 6 o'clock
                rb.velocity = new Vector3(-xVel, rb.velocity.y, rb.velocity.z);
                break;
            case 3: // 9 o'clock
                rb.velocity = new Vector3(rb.velocity.x, -xVel, rb.velocity.z);
                break;
        }

        if (rb.velocity.sqrMagnitude > 0) rb.useGravity = true;
    }

    private void FixedUpdate()
    {
        HandleMove(controls.Player.Move.ReadValue<Vector2>());
        CheckGrabbing();
    }

    private bool CanMoveLeft()
    {
        return !sides.First((s) => s.side == Side.LEFT).touchingWall;
    }

    private bool CanMoveRight()
    {
        return !sides.First((s) => s.side == Side.RIGHT).touchingWall;
    }

    public bool IsGrounded()
    {
        return bottom.grounded;
    }

    public void OnJump(InputAction.CallbackContext ctx)
    {
        if (ctx.performed && IsGrounded())
        {
            switch (worldRotationState)
            {
                case 0: // 12 o'clock
                    rb.AddForce(new Vector3(GetJumpArcForce(), jumpForce), ForceMode.Impulse);
                    break;
                case 1: // 3 o'clock
                    rb.AddForce(new Vector3(-jumpForce, GetJumpArcForce()), ForceMode.Impulse);
                    break;
                case 2: // 6 o'clock
                    rb.AddForce(new Vector3(GetJumpArcForce(), -jumpForce), ForceMode.Impulse);
                    break;
                case 3: // 9 o'clock
                    rb.AddForce(new Vector3(jumpForce, GetJumpArcForce()), ForceMode.Impulse);
                    break;
            }

            grabbing = false;
        }
    }

    private float GetJumpArcForce()
    {
        if (grabbing)
        {
            if (!CanMoveLeft())
            {
                switch (worldRotationState)
                {
                    case 0: // 12 o'clock
                    case 1: // 3 o'clock
                        return jumpArcForce;
                    case 2: // 6 o'clock
                    case 3: // 9 o'clock
                        return -jumpArcForce;
                }
            } else if (!CanMoveRight())
            {
                switch (worldRotationState)
                {
                    case 0: // 12 o'clock
                    case 1: // 3 o'clock
                        return -jumpArcForce;
                    case 2: // 6 o'clock
                    case 3: // 9 o'clock
                        return jumpArcForce;
                }
            }
        }

        return 0f;
    }

    public void OnRestart(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
        {
            LevelManager.instance.OnDeath();
        }
    }

    public void OnGrab(InputAction.CallbackContext ctx)
    {
        if (ctx.started)
        {
            grabbing = true;
            canPlayGrabSound = true;
        }
        else if (ctx.canceled)
        {
            grabbing = false;
        }
    }

    private void CheckGrabbing()
    {
        if (grabbing)
        {
            if (!CanMoveLeft() || !CanMoveRight())
            {
                if (canPlayGrabSound)
                {
                    var clip = Resources.Load<AudioClip>("Audio/grab");
                    sfx.pitch = Random.Range(0.7f, 1.3f);
                    sfx.PlayOneShot(clip);
                    canPlayGrabSound = false;
                }

                rb.useGravity = false;
                rb.velocity = Vector3.zero;
            }
        } else
        {
            rb.useGravity = true;
        }
    }

    private void OnDeath()
    {
        var clip = Resources.Load<AudioClip>("Audio/explode");
        sfx.pitch = Random.Range(0.7f, 1.3f);
        sfx.PlayOneShot(clip);

        rb.isKinematic = true;
    }

    protected override void HandleWorldRotate(int newState)
    {
        base.HandleWorldRotate(newState);
        grabbing = false;
    }

}
