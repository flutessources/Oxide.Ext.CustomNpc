using Oxide.Core;
using Oxide.Ext.CustomNpc.Gameplay.Entities;
using System;
using System.Collections;
using UnityEngine;

namespace Oxide.Ext.CustomNpc.Gameplay.NpcCreator
{
    public class NpcCreator_NpcController
    {
        public const float SELECTED_OVERLAY_TIME = 10.0f;

        public readonly CustomNpc_Entity Npc;
        public readonly NpcCreator_BrainComponent CreatorBrain;
        public readonly string Name;
        public readonly Vector3 StartPosition;

        //private List<Item> m_lastPlayerWearItems = new List<Item>();
        //private List<Item> m_lastPlayerBeltItems = new List<Item>();

        private bool m_onSetWearItemsToPlayer = true;
        private bool m_onSetBeltItemsToPlayer = true;

        public Action<NpcCreator_NpcController> onKill;
        private readonly BasePlayer m_basePlayer;

        private Coroutine m_selectedOverlayCoroutine;
        private bool m_isSelected;

        public NpcCreator_NpcController(BasePlayer player, string name, CustomNpc_Entity npc)
        {
            m_basePlayer = player;
            Name = name;
            Npc = npc;

            StartPosition = npc.Controller.Component.ServerPosition;

            CreatorBrain = npc.Controller.Brain.Component as NpcCreator_BrainComponent;
        }

        public void MovePlayerWearItemsToNpc(PlayerInventory playerInventory)
        {
            if (m_onSetWearItemsToPlayer)
                return;

            //m_lastPlayerWearItems = new List<Item>(playerInventory.containerWear.itemList);
            CopyWearItems(playerInventory, Npc.Controller.Component.inventory);
        }

        public void MoveNpcWearItemsToPlayer(PlayerInventory playerInventory)
        {
            m_onSetWearItemsToPlayer = true;
            CopyWearItems(Npc.Controller.Component.inventory, playerInventory);
            m_onSetWearItemsToPlayer = false;
        }

        private void CopyWearItems(PlayerInventory a, PlayerInventory b)
        {
            // clear
            for (int i = b.containerWear.itemList.Count - 1; i >= 0; i--)
            {
                Item item = b.containerWear.itemList[i];
                item.RemoveFromContainer();
                item.Remove();
            }

            if (a.containerWear == null || a.containerWear.itemList.Count == 0)
                return;

            // copy
            foreach(var item in a.containerWear.itemList)
            {
                var copiedItem = ItemManager.CreateByName(item.info.shortname, 1, item.skin);
                if (copiedItem == null) continue;
                if (!copiedItem.MoveToContainer(b.containerWear)) copiedItem.Remove();
            }
        }


        public void MovePlayerBeltItemsToNpc(PlayerInventory playerInventory)
        {
            if (m_onSetBeltItemsToPlayer)
                return;

            //m_lastPlayerBeltItems = new List<Item>(playerInventory.containerBelt.itemList);
            CopyBeltItems(playerInventory, Npc.Controller.Component.inventory);
        }

        public void MoveNpcBeltItemsToPlayer(PlayerInventory playerInventory)
        {
            m_onSetBeltItemsToPlayer = true;
            CopyBeltItems(Npc.Controller.Component.inventory, playerInventory);
            m_onSetBeltItemsToPlayer = false;
        }

        private void CopyBeltItems(PlayerInventory a, PlayerInventory b)
        {
            // clear
            for (int i = b.containerBelt.itemList.Count - 1; i >= 0; i--)
            {
                Item item = b.containerBelt.itemList[i];
                item.RemoveFromContainer();
                item.Remove();
            }

            if (a.containerBelt == null || a.containerBelt.itemList.Count == 0)
                return;

            // copy
            foreach (var item in a.containerBelt.itemList)
            {
                var copiedItem = ItemManager.CreateByName(item.info.shortname, 1, item.skin);
                if (copiedItem == null) continue;
                if (!copiedItem.MoveToContainer(b.containerBelt)) copiedItem.Remove();
            }

            Npc.Controller.Component.OnDesequipingWeapon();
        }

        //private bool WearItemsChange()
        //{
        //    if (m_lastPlayerWearItems.Count != Npc.Controller.Component.inventory.containerWear.itemList.Count)
        //        return true;

        //    for (int i = 0; i < m_lastPlayerWearItems.Count; i++)
        //    {
        //        var playerItem = m_lastPlayerWearItems[i];
        //        var npcItem = Npc.Controller.Component.inventory.containerWear.itemList[i];

        //        if (playerItem.info.shortname != npcItem.info.shortname && playerItem.skin != npcItem.skin)
        //            return true;
        //    }

        //    return false;
        //}

        public void OnKill()
        {
            onKill?.Invoke(this);

            if (m_isSelected)
            {
                OnUnselect();
            }
        }

        public void OnSelect()
        {
            //StartSelectedNpcOverlayTimer();
            m_isSelected = true;
        }

        public void OnUnselect()
        {
            //StopSelectedNpcOverlayTimer();
            m_isSelected = false;
            m_onSetWearItemsToPlayer = true;
            m_onSetBeltItemsToPlayer = true;
        }

        private void StopSelectedNpcOverlayTimer()
        {
            if (m_selectedOverlayCoroutine != null)
                ServerMgr.Instance.StopCoroutine(m_selectedOverlayCoroutine);

            m_selectedOverlayCoroutine = null;
        }

        private void StartSelectedNpcOverlayTimer()
        {
            if (m_selectedOverlayCoroutine != null)
                StopSelectedNpcOverlayTimer();

            ServerMgr.Instance.StartCoroutine(SelectedNpcOverlay());
        }

        private IEnumerator SelectedNpcOverlay()
        {
            while (m_isSelected)
            {
                m_basePlayer.SendConsoleCommand("ddraw.sphere", SELECTED_OVERLAY_TIME, Color.blue, Npc.Controller.Component.ServerPosition + Vector3.up, 1f);
                yield return CoroutineEx.waitForSeconds(SELECTED_OVERLAY_TIME);
            }
        }
    }
}
