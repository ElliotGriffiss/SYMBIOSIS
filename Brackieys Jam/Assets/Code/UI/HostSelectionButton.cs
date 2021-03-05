using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class HostSelectionButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Button Settings")]
    [SerializeField] SymbioteCreationGUI CreationGUI;
    [SerializeField] private Button Button; 
    [SerializeField] private GameObject UnlockInfoPanel;
    [SerializeField] private GameObject StatInfoPanel;

    [SerializeField] private int HostIndex;

    public void SetupButton(bool Active)
    {
        Button.interactable = Active;
    }

    public void OnButtonpressed()
    {
        CreationGUI.UpdateCurrentHost(HostIndex);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (Button.interactable)
        {
            StatInfoPanel.SetActive(true);
        }
        else
        {
            UnlockInfoPanel.SetActive(true);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (Button.interactable)
        {
            StatInfoPanel.SetActive(false);
        }
        else
        {
            UnlockInfoPanel.SetActive(false);
        }
    }
}
