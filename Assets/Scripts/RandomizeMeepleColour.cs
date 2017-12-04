using UnityEngine;

public class RandomizeMeepleColour : MonoBehaviour
{
    [SerializeField]
    Renderer[] skinParts;

    [SerializeField]
    Renderer body;

    [SerializeField]
    Color[] skinColors;

    void Start()
    {
        var bodyBlock = new MaterialPropertyBlock();
        bodyBlock.SetColor("_Color", Random.ColorHSV(0f, 1f, 0.5f, 0.9f, 0.5f, 0.9f));
        body.SetPropertyBlock(bodyBlock);

        var skinBlock = new MaterialPropertyBlock();
        skinBlock.SetColor("_Color", skinColors[Random.Range(0, skinColors.Length)]);
        foreach (var part in skinParts)
        {
            part.SetPropertyBlock(skinBlock);
        }
    }
}
