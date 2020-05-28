using UnityEngine;
using System.Collections;
using Useful;
using System.Collections.Generic;
using System;

namespace Managers
{
    public class ResourceManager : Singleton<ResourceManager>
    {
        Dictionary<Type, ScriptableObject> m_scriptableObjects = new Dictionary<Type,ScriptableObject>();        

        public static T GetScriptableObject<T>() where T : ScriptableObject
        {
            Type t = typeof(T);
            
            if (Instance == null)
            {
                T resource = Resources.Load<T>("Data/" + t.ToString());
                return resource;
            }

            if (!Instance.m_scriptableObjects.ContainsKey(t))
            {
                T resource = Resources.Load<T>("Data/" + t.ToString());
                Instance.m_scriptableObjects[t] = resource;
                return resource;
            }

            return (T)Instance.m_scriptableObjects[t];
        }
    }
}