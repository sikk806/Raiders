using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class EnragePattern : MonoBehaviour
{
    [SerializeField] float impactTime;
    [SerializeField] float damage;
    public float Damage { get { return damage; } set { damage = value; }}

    [SerializeField] GameObject impact;
    [SerializeField] GameObject decal;

    public GameObject Beams;
    public GameObject Shelter;
    public bool IsSafe;

    float currentTime;
    float decalValue;
    bool doOnce;
    bool decalOn;

    Material decalMaterial;

    List<GameObject> beams1;
    List<GameObject> beams2;
    List<GameObject> beams3;
    List<GameObject> beams4;
    List<GameObject> beams5;
    List<GameObject> beams6;

    List<GameObject> Shelters;


    private void Start()
    {
        currentTime = 0f;
        decalValue = 0f;

        GetComponent<SphereCollider>().enabled = false;

        beams1 = new List<GameObject>();
        beams2 = new List<GameObject>();
        beams3 = new List<GameObject>();
        beams4 = new List<GameObject>();
        beams5 = new List<GameObject>();
        beams6 = new List<GameObject>();


        for (int i = 0; i < 6; i++)
        {
            float angle = 7.5f * i;

            for (int j = 0; j < 8; j++)
            {
                GameObject ob = Instantiate(Beams, transform);
                angle += 45;

                Quaternion rotation = Quaternion.Euler(0, angle, 0);
                ob.transform.rotation = rotation;

                ob.SetActive(false);
                if (i == 0) beams1.Add(ob);
                else if (i == 1) beams2.Add(ob);
                else if (i == 2) beams3.Add(ob);
                else if (i == 3) beams4.Add(ob);
                else if (i == 4) beams5.Add(ob);
                else if (i == 5) beams6.Add(ob);
            }
        }


        var projector = decal.GetComponent<DecalProjector>();
        decalMaterial = new Material(projector.material);
        projector.material = decalMaterial;
        decalMaterial.SetFloat("_DecalRad", 0.01f);

        impact.GetComponent<SphereCollider>().enabled = false;
        doOnce = false;
        decalOn = false;
        IsSafe = false;

        impact.GetComponent<SphereCollider>().enabled = false;
        impact.SetActive(false);

        for (int i = 0; i < 8; i++)
        {
            GameObject ob = Instantiate(Shelter, transform);
            float angle = (45 * i) + 22.5f;

            Quaternion rotation = Quaternion.Euler(0, angle, 0);
            ob.transform.rotation = rotation;
            ob.transform.position = transform.position - ob.transform.forward * 12f;
            //ob.SetActive(false);
        }
    }

    void OnEnable()
    {
        StartCoroutine("LaserBeam");
        StartCoroutine("Impact");
        StartCoroutine("DamageOn");
    }

    void Update()
    {
        currentTime += Time.deltaTime;
        if (decalOn)
        {
            decalValue = Mathf.Lerp(0.01f, 0.5f, currentTime);
            decalValue = Mathf.Clamp(decalValue, 0.01f, 0.5f);
            decalMaterial.SetFloat("_DecalRad", decalValue);
        }
    }

    IEnumerator DamageOn()
    {
        yield return new WaitForSeconds(3f);
        GetComponent<SphereCollider>().enabled = true;
    }

    // 3 + 4n cycle
    IEnumerator LaserBeam()
    {
        int cnt = 0;

        yield return new WaitForSeconds(2f);
        while (cnt < 16)
        {
            yield return new WaitForSeconds(1f);
            if (cnt % 6 == 0)
            {
                foreach (GameObject go in beams1)
                {
                    go.SetActive(true);
                }
            }
            else if (cnt % 6 == 1)
            {
                foreach (GameObject go in beams2)
                {
                    go.SetActive(true);
                }
            }
            else if (cnt % 6 == 2)
            {
                foreach (GameObject go in beams3)
                {
                    go.SetActive(true);
                }
            }
            else if (cnt % 6 == 3)
            {
                foreach (GameObject go in beams4)
                {
                    go.SetActive(true);
                }
            }
            else if (cnt % 6 == 4)
            {
                foreach (GameObject go in beams5)
                {
                    go.SetActive(true);
                }
            }
            else if (cnt % 6 == 5)
            {
                foreach (GameObject go in beams6)
                {
                    go.SetActive(true);
                }
            }
            cnt++;
        }
    }

    // 3 + 5n Cycle
    IEnumerator Impact()
    {
        yield return new WaitForSeconds(3f);

        int cnt = 0;
        while (cnt < 4)
        {
            decal.SetActive(true);
            currentTime = 0;
            decalOn = true;

            // 1초 동안 데칼 표시
            yield return new WaitForSeconds(1f);

            decalOn = false;
            // decal이 비활성화 되면
            decalMaterial.SetFloat("_DecalRad", 0.01f);
            decal.SetActive(false);
            impact.SetActive(true);
            if (!IsSafe)
            {
                // 데미지가 들어오고
                impact.GetComponent<SphereCollider>().enabled = true;
            }
            else
            {
                impact.GetComponent<SphereCollider>().enabled = false;
            }
            yield return new WaitForSeconds(1f);

            // 1초 후에 데미지가 꺼지고
            impact.SetActive(false);
            yield return new WaitForSeconds(3f);

            cnt++;
        }
        decalOn = false;
        gameObject.SetActive(false);
    }

    private void OnTriggerStay(Collider other) {
        if(other.gameObject.tag == "Player")
        {
            other.GetComponent<Hp>().TakeDamage(damage);
        }
    }
}
