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
        MessageRouter.AddHandler<MsgOnPlayerSpawned>(Callback_OnPlayerSpawned);
        MessageRouter.AddHandler<MsgOnObjectDestroyed>(Callback_OnObjectDestroyed);
        MessageRouter.AddHandler<MsgLevelFinishedLoading>(Callback_OnLevelFinishedLoading);
    }

    void OnDisable()
    {
        MessageRouter.RemoveHandler<MsgLevelFinishedLoading>(Callback_OnLevelFinishedLoading);
        MessageRouter.RemoveHandler<MsgOnPlayerSpawned>(Callback_OnPlayerSpawned);
        MessageRouter.RemoveHandler<MsgOnObjectDestroyed>(Callback_OnObjectDestroyed);
    }

    private void Callback_OnPlayerSpawned(MsgOnPlayerSpawned msg)
    {
        Add(msg.Player);
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
        if (players.Contains(player))
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

        Vector2 averagePosition = Vector2.zero;
        int count = 0;
        foreach (GameObject player in players)
        {
            if (player == null)
                continue;
            averagePosition += (Vector2)player.transform.position;
            count++;
        }

        if (count != 0)
            averagePosition /= count;

        averagePosition += Vector2.up * 1.65f;

        Vector2 pos = transform.position;

        Vector2 delta = Vector3.ClampMagnitude(averagePosition-pos, Speed);

        Vector2 target = pos + delta*Time.fixedDeltaTime;
        target.x = Mathf.Clamp(target.x, -2f, 2f);
        target.y = Mathf.Clamp(target.y, -2f, 2f);

        transform.position = new Vector3(target.x, target.y, transform.position.z);
    }
}
