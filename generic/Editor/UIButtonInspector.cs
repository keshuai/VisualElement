using UnityEditor;
using CX;
using NUnit.Framework.Constraints;
using UnityEngine;

namespace CXEditor
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(UIButton))]
    public class UIButtonInspector : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            
            UIButton _this = this.target as UIButton;

            _this.Size = EditorUILayout.Size2DField("Size", _this.Size);

        }

//       [MenuItem("CX/Button Color")]
//       static void SetAllColor()
//       {
//           UIButton[] buttons = UnityEngine.GameObject.Find("App").GetComponentsInChildren<UIButton>();
//           foreach (var button in buttons)
//           {
//               button.ColorNormal = new Color(0.86f, 0.86f, 0.86f, 1f);
//               button.ColorPressed = Color.gray;
//
//               ImageColorVE image = button.GetComponentInChildren<ImageColorVE>();
//               if (image != null)
//                   Destroy(image.gameObject);
//               
//               RectColorVE rect = button.GetComponentInChildren<RectColorVE>();
//               if (rect == null)
//               {
//                   GameObject o = new GameObject();
//                   o.transform.parent = button.transform;
//                   o.transform.localPosition = Vector3.zero;
//                   o.layer = button.gameObject.layer;
//                   rect = o.AddComponent<RectColorVE>();
//            
//                   rect.Size = button.Size;
//                   if (Application.isEditor) o.name = "frame";
//
//                   button.m_Frame = rect;
//               }
//               
//               button.m_Frame.Drawcall.ElementIndexMoveToIndex(button.m_Frame , 4);
//               rect.Color = new Color(0.86f, 0.86f, 0.86f, 1f);
//           }
//       }
    }
}