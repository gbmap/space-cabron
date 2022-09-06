using System.Collections;
using UnityEngine;
using Gmap.Gameplay;
using Gmap.ScriptableReferences;
using Frictionless;
using System.Linq;
using SpaceCabron.Messages;
using SpaceCabron.Gameplay;
using Gmap.CosmicMusicUtensil;
using System.Collections.Generic;

namespace Gmap
{
    public class EnemySpawner : MonoBehaviour, ILevelConfigurable<LevelConfiguration>
    {
        public GameObjectPool EnemyPool;

        private int maxEnemiesAlive;
        public int MaxEnemiesAlive 
        { 
            get { return maxEnemiesAlive + GameObject.FindGameObjectsWithTag("Drone").Length/3; } 
            private set { maxEnemiesAlive = value; } 
        }

        public int EnemiesAlive { get; private set; }

        public bool shouldSpawn = false;
        public int ScoreThreshold = int.MaxValue;

        private bool hasFiredWinMessage = false;

        private float initialTimer = 2f;

        private List<GameObject> enemies = new List<GameObject>();

        void Awake()
        {
            EnemiesAlive = 0;
            MaxEnemiesAlive = 5;
        }

        void OnEnable()
        {
            MessageRouter.AddHandler<MsgOnScoreChanged>(Callback_OnScoreChanged);
            MessageRouter.AddHandler<MsgOnObjectDestroyed>(Callback_OnEnemyDestroyed);
            MessageRouter.AddHandler<MsgLevelStartedLoading>((msg) => { shouldSpawn = false; });
            MessageRouter.AddHandler<MsgLevelFinishedLoading>((msg) => { 
                initialTimer = Time.time + 2f;
                hasFiredWinMessage = false;
                EnemiesAlive = 0;

                if (LevelLoader.CurrentLevelConfiguration is LevelConfiguration)
                    shouldSpawn = true;
            });
        }

        private void Callback_OnEnemyDestroyed(MsgOnObjectDestroyed obj)
        {
            if (!obj.health.CompareTag("Enemy"))
                return;

            var enemy = obj.health.gameObject;
            if (enemies.Contains(enemy))
            {
                enemies.Remove(enemy);
                EnemiesAlive = Mathf.Max(0, EnemiesAlive - 1);
            }

            CheckIfLevelWon();
        }

        void OnDisable()
        {
            UnsubscribeFromScoreChanged();
        }

        private void UnsubscribeFromScoreChanged()
        {
            MessageRouter.RemoveHandler<MsgOnScoreChanged>(Callback_OnScoreChanged);
        }

        private void Callback_OnScoreChanged(MsgOnScoreChanged obj)
        {
            if (obj.Score < ScoreThreshold)
                return;

            if (shouldSpawn)
                SetShouldSpawn(false);

            CheckIfLevelWon();
        }

        private void CheckIfLevelWon()
        {
            if (shouldSpawn)
                return;

            GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
            var allEnemiesAreDead = enemies.All(e => { 
                Health h = e.GetComponent<Health>();
                return h == null || h.IsBeingDestroyed;
            });
            if (enemies.Length > 0 && !allEnemiesAreDead)
                return;

            if (hasFiredWinMessage)
                return;

            FireWinMessage();
        }

        private IEnumerator PlayBossIntroAnimation(GameObject boss)
        {
            yield break;
        }

        private void FireWinMessage()
        {
            MessageRouter.RaiseMessage(new MsgLevelWon());
            hasFiredWinMessage = true;
        }

        private void DestroyAllEnemies()
        {
            GameObject.FindGameObjectsWithTag("Enemy").ToList().ForEach(e => {
                Health h = e.GetComponentInChildren<Health>();
                while (h != null && h.CurrentHealth > 0)
                    h.TakeDamage(null, null);
            }); 
        }

        private void SetShouldSpawn(bool v)
        {
            shouldSpawn = v;
        }

        public void Configure(LevelConfiguration configuration)
        {
            ScoreThreshold = configuration.Gameplay.ScoreThreshold;
            EnemyPool = configuration.Gameplay.EnemyPool;
            MaxEnemiesAlive = configuration.Gameplay.MaxEnemiesAlive;
        }

        public void SpawnNext()
        {
            if (Time.time < initialTimer)
                return; 

            if (!shouldSpawn || EnemyPool.Length == 0)
                return;

            if (EnemiesAlive >= MaxEnemiesAlive)
                return;

            EnemiesAlive++;
            GameObject instance = SpawnNext(EnemyPool, GetSpawnX());
        }

        private float GetSpawnX()
        {
            float t = 0f;
            Collider2D c = null;
            int tries = 0;
            do
            {
                t = Random.Range(0.15f, 0.85f);
                Vector3 p = GetEnemyPosition(t);
                c = Physics2D.OverlapCircle(p, 0.5f);
            } while (c != null && c.gameObject.CompareTag("Enemy") && tries++ < 10);
            return t;
        }

        public GameObject SpawnNext(GameObjectPool pool, float t)
        {
            var enemy = Instantiate(
                pool.GetNext(),
                GetEnemyPosition(t),
                Quaternion.identity
            );
            enemies.Add(enemy);
            return enemy;
        }
        
        private Vector3 GetRandomEnemyPosition()
        {
            return GetEnemyPosition(Random.Range(0.2f, 0.8f));
        }

        private Vector3 GetEnemyPosition(float t)
        {
            Vector3 p = new Vector3(Mathf.Clamp01(t), 0.9f, 0f);
            Vector3 pos = Camera.main.ViewportToWorldPoint(p);
            pos.z = 0f;
            return pos;
        }
    }
}