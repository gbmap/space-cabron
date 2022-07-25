using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Gmap.Gameplay;
using Gmap.ScriptableReferences;
using SpaceCabron.Gameplay;

namespace SpaceCabron
{
    public class EnemySpawner : MonoBehaviour, ILevelConfigurable<LevelConfiguration>
    {
        public List<GameObject> Enemies;
        public GameObjectPool EnemyPool;

        private bool shouldSpawn = false;

        public void Configure(LevelConfiguration configuration)
        {
            StartCoroutine(WaitAndActivate());
        }

        private IEnumerator WaitAndActivate()
        {
            shouldSpawn = false;
            yield return new WaitForSeconds(2f);
            shouldSpawn = true;
        }

        public void SpawnNext()
        {
            if (!shouldSpawn || Enemies.Count == 0)
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