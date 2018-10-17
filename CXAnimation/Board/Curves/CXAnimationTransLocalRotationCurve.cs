/*******************************************
 **  CX Game  UTF-16 Little-Endian 模版   **
 *******************************************/

using UnityEngine;
using System.Collections;


public class CXAnimationTransLocalRotationCurve : CXAnimationTransCurve 
{
	public override void SetValue (Vector3 v)
	{
		this.Target.localRotation = Quaternion.Euler(v);
	}
}