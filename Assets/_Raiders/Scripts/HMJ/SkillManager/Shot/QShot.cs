using System.Threading;
using UnityEngine;
using UnityEngine.Purchasing;

public class QShot : MonoBehaviour
{
    private void OnParticleCollision(GameObject other)
    {
        Debug.Log(other.name);
        if (other.CompareTag("Enemy") || other.CompareTag("Boss1") || other.CompareTag("Boss2"))
        {
            Debug.Log(other.name);
            Player.Instance.DrainHp();
            Hp enemy = other.GetComponent<Hp>();
            enemy.TakeDamage(Q.CalculatingDamage());
        }

    }

}
