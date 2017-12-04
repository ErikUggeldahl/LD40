using UnityEngine;

public class FireballEventReceiver : MonoBehaviour
{
    [SerializeField]
    CharacterControl control;

    void CreateFireball()
    {
        control.CreateFireball();
    }

    void ReleaseFireball()
    {
        if (control != null)
            control.ReleaseFireball();
    }
}
