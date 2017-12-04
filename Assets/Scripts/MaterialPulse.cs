using UnityEngine;

public class MaterialPulse : MonoBehaviour
{
    [SerializeField]
    new Renderer renderer;

    float duration = 0.5f;

    void Update()
    {
        var lerp = Mathf.PingPong(Time.time, duration) / duration;
        var color = Color.Lerp(Color.white, Color.clear, lerp);
        renderer.material.color = color;
    }
}
