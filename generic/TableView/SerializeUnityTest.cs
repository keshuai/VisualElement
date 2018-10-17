using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SerializeUnityTest : MonoBehaviour, ISerializationCallbackReceiver 
{

	public void OnAfterDeserialize ()
	{
		Debug.Log("OnAfterDeserialize");
	}

	public void OnBeforeSerialize ()
	{
		Debug.Log("OnBeforeSerialize");
	}
}
