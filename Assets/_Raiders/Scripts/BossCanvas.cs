using TMPro;
using UnityEngine;

public class BossCanvas : MonoBehaviour
{
    public TextMeshProUGUI TxtDeathCount;
    public TextMeshProUGUI TxtTimer;
    public TextMeshProUGUI TxtStackCount;
    public TextMeshProUGUI PatternTimer;
    public GameObject BuffPanel;
    
    public PatternSc _patternSc;
    public GameObject Barrirer;

    void Start()
    {
        BuffPanel.SetActive(false);
        TxtDeathCount.text = "Death Count: " + GameManager.Instance.DeathCount;
        TxtTimer.text = "Timer: " + GameManager.Instance.PlayTime;
        _patternSc = SkillObjectPools.Instance.GetBarrierObject("Barrier").GetComponent<PatternSc>();
        Barrirer = SkillObjectPools.Instance.GetBarrierObject("Barrier");
    }

    void Update()
    {
        TxtDeathCount.text = "Death Count: " + GameManager.Instance.DeathCount;
        GameManager.Instance.NegaitveGameTime();
        TxtTimer.text = "Timer: " + GameManager.Instance.makeTime();
        
        if (Barrirer.activeSelf)
        {
            ShowBuffPanel();
            
        }
    }

    public void ShowBuffPanel()
    {
        Debug.Log("ShowBuffPanel");
        BuffPanel.SetActive(true);

        if (_patternSc != null)
        {
            PatternTimer.text = Mathf.FloorToInt(_patternSc.PatternMaxTime).ToString();
            UpdateStackCount();
        }
    }

    public void UpdateStackCount()
    {
        if (_patternSc != null)
        {
            TxtStackCount.text = " X " +  _patternSc.CurrentSheidStack;
        }
        else
        {
            Debug.LogWarning("Cannot update stack count: _patternSc is NULL!");
        }
    }

   
}