using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Menu : MonoBehaviour
{

    private string menuText = "This is some menu text";
    public Text menu;

    private void Update()
    {
        menu.text = menuText;
    }
}
