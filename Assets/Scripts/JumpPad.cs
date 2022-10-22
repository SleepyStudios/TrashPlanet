using UnityEngine;

public class JumpPad : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player") || collision.gameObject.CompareTag("Box") || collision.gameObject.CompareTag("Trash"))
        {
            if (collision.gameObject.GetComponent<PhysicsEntityBase>().CheckIfHitJumpPadAtCorrectAngle(this))
            {
                collision.gameObject.GetComponent<PhysicsEntityBase>().OnJumpPadHit(50f);
                GetComponent<AudioSource>().pitch = Random.Range(0.7f, 1.3f);
                GetComponent<AudioSource>().Play();
            }
        }
    }
}
