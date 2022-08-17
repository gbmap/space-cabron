using System.Collections.Generic;
using System.Linq;
using Frictionless;
using Gmap.Gameplay;
using SpaceCabron.Messages;
using UnityEngine;

public class CameraFollowPlayer : MonoBehaviour
{
    public float Speed = 1f;
    List<GameObject> players = new List<GameObject>();

    void Start()
    {
        Add(GameObject.FindGameObjectsWithTag("Player"));
    }

    void OnEnable()
    {
        MessageRouter.AddHandler<MsgSpawnPlayer>(Callback_OnSpawnPlayer);
        MessageRouter.AddHandler<MsgOnObjectDestroyed>(Callback_OnObjectDestroyed);
        MessageRouter.AddHandler<MsgLevelFinishedLoading>(Callback_OnLevelFinishedLoading);
    }

    void OnDisable()
    {
        MessageRouter.RemoveHandler<MsgSpawnPlayer>(Callback_OnSpawnPlayer);
        MessageRouter.RemoveHandler<MsgLevelFinishedLoading>(Callback_OnLevelFinishedLoading);
    }

    private void Callback_OnSpawnPlayer(MsgSpawnPlayer msg)
    {
        msg.OnSpawned += (GameObject player) =>
        {
            players.Add(player);
        };
    }

    private void Callback_OnObjectDestroyed(MsgOnObjectDestroyed obj)
    {
        if (obj.health == null)
            return;

        if (!obj.health.CompareTag("Player"))
            return;
        
        if (players.Contains(obj.health.gameObject))
            players.Remove(obj.health.gameObject);
    }


    private void Callback_OnLevelFinishedLoading(MsgLevelFinishedLoading obj)
    {
        Add(GameObject.FindGameObjectsWithTag("Player"));
    }

    private void Add(GameObject player)
    {
        if (!players.Contains(player))
            return;
        players.Add(player);
    }

    private void Add(GameObject[] players)
    {
        players = players.Where(p => !this.players.Contains(p)).ToArray();
        if (players.Length == 0)
            return;
        this.players.AddRange(players);
    }

    void FixedUpdate()
    {
        if (players.Count == 0)
            return;

        Vector2 mean = players.Select(p => p.transform.position)
                              .Aggregate((a, b) => a + b) / players.Count;

        mean += Vector2.up * 1.65f;

        Vector2 pos = transform.position;

        Vector2 delta = Vector3.ClampMagnitude(mean-pos, Speed);

        Vector2 target = pos + delta*Time.fixedDeltaTime;
        target.x = Mathf.Clamp(target.x, -2f, 2f);
        target.y = Mathf.Clamp(target.y, -2f, 2f);

        transform.position = new Vector3(target.x, target.y, transform.position.z);
    }
}
