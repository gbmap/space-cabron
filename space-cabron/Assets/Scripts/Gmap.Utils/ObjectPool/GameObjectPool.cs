using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace ObjectPool {
    public class GameObjectPool {
        GameObject m_prefab;

        List<GameObject> m_pool;
        Stack<GameObject> m_unused;

        public GameObjectPool( GameObject pPrefab ) {
            m_prefab = pPrefab;
        }

        void InstantiateAndAddToPool() {
            GameObject obj = GameObject.Instantiate(m_prefab) as GameObject;
            obj.SetActive(false);
            obj.AddComponent<ObjectPoolBehavior>().Destroyed += OnPoolableObjectDestroyed;
            m_unused.Push(obj);
            m_pool.Add(obj);
        }

        public void InitPool( int pQuantity ) {
            m_unused = new Stack<GameObject>(pQuantity);
            m_pool = new List<GameObject>(pQuantity);

            for (int i = 0; i < pQuantity; i++)
                InstantiateAndAddToPool();
        }

        public void DestroyPool() {
            foreach (GameObject obj in m_pool)
                GameObject.Destroy(obj);
        }

        public GameObject Instantiate( Vector3 pPos, Quaternion pRot ) {
            if (m_unused.Count == 0) {
                Debug.Log("[ObjPool::Instantiate] Maximum amount of enabled objects reached, instantiating more.");
                for (int i = 0; i < m_pool.Count * 0.25; i++)
                    InstantiateAndAddToPool();
            }

            GameObject obj = null;
            do 
            {
                obj = m_unused.Pop();
            } while (obj == null && m_unused.Count > 0);

            if (m_unused.Count == 0)
                return Instantiate(pPos, pRot);

            obj.SetActive(true);
            obj.transform.position = pPos;
            obj.transform.rotation = pRot;
            return obj;
        }

        void OnPoolableObjectDestroyed( GameObject pObj ) {
            pObj.transform.position = Vector3.zero;
            pObj.transform.rotation = Quaternion.identity;

            /* This call is redundant. */
            //pObj.SetActive(false);

            m_unused.Push(pObj);
        }

    }
}
