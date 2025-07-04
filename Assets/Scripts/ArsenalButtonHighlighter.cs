using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class ButtonHighlighter : MonoBehaviour
{
    [SerializeField] private List<Button> buttons;
    private Button currentHighlighted;

    private Color activeBg = Color.black;
    private Color activeText = Color.white;
    private Color inactiveBg = Color.white;
    private Color inactiveText = Color.black;

    void Start()
    {
        foreach (var btn in buttons)
        {
            btn.onClick.AddListener(() => OnButtonClicked(btn));
        }
        // Optional: initialer Zustand
        if (buttons.Count > 0)
            OnButtonClicked(buttons[0]);
    }

    private void OnButtonClicked(Button clickedButton)
    {
        foreach (var btn in buttons)
        {
            var image = btn.GetComponent<Image>();
            var text = btn.GetComponentInChildren<TMP_Text>();

            if (btn == clickedButton)
            {
                image.color = activeBg;
                text.color = activeText;
                currentHighlighted = btn;
            }
            else
            {
                image.color = inactiveBg;
                text.color = inactiveText;
            }
        }
    }
}
