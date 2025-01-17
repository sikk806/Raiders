using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Pattern : MonoBehaviour
{
    // 패턴 총시간
    public float PatternTime = 60f;
    [SerializeField] public float remainPatternTime;

    public float currenttime = 2;
    // 시작 위치
    public Transform StartPoint;

    // 기둥 프리팹 리스트 및 소환된 기둥 리스트
    public List<GameObject> StatuePrefabs;
    public List<GameObject> SpwanStatuePrefabs;

    // 패턴 성공이 될 기둥
    // 각 기둥과 베리어 identicator로 확인
    public int PatternClearStatueidenticator = -1;

    // 기둥 소환 위치
    public List<Transform> StatuePosition;

    // 랜덤 베리어 리스트
    public List<GameObject> BarrierList;

    //선택된 베리어
    public GameObject BarrierPrefab;

    //패턴이 진행중이니?
    public bool isPatternRunning;
    public bool isPatternEnd;

    public void UpdatePattern()
    {
        if (isPatternRunning) remainPatternTime -= Time.deltaTime;

    }

    // 패턴 실패 조건 체크
    private void PatternFailure()
    {
        // 패턴 실패 조건
        if (remainPatternTime <= 0)
        {
            Debug.Log("패턴 실패!");
            EndPattern();
        }
    }

    public void StartPattern()
    {
        isPatternRunning = true;

        // 0번: 패턴 시간 초기화
        remainPatternTime = PatternTime;

        // 1번: 시작 위치로 이동
        transform.position = StartPoint.position;

        // 2번: 베리어 생성 및 생성된 베리어선언
        var SelectBarrier = Instantiate(BarrierList[Random.Range(0, BarrierList.Count)], transform.position,
            Quaternion.identity);
        BarrierPrefab = SelectBarrier;

        // 3번: 기둥 소환 
        for (var i = 0; i < StatuePrefabs.Count; i++)
        {
            var statue = Instantiate(StatuePrefabs[i], StatuePosition[i].position, Quaternion.identity);
            statue.transform.LookAt(transform.position);
            SpwanStatuePrefabs.Add(statue); // 소환된 기둥 리스트에 추가
        }

        //3-1 소환한 기둥 중 패턴 클리어 기둥 선언
        PatternClearStatueidenticator = CheckBarrierEqualsStatue(BarrierPrefab, SpwanStatuePrefabs);

        Debug.Log("패턴 시작!");
    }

    private int CheckBarrierEqualsStatue(GameObject barrier, List<GameObject> statue)
    {
        for (var i = 0; i < statue.Count; i++)
            if (barrier.GetComponent<Identicator>().identicator == statue[i].GetComponent<Identicator>().identicator)
            {
                Debug.Log(statue[i].GetComponent<Identicator>().identicator);
                return statue[i].GetComponent<Identicator>().identicator;
            }

        Debug.Log("없음");
        return -1;
    }


    //하나만 켜져있 을때  그리고 베리어 색과 같아질때 
    public bool CheckPatternClearCondition()
    {
        // 남아 있는 기둥 리스트 갱신
        // SpwanStatuePrefabs.RemoveAll(statue => statue == null);

        // var isPatternSuccess = SpwanStatuePrefabs[0].GetComponent<Identicator>().identicator == PatternClearStatueidenticator;
        for (var i = 0; i < SpwanStatuePrefabs.Count; i++)
        {
            var isPatternStatue = i == PatternClearStatueidenticator;

            // 패턴 성공을 확인하는 기둥의 경우
            if (isPatternStatue)
            {
                //true일때 진입 
                //활성화가 되어있지않은가?
                if (!SpwanStatuePrefabs[i].activeSelf)
                {
                    return false; // 조건 만족하지 않음
                }
            }
            else
            {
                // 다른 기둥이 활성화되어 있으면 실패
                if (SpwanStatuePrefabs[i].activeSelf)
                {
                    return false;
                }
            }
        }
        return true;
    }
    
    public void EndPattern()
    {
        isPatternRunning = false;
        isPatternEnd = true;
        
        // 소환된 기둥 정리 
        
        foreach (var statue in SpwanStatuePrefabs) statue.SetActive(false);

        BarrierPrefab.SetActive(false);
        SpwanStatuePrefabs.Clear();
    }

   
}