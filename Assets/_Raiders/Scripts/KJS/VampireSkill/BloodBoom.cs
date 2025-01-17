using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class BloodBoom : MonoBehaviour
{
    [SerializeField] float boomTime; // default 1 seconds; 터지는 시간
    [SerializeField] float destroyTime; // default 3 seconds; // 스킬이 끝나는 시간
    [SerializeField] float damage; // 스킬 데미지

    [SerializeField] GameObject[] particles; // 스킬 파티클 > 파티클 조정 시간을 위해 사용
    [SerializeField] GameObject decal; // 미리 표시하는 데칼

    float currentTime; // 진행 시간 체크
    float decalValue; // 데칼이 점점 커지는 상황에 사용
    bool doOnce; // Collider, Particle Active 한번만 하도록 사용하기 위한 변수

    Material decalMaterial;

    
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
