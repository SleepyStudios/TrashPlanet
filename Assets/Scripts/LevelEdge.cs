using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelEdge : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            CreateSparks(collision);

            LevelManager.instance.OnDeath();
        } else if (collision.gameObject.CompareTag("Trash"))
        {
            CreateSparks(collision);
            collision.gameObject.GetComponent<Trash>().Shrink();
        }
    }

    private void CreateSparks(Collision collision)
    {
        var sparks = Instantiate(Resources.Load("Sparks"), collision.transform.position, Quaternion.identity) as GameObject;
        sparks.transform.rotation = Quaternion.LookRotation(transform.position - collision.transform.position);
    }
}
