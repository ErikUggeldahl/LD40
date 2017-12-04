using UnityEngine;

public class FireballFlamesDestructor : MonoBehaviour
{
    [SerializeField]
    public ParticleSystem flames;

    bool destroying = false;

    private void Start()
    {
        destroying = false;
    }

    void Update()
    {
        if (destroying && !flames.IsAlive())
        {
            Destroy(gameObject);
        }
    }

    public void Play()
    {
        flames.Play();
    }

    public void BurnOut()
    {
        transform.parent = null;
        flames.Stop();
        destroying = true;
    }
}
