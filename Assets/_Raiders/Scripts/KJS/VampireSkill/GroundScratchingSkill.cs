using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundScratchingSkill : MonoBehaviour
{
    [SerializeField] GameObject hitPrefab;
    [SerializeField] float destroyTime; // default 2 seconds;
    [SerializeField] float damage;
    [SerializeField] float movementSpeed;
    [SerializeField] float damageTerm;

    float currentTime;
    bool canHit;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    private void OnEnable()
    {
        GetComponent<BoxCollider>().enabled = true;
        currentTime = 0;
        canHit = true;
        StartCoroutine("TakeDamage");
    }

    // Update is called once per frame
    void Update()
    {
        currentTime += Time.deltaTime;
        if (currentTime > destroyTime)
        {
            Deactive();
        }
        transform.Translate(transform.forward * movementSpeed * Time.deltaTime, Space.World);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            if (canHit)
            {
                // 데미지 주는 코드 추가해야함.
                canHit = false;
                GetComponent<BoxCollider>().enabled = false;
            }
        }
    }

    IEnumerator TakeDamage()
    {
        while (true)
        {
            canHit = true;
            GetComponent<BoxCollider>().enabled = true;
            yield return new WaitForSeconds(damageTerm);
        }
    }

    void Deactive()
    {
        // GameManager에서 ObjectPool의 bool 값을 false로 만드는 코드 넣어야함.
        StopAllCoroutines();
        GetComponent<BoxCollider>().enabled = true;
        gameObject.SetActive(false);
    }
}
