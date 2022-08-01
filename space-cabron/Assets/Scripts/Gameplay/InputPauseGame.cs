using Frictionless;
using Gmap.Gameplay;
using SpaceCabron.Gameplay;
using SpaceCabron.Messages;
using UnityEngine;

namespace SpaceCabron
{
    public class InputPauseGame : MonoBehaviour, IBrainHolder<InputState>
    {
        IBrain<InputState> brain;
        public IBrain<InputState> Brain
        {
            get { return brain; }
            set { brain = value; }
        }

        // Update is called once per frame
        void Update()
        {
            InputState input = Brain.GetInputState(new InputStateArgs
            {
                Object = gameObject
            });
            if (input.Pause)
            {
                MessageRouter.RaiseMessage(
                    new MsgPauseGame(Mathf.Approximately(Time.timeScale, 1f))
                );
            }
        }
    }
}