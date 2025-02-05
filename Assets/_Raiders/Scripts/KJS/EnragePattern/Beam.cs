using System.Collections;
using UnityEditor.Rendering.Universal;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Beam : MonoBehaviour
{
    public float DamagePerTick;

    [SerializeField] GameObject decal;
    [SerializeField] GameObject[] particles;

    Material decalMaterial;
    BoxCollider boxCollider;

    void Start()
    {
        boxCollider = GetComponent<BoxCollider>();
        var projector = decal.GetComponent<DecalProjector>();
        decalMaterial = new Material(projector.material);
        projector.material = decalMaterial;
        decalMaterial.SetFloat("_DecalRad", 1f);
    }

    void OnEnable()
    {
        StartCoroutine("Deactive");
        decalMaterial.SetFloat("_DecalRad", 1f);
        boxCollider.enabled = false;
    }

    void OnDisable()
    {
        //gameObject.SetActive(false);
    }

    IEnumerator Deactive()
    {
        decal.SetActive(true);
        
        yield return new WaitForSeconds(0.5f);
        decal.SetActive(false);
        AudioMixerController.Instance.BossStartClip("Enrage");
        foreach (var particle in particles)
        {
            particle.SetActive(true);
        }
        boxCollider.enabled = true;
        yield return new WaitForSeconds(1.25f);
        foreach (var particle in particles)
        {
            particle.SetActive(false);
        }
        gameObject.SetActive(false);
        boxCollider.enabled = false;
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            other.GetComponent<Hp>().TakeDamage(DamagePerTick);
        }
    }
}
