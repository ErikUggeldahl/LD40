using System.Collections;
using UnityEngine;

public class EnemyControl : MonoBehaviour
{
    [SerializeField]
    float moveForce;

    [SerializeField]
    float rockJumpForce;

    [SerializeField]
    new Rigidbody rigidbody;

    [SerializeField]
    Animator animator;
    int upperBodyLayerIndex;

    [SerializeField]
    Transform player;

    [SerializeField]
    GameObject iceLance;

    [SerializeField]
    GameObject iceMeteor;

    [SerializeField]
    Transform castAnchor;

    [SerializeField]
    WildnessController wildnessController;

    [SerializeField]
    AudioSource audio;

    [SerializeField]
    AudioClip jumpClip;

    [SerializeField]
    AudioClip iceLanceClip;

    [SerializeField]
    AudioClip teleportClip;

    [SerializeField]
    AudioClip iceMeteorClip;

    enum Strategy
    {
        StrafeLeft = 0,
        StrafeRight,
        Approach,
        Retreat,
        MaintainDistance,
        Teleport,
        IceMeteor,
        LanceBurst,
    }
    int strategyCount = System.Enum.GetNames(typeof(Strategy)).Length;
    Strategy strategy;

    const float LANCE_FORCE = 10f;
    const float LANCE_BURST_FORCE = 20f;

    const float SHORT_MAINTAIN_DISTANCE = 30f;
    const float LONG_MAINTAIN_DISTANCE = 60f;

    void Start()
    {
        upperBodyLayerIndex = animator.GetLayerIndex("UpperBody");

        strategy = Strategy.MaintainDistance;

        Invoke("UpdateStrategy", 10f);
    }

    void Update()
    {
        var toPlayer = player.position - transform.position;

        switch (strategy)
        {
            case Strategy.StrafeLeft:
                Move(-transform.right); break;
            case Strategy.StrafeRight:
                Move(transform.right); break;
            case Strategy.Approach:
                if (toPlayer.magnitude > 4f)
                {
                    Move(transform.forward);
                }
                else
                {
                    strategy = Strategy.LanceBurst;
                }
                break;
            case Strategy.Retreat:
                Move(-transform.forward); break;
        }

        if (strategy == Strategy.Teleport)
        {
            Teleport();
            strategy = Strategy.MaintainDistance;
        }
        else if (strategy == Strategy.IceMeteor)
        {
            SummonIceMeteor();
            strategy = Strategy.MaintainDistance;
        }
        else if (strategy == Strategy.LanceBurst)
        {
            StartCoroutine(LanceBurst());
            strategy = Strategy.MaintainDistance;
        }
        else if (strategy == Strategy.MaintainDistance)
        {
            var safeDistance = wildnessController.Wildness <= 40f ? SHORT_MAINTAIN_DISTANCE : LONG_MAINTAIN_DISTANCE;

            if (toPlayer.magnitude > safeDistance + 2f)
            {
                Move(transform.forward);
            }
            else if (toPlayer.magnitude < safeDistance - 2f)
            {
                Move(-transform.forward);
            }
        }

        transform.LookAt(player);

        var forwardVelocity = transform.InverseTransformDirection(rigidbody.velocity).z;
        animator.SetFloat("ForwardVelocity", forwardVelocity);
    }

    void UpdateStrategy()
    {
        animator.SetLayerWeight(upperBodyLayerIndex, 0f);

        strategy = (Strategy)Random.Range(0, strategyCount);
        //strategy = Strategy.MaintainDistance;
        //Debug.Log("Strategy: " + strategy);
        Invoke("UpdateStrategy", Random.Range(0.5f, 4f));

        ShootLance();
    }

    private void Move(Vector3 direction)
    {
        var movement = direction * Time.deltaTime * moveForce;
        rigidbody.AddForce(movement, ForceMode.Impulse);
    }

    public void Jump()
    {
        rigidbody.AddForce(Vector3.up * rockJumpForce, ForceMode.Force);

        if (!audio.isPlaying)
            audio.PlayOneShot(jumpClip);
    }

    public void Teleport()
    {
        var newPosition = Random.insideUnitCircle * 50f;
        transform.position = new Vector3(newPosition.x, 7f, newPosition.y);

        audio.PlayOneShot(teleportClip);
    }

    public void HitWall()
    {
        var random = Random.value;
        if (random < 0.4f)
        {
            strategy = Strategy.StrafeLeft;
        }
        else if (random < 0.8f)
        {
            strategy = Strategy.StrafeRight;
        }
        else
        {
            Teleport();
        }
    }

    void ShootLance(float force = LANCE_FORCE)
    {
        var toPlayer = (player.position - transform.position).normalized;
        var lance = Instantiate(iceLance, castAnchor.position, Quaternion.LookRotation(toPlayer, Vector3.up));
        lance.GetComponent<Rigidbody>().AddForce(toPlayer * 10f, ForceMode.Impulse);

        audio.PlayOneShot(iceLanceClip);
    }

    IEnumerator LanceBurst()
    {
        animator.SetLayerWeight(upperBodyLayerIndex, 1f);
        animator.SetBool("Flamethrowing", true);
        for (int i = 0; i < 5; i++)
        {
            yield return new WaitForSeconds(0.2f);
            ShootLance(LANCE_BURST_FORCE);
        }
        animator.SetLayerWeight(upperBodyLayerIndex, 0f);
        animator.SetBool("Flamethrowing", false);
    }

    void SummonIceMeteor()
    {
        animator.SetLayerWeight(upperBodyLayerIndex, 1f);
        animator.SetTrigger("IceMeteor");
        var meteor = Instantiate(iceMeteor, new Vector3(player.position.x, 20f, player.position.z), Random.rotation);
        meteor.GetComponent<Rigidbody>().AddForce(Vector3.down * 15f, ForceMode.VelocityChange);

        audio.PlayOneShot(iceMeteorClip);
    }
}
