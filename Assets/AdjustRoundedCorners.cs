using UnityEngine;
using TMPro;
using Nobi.UiRoundedCorners;

public class AdjustRoundedCorners : MonoBehaviour
{
    [SerializeField] private TMP_Dropdown dropdown;
    [SerializeField] private ImageWithIndependentRoundedCorners target;
    [SerializeField] private Vector4 cornersOpen = new Vector4(40, 40, 0, 0);
    [SerializeField] private Vector4 cornersClosed = new Vector4(40, 40, 40, 40);


    private GameObject templateGO;
    private bool wasOpen = false;

    void Start()
    {
        if (dropdown != null)
            templateGO = dropdown.template.gameObject;
    }

    void Update()
    {
        if (dropdown == null || target == null) return;

        // Suche nach dem aktiven (zur Laufzeit erzeugten) Dropdown-List-Objekt
        GameObject dynamicDropdownList = GameObject.Find("Dropdown List");

        bool isOpen = dynamicDropdownList != null;

        if (isOpen && !wasOpen)
        {
            target.r = cornersOpen;
            target.Refresh();
            wasOpen = true;
        }
        else if (!isOpen && wasOpen)
        {
            target.r = cornersClosed;
            target.Refresh();
            wasOpen = false;
        }
    }
}
