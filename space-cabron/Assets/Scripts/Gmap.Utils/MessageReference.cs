using System;
using System.Linq;
using System.Reflection;
using Frictionless;
using UnityEngine;

namespace Gmap.Utils
{
    [System.Serializable]
    public class MessageReference<T> : ISerializationCallbackReceiver
    {
        public string messageToRaise;

        [SerializeField]
        private string[] parameterNames = new string[10];
        private object[] parameters = new object[10];

        [SerializeField]
        private string[] serializedParameters = new string[10];

        [SerializeField]
        private GameObject[] parameterGameObjects = new GameObject[10];

        public void Raise()
        {
            Type messageType = typeof(T).Assembly.GetTypes().Where(t=>t.Name == messageToRaise).First();
            // Type messageType = Type.GetType(messageToRaise);
            object msg = Activator.CreateInstance(messageType);
            messageType.GetFields(BindingFlags.Public | BindingFlags.Instance).ToList().ForEach(field =>
            {
                int index = Array.IndexOf(parameterNames, field.Name);
                object parameter = parameters[index];
                if (parameter is string && ((string)parameter) == "go")
                    field.SetValue(msg, parameterGameObjects[index]);
                else
                    field.SetValue(msg, parameter);
            });
            MessageRouter.RaiseMessage(msg);
        }

        public static Type[] GetMessageTypes()
        {
            Type baseType = typeof(T);
            return baseType.Assembly
                            .GetTypes()
                            .Where(t => t.Namespace == baseType.Namespace)
                            .ToArray();
        }

        public void OnBeforeSerialize()
        {
        }

        public void OnAfterDeserialize()
        {
            if (parameters == null || parameters.Length != serializedParameters.Length)
                parameters = new object[serializedParameters.Length];

            for (int i = 0; i < serializedParameters.Length; i++)
            {
                if (string.IsNullOrEmpty(serializedParameters[i]))
                    continue;
                else if (serializedParameters[i] == "go")
                    parameters[i] = parameterGameObjects[i];
                else
                    parameters[i] = SimpleSerializer.Deserialize(serializedParameters[i]);
            }
        }

        public static string GetSerializedParameter(MessageReference<T> r, string name)
        {
            int index = System.Array.IndexOf(r.parameterNames, name);
            if (index < 0)
                throw new System.Exception($"Parameter not found with name {name}");
            return r.serializedParameters[index];
        }

    }
}