using TMPro;
using UnityEngine;

public class BossCanvas : MonoBehaviour
{
    public TextMeshProUGUI TxtDeathCount;
    public TextMeshProUGUI TxtTimer;
    public TextMeshProUGUI TxtStackCount;
    public GameObject BuffPanel;
    
    public PatternSc _patternSc;

    void Start()
    {
        TxtDeathCount.text = "Death Count: " + GameManager.Instance.DeathCount;
        TxtTimer.text = "Timer: " + GameManager.Instance.PlayTime;
    }

    void Update()
    {
        TxtDeathCount.text = "Death Count: " + GameManager.Instance.DeathCount;
        GameManager.Instance.NegaitveGameTime();
        TxtTimer.text = "Timer: " + GameManager.Instance.makeTime();
        
        if (BuffPanel.activeSelf)
        {
            _patternSc = SkillObjectPools.Instance.GetBarrierObject("Barrier").GetComponent<PatternSc>();
            ShowBuffPanel();
        }
        
    }

    public void ShowBuffPanel()
    {
        Debug.Log("ShowBuffPanel");
        BuffPanel.SetActive(true);

        if (_patternSc != null)
        {
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