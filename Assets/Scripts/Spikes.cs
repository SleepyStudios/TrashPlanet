using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spikes : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            var sparks = Instantiate(Resources.Load("Sparks"), other.transform.position, Quaternion.identity) as GameObject;
            sparks.transform.rotation = Quaternion.LookRotation(transform.position - other.transform.position);

            GetComponent<AudioSource>().pitch = Random.Range(0.7f, 1.3f);
            GetComponent<AudioSource>().Play();

            LevelManager.instance.OnDeath();
        }
        else if (other.gameObject.CompareTag("Trash"))
        {
            CreateSparks(other);
            other.GetComponent<Trash>().Shrink();
        }
    }
    private void CreateSparks(Collider collision)
    {
        var sparks = Instantiate(Resources.Load("Sparks"), collision.transform.position, Quaternion.identity) as GameObject;
        sparks.transform.rotation = Quaternion.LookRotation(transform.position - collision.transform.position);
    }
}
