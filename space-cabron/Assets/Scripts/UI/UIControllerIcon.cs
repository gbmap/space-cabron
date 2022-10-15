using UnityEngine;
using UnityEngine.UI;

namespace SpaceCabron.UI
{
    public class UIControllerIcon : MonoBehaviour
    {
        public Image Image;
        public Sprite KeyboardIcon;
        public Sprite JoystickIcon;

        void Update()
        {
            var player = Rewired.ReInput.players.GetPlayer(0);
            foreach (var joystick in Rewired.ReInput.controllers.Joysticks) {
                Debug.Log(joystick.name);
                if (player.controllers.ContainsController(joystick)) {
                    Image.sprite = JoystickIcon;
                    return;
                }
            }
            Image.sprite = KeyboardIcon;
        }

    }
}