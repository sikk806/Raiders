using TMPro;
using UnityEngine;

public class CountDown : MonoBehaviour
{
    public TMP_Text Timer;
    float currentTime = 5f;
    void Start()
    {
        Timer.text = "5";
    }

    // Update is called once per frame
    void Update()
    {
        currentTime -= Time.deltaTime;
        if(currentTime > 1f)
        {
            Timer.text = currentTime.ToString("F0");
        }
        else if(currentTime <= 1f && currentTime >= 0f)
        {
            Timer.text = currentTime.ToString("F1");
        }
        else if(currentTime < 0f)
        {
            Timer.text = "";
        }

    }
}
