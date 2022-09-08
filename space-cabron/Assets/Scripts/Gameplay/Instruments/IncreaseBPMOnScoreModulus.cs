using Frictionless;
using Gmap.CosmicMusicUtensil;
using UnityEngine;
using SpaceCabron.Messages;
using Gmap.Gameplay;
using SpaceCabron.Gameplay;

namespace Gmap.Instruments
{
    public abstract class OnScoreModulusBehaviour : MonoBehaviour
    {
        public int ScoreModulus = 500;
        public bool Geometric = false;

        protected ITurntable turntable;
        int counter = 1;
        int lastScore = 0;
        int scoreAccumulator = 0;

        protected abstract void HandleEvent();

        protected virtual void Awake()
        {
            turntable = GetComponent<ITurntable>();
            MessageRouter.AddHandler<SpaceCabron.Messages.MsgOnScoreChanged>(Callback_OnScoreChanged);
        }

        protected virtual void OnDestroy()
        {
            MessageRouter.RemoveHandler<SpaceCabron.Messages.MsgOnScoreChanged>(Callback_OnScoreChanged);
        }

        private void Callback_OnScoreChanged(SpaceCabron.Messages.MsgOnScoreChanged msg)
        {
            int distance = Mathf.Abs(lastScore - msg.Score);
            scoreAccumulator += distance;
            if (scoreAccumulator >= ScoreModulus)
            {
                HandleEvent();
                scoreAccumulator = 0;
                counter++;
            }
            lastScore = msg.Score;

            // bool target; 
            // if (Geometric)
            //     target = (msg.Score % (ScoreModulus*counter*2)) == 0;
            // else 
            //     target = msg.Score % ScoreModulus == 0;

            // if (target)
            // {
            //     HandleEvent();
            //     counter++;
            // }
        }
    }


    [RequireComponent(typeof(TurntableBehaviour))]
    public class IncreaseBPMOnScoreModulus : OnScoreModulusBehaviour, ILevelConfigurable<LevelConfiguration>
    {
        public int IncreaseValue = 5;

        public void Configure(LevelConfiguration configuration)
        {
            EnemySpawner spawner = GetComponentInParent<EnemySpawner>();
            if (spawner == null)
                return;

            IncreaseValue = configuration.Gameplay.EnemyBPMIncreaseValue;
            ScoreModulus = configuration.Gameplay.EnemyBPMScoreModulusToIncrease;
        }

        protected override void HandleEvent()
        {
            if (turntable != null)
                turntable.BPM += IncreaseValue;
        }
    }
}