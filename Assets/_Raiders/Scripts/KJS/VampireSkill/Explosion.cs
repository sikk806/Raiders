using UnityEngine;

public class Explosion : MonoBehaviour
{
    [SerializeField] float destroyTime; // default 2 seconds; 스킬이 끝나는 시간
    [SerializeField] float damage; // 데미지

    float currentTime; // 스킬 실행시간 체크

    private void OnEnable()
    {
        currentTime = 0;
    }

    void Update()
    {
        currentTime += Time.deltaTime;
        if (currentTime > destroyTime)
        {
            Deactive();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            other.GetComponent<Hp>().TakeDamage(damage);
        }
    }

    void Deactive()
    {
        // GameManager에서 ObjectPool의 bool 값을 false로 만드는 코드 넣어야함.
        StopAllCoroutines();
        gameObject.SetActive(false);
    }
}
