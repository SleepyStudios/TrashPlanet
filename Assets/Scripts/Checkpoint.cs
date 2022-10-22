using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.GetComponent<Animator>().SetBool("shrink", true);
            LevelManager.instance.HandleLevelComplete();
        }
    }
}
