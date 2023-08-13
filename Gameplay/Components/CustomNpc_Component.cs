using System;
using UnityEngine;


namespace Oxide.Ext.CustomNpc.Gameplay.Components
{
    public class CustomNpc_Component : ScientistNPC
    {
        public Action onServerInstantiated;
        public Action onDestroy;

        public bool IsEquipingWeapon { get; private set; }
        public bool IsReloadGrenadeLauncher { get; private set; }
        public bool IsReloadFlameThrower { get; private set; }
        public bool IsHealing { get; private set; }

        public BaseEntity CurrentTarget { get; private set; }
        public AttackEntity CurrentWeapon { get; private set; }

        public Vector3 HomePosition { get; private set; }

        public void Setup(CustomNpcBrain_Component brainComponent, Vector3 homePosition)
        {
            HomePosition = homePosition;
            skinID = 11185464824609;
            Brain = brainComponent;
            enableSaving = false;
            gameObject.AwakeFromInstantiate();
        }

        private void OnDestroy()
        {
            CancelInvoke();
            onDestroy?.Invoke();
        }

        protected override string OverrideCorpseName() => displayName;

        public override void ServerInit()
        {
            base.ServerInit();

            onServerInstantiated?.Invoke();
        }

        public void OnEquipingWeapon(AttackEntity weapon)
        {
            CurrentWeapon = weapon;
            IsEquipingWeapon = true;
        }

        public void OnDesequipingWeapon()
        {
            CurrentWeapon = null;
        }

        public void OnFinishEquipingWeapon()
        {
            IsEquipingWeapon = false;
        }


        public void SetTarget(BaseEntity target)
        {
            CurrentTarget = target;
        }

        public void OnHeal()
        {
            IsHealing = true;
        }

        public void OnFinishHeal()
        {
            IsHealing = false;
        }
    }
}
