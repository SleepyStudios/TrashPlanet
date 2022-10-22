using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CogController : MonoBehaviour
{
    public bool clockwise = true;
    public int speed = 10;
    int direction = 1;

    // Start is called before the first frame update
    void Start()
    {
        if (clockwise)
        {
            direction = -1;
        }
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(direction * speed * 10 * Time.deltaTime, 0f, 0f, Space.Self);
    }
}
