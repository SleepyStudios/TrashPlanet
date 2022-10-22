using System;
using UnityEngine;
using Cinemachine;

public class WorldRotationController : MonoBehaviour
{
    public static event Action<int> OnRotate;

    private float tmrRotate;

    private Transform cam;
    private Quaternion nextRot;

    private Player player;

    public float rotationSpeed = 3f;

    private CinemachineImpulseSource impulseSource;
    private bool warned;

    private new GameObject light;

    private void Start()
    {
        cam = transform;
        nextRot = transform.rotation;
        player = FindObjectOfType<Player>();
        light = GameObject.FindGameObjectWithTag("Light");

        impulseSource = GetComponent<CinemachineImpulseSource>();

        SetGravity();
    }

    private int GetRotationState()
    {
        var value = nextRot.eulerAngles.z;
        var nearestMultiple = (int)Math.Round(value / 90f, MidpointRounding.AwayFromZero) * 90;
        return nearestMultiple / 90;
    }

    private void SetGravity()
    {
        float g = 34.81f;

        switch (GetRotationState())
        {
            case 0: // 12 o'clock
                Physics.gravity = new Vector3(0, -g, 0);
                break;
            case 1: // 3 o'clock
                Physics.gravity = new Vector3(g, 0, 0);
                break;
            case 2: // 6 o'clock
                Physics.gravity = new Vector3(0, g, 0);
                break;
            case 3: // 9 o'clock
                Physics.gravity = new Vector3(-g, 0, 0);
                break;
        }
    }

    void FixedUpdate()
    {
        if (LevelManager.instance.InLevelCompleteUI()) return;

        tmrRotate += Time.fixedDeltaTime;
        if (tmrRotate >= 9f && !warned)
        {
            impulseSource.GenerateImpulse();
            warned = true;

            GetComponent<AudioSource>().pitch = UnityEngine.Random.Range(0.8f, 1.2f);
            GetComponent<AudioSource>().Play();
        }

        if (tmrRotate >= 10f)
        {
            warned = false;

            float rotSpeed = 90f;

            nextRot = Quaternion.Euler(new Vector3(cam.eulerAngles.x, cam.eulerAngles.y, cam.eulerAngles.z + rotSpeed));
            OnRotate?.Invoke(GetRotationState());
            SetGravity();

            tmrRotate = 0;
        }

        cam.rotation = Quaternion.RotateTowards(cam.rotation, nextRot, 90f * rotationSpeed * Time.fixedDeltaTime);
        player.transform.rotation = Quaternion.RotateTowards(player.transform.rotation, nextRot, 90f * rotationSpeed * Time.fixedDeltaTime);
        light.transform.rotation = Quaternion.RotateTowards(light.transform.rotation, nextRot, 90f * rotationSpeed * Time.fixedDeltaTime);
    }
}
