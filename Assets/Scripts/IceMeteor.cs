using UnityEngine;

public class IceMeteor : MonoBehaviour
{
    bool hasLanded = false;

    private void Start()
    {
        Invoke("DestroySelf", 10f);
    }

    void DestroySelf()
    {
        Destroy(gameObject);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!hasLanded && collision.gameObject.tag == "Player")
        {
            GetComponent<AudioSource>().Play();
            collision.gameObject.GetComponent<Health>().TakeDamage(40f);
        }
        hasLanded = true;
    }
}
