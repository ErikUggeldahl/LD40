using UnityEngine;
using UnityEngine.SceneManagement;

public class Health : MonoBehaviour
{
    [SerializeField]
    Transform healthbar;

    float maxHealth = 100f;
    float health = 100f;
    public float CurrentHealth { get { return health; } }

    public bool resistant = false;

    public void TakeDamage(float damage)
    {
        if (resistant && damage > 0f)
            damage /= 2f;

        health = Mathf.Clamp(health - damage, 0f, maxHealth);
        healthbar.localScale = new Vector3(health / maxHealth, 1f, 1f);

        if (health == 0)
        {
            if (tag == "Player")
            {
                SceneManager.LoadScene(3);
            }
            else
            {
                SceneManager.LoadScene(2);
            }
        }
    }
}
