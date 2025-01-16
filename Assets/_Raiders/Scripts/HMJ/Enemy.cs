using UnityEngine;

//Temp Script

public class Enemy : MonoBehaviour
{
    public float CurrentHp = 100f;

    public void TakeDamage(float damage)
    {
        CurrentHp -= damage;

        if (CurrentHp <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log("Àû »ç¸Á");
    }
}
