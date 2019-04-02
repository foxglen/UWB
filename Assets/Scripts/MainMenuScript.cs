using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuScript : MonoBehaviour
{
	public GameObject menu;
	public GameObject player;
	public GameObject camera;
	public GameObject infoBox;
	public GameObject terrain;

	private void Start()
	{
		player.SetActive(false);
		Cursor.visible = true;
		Cursor.lockState = CursorLockMode.None;
	}
	
	private void Update()
	{
		Cursor.visible = true;
		Cursor.lockState = CursorLockMode.None;
	}

	public void ButtonClicked()
	{
		if (infoBox.GetComponent<InputField>().text != null)
		{
			camera.SetActive(false);
			menu.SetActive(false);
			player.SetActive(true);
			terrain.GetComponent<mapGeneration>().StartGame(infoBox.GetComponent<InputField>().text);
		}
	}
}
