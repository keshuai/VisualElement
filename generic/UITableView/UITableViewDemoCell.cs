/*******************************************
 **  CX Game  UTF-16 Little-Endian 模版   **
 *******************************************/
#if UNITY_EDITOR
using UnityEngine;
using System.Collections;
using CX;


public class UITableViewDemoCell : UITableViewCell
{
	public RectColorVE Sprite;
	public Label Label;
	void Awake ()
	{
//		this.Sprite = CXUIPool.CreateSprite(this.transform);
//		this.Label = CXUIPool.CreateLabel(this.transform);
		
		GameObject o = new GameObject();
		o.transform.SetParent(this.transform, false);
		this.Sprite = o.AddComponent<RectColorVE>();
		
		o = new GameObject();
		o.transform.SetParent(this.transform, false);
		this.Label = o.AddComponent<Label>();

//		this.Sprite.atlas = UITableViewDemo.__DemoAtlas;
//		this.Sprite.spriteName = UITableViewDemo.__DemoSlicedSpriteName;
//		this.Sprite.type = UISprite.Type.Sliced;
//
//		this.Label.trueTypeFont = UITableViewDemo.__DemoFont;
//		this.Label.effectStyle = UILabel.Effect.Outline;
		this.Label.fontSize = 36;
		this.Label.Width = 500;
		this.Label.Height = 36;
//		this.Label.depth = 1;
	}
}

#endif