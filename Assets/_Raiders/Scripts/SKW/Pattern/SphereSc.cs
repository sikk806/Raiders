using System;
using UnityEngine;

public class SphereSc : MonoBehaviour
{

    public int indicator;
    public GameObject target; // 따라갈 목표
    public float moveSpeed = 2f; // 이동 속도
    public float stoppingDistance = 1f; // 목표와의 최소 거리

    void Update()
    {
        if (target == null)
        {
            Debug.LogWarning("Target이 설정되지 않았습니다!");
            return;
        }

        // 목표와의 거리 계산
        float distance = Vector3.Distance(transform.position, target.transform.position);

        // 목표에 도달하지 않았다면 이동
        if (distance > stoppingDistance)
        {
            // 목표 방향 계산
            Vector3 direction = (target.transform.position - transform.position).normalized;

            // 이동 처리
            transform.position += direction * moveSpeed * Time.deltaTime;

            // 목표를 바라보게 회전 (선택 사항)
            transform.rotation = Quaternion.LookRotation(direction);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag=="Barrier")
        {
            gameObject.SetActive(false);
        }
    }
}
