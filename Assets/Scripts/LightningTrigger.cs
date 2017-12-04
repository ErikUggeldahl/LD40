using UnityEngine;

public class LightningTrigger : MonoBehaviour
{
    int characterLayer;
    int meepleLayer;

    private void Start()
    {
        characterLayer = LayerMask.NameToLayer("Character");
        meepleLayer = LayerMask.NameToLayer("Meeple");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == characterLayer)
        {
            other.GetComponentInParent<Health>().TakeDamage(20f);
        }
        else if (other.gameObject.layer == meepleLayer)
        {
            other.GetComponentInParent<MeepleDestroyer>().Destroy();
        }
    }
}
