using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelProgressCanvas : MonoBehaviour
{
    [SerializeField] private Text Text;

    public void SetText(int numCollected, int numNeeded)
    {
        Text.text = numCollected + "/" + numNeeded;
    }
}
