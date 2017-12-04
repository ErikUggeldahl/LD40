using System.Collections;
using UnityEngine;

public class Storm : MonoBehaviour
{
    [SerializeField]
    GameObject lightning;

    public WildnessController wildness;

    void Start()
    {
        StartCoroutine(RunStorm());
    }

    IEnumerator RunStorm()
    {
        var numberOfStrikes = (int)(wildness.Wildness / 5f);
        for (int i = 0; i < numberOfStrikes; i++)
        {
            CreateLightningStrike();
            yield return new WaitForSeconds(Random.Range(0.5f, 3f));
        }
        Destroy(gameObject);
    }

    void CreateLightningStrike()
    {
        var location = Random.insideUnitCircle * 55f;
        Instantiate(lightning, new Vector3(location.x, 0f, location.y), Quaternion.identity);
    }
}
