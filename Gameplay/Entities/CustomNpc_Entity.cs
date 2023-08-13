using Oxide.Ext.CustomNpc.Gameplay.Controllers;
using UnityEngine;

namespace Oxide.Ext.CustomNpc.Gameplay.Entities
{
    public class CustomNpc_Entity
    {
        public readonly GameObject GameObject;
        public readonly CustomNpc_Controller Controller;

        public CustomNpc_Entity(GameObject gameObject, CustomNpc_Controller controller)
        {
            GameObject = gameObject;
            Controller = controller;
        }

        public void Start(CustomNpcBrain_Controller brain)
        {
            Controller.Start(brain);
        }
    }
}
