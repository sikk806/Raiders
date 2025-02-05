using UnityEngine;
using UnityEngine.EventSystems;

public class IconTooltip : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public GameObject tooltipText; // Inspector에서 할당
    
    public void OnPointerEnter(PointerEventData eventData)
    {
        tooltipText.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        tooltipText.SetActive(false);
    }
}
