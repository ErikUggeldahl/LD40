using UnityEngine;

public class SelfDestruct : MonoBehaviour
{
    [SerializeField]
    float time;

    void Start()
    {
        Invoke("Destroy", time);
    }

    void Destroy()
    {
        Destroy(gameObject);
    }
}
