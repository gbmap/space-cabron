using System.Collections;
using System.Collections.Generic;
using Gmap.Gameplay;
using UnityEngine;

namespace SpaceCabron.Gameplay
{
    public enum EColor
    {
        None=-1,
        Pink=0,
        Yellow=1, 
        Green=2,
        Blue=3,
    }

    class ColorBullet : Bullet
    {
        EColor color;
        public EColor Color 
        { 
            get { return color; }
            set 
            { 
                Renderer r = GetComponent<Renderer>();
                r.material.SetFloat("_IsResistant", 1f);
                r.material.SetFloat("_ColorIndex",
                                    (int)value);
                color = value; 

                TrailRenderer tr = GetComponentInChildren<TrailRenderer>();
                if (tr)
                    tr.material.SetFloat("_ColorIndex", (int)value);
            }
        }
    }
}