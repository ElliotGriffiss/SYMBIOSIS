using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnOffAfterDuration : MonoBehaviour
{
    private WaitForSeconds WaitFor = new WaitForSeconds(1f);

    private void Start()
    {
        StartCoroutine(WaitForTurnOff());
    }

    private IEnumerator WaitForTurnOff()
    {
        yield return WaitFor;
        gameObject.SetActive(false);
    }
}
