using UnityEngine;
using Frictionless;
using SpaceCabron.Gameplay.Interactables;
using System.Collections.Generic;
using SpaceCabron.Upgrades;
using UnityEngine.UI;
using Gmap.Gameplay;
using System;

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

        ImprovisationToIcon improvisationToIcon;

        void Awake()
        {
            improvisationToIcon = Resources.Load<ImprovisationToIcon>("ImprovisationToIcon");
        }

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

            Sprite sprite = improvisationToIcon.GetIcon(msg.Improvisation);
            AddItem(sprite, true);
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
                    items.Remove(item);
                    Destroy(item.Item);
                    break;
                }
            }
        }

        private void Callback_OnUpgradeTaken(MsgOnUpgradeTaken msg)
        {
            // if (!(msg.Upgrade is ImprovisationUpgrade))
            //     return;

            // ImprovisationUpgrade upg = msg.Upgrade as ImprovisationUpgrade;
            // AddItem(upg.Improvisation.Icon, false);
        }

        private void AddItem(Sprite icon, bool temporary)
        {
            var instance = Instantiate(ItemPrefab);
            instance.transform.parent = transform;
            
            var renderer = instance.GetComponent<Image>();
            renderer.sprite = icon;

            items.Add(new UIUpgradeItem {
                Item = instance,
                Temporary = temporary
            });
        }

        private void Callback_OnUpgradeRemoved(MsgOnUpgradeRemoved msg)
        {
            // if (!(msg.Upgrade is ImprovisationUpgrade))
            //     return;
            
            // ImprovisationUpgrade upg = msg.Upgrade as ImprovisationUpgrade;

            // var item = items.FirstOrDefault(i => i.Upgrade == upg);
            // if (item == null)
            //     return;

            // items.Remove(item);
            // GameObject.Destroy(item.Item);
        }
    }
}