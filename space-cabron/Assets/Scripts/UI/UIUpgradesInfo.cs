using UnityEngine;
using Frictionless;
using System.Collections.Generic;
using Gmap.Gameplay;
using Gmap.CosmicMusicUtensil;
using SpaceCabron.Gameplay;

namespace SpaceCabron.UI
{
    public class UIUpgradesInfo : MonoBehaviour
    {
        public GameObject ItemPrefab;

        private class UIUpgradeItem
        {
            public UIUpgradeInfoItem Item;
            public bool Temporary;
        }
        private List<UIUpgradeItem> items = new List<UIUpgradeItem>();


        void OnEnable()
        {
            MessageRouter.AddHandler<MsgOnImprovisationAdded>(Callback_OnImprovisationAdded);
            MessageRouter.AddHandler<MsgOnImprovisationRemoved>(Callback_OnImprovisationRemoved);
            MessageRouter.AddHandler<MsgOnObjectDestroyed>(Callback_OnObjectDestroyed);
        }

        void OnDisable()
        {
            MessageRouter.RemoveHandler<MsgOnImprovisationAdded>(Callback_OnImprovisationAdded);
            MessageRouter.RemoveHandler<MsgOnImprovisationRemoved>(Callback_OnImprovisationRemoved);
            MessageRouter.RemoveHandler<MsgOnObjectDestroyed>(Callback_OnObjectDestroyed);
        }

        private void Callback_OnImprovisationAdded(MsgOnImprovisationAdded msg)
        {
            if (msg.Object.name != "PlayerInstrument")
                return;

            // if (msg.Object.transform.parent == null)
            //     return;

            // if (!msg.Object.transform.parent.CompareTag("Player"))
            //     return;

            if (!UIUpgradeInfoItem.HasIcon(msg.Improvisation))
                return;

            AddItem(msg);
        }

        private void Callback_OnImprovisationRemoved(MsgOnImprovisationRemoved msg)
        {
            RemoveItem(msg.Improvisation);
        }

        private void Callback_OnObjectDestroyed(MsgOnObjectDestroyed msg)
        {
            // if (!msg.health.CompareTag("Player"))
            //     return;

            // for (int i = items.Count-1; i >= 0; i--)
            // {
            //     UIUpgradeItem item = items[i];
            //     // if (item.Temporary)
            //     {
            //         Destroy(item.Item.gameObject);
            //         items.Remove(item);
            //     }
            // }
        }

        private void AddItem(
            MsgOnImprovisationAdded msg
        ) {
            var instance = Instantiate(ItemPrefab);

            UIUpgradeInfoItem infoItem = instance.GetComponent<UIUpgradeInfoItem>();
            infoItem.Configure(
                msg.Turntable, msg.Improvisation, msg.Duration
            );
            
            items.Add(new UIUpgradeItem {
                Item = infoItem,
                Temporary = msg.Duration != -1
            });
            instance.transform.parent = transform;
        }

        private void RemoveItem(Improvisation improvisation)
        {
            for (int i = items.Count-1; i >= 0; i--)
            {
                UIUpgradeItem item = items[i];
                if (item.Item.Improvisation == improvisation)
                {
                    Destroy(item.Item.gameObject);
                    items.Remove(item);
                    break;
                }
            }
        }
    }
}