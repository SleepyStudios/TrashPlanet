using UnityEngine;

public enum Side
{
    LEFT = 0,
    RIGHT = 1
}

public class PlayerSide : MonoBehaviour
{
    public bool touchingWall;
    public Side side;

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Wall"))
        {
            touchingWall = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Wall"))
        {
            touchingWall = false;
        }
    }
}
