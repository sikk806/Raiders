using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class MarbleObjectPool : MonoBehaviour
{
    public GameObject[] Marbles;

    List<GameObject> objs;

    int i = 0;

    void Awake()
    {
        //DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        objs = new List<GameObject>();
        transform.position = Vector3.zero + Vector3.up * 1;
        foreach (var marble in Marbles)
        {
            GameObject ob = Instantiate(marble, transform);
            ob.SetActive(false);
            objs.Add(ob);
        }

        StartCoroutine("SpawnMarble");
    }

    IEnumerator SpawnMarble()
    {
        while (i < 100)
        {
            yield return new WaitForSeconds(10.0f);
            i++;
            int marbleNum = Random.Range(0, 5);
            GameObject ob = objs[marbleNum];
            ob.SetActive(true);
            ob.transform.position = transform.position + new Vector3(Random.Range(-10f, 10f), transform.position.y, Random.Range(-10f, 10f));
        }
    }
}
