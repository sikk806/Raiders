using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using Random = UnityEngine.Random;


public class PatternSc : MonoBehaviour
{
    public enum BarrierType
    {
        Red,
        Blue,
    }
    public BarrierType barrierType = BarrierType.Red;
    //테스트용 베리어 프리펩 
    public List<GameObject> Barrierprefabs;

    public Hp hp;

    //최대로 쌓을 수 있는 스택
    public ulong MaxSheidStack = 6;
    //기본 스택
    public ulong DefaultSheidStack = 2;
    public ulong CurrentSheidStack;
    
    private Material material;
    private Color colorRed = Color.red;  // 첫 번째 색상 (빨강)
    private Color colorBlue = Color.blue; // 두 번째 색상 (파랑)
    private float timer = 0f;
    private float switchInterval = 5f; // 5초 간격
    
    public float PatternMaxTime = 60f;
    
    //소환
    public int spwanCount = 5;
    
    void Start()
    {
        // Renderer에서 Material 가져오기
        material = GetComponent<Renderer>().material;

        // 초기 색상 설정
        material.SetColor("_EmissionColor", colorRed);
        hp = GetComponent<Hp>();
        CurrentSheidStack = DefaultSheidStack;
    }

    void Update()
    {
        // 타이머를 현재 프레임 시간만큼 증가
        timer += Time.deltaTime;
        //패턴 남은 시간
        PatternMaxTime -= Time.deltaTime;
        
        
        // 5초가 지나면 색상 전환 + 구체 생성
        if (timer >= switchInterval)
        {
            // 색상 전환
            if (material.GetColor("_EmissionColor") == colorRed)
            {
                material.SetColor("_EmissionColor", colorBlue);
                barrierType = BarrierType.Blue;
            }
            else
            {
                material.SetColor("_EmissionColor", colorRed);
                barrierType = BarrierType.Red;
            }
            
            //구체 소환
            SpawnSphere();

            // 타이머 초기화
            timer = 0f;
        }


    }
    
    //패턴 성공 실패 체크함수
    public bool CheckPatternSucceed()
    {
        //시간초가 다지나 갔을때 
        if (PatternMaxTime < 0)
        {
            return false;
        }
        /*
         * 패턴 파훼에 성공 했을때
         * 예) 베리어의 피가 0 이 되었을때
         */
        // if (Hp < 0 )
        // {
        //     Debug.Log("패턴 성공");
        // }
        
        Debug.Log("두조건에 부합하지않은 버그 발생");
        return false;
    }

    /*
     * 0번지 red
     * 1번지 blue
     */
    public void SpawnSphere()
    {
        int currentCount = 0;
        List<Vector3> spawnedPositions = new List<Vector3>(); // 소환된 위치 저장
        float minDistance = 2f; // 프리팹들 사이의 최소 거리

        while (currentCount < spwanCount)
        {
            // 랜덤한 각도 및 반지름 생성
            float angle = Random.Range(0f, 360f);
            float radians = angle * Mathf.Deg2Rad;
            float radius = Random.Range(7f, 12f);
            float x = radius * Mathf.Cos(radians);
            float y = radius * Mathf.Sin(radians);
            Vector3 newPosition = new Vector3(x, 1, y);

            // 거리 조건 확인
            bool isValidPosition = true;
            foreach (var position in spawnedPositions)
            {
                if (Vector3.Distance(newPosition, position) < minDistance)
                {
                    isValidPosition = false;
                    break;
                }
            }
            if (isValidPosition)
            {
                // 소환 전 prefab이 null인지 체크
                if (Barrierprefabs != null)
                {
                    GameObject barrier = Instantiate(Barrierprefabs[Random.Range(0,Barrierprefabs.Count)], transform.position, Quaternion.identity);
                    barrier.transform.position = newPosition; // 소환 위치 설정
                    spawnedPositions.Add(newPosition); // 소환된 위치 저장
                    currentCount++;
                }
                else
                {
                    Debug.LogWarning("Barrierprefabs가 설정되지 않았습니다.");
                    break;
                }
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Sphere"))
        {
            // 구체의 indicator 값 확인
            var indicator = other.GetComponent<SphereSc>().indicator;

            // 같은 색인지 확인
            if ((int)barrierType == indicator)
            {
                if (CurrentSheidStack > MaxSheidStack)
                {
                    CurrentSheidStack = MaxSheidStack;
                    Debug.Log("최대 스택 더이상 안올라감");
                    return;
                }
                // 방어력 증가
                CurrentSheidStack += 1;
                hp.Defence += 0.1f;
                Debug.Log($"스택 증가! 현재 스택: {CurrentSheidStack}, 방어력: {hp.Defence}");
            }
            else
            {
                // 방어력 감소
                CurrentSheidStack -= 1;
                hp.Defence = Mathf.Max(hp.Defence - 0.1f, 0f); // 방어력 음수 방지
                Debug.Log($"스택 감소! 현재 스택: {CurrentSheidStack}, 방어력: {hp.Defence}");
            }
        }
    }
}