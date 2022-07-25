using Frictionless;
using Gmap.CosmicMusicUtensil;
using UnityEngine;

namespace SpaceCabron.Instruments
{
    public abstract class OnScoreModulusBehaviour : MonoBehaviour
    {
        public int ScoreModulus = 500;
        public bool Geometric = false;

        protected ITurntable turntable;
        int counter = 1;

        protected abstract void HandleEvent();

        protected virtual void Awake()
        {
            turntable = GetComponent<ITurntable>();
            ServiceFactory.Instance.Resolve<MessageRouter>().AddHandler<Messages.MsgOnScoreChanged>(Callback_OnScoreChanged);
        }

        protected virtual void OnDestroy()
        {
            ServiceFactory.Instance.Resolve<MessageRouter>().RemoveHandler<Messages.MsgOnScoreChanged>(Callback_OnScoreChanged);
        }

        private void Callback_OnScoreChanged(Messages.MsgOnScoreChanged msg)
        {
            bool target; 
            if (Geometric)
                target = (msg.Score % (ScoreModulus*counter*2)) == 0;
            else 
                target = msg.Score % ScoreModulus == 0;

            if (target)
            {
                HandleEvent();
                counter++;
            }
        }
    }


    [RequireComponent(typeof(TurntableBehaviour))]
    public class IncreaseBPMOnScoreModulus : OnScoreModulusBehaviour
    {
        public int IncreaseValue = 5;

        protected override void HandleEvent()
        {
            if (turntable != null)
                turntable.BPM += IncreaseValue;
        }
    }
}