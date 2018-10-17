using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CX
{
    public class UIScrollBar : MonoBehaviour
    {
        public RectColorVE ForegroundVE;
        public RectColorVE BackgroundVE;

        public float Alpha;

        public float Value;
        public float BarSize;


        private void Awake()
        {
            GameObject o = new GameObject();
            o.transform.SetParent(this.transform, false);
            this.ForegroundVE = o.AddComponent<RectColorVE>();
            
            o = new GameObject();
            o.transform.SetParent(this.transform, false);
            this.BackgroundVE = o.AddComponent<RectColorVE>();
        }
    }
}
