using UnityEngine;
using TMPro;

public class Damage : MonoBehaviour
{
    public static Damage Instance;
    public TextMeshPro Prefab; // 3D 공간에서 사용할 텍스트
    public float FloatSpeed = 1f;
    public float DisappearTime = 1f;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    public void ShowDamage(int damage, Vector3 position)
    {
        TextMeshPro damageText = Instantiate(Prefab, position, Quaternion.identity);
        damageText.text = damage.ToString();
        StartCoroutine(FadeOutAndMove(damageText));
    }

    private System.Collections.IEnumerator FadeOutAndMove(TextMeshPro text)
    {
        float elapsedTime = 0f;
        Vector3 startPosition = text.transform.position;

        while (elapsedTime < DisappearTime)
        {
            elapsedTime += Time.deltaTime;
            text.transform.position = startPosition + Vector3.up * (FloatSpeed * Time.deltaTime);
            text.alpha = Mathf.Lerp(1f, 0f, elapsedTime / DisappearTime);

            // 카메라를 바라보도록 설정
            text.transform.LookAt(Camera.main.transform);
            text.transform.Rotate(0, 180f, 0); // 텍스트가 반대로 보이지 않도록 회전

            yield return null;
        }

        Destroy(text.gameObject);
    }
}
