using UnityEngine;

public class IllusionMove : MonoBehaviour
{
    public Transform player;

    enum Direction {
        Forward = 0,
        Reverse,
        Left,
        Right
    }
    Direction direction;

    void Start()
    {
        direction = (Direction)Random.Range(0, 4);
    }

    void Update()
    {
        transform.LookAt(player);

        var distance = (player.position - transform.position).magnitude;
        if (direction == Direction.Forward && distance < 5f)
            direction = Direction.Reverse;
        else if (direction == Direction.Reverse && distance > 80f)
            direction = Direction.Forward;

        Vector3 movement;
        switch (direction)
        {
            case Direction.Forward:
                movement = transform.forward; break;
            case Direction.Reverse:
                movement = -transform.forward; break;
            case Direction.Left:
                movement = -transform.right; break;
            case Direction.Right:
                movement = transform.right; break;
            default:
                movement = Vector3.zero; break;
        }

        movement.Scale(new Vector3(1f, 0f, 1f));

        transform.Translate(movement * 5f * Time.deltaTime, Space.World);
    }
}
