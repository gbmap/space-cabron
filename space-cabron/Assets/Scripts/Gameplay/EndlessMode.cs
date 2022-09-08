using System.Collections;
using System.Collections.Generic;
using Gmap.Gameplay;
using SpaceCabron.Gameplay;
using SpaceCabron.Gameplay.Level;
using UnityEngine;

public class EndlessMode : MonoBehaviour
{
    public BaseLevelConfiguration LevelInf;

    void Start()
    {
        LevelLoader.Load(LevelInf);
    }
}
