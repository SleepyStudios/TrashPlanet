using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);

        if (FindObjectsOfType<MusicManager>().Length > 1)
        {
            DestroyImmediate(gameObject);
        } else
        {
            GetComponent<AudioSource>().Play();
        }
    }
}
