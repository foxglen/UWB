using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Script : MonoBehaviour
{
    public GameObject menu;

    public void ButtonClicked()
    {
        menu.SetActive(false);
    }
}
