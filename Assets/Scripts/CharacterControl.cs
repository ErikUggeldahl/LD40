using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterControl : MonoBehaviour
{
    public const float MOVE_FORCE = 15f;

    [SerializeField]
    public float moveForce;

    [SerializeField]
    float jumpForce;

    [SerializeField]
    new Rigidbody rigidbody;

    [SerializeField]
    BoxCollider jumpBox;

    [SerializeField]
    Animator animator;
    int upperBodyLayerIndex;

    [SerializeField]
    ParticleSystem flamethrowerParticles;

    [SerializeField]
    Collider flamethrowerCollider;

    [SerializeField]
    ParticleSystem fireballCast;

    [SerializeField]
    GameObject fireballObj;
    GameObject createdFireball;

    [SerializeField]
    Transform fireballAnchor;

    [SerializeField]
    WildnessController wildnessController;

    [SerializeField]
    AudioSource audio;

    [SerializeField]
    AudioClip jumpClip;

    [SerializeField]
    AudioClip flamethrowingClip;

    [SerializeField]
    AudioClip throwFireballClip;

    int groundContacts = 0;

    const float FLAMETHROWER_WILDNESS_PER_SECOND = 2f;
    const float FLAMETHROWER_MIN_SCALE = 1f;
    const float FLAMETHROWER_MAX_SCALE = 4f;

    const float FIREBALL_WILDNESS_PER_THROW = 4f;
    const float FIREBALL_MIN_SCALE = 0.25f;
    const float FIREBALL_MAX_SCALE = 1.75f;

    public bool ReverseControls { get; set; }

    enum State
    {
        Free,
        Locked,
        Flamethrowing,
        Fireballing,
    }
    State state = State.Free;

    void Start()
    {
        upperBodyLayerIndex = animator.GetLayerIndex("UpperBody");
        flamethrowerCollider.enabled = false;
        ReverseControls = false;
    }

    void Update()
    {
        UpdateState();

        if (state == State.Free)
        {
            var reverseModifier = ReverseControls ? -1f : 1f;
            var horizontal = Input.GetAxis("Horizontal") * reverseModifier;
            var vertical = Input.GetAxis("Vertical") * reverseModifier;
            var movement = (transform.forward * vertical + transform.right * horizontal).normalized * Time.deltaTime * moveForce;

            if (groundContacts == 0)
            {
                movement *= 0.25f;
            }

            rigidbody.AddForce(movement, ForceMode.Impulse);

            if (Input.GetKeyDown(KeyCode.Space) && groundContacts > 0)
            {
                rigidbody.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
                audio.PlayOneShot(jumpClip);
            }
        }

        if (Input.GetMouseButton(1))
        {
            float rotation = Input.GetAxis("Mouse X");
            transform.Rotate(Vector3.up, rotation);
        }

        if (Input.GetMouseButtonDown(1))
        {
            Cursor.lockState = CursorLockMode.Locked;
        }
        else if (Input.GetMouseButtonUp(1))
        {
            Cursor.lockState = CursorLockMode.None;
        }

        var forwardVelocity = transform.InverseTransformDirection(rigidbody.velocity).z;
        animator.SetFloat("ForwardVelocity", forwardVelocity);

        if (state == State.Flamethrowing)
        {
            wildnessController.AddWildness(FLAMETHROWER_WILDNESS_PER_SECOND * Time.deltaTime);
            flamethrowerParticles.transform.localScale = Vector3.one * Mathf.Lerp(FLAMETHROWER_MIN_SCALE, FLAMETHROWER_MAX_SCALE, wildnessController.Wildness / 100f);
        }
    }

    void UpdateState()
    {
        if (state == State.Free && Input.GetKeyDown(KeyCode.LeftShift))
        {
            state = State.Locked;
        }
        else if (state == State.Locked && Input.GetKeyUp(KeyCode.LeftShift))
        {
            state = State.Free;
        }

        if (state == State.Locked && Input.GetKey(KeyCode.Space))
        {
            if (Input.GetKey(KeyCode.W))
            {
                state = State.Flamethrowing;
                EnterFlamethrowing();
            }
            else if (Input.GetKey(KeyCode.S))
            {
                state = State.Fireballing;
                EnterFireballing();
            }
        }
        else if (state == State.Flamethrowing && !(Input.GetKey(KeyCode.Space) && Input.GetKey(KeyCode.W)))
        {
            state = Input.GetKey(KeyCode.LeftShift) ? State.Locked : State.Free;
            ExitFlamethrowing();            
        }
        else if (state == State.Fireballing && !(Input.GetKey(KeyCode.Space) && Input.GetKey(KeyCode.S)))
        {
            state = Input.GetKey(KeyCode.LeftShift) ? State.Locked : State.Free;
            ExitFireballing();
        }
    }

    void EnterFlamethrowing()
    {
        animator.SetLayerWeight(upperBodyLayerIndex, 1f);
        animator.SetBool("Flamethrowing", true);
        flamethrowerParticles.Play();
        flamethrowerCollider.enabled = true;

        audio.clip = flamethrowingClip;
        audio.Play();
    }
    void ExitFlamethrowing()
    {
        animator.SetLayerWeight(upperBodyLayerIndex, 0f);
        animator.SetBool("Flamethrowing", false);
        flamethrowerParticles.Stop();
        flamethrowerCollider.enabled = false;

        audio.Stop();
    }

    void EnterFireballing()
    {
        animator.SetLayerWeight(upperBodyLayerIndex, 1f);
        animator.SetBool("Fireballing", true);
        fireballCast.Play();
    }

    void ExitFireballing()
    {
        animator.SetLayerWeight(upperBodyLayerIndex, 0f);
        animator.SetBool("Fireballing", false);
        fireballCast.Stop();

        if (createdFireball != null)
        {
            Destroy(createdFireball);
            createdFireball = null;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        groundContacts++;
    }

    void OnTriggerExit(Collider other)
    {
        groundContacts--;
    }

    public void CreateFireball()
    {
        createdFireball = GameObject.Instantiate(fireballObj, fireballAnchor.position, Quaternion.identity, fireballAnchor);
        createdFireball.transform.localScale = Vector3.one * Mathf.Lerp(FIREBALL_MIN_SCALE, FIREBALL_MAX_SCALE, wildnessController.Wildness / 100f);
    }

    public void ReleaseFireball()
    {
        if (!createdFireball)
            return;

        createdFireball.GetComponent<FireballControl>().GoLive((transform.forward + transform.up * 0.5f).normalized);

        createdFireball = null;

        wildnessController.AddWildness(FIREBALL_WILDNESS_PER_THROW);

        audio.PlayOneShot(throwFireballClip);
    }
}
