using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WinTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        TitanicController titanic =  other.gameObject.GetComponentInParent<TitanicController>();
        if (titanic != null)
        {
            Debug.Log("Titanic entered, win");
        }
    }
}
