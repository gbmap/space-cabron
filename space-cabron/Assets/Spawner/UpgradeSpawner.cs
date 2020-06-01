using Frictionless;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeSpawner : MonoBehaviour
{
    public GameObject Upgrade;

    MessageRouter _router;

    // Start is called before the first frame update
    void OnEnable()
    {
        _router = ServiceFactory.Instance.Resolve<MessageRouter>();
        _router.AddHandler<MsgOnUpgradeTaken>(OnUpgradeToken);
    }

    private void OnDisable()
    {
        _router?.RemoveHandler<MsgOnUpgradeTaken>(OnUpgradeToken);
    }

    private void OnUpgradeToken(MsgOnUpgradeTaken obj)
    {
        FindObjectOfType<BeatMaker>().BPM += obj.Value;
    }

}
