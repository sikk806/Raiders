using UnityEngine;
using UnityEngine.UIElements;

public class MoveUpDown : MonoBehaviour
{
    float currentTime;
    
    void Start()
    {
        currentTime = 0;
    }

    void Update()
    {
        currentTime += Time.deltaTime;
        transform.position = transform.position + Vector3.up * Mathf.Sin(currentTime) * Time.deltaTime / 4;
    }
}
