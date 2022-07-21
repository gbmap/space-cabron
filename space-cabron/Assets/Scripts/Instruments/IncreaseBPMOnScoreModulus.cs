using Frictionless;
using Gmap.CosmicMusicUtensil;
using SpaceCabron.Scoreboard.Messages;
using UnityEngine;

namespace SpaceCabron.Instruments
{
    [RequireComponent(typeof(TurntableBehaviour))]
    public class IncreaseBPMOnScoreModulus : MonoBehaviour
    {
        public int ScoreModulus = 500;
        public int IncreaseValue = 5;
        TurntableBehaviour turntable;

        void Awake()
        {
            turntable = GetComponent<TurntableBehaviour>();
            ServiceFactory.Instance.Resolve<MessageRouter>().AddHandler<Scoreboard.Messages.MsgOnScoreChanged>(Callback_OnScoreChanged);
        }

        void OnDestroy()
        {
            ServiceFactory.Instance.Resolve<MessageRouter>().RemoveHandler<Scoreboard.Messages.MsgOnScoreChanged>(Callback_OnScoreChanged);
        }

        private void Callback_OnScoreChanged(MsgOnScoreChanged msg)
        {
            if (msg.Score % ScoreModulus == 0)
            {
                int choice = Random.Range(0, 3);
                switch (choice)
                {
                    default:
                    case 0:
                        turntable.BPM += IncreaseValue;
                        break;
                    case 1:
                        var m = new BreakNoteModifier();
                        if (Random.value > 0.85f)
                            m.NumberOfNotes = 3;
                        else
                            m.NumberOfNotes = 2;
                        turntable.SetMelody(m.Apply(turntable.Melody));
                        break;
                    case 2:
                        var s = new ShiftNoteModifier();
                        s.Step = Random.Range(-12, 12);
                        turntable.SetMelody(s.Apply(turntable.Melody));
                    break;

                }
            }
        }
    }
}