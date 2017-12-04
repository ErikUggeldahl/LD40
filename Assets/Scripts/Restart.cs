using UnityEngine;
using UnityEngine.SceneManagement;

public class Restart : MonoBehaviour
{
    void Start()
    {
        Invoke("GoToArena", 5f);
    }

    void GoToArena()
    {
        SceneManager.LoadScene(1);
    }
}
