using UnityEngine;

public class FireballExplosionDestructor : MonoBehaviour
{
    [SerializeField]
    public ParticleSystem explosion;

    private void Update()
    {
        if (!explosion.IsAlive(true))
        {
            Destroy(gameObject);
        }
    }
}
