using UnityEngine;

public class EnemyStuckTrigger : MonoBehaviour
{
    [SerializeField]
    EnemyControl control;

    int rocksLayer;
    int wallsLayer;

    private void Start()
    {
        rocksLayer = LayerMask.NameToLayer("Rocks");
        wallsLayer = LayerMask.NameToLayer("Walls");
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.layer == rocksLayer)
        {
            control.Jump();
        }
        else if (other.gameObject.layer == wallsLayer)
        {
            control.HitWall();
        }
    }
}
