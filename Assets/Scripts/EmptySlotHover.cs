using UnityEngine;
using UnityEngine.EventSystems;

public class EmptySlotHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{

    public bool isHovering = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        Debug.Log("Pointer entered button!");
        isHovering = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Debug.Log("Pointer exited button!");
        isHovering = false;
    }
}
