using System.Collections;
using UnityEngine;
using System.Linq;

[RequireComponent(typeof(Rigidbody))]
public class PhysicsEntityBase : MonoBehaviour
{
    protected Rigidbody rb;

    protected int worldRotationState;

    protected void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    protected void OnEnable()
    {
        WorldRotationController.OnRotate += HandleWorldRotate;
    }

    protected void OnDisable()
    {
        WorldRotationController.OnRotate -= HandleWorldRotate;
    }

    protected virtual void HandleWorldRotate(int newState)
    {
        worldRotationState = newState;
    }

    public void TeleportTo(Portal p)
    {
        StartCoroutine(HandleTeleport(p));
    }

    IEnumerator HandleTeleport(Portal p)
    {
        p.HandleTeleportedTo(gameObject);

        GetComponent<Animator>().SetBool("shrink", true);

        rb.velocity = Vector3.zero;

        yield return new WaitForSeconds(0.3f);

        rb.MovePosition(p.transform.position);

        yield return new WaitForSeconds(0.2f);

        GetComponent<Animator>().SetBool("shrink", false);

        yield return null;
    }

    public void OnJumpPadHit(float force)
    {
        switch (worldRotationState)
        {
            case 0: // 12 o'clock
                rb.AddForce(new Vector3(0f, force), ForceMode.Impulse);
                break;
            case 1: // 3 o'clock
                rb.AddForce(new Vector3(-force, 0f), ForceMode.Impulse);
                break;
            case 2: // 6 o'clock
                rb.AddForce(new Vector3(0f, -force), ForceMode.Impulse);
                break;
            case 3: // 9 o'clock
                rb.AddForce(new Vector3(force, 0f), ForceMode.Impulse);
                break;
        }
    }

    public bool CheckIfHitJumpPadAtCorrectAngle(JumpPad jumpPad)
    {
        Vector3 dir = Vector3.zero;
        switch (worldRotationState)
        {
            case 0: // 12 o'clock
                dir = Vector3.up;
                break;
            case 1: // 3 o'clock
                dir = Vector3.left;
                break;
            case 2: // 6 o'clock
                dir = Vector3.down;
                break;
            case 3: // 9 o'clock
                dir = Vector3.right;
                break;
        }

        RaycastHit[] hits = Physics.RaycastAll(jumpPad.transform.position, dir, 0.3f);
        return hits.Any((h) => h.transform.gameObject.name == gameObject.name);
    }
}
