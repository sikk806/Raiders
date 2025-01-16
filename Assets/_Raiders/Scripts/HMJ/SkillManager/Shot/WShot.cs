using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WShot : MonoBehaviour
{
    private BoxCollider collider;
    private List<Enemy> enemiesInRange = new List<Enemy>(); // ���� �� ���� ���
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
            enemy.TakeDamage(W.Instance.CalculatingDamage); // ������ ����
            yield return new WaitForSeconds(1f); // ������ �������� ������
        }
    }
}