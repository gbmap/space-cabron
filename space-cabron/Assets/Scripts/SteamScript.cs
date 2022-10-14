using System.Collections;
using System.Collections.Generic;
using Steamworks;
using UnityEngine;

public class SteamScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        if (SteamManager.Initialized) {
            string name = SteamFriends.GetPersonaName();
            Debug.Log(name);
        }
    }
}
