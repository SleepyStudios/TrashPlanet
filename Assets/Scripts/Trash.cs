using UnityEngine;

public class Trash : PhysicsEntityBase
{
    private bool destroying;

    private new void Start()
    {
        base.Start();
        rb.isKinematic = true;
        Invoke("WakeUpRB", 1f);
    }

    private void WakeUpRB()
    {
        rb.isKinematic = false;
    }

    public void Shrink()
    {
        if (!destroying)
        {
            GetComponent<AudioSource>().pitch = Random.Range(0.7f, 1.3f);
            GetComponent<AudioSource>().Play();
        }

        destroying = true;
    }

    private void Update()
    {
        if (destroying)
        {
            transform.localScale = Vector3.Lerp(transform.localScale, Vector3.zero, 0.3f * Time.deltaTime);
        }
    }
}
