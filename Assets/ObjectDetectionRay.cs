using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class ObjectDetectionRay : MonoBehaviour
{
	private Camera cam;
	public GameObject objectInfo;

	// Use this for initialization
	void Start ()
	{
		cam = GetComponent<Camera>();
		objectInfo = GameObject.Find("ObjectInfoText");
	}
	
	// Update is called once per frame
	void Update ()
	{
		RaycastHit hit;
		Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
		if (Physics.Raycast(ray, out hit))
		{
			String retrievedInfo = hit.transform.gameObject.GetComponent<ObjectInfo>().objectInfo.Replace("{","");
			retrievedInfo = retrievedInfo.Replace("}", "");
			retrievedInfo = hit.transform.gameObject.name + "\n" + retrievedInfo;
			objectInfo.GetComponent<Text>().text = retrievedInfo;

		}
	}
}
