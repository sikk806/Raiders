using UnityEngine;

public class QShot : MonoBehaviour
{
    Vector3 vector3 = new Vector3(0,2.5f,1);
    private void OnParticleCollision(GameObject other)
    {
        Debug.Log(other.name);
        if (other.CompareTag("Enemy") || other.CompareTag("Boss1") || other.CompareTag("Boss2"))
        {
            Player.Instance.DrainHp();
            Hp enemy = other.GetComponent<Hp>();
            enemy.TakeDamage(Q.CalculatingDamage());
            Damage.Instance.ShowDamage((int)Q.CalculatingDamage(), other.transform.position+vector3);
        }

    }

}
