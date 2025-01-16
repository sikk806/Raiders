using UnityEngine;

public class Explosion : MonoBehaviour
{
    [SerializeField] float destroyTime; // default 2 seconds;
    [SerializeField] float damage;

    float currentTime;

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
            // 데미지 주는 코드 추가해야함. 
            
            Debug.Log("Damage");
        }
    }

    void Deactive()
    {
        // GameManager에서 ObjectPool의 bool 값을 false로 만드는 코드 넣어야함.
        StopAllCoroutines();
        gameObject.SetActive(false);
    }
}
