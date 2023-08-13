using Oxide.Ext.CustomNpc.Gameplay.Components;
using System.Linq;
using UnityEngine.AI;
using UnityEngine;
using Oxide.Ext.CustomNpc.Gameplay.Controllers;

namespace Oxide.Ext.CustomNpc.Gameplay.AI
{
    public class CustomNpc_Pathfinding
    {
        private CustomNpc_Controller m_controller;
        private CustomNpc_Component m_component => m_controller.Component;
        public CustomNpc_Pathfinding(CustomNpc_Controller controller)
        {
            m_controller = controller;
        }

        public bool IsEqualsVector3(Vector3 a, Vector3 b) => Vector3.Distance(a, b) < 0.1f;

        public void SetDestination(Vector3 pos, float radius, BaseNavigator.NavigationSpeed speed)
        {
            Vector3 sample = GetSamplePosition(pos, radius);
            sample.y += 2f;
            if (!IsEqualsVector3(sample, m_controller.Brain.Component.Navigator.Destination)) m_controller.Brain.Component.Navigator.SetDestination(sample, speed);
        }

        public Vector3 GetSamplePosition(Vector3 source, float radius)
        {
            NavMeshHit navMeshHit;
            if (NavMesh.SamplePosition(source, out navMeshHit, radius, m_component.NavAgent.areaMask))
            {
                NavMeshPath path = new NavMeshPath();
                if (NavMesh.CalculatePath(m_controller.GameObject.transform.position, navMeshHit.position, m_component.NavAgent.areaMask, path))
                {
                    if (path.status == NavMeshPathStatus.PathComplete) return navMeshHit.position;
                    else return path.corners.Last();
                }
            }
            return source;
        }

        public Vector3 GetRandomPositionAround(Vector3 source, float radius)
        {
            Vector2 vector2 = UnityEngine.Random.insideUnitCircle * radius;
            return source + new Vector3(vector2.x, 0f, vector2.y);
        }
    }
}
