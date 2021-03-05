using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ParasiteSelectionButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Button Settings")]
    [SerializeField] SymbioteCreationGUI CreationGUI;
    [SerializeField] private Button Button;
    [SerializeField] private GameObject UnlockInfoPanel;
    [SerializeField] private GameObject StatInfoPanel;

    [SerializeField] private Image UnlockProgressSlider;
    [SerializeField] private Text UnlockProgressText;

    [SerializeField] private int ParasiteIndex;

    public void SetupButton(bool Active, int totalKills, int parasiteUnlockRequirements)
    {
        Button.interactable = Active;
        UnlockProgressText.text = totalKills + "/" + parasiteUnlockRequirements;

        UnlockProgressSlider.fillAmount = totalKills / parasiteUnlockRequirements;
    }

    public void OnButtonpressed()
    {
        CreationGUI.UpdateCurrentParasite(ParasiteIndex);
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
