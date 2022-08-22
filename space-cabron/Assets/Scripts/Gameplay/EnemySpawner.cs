using System.Collections;
using UnityEngine;
using Gmap.Gameplay;
using Gmap.ScriptableReferences;
using Frictionless;
using System.Linq;
using SpaceCabron.Messages;
using SpaceCabron.Gameplay;
using Gmap.CosmicMusicUtensil;

namespace Gmap
{
    public class EnemySpawner : MonoBehaviour, ILevelConfigurable<LevelConfiguration>
    {
        public GameObjectPool EnemyPool;
        public GameObjectPool BossPool;

        public bool shouldSpawn = false;
        public int ScoreThreshold = int.MaxValue;

        private bool hasFiredWinMessage = false;

        private float initialTimer = 2f;

        void OnEnable()
        {
            MessageRouter.AddHandler<MsgOnScoreChanged>(Callback_OnScoreChanged);
            MessageRouter.AddHandler<MsgOnObjectDestroyed>(Callback_OnEnemyDestroyed);
            MessageRouter.AddHandler<MsgLevelStartedLoading>((msg) => { shouldSpawn = false; });
            MessageRouter.AddHandler<MsgLevelFinishedLoading>((msg) => { 
                initialTimer = Time.time + 2f;
                hasFiredWinMessage = false;

                if (LevelLoader.CurrentLevelConfiguration is LevelConfiguration)
                    shouldSpawn = true;
            });
        }

        private void Callback_OnEnemyDestroyed(MsgOnObjectDestroyed obj)
        {
            if (!obj.health.CompareTag("Enemy"))
                return;

            CheckIfShouldSpawnBoss();
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

            CheckIfShouldSpawnBoss();
        }

        private void CheckIfShouldSpawnBoss()
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

            GameObject boss = SpawnBossIfAny();
            if (boss == null)
                FireWinMessage();
            else
                StartCoroutine(PlayBossIntroAnimation(boss));
        }

        private GameObject SpawnBossIfAny()
        {
            if (BossPool == null || BossPool.Length == 0)
                return null;
            
            return SpawnNext(BossPool, 0.5f);
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
            BossPool = configuration.Gameplay.BossPool;
        }

        public void SpawnNext()
        {
            if (Time.time < initialTimer)
                return; 

            if (!shouldSpawn || EnemyPool.Length == 0)
                return;

            GameObject instance = SpawnNext(EnemyPool, Random.Range(0.15f, 0.85f));
        }

        public GameObject SpawnNext(GameObjectPool pool, float t)
        {
            return Instantiate(
                pool.GetNext(),
                GetEnemyPosition(t),
                Quaternion.identity
            );
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