using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ObjectInfo : MonoBehaviour
{
	public string objectInfo;

	public void SetInfo(string info)
	{
		objectInfo = info;
	}

	public string GetInfo()
	{
		return objectInfo;
	}
}
