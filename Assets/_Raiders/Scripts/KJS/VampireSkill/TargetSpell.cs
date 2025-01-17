using System.Collections;
using UnityEngine;

public class TargetSpell : MonoBehaviour
{
    [SerializeField] float destroyTime; // default : 6 seconds
    [SerializeField] float damage;
    [SerializeField] float damageTerm;

    float currentTime;
    bool canHit;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    private void OnEnable()
    {
        currentTime = 0;
        GetComponent<CapsuleCollider>().enabled = false;
        StartCoroutine("TakeDamage");
    }

    // Update is called once per frame
    void Update()
    {
        currentTime += Time.deltaTime;
        if(currentTime > destroyTime)
        {
            Deactive();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            if (canHit)
            {
                // 데미지 주는 코드 추가해야함.
                Debug.Log("Damage");
                canHit = false;
                GetComponent<CapsuleCollider>().enabled = false;
            }
        }
    }

    IEnumerator TakeDamage()
    {
        yield return new WaitForSeconds(3f);
        while (true)
        {
            canHit = true;
            GetComponent<CapsuleCollider>().enabled = true;
            yield return new WaitForSeconds(damageTerm);
        }
    }

    void Deactive()
    {
        // GameManager에서 ObjectPool의 bool 값을 false로 만드는 코드 넣어야함.
        StopAllCoroutines();
        canHit = false;
        GetComponent<CapsuleCollider>().enabled = false;
        gameObject.SetActive(false);
    }
}
