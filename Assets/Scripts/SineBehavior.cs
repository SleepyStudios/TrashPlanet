using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SineBehavior : MonoBehaviour
{
    private Vector3 _startPosition;

    // Start is called before the first frame update
    void Start()
    {
        _startPosition = transform.localPosition;
    }

    // Update is called once per frame
    void Update()
    {
        transform.localPosition = _startPosition + new Vector3(0.0f, Mathf.Sin(Time.time * 4) * 0.2f, 0.0f);
    }
}
