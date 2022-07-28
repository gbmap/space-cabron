using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Gmap.Gameplay;
using Gmap.ScriptableReferences;
using SpaceCabron.Gameplay;
using Frictionless;
using SpaceCabron.Messages;
using System.Linq;

namespace SpaceCabron
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
            if (obj.Score <= scoreThreshold)
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
            
            return Instantiate(BossPool.GetNext(), Vector3.zero, Quaternion.identity);
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

            int index = Random.Range(0, Enemies.Count);
            var enemy = EnemyPool.GetNext();

            var position = new Vector3(Random.Range(0.15f, 0.85f), 0.9f, 0f);
            position = Camera.main.ViewportToWorldPoint(position);
            position.z = 0f;

            Instantiate(enemy, position, Quaternion.identity);
        }
    }
}