using UnityEngine;

public class IceLanceProjectile : MonoBehaviour
{
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
        if (collision.gameObject.tag == "Player")
        {
            GetComponent<AudioSource>().Play();
            collision.gameObject.GetComponent<Health>().TakeDamage(10f);
        }
    }
}
