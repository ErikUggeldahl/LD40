using UnityEngine;

public class FaceLook : MonoBehaviour
{
    public Transform player;

    void Update()
    {
        transform.LookAt(player.position);
    }
}
