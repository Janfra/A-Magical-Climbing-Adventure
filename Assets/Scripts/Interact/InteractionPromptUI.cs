using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class InteractionPromptUI : MonoBehaviour
{
    [SerializeField] private GameObject uiPanel;
    [SerializeField] private TextMeshProUGUI promptText;
    public bool IsDisplayed = false;

    private void Start() {
        uiPanel.SetActive(false);
    }

    private void LateUpdate() {
        
    }

    public void SetUp(string prompt)
    {
        promptText.text = prompt;
        uiPanel.SetActive(true);
        IsDisplayed = true;
    }

    public void Close()
    {
        uiPanel.SetActive(false);
        IsDisplayed = false;
    }
}
