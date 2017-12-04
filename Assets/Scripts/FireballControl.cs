using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireballControl : MonoBehaviour
{
    [SerializeField]
    FireballFlamesDestructor flames;

    [SerializeField]
    GameObject explosionObj;

    public void GoLive(Vector3 forceDirection)
    {
        foreach (var child in GetComponentsInChildren<Transform>())
        {
            child.localScale = transform.localScale;
        }

        transform.parent = null;

        var rigidbody = GetComponent<Rigidbody>();
        rigidbody.isKinematic = false;
        rigidbody.AddForce(forceDirection * 20f, ForceMode.Impulse);

        var collider = GetComponent<SphereCollider>();
        collider.enabled = true;

        flames.Play();
    }

    private void OnCollisionEnter(Collision collision)
    {
        flames.BurnOut();

        var explosion = Instantiate(explosionObj, transform.position, Quaternion.identity);
        explosion.transform.localScale = transform.localScale;
        foreach (var child in explosion.GetComponentsInChildren<Transform>())
        {
            child.localScale = transform.localScale;
        }

        Destroy(gameObject);
    }
}
