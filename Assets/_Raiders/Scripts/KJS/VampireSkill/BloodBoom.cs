using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class BloodBoom : MonoBehaviour
{
    [SerializeField] float boomTime; // default 1 seconds;
    [SerializeField] float destroyTime; // default 3 seconds;
    [SerializeField] float damage;

    [SerializeField] GameObject[] particles;
    [SerializeField] GameObject decal;

    float currentTime;
    float decalValue;
    bool doOnce;

    Material decalMaterial;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void OnEnable()
    {
        currentTime = 0f;
        decalValue = 0f;
        decal.SetActive(true);

        var projector = decal.GetComponent<DecalProjector>();
        decalMaterial = new Material(projector.material);
        projector.material = decalMaterial;
        decalMaterial.SetFloat("_DecalRad", decalValue);

        foreach (var particle in particles)
        {
            particle.SetActive(false);
        }

        GetComponent<SphereCollider>().enabled = false;
        doOnce = false;
    }

    // Update is called once per frame
    void Update()
    {
        currentTime += Time.deltaTime;

        if (currentTime < boomTime)
        {
            decalValue = Mathf.Lerp(0f, 0.5f, currentTime);
            decalValue = Math.Clamp(decalValue, 0f, 0.5f);
            decalMaterial.SetFloat("_DecalRad", decalValue);
        }

        if (currentTime > boomTime && currentTime < destroyTime && !doOnce)
        {
            doOnce = true;
            GetComponent<SphereCollider>().enabled = true;

            decal.SetActive(false);
            foreach (var particle in particles)
            {
                particle.SetActive(true);
            }
        }


        if (currentTime > destroyTime)
        {
            Deactive();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            // 데미지 주는 코드 추가해야함. 

            Debug.Log("Damage");
        }
    }

    void Deactive()
    {
        // GameManager에서 ObjectPool의 bool 값을 false로 만드는 코드 넣어야함.
        StopAllCoroutines();

        doOnce = false;
        GetComponent<SphereCollider>().enabled = false;
        foreach (var particle in particles)
        {
            particle.SetActive(false);
        }
        gameObject.SetActive(false);
    }
}
