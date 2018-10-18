/*******************************************
 **  CX Game  UTF-16 Little-Endian 模版   **
 *******************************************/

using UnityEngine;
using System.Collections;


public class CXTouchEvent : MonoBehaviour
{
	[SerializeField] private CX.Drawcall _view;
	[SerializeField] public CX.VEle element;
	public CX.Drawcall view 
	{
		get { return this.element == null? _view : this.element.Drawcall; }
		set { _view = value; }
	}

	public virtual void OnTouchEvent ( CXTouchParser touchParser, CXTouch touch ){}
	public virtual void OnTouchCancel ( CXTouch touch ){}
	
	// OnDisable 时应该进行事件取消处理 尤其是焦点事件
}