using System.Collections;
using System.Collections.Generic;
using Gmap.ScriptableReferences;
using UnityEngine;

namespace SpaceCabron
{
    public class EnemySpawner : MonoBehaviour
    {
        public List<GameObject> Enemies;
        public GameObjectPool EnemyPool;

        public void SpawnNext()
        {
            if (Enemies.Count == 0)
                return;

            int index = Random.Range(0, Enemies.Count);
            var enemy = EnemyPool.GetNext();

            var position = new Vector3(Random.Range(0.15f, 0.85f), 0.9f, 0f);
            position = Camera.main.ViewportToWorldPoint(position);
            position.z = 0f;

            Instantiate(enemy, position, Quaternion.identity);
        }
    }
}