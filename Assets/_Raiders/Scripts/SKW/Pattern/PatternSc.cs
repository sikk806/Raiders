using System;
using System.Collections.Generic;
using System.Threading;
using TMPro;
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
    private List<GameObject> barrierPool = new List<GameObject>(); // 오브젝트 풀 (큐 대신 리스트)
    
    private float dissolveValue = 4.0f; // 기본값 4
    public float dissolveSpeed = 3f; // 초당 감소 속도

    public Hp hp;
    public Hp PlayerHp;

    public BehaviourAI Boss;

    //최대로 쌓을 수 있는 스택
    public float MaxSheidStack = 6;
    //기본 스택
    public float DefaultSheidStack = 2;
    public float CurrentSheidStack;
    
    private Material material;
    private Color colorRed = Color.red;  // 첫 번째 색상 (빨강)
    private Color colorBlue = Color.blue; // 두 번째 색상 (파랑)
    private float timer = 0f;
    private float switchInterval = 5f; // 5초 간격
    
    public float PatternMaxTime = 10f;
    
    bool isCleared = false;
    //소환
    public int spwanCount = 5;
    
    
    void Start()
    {
        // Renderer에서 Material 가져오기
        material = GetComponent<Renderer>().material;
        
        CurrentSheidStack = DefaultSheidStack;
        // 초기 색상 설정
        material.SetColor("_EmissionColor", colorRed);
        
                
        // 초기 오브젝트 풀 생성 (프리팹 리스트에서 랜덤하게 생성)
        for (int i = 0; i < 5; i++) // 풀 크기 조절 가능
        {
            GameObject obj1 = Instantiate(Barrierprefabs[0]); // 첫 번째 배리어
            GameObject obj2 = Instantiate(Barrierprefabs[1]); // 두 번째 배리어
            obj1.SetActive(false);
            obj2.SetActive(false);
            barrierPool.Add(obj1);
            barrierPool.Add(obj2);
        }
 
        
    }

    private void OnEnable()
    {
        hp = GameObject.FindGameObjectWithTag("Boss1").GetComponent<Hp>();
        PlayerHp= GameObject.FindGameObjectWithTag("Player").GetComponent<Hp>();
        Boss = GameObject.FindGameObjectWithTag("Boss1").GetComponent<BehaviourAI>();
        hp.Defence = 0.2f;
    }

    void Update()
    {
        if (!isCleared)
        {
            // 타이머를 현재 프레임 시간만큼 증가
            timer += Time.deltaTime;
            //패턴 남은 시간
            PatternMaxTime -= Time.deltaTime;
            StartPattern();// 패턴을 위한 기믹 시작
            if (hp.Barrier <= 0 && PatternMaxTime > 0)
            {
                //베리어가 0보다 작아지면 죽기
                PatternCleared();
                DeactivateObjectPool();
                Debug.Log("패턴 성공 기믹  if  구문 들어옴");
            }else if (PatternMaxTime < 0)
            {
                Debug.Log("패턴 실패 기믹 else if  구문 들어옴");
                PatternFailed();
                DeactivateObjectPool();
            }
        }
    }
    public void StartPattern()
    {
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

    void PatternCleared()
    {
            dissolveValue -= dissolveSpeed * Time.deltaTime;
            dissolveValue = Mathf.Max(dissolveValue, 0f); // 최소값 0으로 고정
            material.SetFloat("_Dissolve",dissolveValue);
            //베리어 사라지는 로직
            if (dissolveValue <= 0)
            {
                isCleared = true;
                var behaviourAI = GameObject.FindGameObjectWithTag("Boss1").GetComponent<BehaviourAI>();
                hp.Defence = 0f;
                gameObject.SetActive(false);
                behaviourAI.OnAnimationFinished();
            }
            Debug.Log("베리어다깨짐");
    }

    void PatternFailed()
    {
        /*
         * 1.보스 hp를 풀피로 회복 
         * 2.플레이어의 체력을 많은 데미지 주기
         */
        //1번
        
        hp.currentHp = hp.maxHp;
        Debug.Log("보스 피회복");
        //2번
        PlayerHp.currentHp -= 50f;
        Debug.Log("플레이어 데미지 주기");
        Boss.GetComponent<SkillController>().SetfalseAll();
        Boss.StandonAnimation();
        Boss.OnAnimationFinished();
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
                    var barrier = GetRandomBarrier(transform.position);
                    barrier.transform.position = new Vector3(newPosition.x,newPosition.y+1,newPosition.z); // 소환 위치 설정
                    
                    
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
    
    public GameObject GetRandomBarrier(Vector3 position)
    {
        if (barrierPool.Count == 0) return null;

        // 🔥 랜덤하게 하나 선택
        int randomIndex = Random.Range(0, barrierPool.Count);
        GameObject barrier = barrierPool[randomIndex];

        
        barrier.SetActive(true);
        barrier.transform.position = position;
        
        return barrier;
    }
    
    public void DeactivateObjectPool()
    {
        foreach (GameObject barrier in barrierPool)
        {
            barrier.SetActive(false); // 오브젝트 비활성화
        }
        Debug.Log("오브젝트 풀의 모든 오브젝트를 비활성화했습니다.");
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
                // 방어력 증가
                CurrentSheidStack += 1;
                hp.Defence += 0.1f;
                if (hp.Defence >= 0.6f)
                {
                    hp.Defence = 0.6f;
                    Debug.Log("더이상 오르지마");
                }
                
                if (CurrentSheidStack >= MaxSheidStack)
                {
                    CurrentSheidStack = MaxSheidStack;
                    Debug.Log("최대 스택 더이상 안올라감");
                    
                }
                
              
                Debug.Log($"스택 증가! 현재 스택: {CurrentSheidStack}, 방어력: {hp.Defence}");
            }
            else
            {
                // 방어력 감소
                CurrentSheidStack -= 1;
                hp.Defence = Mathf.Max(hp.Defence - 0.1f, 0f); // 방어력 음수 방지
                if (CurrentSheidStack <= 0)
                {
                    CurrentSheidStack = 0;
                    Debug.Log("최소 스택 더이상 안내려감");
                    
                }
                if (hp.Defence <= 0f)
                {
                    hp.Defence = 0f;
                    Debug.Log("더이상 내려가지마");
                }
                Debug.Log($"스택 감소! 현재 스택: {CurrentSheidStack}, 방어력: {hp.Defence}");
            }
        }
    }
}