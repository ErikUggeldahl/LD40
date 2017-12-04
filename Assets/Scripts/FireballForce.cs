using UnityEngine;

public class FireballForce : MonoBehaviour
{
    int characterLayer;
    int meepleLayer;

    bool hitEnemy = false;

    private void Start()
    {
        characterLayer = LayerMask.NameToLayer("Character");
        meepleLayer = LayerMask.NameToLayer("Meeple");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == characterLayer && other.gameObject.tag != "Player")
        {
            if (!hitEnemy)
            {
                other.GetComponentInParent<Health>().TakeDamage(30f);
                hitEnemy = true;
            }
        }
        else if (other.gameObject.layer == meepleLayer)
        {
            other.GetComponentInParent<MeepleDestroyer>().Destroy();
        }

        var rigidbody = other.GetComponentInChildren<Rigidbody>();
        if (rigidbody == null)
        {
            rigidbody = other.transform.parent.GetComponentInChildren<Rigidbody>();
        }
        if (rigidbody != null)
        {
            rigidbody.AddExplosionForce(30f * transform.lossyScale.x, transform.position, 8f, 0.1f, ForceMode.Impulse);
        }
    }
}
