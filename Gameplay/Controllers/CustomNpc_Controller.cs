using Oxide.Core;
using Oxide.Ext.CustomNpc.Gameplay.Components;
using Oxide.Ext.CustomNpc.Gameplay.Configurations;
using Oxide.Ext.CustomNpc.Gameplay.Controllers.NpcAttire;
using Oxide.Ext.CustomNpc.Gameplay.Data;
using Oxide.Ext.CustomNpc.PluginExntesions;
using Rust;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using Facepunch;
using static BaseEntity;

using System.Collections;
using Oxide.Ext.CustomNpc.Gameplay.AI;

namespace Oxide.Ext.CustomNpc.Gameplay.Controllers
{
    public class CustomNpc_Controller
    {
        public GameObject GameObject => Component.gameObject;
        public  CustomNpc_Component Component { get; private set; }
        public CustomNpcBrain_Controller Brain { get; private set; }
        public CustomNpc_Pathfinding Pathfinding { get; private set; }
        public CustomNpc_Configuration Configuration { get; private set; }

        public float DistanceToTarget => Vector3.Distance(GameObject.transform.position, Component.CurrentTarget.transform.position);
        public float DistanceFromHome => Vector3.Distance(GameObject.transform.position, Component.HomePosition);
        public bool IsBehindBarricade() => Component.CanSeeTarget(Component.CurrentTarget) && HasNearbyBarricade();

        private Coroutine m_healCoroutine;

        private bool m_isInit;

        #region Setup
        public virtual void Initialize(CustomNpc_Component component, CustomNpc_Configuration configuration)
        {
            Component = component;
            Configuration = configuration;
            Pathfinding = new CustomNpc_Pathfinding(this);

            m_isInit = true;
        }

        public virtual void Start(CustomNpcBrain_Controller brain)
        {
            if (m_isInit == false)
            {
                Interface.Oxide.LogError("[CustomNpc] Start npc without initialization !!");
                return;
            }

            Brain = brain;
            Component.Setup(brain.Component, GameObject.transform.position);

            RegisterEvents();

            Component.Spawn();
        }

        private void RegisterEvents()
        {
            Component.onServerInstantiated += OnSpawn;
            Component.onDestroy += OnDestroy;
        }

        protected virtual void OnSpawn()
        {
            if (m_isInit == false)
                return;

            Setup();
        }

        protected virtual void OnDestroy()
        {
            if (m_isInit == false)
                return;

            if (m_healCoroutine != null) ServerMgr.Instance.StopCoroutine(m_healCoroutine);

            if (Brain.Component.CurrentState != null)
            {
                Brain.Component.CurrentState.StateLeave(Brain.Component, Component);
            }

            Component.onServerInstantiated -= OnSpawn;
            Component.onDestroy -= OnDestroy;
        }

        protected virtual void Setup()
        {
            if (m_isInit == false)
                return;

            Component.displayName = Configuration.Name;

            SetupNavAgent();
            SetupHealth();
            SetupDamages();
            SetupSound();
            SetupInventory();

            Component.InvokeRepeating(Component.LightCheck, 1f, 30f); // Todo : Config
            Component.InvokeRepeating(Update, 1f, 2f); // Todo : Config
        }

        protected virtual void SetupNavAgent()
        {
            if (Component.NavAgent == null)
                Component.NavAgent = GameObject.GetComponent<NavMeshAgent>();

            if (Component.NavAgent != null)
            {
                Component.NavAgent.areaMask = Configuration.AreaMask;
                Component.NavAgent.agentTypeID = Configuration.AgentTypeID;
            }
        }

        protected virtual void SetupHealth()
        {
            Component.startHealth = Configuration.StartHealth;
            Component._maxHealth = Configuration.MaxHealth;
            Component._health = Configuration.StartHealth;
        }

        protected virtual void SetupDamages()
        {
            Component.damageScale = Configuration.DamageScale;
        }

        protected virtual void SetupSound()
        {
            if (Configuration.DisableRadio)
            {
                Component.CancelInvoke(Component.PlayRadioChatter);
                Component.RadioChatterEffects = Array.Empty<GameObjectRef>();
                Component.DeathEffects = Array.Empty<GameObjectRef>();
            }
        }

        protected virtual void SetupInventory()
        {
            ClearItems(Component.inventory.containerWear);
            ClearItems(Component.inventory.containerBelt);

            NpcAttire_Controller_Base attire = null;

            if (string.IsNullOrEmpty(Configuration.Kit) == false
                && PluginsExtensionsManager.Instance.LoadedPlugins.ContainsKey(NpcAttire_Kits_Controller.PLUGIN_NAME))
            {
                attire = new NpcAttire_Kits_Controller(this, Configuration.Kit);
            }
            else
            {
                attire = new NpcAttire_Default_Controller(Component.inventory, Configuration.BeltItems, Configuration.WearItems);
            }

            attire.Equip();
        }
        #endregion

        private void ClearItems(ItemContainer container)
        {
            for (int i = container.itemList.Count - 1; i >= 0; i--)
            {
                Item item = container.itemList[i];
                item.RemoveFromContainer();
                item.Remove();
            }
        }

        protected virtual void Update()
        {
            if (m_isInit == false)
                return;
        }


        protected virtual void SetAimTowardsTarget()
        {
            Vector3 aimDirection = (Component.CurrentTarget.transform.position - GameObject.transform.position).normalized;
            Component.SetAimDirection(aimDirection);
        }

        private IEnumerable<Barricade> GetBarricadeHits(RaycastHit[] hits)
        {
            foreach (var hit in hits)
            {
                var barricade = hit.GetEntity() as Barricade;
                if (barricade != null && CoversData.Barricades.Contains(barricade.ShortPrefabName))
                {
                    yield return barricade;
                }
            }
        }

        private bool HasNearbyBarricade()
        {
            SetAimTowardsTarget();

            RaycastHit[] hits = UnityEngine.Physics.RaycastAll(Component.eyes.HeadRay());
            GamePhysics.Sort(hits);

            return GetBarricadeHits(hits).Any(barricade => Vector3.Distance(GameObject.transform.position, barricade.transform.position) < DistanceToTarget);
        }

        #region Weapons

        #region Grenade Launcher
        public bool IsReloadGrenadeLauncher { get; private set; } = false;
        protected int m_availableAmmoInGrenadeLauncher = 6;

        public virtual void FireGrenadeLauncher()
        {
            RaycastHit raycastHit;
            Component.SignalBroadcast(Signal.Attack, string.Empty);
            Vector3 vector3 = Component.IsMounted() ? Component.eyes.position + new Vector3(0f, 0.5f, 0f) : Component.eyes.position;
            Vector3 modifiedAimConeDirection = AimConeUtil.GetModifiedAimConeDirection(0.675f, Component.eyes.BodyForward());
            float distance = 1f;
            if (UnityEngine.Physics.Raycast(vector3, modifiedAimConeDirection, out raycastHit, distance, 1236478737))
                distance = raycastHit.distance - 0.1f;

            TimedExplosive grenade = GameManager.server.CreateEntity("assets/prefabs/ammo/40mmgrenade/40mm_grenade_he.prefab", vector3 + modifiedAimConeDirection * distance) as TimedExplosive;
            grenade.creatorEntity = Component;
            ServerProjectile serverProjectile = grenade.GetComponent<ServerProjectile>();
            serverProjectile.InitializeVelocity(Component.GetInheritedProjectileVelocity(modifiedAimConeDirection) + modifiedAimConeDirection * serverProjectile.speed * 2f);
            grenade.Spawn();
            m_availableAmmoInGrenadeLauncher--;
            if (m_availableAmmoInGrenadeLauncher == 0)
            {
                IsReloadGrenadeLauncher = true;
                Component.Invoke(FinishReloadGrenadeLauncher, 8f);
            }
        }

        protected void FinishReloadGrenadeLauncher()
        {
            m_availableAmmoInGrenadeLauncher = 6;
            IsReloadGrenadeLauncher = false;
        }
        #endregion Multiple Grenade Launcher

        #region Flame Thrower
        public bool IsReloadFlameThrower { get; private set; }

        public virtual void FireFlameThrower()
        {
            FlameThrower flameThrower = Component.CurrentWeapon as FlameThrower;
            if (flameThrower == null || flameThrower.IsFlameOn()) return;
            if (flameThrower.ammo <= 0)
            {
                IsReloadFlameThrower = true;
                Component.Invoke(FinishReloadFlameThrower, 4f);
                return;
            }
            flameThrower.SetFlameState(true);
            Component.Invoke(flameThrower.StopFlameState, 0.25f);
        }

        protected void FinishReloadFlameThrower()
        {
            FlameThrower flameThrower = Component.CurrentWeapon as FlameThrower;
            if (flameThrower == null) return;
            flameThrower.TopUpAmmo();
            IsReloadFlameThrower = false;
        }
        #endregion Flame Thrower

        #region Melee Weapon
        public void UseMeleeWeapon()
        {
            BaseMelee weapon = Component.CurrentWeapon as BaseMelee;
            if (weapon.HasAttackCooldown())
                return;

            weapon.StartAttackCooldown(weapon.repeatDelay * 2f);
            Component.SignalBroadcast(Signal.Attack, string.Empty, null);

            if (weapon.swingEffect.isValid)
                Effect.server.Run(weapon.swingEffect.resourcePath, weapon.transform.position, Vector3.forward, Component.net.connection, false);

            Vector3 vector31 = Component.eyes.BodyForward();

            for (int i = 0; i < 2; i++)
            {
                List<RaycastHit> list = Pool.GetList<RaycastHit>();
                GamePhysics.TraceAll(new Ray(Component.eyes.position - (vector31 * (i == 0 ? 0f : 0.2f)), vector31), (i == 0 ? 0f : weapon.attackRadius), list, weapon.effectiveRange + 0.2f, 1219701521, QueryTriggerInteraction.UseGlobal, null);
                bool flag = false;
                for (int j = 0; j < list.Count; j++)
                {
                    RaycastHit item = list[j];
                    BaseEntity entity = item.GetEntity();
                    if (entity != null && (entity == null || entity != Component && !entity.EqualNetID(Component)) && (entity == null || !entity.isClient))
                    {
                        float single = weapon.damageTypes.Sum(x => x.amount);
                        entity.OnAttacked(new HitInfo(Component, entity, DamageType.Slash, single * weapon.npcDamageScale * Configuration.DamageScale));
                        HitInfo hitInfo = Pool.Get<HitInfo>();
                        hitInfo.HitEntity = entity;
                        hitInfo.HitPositionWorld = item.point;
                        hitInfo.HitNormalWorld = -vector31;
                        if (entity is BaseNpc || entity is BasePlayer) hitInfo.HitMaterial = StringPool.Get("Flesh");
                        else hitInfo.HitMaterial = StringPool.Get(item.GetCollider().sharedMaterial != null ? item.GetCollider().sharedMaterial.GetName() : "generic");
                        weapon.ServerUse_OnHit(hitInfo);
                        Effect.server.ImpactEffect(hitInfo);
                        Pool.Free(ref hitInfo);
                        flag = true;
                        if (entity == null || entity.ShouldBlockProjectiles()) break;
                    }
                }
                Pool.FreeList(ref list);
                if (flag) break;
            }
        }
        #endregion Melee Weapon
        #endregion


        #region Targeting
        public virtual void SetBestTarget()
        {
            BaseEntity bestTarget = null;
            float highestScore = -1f;

            foreach (BaseEntity potentialTarget in Brain.Component.Senses.Players)
            {
                if (!CanTargetEntity(potentialTarget)) continue;

                float targetScore = CalculateTargetScore(potentialTarget);
                if (targetScore > highestScore)
                {
                    bestTarget = potentialTarget;
                    highestScore = targetScore;
                }
            }

            Component.SetTarget(bestTarget);
        }

        protected virtual float CalculateTargetScore(BaseEntity entity)
        {
            float distanceScore = 1f - Mathf.InverseLerp(1f, Brain.Component.SenseRange, Vector3.Distance(entity.transform.position, GameObject.transform.position));
            float visionScore = Mathf.InverseLerp(Brain.Component.VisionCone, 1f, Vector3.Dot((entity.transform.position - Component.eyes.position).normalized, Component.eyes.BodyForward())) / 2f;
            float losBonus = Brain.Component.Senses.Memory.IsLOS(entity) ? 2f : 0f;

            return distanceScore + visionScore + losBonus;
        }

        protected virtual bool CanTargetEntity(BaseEntity target)
        {
            if (target == null || target.IsDestroyed || target.Health() <= 0f) return false;

            BasePlayer player = target as BasePlayer;
            if (player != null)
            {
                return CanTargetPlayer(player);
            }
            else
            {
                Drone drone = target as Drone;
                if (drone != null)
                {
                    return CanTargetDrone(drone);
                }
            }

            return false;
        }

        private bool CanTargetPlayer(BasePlayer player)
        {
            if (player.IsDead()) return false;
            if (player.skinID != 0 && NpcSkinsData.PlayerSkinIDs.Contains(player.skinID)) return true;
            if (player.userID.IsSteamId()) return !player.IsSleeping() && !player.IsWounded() && !player._limitedNetworking;
            NPCPlayer npcPlayer = player as NPCPlayer;
            if (npcPlayer != null) return CanTargetNpcPlayer(npcPlayer);
            return false;
        }

        protected virtual bool CanTargetNpcPlayer(NPCPlayer npcPlayer)
        {
            return npcPlayer is FrankensteinPet || Configuration.CanTargetOtherNpc;
        }

        protected virtual bool CanTargetDrone(Drone drone)
        {
            return !(Component.CurrentWeapon is BaseMelee);
        }

        #endregion


        public void Heal()
        {
            m_healCoroutine = ServerMgr.Instance.StartCoroutine(HealCoroutine());
        }

        private IEnumerator HealCoroutine()
        {
            Component.OnHeal();
            Item syringe = Component.inventory.containerBelt.itemList.FirstOrDefault(x => x.info.shortname == "syringe.medical");
            Component.OnDesequipingWeapon();
            Component.UpdateActiveItem(syringe.uid);
            MedicalTool medicalTool = syringe.GetHeldEntity() as MedicalTool;
            yield return CoroutineEx.waitForSeconds(1.5f);

            if (medicalTool != null) medicalTool.ServerUse();
            Component.InitializeHealth(
                Component.health + 15f > Configuration.MaxHealth
                ? Configuration.MaxHealth
                : Component.health + 15f, Configuration.MaxHealth);

            yield return CoroutineEx.waitForSeconds(3f);

            Component.OnFinishHeal();
        }
    }
}
