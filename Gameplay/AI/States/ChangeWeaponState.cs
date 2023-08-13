using JetBrains.Annotations;
using Oxide.Core;
using Oxide.Ext.CustomNpc.Gameplay.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Oxide.Ext.CustomNpc.Gameplay.AI.States
{
    public class ChangeWeaponState : CustomAIState
    {
        private AttackEntity m_lastBestWeapon;
        private Item m_lastBestWeaponItem;
        public ChangeWeaponState() : base(AIState.Cooldown)
        {
           
        }

        public override bool CanEnter()
        {
            base.CanEnter();

            if (CanEquipWeapon() == false)
                return false;

            var bestWeapon = GetBestWeaponToEquip();
            if (bestWeapon == null)
                return false;

            if (bestWeapon == m_npc.Component.CurrentWeapon)
                return false;

            return true;
        }

        public override void StateEnter(BaseAIBrain brain, BaseEntity entity)
        {
            base.StateEnter(brain, entity);
            EquipWeapon();
        }

        public override bool CanLeave()
        {
            return true;
        }

        public override float GetWeight()
        {
            return 75f;
        }

        private bool CanEquipWeapon()
        {
            if (m_npc.Component.inventory == null || m_npc.Component.inventory.containerBelt == null) return false;
            if (m_npc.Component.IsEquipingWeapon) return false;
            return true;
        }

        protected virtual AttackEntity GetBestWeaponToEquip()
        {
            float distanceToTarget = m_npc.Component.CurrentTarget != null ? m_npc.DistanceToTarget : 25; // Default distance

            List<Item> availableWeapons = m_npc.Component.inventory.containerBelt.itemList
                .Where(item => GetWeaponRangeType(item) != EWeaponRangeType.NONE)
                .ToList();

            if (!availableWeapons.Any())
                return null;

            EWeaponRangeType desiredWeaponType = GetDesiredWeaponTypeForDistance(distanceToTarget);
            Item bestWeapon = availableWeapons.FirstOrDefault(item => GetWeaponRangeType(item) == desiredWeaponType);

            if (bestWeapon == null)
                bestWeapon = availableWeapons.First();

            var heldEntity = bestWeapon.GetHeldEntity();
            if (heldEntity == null)
                return null;

            var attackEntity = heldEntity as AttackEntity;
            if (attackEntity == null)
                return null;

            m_lastBestWeapon = attackEntity;
            m_lastBestWeaponItem = bestWeapon;

            return attackEntity;
        }

        // Todo : use config ?
        private EWeaponRangeType GetDesiredWeaponTypeForDistance(float distance)
        {
            if (distance <= 8) return EWeaponRangeType.ShortDistance;
            if (distance <= 16) return EWeaponRangeType.MidleDistance;
            if (distance <= 32) return EWeaponRangeType.HighDistance;
            return EWeaponRangeType.LongDistance;
        }

        public enum EWeaponRangeType
        {
            Melee = 0,
            ShortDistance = 1,
            MidleDistance = 2,
            HighDistance = 3,
            LongDistance = 4,
            NONE = -1
        }

        private void EquipWeapon()
        {
            m_npc.Component.OnEquipingWeapon(m_lastBestWeapon);
            m_npc.Component.UpdateActiveItem(m_lastBestWeaponItem.uid);
            m_lastBestWeapon.TopUpAmmo();

            BaseProjectile baseProjectile = m_lastBestWeapon as BaseProjectile;
            if (baseProjectile != null)
            {
                WeaponsData.Range range = null;

                if (WeaponsData.WeaponsRange.TryGetValue(m_lastBestWeaponItem.info.shortname, out range))
                {
                    m_lastBestWeapon.effectiveRange = range.EffectiveRange;
                    m_lastBestWeapon.attackLengthMin = range.AttackLengthMin;
                    m_lastBestWeapon.attackLengthMax = range.AttackLengthMax;
                }

                m_lastBestWeapon.aiOnlyInRange = true;

                if (baseProjectile.MuzzlePoint == null)
                    baseProjectile.MuzzlePoint = baseProjectile.transform;

                var belt = m_npc.Configuration.BeltItems.FirstOrDefault(x => x.ShortName == m_lastBestWeaponItem.info.shortname);

                if (belt != null)
                {
                    string ammo = belt.Ammo;
                    if (!string.IsNullOrEmpty(ammo))
                    {
                        if (baseProjectile.primaryMagazine != null)
                        {
                            var ammoType = ItemManager.FindItemDefinition(ammo);
                            if (ammoType != null)
                            {
                                baseProjectile.primaryMagazine.ammoType = ammoType;
                                baseProjectile.SendNetworkUpdateImmediate();
                            }
                        }
                    }
                }

            }
            else
            {
                Chainsaw chainSaw = m_lastBestWeapon as Chainsaw;
                if (chainSaw != null)
                {
                    chainSaw.ServerNPCStart();
                }
            }

            m_npc.Component.Invoke(m_npc.Component.OnFinishEquipingWeapon, 1.5f);         
        }

        private EWeaponRangeType GetWeaponRangeType(Item item)
        {
            if (WeaponsData.MeleeWeapons.Contains(item.info.shortname)) return EWeaponRangeType.Melee;
            if (WeaponsData.FirstDistanceWeapons.Contains(item.info.shortname)) return EWeaponRangeType.ShortDistance;
            if (WeaponsData.SecondDistanceWeapons.Contains(item.info.shortname)) return EWeaponRangeType.MidleDistance;
            if (WeaponsData.ThirdDistanceWeapons.Contains(item.info.shortname)) return EWeaponRangeType.HighDistance;
            if (WeaponsData.FourthDistanceWeapons.Contains(item.info.shortname)) return EWeaponRangeType.LongDistance;
            return EWeaponRangeType.NONE;
        }
    }
}
