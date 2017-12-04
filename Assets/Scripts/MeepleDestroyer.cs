using UnityEngine;

public class MeepleDestroyer : MonoBehaviour
{
    [SerializeField]
    GameObject[] parts;

    [SerializeField]
    Animator animator;

    public void Destroy()
    {
        Destroy(animator);

        gameObject.layer = 0;

        foreach (var part in parts)
        {
            part.layer = 0;
            part.transform.parent = null;
            part.GetComponent<Rigidbody>().isKinematic = false;
        }

        Destroy(gameObject);
    }
}
