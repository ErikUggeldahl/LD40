using UnityEngine;

public class FlamethrowerTrigger : MonoBehaviour
{
    int characterLayer;
    int meepleLayer;

    private void Start()
    {
        characterLayer = LayerMask.NameToLayer("Character");
        meepleLayer = LayerMask.NameToLayer("Meeple");
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.layer == characterLayer)
        {
            other.GetComponentInParent<Health>().TakeDamage(0.35f);
        }
        else if (other.gameObject.layer == meepleLayer)
        {
            other.GetComponentInParent<MeepleDestroyer>().Destroy();
        }
    }
}
