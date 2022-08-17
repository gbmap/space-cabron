using UnityEngine;
using Frictionless;
using SpaceCabron.Gameplay.Interactables;
using System.Collections.Generic;
using SpaceCabron.Upgrades;
using UnityEngine.UI;
using Gmap.Gameplay;
using System;
using Gmap.CosmicMusicUtensil;

namespace SpaceCabron.UI
{
    public class UIUpgradesInfo : MonoBehaviour
    {
        public GameObject ItemPrefab;

        private class UIUpgradeItem
        {
            public GameObject Item;
            public bool Temporary;
        }
        private List<UIUpgradeItem> items = new List<UIUpgradeItem>();


        void OnEnable()
        {
            MessageRouter.AddHandler<MsgOnImprovisationAdded>(Callback_OnImprovisationAdded);
            MessageRouter.AddHandler<MsgOnObjectDestroyed>(Callback_OnObjectDestroyed);
        }

        void OnDisable()
        {
            MessageRouter.RemoveHandler<MsgOnImprovisationAdded>(Callback_OnImprovisationAdded);
            MessageRouter.RemoveHandler<MsgOnObjectDestroyed>(Callback_OnObjectDestroyed);
        }

        private void Callback_OnImprovisationAdded(MsgOnImprovisationAdded msg)
        {
            if (msg.Object.transform.parent == null)
                return;

            if (!msg.Object.transform.parent.CompareTag("Player"))
                return;

            AddItem(msg.Turntable, msg.Improvisation, true);
        }

        private void Callback_OnObjectDestroyed(MsgOnObjectDestroyed msg)
        {
            if (!msg.health.CompareTag("Player"))
                return;

            for (int i = items.Count-1; i >= 0; i--)
            {
                UIUpgradeItem item = items[i];
                if (item.Temporary)
                {
                    Destroy(item.Item);
                    items.Remove(item);
                }
            }
        }

        private void Callback_OnUpgradeTaken(MsgOnUpgradeTaken msg)
        {
        }

        private void AddItem(ITurntable turntable, Improvisation improvisation, bool temporary)
        {
            var instance = Instantiate(ItemPrefab);
            instance.transform.parent = transform;

            instance.GetComponent<UIUpgradeInfoItem>().Configure(
                turntable, improvisation
            );
            
            items.Add(new UIUpgradeItem {
                Item = instance,
                Temporary = temporary
            });
        }

        private void Callback_OnUpgradeRemoved(MsgOnUpgradeRemoved msg)
        {

        }
    }
}