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
        public List<GameObject> Enemies;
        public GameObjectPool EnemyPool;
        public GameObjectPool BossPool;

        public bool shouldSpawn = false;
        private int scoreThreshold = int.MaxValue;

        void OnEnable()
        {
            MessageRouter router = ServiceFactory.Instance.Resolve<MessageRouter>();
            router.AddHandler<MsgOnScoreChanged>(Callback_OnScoreChanged);
        }

        void OnDisable()
        {
            UnsubscribeFromScoreChanged();
        }

        private void UnsubscribeFromScoreChanged()
        {
            MessageRouter router = ServiceFactory.Instance.Resolve<MessageRouter>();
            router.RemoveHandler<MsgOnScoreChanged>(Callback_OnScoreChanged);
        }

        private void Callback_OnScoreChanged(MsgOnScoreChanged obj)
        {
            if (obj.Score < scoreThreshold)
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
            ServiceFactory.Instance.Resolve<MessageRouter>().RaiseMessage(new MsgLevelWon());
        }

        private void DestroyAllEnemies()
        {
            GameObject.FindGameObjectsWithTag("Enemy").ToList().ForEach(e => {
                Health h = e.GetComponent<Health>();
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
            scoreThreshold = configuration.Gameplay.ScoreThreshold;
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
            if (!shouldSpawn || Enemies.Count == 0)
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