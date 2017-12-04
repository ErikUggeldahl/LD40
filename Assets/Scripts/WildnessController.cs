using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WildnessController : MonoBehaviour
{
    public float wildness = 0.0f;
    public float Wildness { get { return wildness; } }
    int wildnessLevel = 0;
    float skyboxExposure;

    [SerializeField]
    GameObject[] uiElements;

    [SerializeField]
    TextMesh wildMagicLabel;

    [SerializeField]
    GameObject storm;

    [SerializeField]
    Transform enemy;

    [SerializeField]
    PhysicMaterial normalMaterial;

    [SerializeField]
    PhysicMaterial greaseMaterial;

    [SerializeField]
    GameObject illusionObject;
    List<GameObject> illusions;

    [SerializeField]
    GameObject kingHead;

    List<System.Action> wildMagicTable;

    private void Start()
    {
        wildMagicTable = new List<System.Action>();
        wildMagicTable.Add(WildMagicStorm);
        wildMagicTable.Add(WildMagicInvisibleEnemy);
        wildMagicTable.Add(WildMagicRandomTeleport);
        wildMagicTable.Add(WildMagicReverseControls);
        wildMagicTable.Add(WildMagicGreased);
        wildMagicTable.Add(WildMagicIllusions);
        wildMagicTable.Add(WildMagicGrow);
        wildMagicTable.Add(WildMagicEnemyShrink);
        wildMagicTable.Add(WildMagicHeal);
        wildMagicTable.Add(WildMagicDamageResist);
        wildMagicTable.Add(WildMagicWatchingKing);

        wildnessLevel = (int)(wildness / 20f);

        skyboxExposure = RenderSettings.skybox.GetFloat("_Exposure");
        var skyBoxCopy = new Material(RenderSettings.skybox);
        RenderSettings.skybox = skyBoxCopy;
    }

    public void AddWildness(float addedWildness)
    {
        wildness = Mathf.Clamp(wildness + addedWildness, 0f, 100f);

        if ((int)(wildness / 20f) > wildnessLevel)
        {
            wildnessLevel = (int)(wildness / 20f);
            NewWildness();
        }
        else
        {
            var roll = Random.value * 10f;
            if (roll <= addedWildness)
                CreateWildMagic();
        }
    }

    void CreateWildMagic()
    {
        wildMagicTable[Random.Range(0, wildMagicTable.Count)]();
    }

    void NewWildness()
    {
        var ui = uiElements[wildnessLevel - 1];
        ui.SetActive(true);

        switch (wildnessLevel)
        {
            case 5:
                StartCoroutine(DarkenSkybox());
                break;
        }

        CreateWildMagic();
    }

    void SetLabel(string text)
    {
        wildMagicLabel.text = text;
        StopCoroutine(ClearLabel());
        StartCoroutine(ClearLabel());
    }

    IEnumerator ClearLabel()
    {
        yield return new WaitForSeconds(3f);
        wildMagicLabel.text = string.Empty;
    }

    IEnumerator DarkenSkybox()
    {
        while (skyboxExposure > 0.0f)
        {
            skyboxExposure = Mathf.Clamp(skyboxExposure - 0.25f * Time.deltaTime, 0.0f, skyboxExposure);
            RenderSettings.skybox.SetFloat("_Exposure", skyboxExposure);
            DynamicGI.UpdateEnvironment();
            yield return null;
        }
    }

    void WildMagicStorm()
    {
        SetLabel("Wild Magic: Storm");
        Instantiate(storm);
        storm.GetComponent<Storm>().wildness = this;
    }

    void WildMagicInvisibleEnemy()
    {
        SetLabel("Wild Magic: Invisible Enemy");
        foreach (var renderer in enemy.GetComponentsInChildren<Renderer>())
        {
            renderer.enabled = false;
        }
        Invoke("RevealEnemy", wildness / 10f);
    }

    void RevealEnemy()
    {
        foreach (var renderer in enemy.GetComponentsInChildren<Renderer>())
        {
            renderer.enabled = true;
        }
    }

    void WildMagicRandomTeleport()
    {
        SetLabel("Wild Magic: Teleport Randomly");
        StartCoroutine(TeleportRandomly());
    }

    IEnumerator TeleportRandomly()
    {
        for (int i = 0; i < wildnessLevel; i++)
        {
            var newPosition = Random.insideUnitCircle * 50f;
            transform.position = new Vector3(newPosition.x, 7f, newPosition.y);
            yield return new WaitForSeconds(0.5f);
        }
    }

    void WildMagicReverseControls()
    {
        SetLabel("Wild Magic: Confusion");
        GetComponent<CharacterControl>().ReverseControls = true;
        Invoke("RestoreControls", wildness / 10f);
    }

    void RestoreControls()
    {
        GetComponent<CharacterControl>().ReverseControls = false;
    }

    void WildMagicGreased()
    {
        SetLabel("Wild Magic: Greased self");

        GetComponentInChildren<CapsuleCollider>().material = greaseMaterial;
        GetComponent<CharacterControl>().moveForce = 4f;
        GetComponent<Rigidbody>().drag = 0.25f;

        Invoke("RestoreFriction", wildness / 5f);
    }

    void RestoreFriction()
    {
        GetComponentInChildren<CapsuleCollider>().material = normalMaterial;
        GetComponent<CharacterControl>().moveForce = CharacterControl.MOVE_FORCE;
        GetComponent<Rigidbody>().drag = 1f;
    }

    void WildMagicIllusions()
    {
        SetLabel("Wild Magic: Illusions");

        var illusionCount = (int)(wildness / 5f);
        ClearIllusions();
        illusions = new List<GameObject>(illusionCount);
        for (int i = 0; i < illusionCount; i++)
        {
            var randomXZ = Random.insideUnitSphere * 40f;
            var x = randomXZ.x;
            if (x > 0f && x < 5f) x = 5f;
            else if (x < 0f && x > -5f) x = -5f;
            var z = randomXZ.y;
            if (z > 0f && z < 5f) z = 5f;
            else if (z < 0f && z > -5f) z = -5f;

            var randomLocation = Vector3.Scale(transform.position, new Vector3(1f, 0f, 1f)) + new Vector3(x, 0f, z);

            var illusion = Instantiate(illusionObject, randomLocation, Quaternion.identity);
            illusion.GetComponent<IllusionMove>().player = transform;
            illusions.Add(illusion);
        }

        Invoke("ClearIllusions", wildness / 2f);
    }

    void ClearIllusions()
    {
        if (illusions == null)
            return;

        foreach (var illusion in illusions)
        {
            Destroy(illusion);
        }
        illusions.Clear();
    }

    void WildMagicGrow()
    {
        SetLabel("Wild Magic: Grow");
        var factor = Mathf.Lerp(1.1f, 1.5f, wildness / 100f);
        transform.localScale *= factor;
    }

    void WildMagicEnemyShrink()
    {
        SetLabel("Wild Magic: Enemy Shrink");
        var factor = Mathf.Lerp(0.95f, 0.75f, wildness / 100f);
        enemy.localScale *= factor;
        if (enemy.localScale.x < 0.4f)
        {
            enemy.localScale = Vector3.one * 0.4f;
        }
    }

    void WildMagicHeal()
    {
        SetLabel("Wild Magic: Heal");
        var health = GetComponent<Health>();
        float healing = Mathf.Min(wildness, 100f - health.CurrentHealth);
        GetComponent<Health>().TakeDamage(-healing);
    }

    void WildMagicDamageResist()
    {
        SetLabel("Wild Magic: Damage Resist");
        GetComponent<Health>().resistant = true;
        Invoke("LoseResist", wildness / 2f);
    }

    void LoseResist()
    {
        GetComponent<Health>().resistant = false;
    }

    void WildMagicWatchingKing()
    {
        SetLabel("Wild Magic: The Watching King");
        kingHead.SetActive(true);
        Invoke("DisableKingHead", wildness);
    }

    void DisableKingHead()
    {
        kingHead.SetActive(false);
    }
}
