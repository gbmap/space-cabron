using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Gmap.Gameplay;
using Gmap.ScriptableReferences;
using Frictionless;
using System.Linq;
using SpaceCabron.Messages;

namespace Gmap
{
    public class EnemySpawner : MonoBehaviour, ILevelConfigurable<LevelConfiguration>
    {
        public GameObjectPool EnemyPool;
        public GameObjectPool BossPool;

        public bool shouldSpawn = false;
        public int ScoreThreshold = int.MaxValue;

        void OnEnable()
        {
            MessageRouter.AddHandler<MsgOnScoreChanged>(Callback_OnScoreChanged);
            MessageRouter.AddHandler<MsgLevelStartedLoading>((msg) => { shouldSpawn = false; });
            MessageRouter.AddHandler<MsgLevelFinishedLoading>((msg) => { shouldSpawn = true; });
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

            if (!shouldSpawn)
                return;

            UnsubscribeFromScoreChanged();
            SetShouldSpawn(false);
            DestroyAllEnemies();
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

        private bool GetShouldSpawn()
        {
            return shouldSpawn;
        }

        public void Configure(LevelConfiguration configuration)
        {
            ScoreThreshold = configuration.Gameplay.ScoreThreshold;
            EnemyPool = configuration.Gameplay.EnemyPool;
            BossPool = configuration.Gameplay.BossPool;
            StartCoroutine(WaitAndActivate());
        }

        private IEnumerator WaitAndActivate()
        {
            SetShouldSpawn(false);
            yield return new WaitForSeconds(2f);
            SetShouldSpawn(true);
        }

        public void SpawnNext()
        {
            if (!shouldSpawn || EnemyPool.Length == 0)
                return;

            SpawnNext(EnemyPool, Random.Range(0.15f, 0.85f));
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
            return GetEnemyPosition(Random.Range(0.15f, 0.85f));
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