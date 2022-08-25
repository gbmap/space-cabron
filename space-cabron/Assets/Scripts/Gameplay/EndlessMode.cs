using System.Collections;
using System.Collections.Generic;
using Gmap.Gameplay;
using SpaceCabron.Gameplay;
using UnityEngine;

public class EndlessMode : MonoBehaviour
{
    public LevelConfiguration LevelInf;

    void Start()
    {
        LevelLoader.Load(LevelInf);
    }
}
