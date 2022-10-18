using Frictionless;
using Gmap.Gameplay;
using SpaceCabron.Gameplay.Multiplayer;
using SpaceCabron.Messages;
using UnityEngine;

public class PlayerWaitToSpawn : MonoBehaviour
{
    Rewired.Player player;
    private int Index
    {
        get {
            return Mathf.Clamp(gameObject.name[gameObject.name.Length - 1] - '0', 0, 1);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        player = Rewired.ReInput.players.GetPlayer(Index);
        MessageRouter.AddHandler<MsgLevelWon>(Callback_LevelWon);
    }

    private void Callback_LevelWon(MsgLevelWon obj)
    {
        Destroy(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        var drones = GameObject.FindGameObjectsWithTag("Drone");
        bool hasPressedPause = player.GetButtonDown("Pause");
        if (MultiplayerManager.PlayerCount <= 1 || drones.Length == 0)
            return;

        int droneIndex = Random.Range(0, drones.Length);
        var drone = drones[droneIndex];
        Health h = drone.GetComponent<Health>();
        h.Destroy();

        MessageRouter.RaiseMessage(new MsgSpawnPlayer
        {
            PlayerIndex = Index,
            Position = drone.transform.position
        });

        Destroy(this.gameObject);
    }
}
