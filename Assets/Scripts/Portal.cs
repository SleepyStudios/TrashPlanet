using UnityEngine;

public class Portal : MonoBehaviour
{
    public Portal goTo;
    private GameObject lastTeleported;

    private bool IsValidEntity(GameObject go)
    {
        return go.CompareTag("Player") || go.CompareTag("Box") || go.CompareTag("Trash");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (IsValidEntity(other.gameObject) && other.gameObject != lastTeleported)
        {
            GetComponent<AudioSource>().pitch = Random.Range(0.7f, 1.3f);
            GetComponent<AudioSource>().Play();

            other.GetComponent<PhysicsEntityBase>().TeleportTo(goTo);
        }
    }

    public void HandleTeleportedTo(GameObject go)
    {
        lastTeleported = go;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject == lastTeleported)
        {
            lastTeleported = null;
        }
    }
}
