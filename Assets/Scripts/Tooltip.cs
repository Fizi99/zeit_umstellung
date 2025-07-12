using UnityEngine;

public class Tooltip : MonoBehaviour
{
    [SerializeField] GameObject tooltipWindow;
    [SerializeField] GameObject tooltipBtn;

    private bool isActive = false;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (this.isActive)
        {
            // if tapped, go to next part of tutorial
            if (Input.GetMouseButtonDown(0))
            {
                CloseTooltip();
            }
        }
    }

    public void OpenTooltip()
    {
        this.tooltipWindow.SetActive(true);
        this.isActive = true;
    }

    public void CloseTooltip()
    {
        this.tooltipWindow.SetActive(false);
        this.isActive = false;
    }
}
