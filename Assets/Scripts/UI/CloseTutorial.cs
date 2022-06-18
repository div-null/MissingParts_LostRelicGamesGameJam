using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class CloseTutorial : MonoBehaviour
{
    public void CloseWindow()
    {
        transform.parent.gameObject.SetActive(false);
    } 
}
