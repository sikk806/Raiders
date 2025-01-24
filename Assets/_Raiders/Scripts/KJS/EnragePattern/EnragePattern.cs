using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnragePattern : MonoBehaviour
{
    [SerializeField] float impactTime;
    [SerializeField] float damage;
    [SerializeField] GameObject impactParticle;

    [SerializeField] GameObject impact;
    [SerializeField] GameObject decal;

    public GameObject Beams;

    float currentTime;
    List<GameObject> beams1;
    List<GameObject> beams2;
    List<GameObject> beams3;
    List<GameObject> beams4;


    void Start()
    {
        beams1 = new List<GameObject>();
        beams2 = new List<GameObject>();
        beams3 = new List<GameObject>();
        beams4 = new List<GameObject>();

        for (int i = 0; i < 4; i++)
        {
            float angle = 7.5f * i;

            for (int j = 0; j < 12; j++)
            {
                GameObject ob = Instantiate(Beams, transform);
                angle += 30;
                float angleRad = angle * Mathf.Deg2Rad;

                Quaternion rotation = Quaternion.Euler(0, angle, 0);
                ob.transform.rotation = rotation;
                ob.transform.position += ob.transform.forward;

                ob.SetActive(false);
                if(i == 0) beams1.Add(ob);
                else if(i == 1) beams2.Add(ob);
                else if(i == 2) beams3.Add(ob);
                else if(i == 3) beams4.Add(ob);
            }
        }

        StartCoroutine("LaserBeam");
        StartCoroutine("Impact");
    }

    void Update()
    {
        currentTime += Time.deltaTime;
        if (currentTime > 5)
        {

        }
    }

    IEnumerator LaserBeam()
    {
        int cnt = 0;
        while(cnt < 40)
        {
            yield return new WaitForSeconds(1.0f);
            if(cnt % 4 == 0)
            {
                foreach(GameObject go in beams1)
                {
                    go.SetActive(true);
                }
            }
            else if(cnt % 4 == 1)
            {
                foreach(GameObject go in beams2)
                {
                    go.SetActive(true);
                }
            }
            else if(cnt % 4 == 2)
            {
                foreach(GameObject go in beams3)
                {
                    go.SetActive(true);
                }
            }
            else if(cnt % 4 == 3)
            {
                foreach(GameObject go in beams4)
                {
                    go.SetActive(true);
                }
            }
            cnt++;
        }
    }

    IEnumerator Impact()
    {
        yield return new WaitForSeconds(5f);

        int cnt = 0;
        while (cnt < 8)
        {
            impactParticle.SetActive(true);
            yield return new WaitForSeconds(3f);
            impactParticle.SetActive(false);
            cnt++;
        }
    }
}
