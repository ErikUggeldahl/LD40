using UnityEngine;

public class RandomizeMeepleCheer : MonoBehaviour
{
    [SerializeField]
    Animator animator;

    void RandomizeCheer()
    {
        animator.SetFloat("Cheer", Random.value);
    }
}
