using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RShot : MonoBehaviour
{
    private BoxCollider collider;
    private List<Enemy> enemiesInRange = new List<Enemy>(); // 범위 내 몬스터 목록
   
    private void Start()
    {
        collider = GetComponent<BoxCollider>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            Enemy enemy = other.GetComponent<Enemy>();
            if (enemy != null && !enemiesInRange.Contains(enemy))
            {
                enemiesInRange.Add(enemy);
                StartCoroutine(DamageOverTime(enemy));
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // 몬스터가 범위를 나갈 때
        if (other.CompareTag("Enemy"))
        {
            Enemy enemy = other.GetComponent<Enemy>();
            if (enemy != null && enemiesInRange.Contains(enemy))
            {
                enemiesInRange.Remove(enemy);
            }
        }
    }

    private IEnumerator DamageOverTime(Enemy enemy)
    {
        while (enemiesInRange.Contains(enemy))
        {
            enemy.TakeDamage(R.Instance.CalculatingDamage); // 데미지 적용
            yield return new WaitForSeconds(1f); // 지정된 간격으로 데미지
        }
    }
}
