using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WShot : MonoBehaviour
{
    private BoxCollider collider;
    private List<Hp> enemiesInRange = new List<Hp>(); // 범위 내 몬스터 목록
    Vector3 vector3 = new Vector3(0, 2.5f, 1);
    private void Start()
    {
        collider = GetComponent<BoxCollider>();
    }
    
    private void OnTriggerEnter(Collider other)
    {

        if (other.CompareTag("Enemy")||other.CompareTag("Boss1")||other.CompareTag("Boss2"))
        {

            Hp enemy = other.GetComponent<Hp>();
            if (enemy != null && !enemiesInRange.Contains(enemy))
            {
                enemiesInRange.Add(enemy);
                StartCoroutine(DamageOverTime(enemy));
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Enemy")||other.CompareTag("Boss1")||other.CompareTag("Boss2"))
        {
 
            Hp enemy = other.GetComponent<Hp>();
            if (enemy != null && enemiesInRange.Contains(enemy))
            {
                enemiesInRange.Remove(enemy);
            }
        }
    }
    private IEnumerator DamageOverTime(Hp enemy)
    {
        while (enemiesInRange.Contains(enemy))
        {
            Player.Instance.DrainHp();
            enemy.TakeDamage(W.CalculatingDamage()); // 데미지 적용
            Damage.Instance.ShowDamage((int)W.CalculatingDamage(), enemy.transform.position+vector3);
            yield return new WaitForSeconds(1f); // 지정된 간격으로 데미지
        }
    }
}