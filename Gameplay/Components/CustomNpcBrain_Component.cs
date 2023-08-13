using Oxide.Ext.CustomNpc.Gameplay.Controllers;
using System;

namespace Oxide.Ext.CustomNpc.Gameplay.Components
{
    public class CustomNpcBrain_Component : ScientistBrain
    {
        public Action onAddStates;
        public Action onInitializeAI;
        public Action<float> onThink;

        protected CustomNpcBrain_Controller m_controller;

        public virtual void Setup(CustomNpcBrain_Controller controller)
        {
            m_controller = controller;
        }

        public override void AddStates()
        {
            onAddStates?.Invoke();
        }

        public override void InitializeAI()
        {
            onInitializeAI?.Invoke();
        }

        public override void Think(float delta)
        {
            onThink?.Invoke(delta);
        }

        public void SetThinkRate(float rate)
        {
            thinkRate = rate;
        }

        public void SetLastThinkTime(float time)
        {
            lastThinkTime = time;
        }
    }
}
