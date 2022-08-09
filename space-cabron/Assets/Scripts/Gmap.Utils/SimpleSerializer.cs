using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gmap.Utils
{
    public class SimpleSerializer : MonoBehaviour
    {
        public static string Serialize(object parameter)
        {
            if (parameter == null)
                return null;

            if (parameter.GetType() == typeof(int))
                return "i"+parameter.ToString();
            if (parameter.GetType() == typeof(string))
                return "s"+parameter.ToString();
            if (parameter.GetType() == typeof(float))
            {
                byte[] bytes = BitConverter.GetBytes((float)parameter);
                return "f"+Convert.ToBase64String(bytes);
            }
            if (parameter.GetType() == typeof(bool))
                return "b"+parameter.ToString();
            if (parameter.GetType() == typeof(Vector2))
            {
                Vector2 v = (Vector2)parameter;
                string f1 = Serialize(v.x);
                string f2 = Serialize(v.y);
                return "v2"+f1+";"+f2;
            }
            if (parameter.GetType() == typeof(Vector3))
            {
                Vector3 v = (Vector3)parameter;
                string f1 = Serialize(v.x);
                string f2 = Serialize(v.y);
                string f3 = Serialize(v.z);
                return "v3"+f1+";"+f2+";"+f3;
            }
            if (parameter.GetType() == typeof(GameObject))
            {
                return "go";
            }
            
            throw new System.Exception("Unsupported parameter type: "+parameter.GetType().ToString());
        }

        public static object Deserialize(string parameter)
        {
            if (parameter.StartsWith("i"))
                return int.Parse(parameter.Substring(1));
            if (parameter.StartsWith("s"))
                return parameter.Substring(1);
            if (parameter.StartsWith("f"))
            {
                byte[] bytes = Convert.FromBase64String(parameter.Substring(1));
                return BitConverter.ToSingle(bytes);
            }
            if (parameter.StartsWith("b"))
                return bool.Parse(parameter.Substring(1));  
            if (parameter.StartsWith("v2"))
            {
                string v = parameter.Substring(2);
                string[] floats = v.Split(';');
                return new Vector2(
                    (float)Deserialize(floats[0]),
                    (float)Deserialize(floats[1])
                );
            }
            if (parameter.StartsWith("v3"))
            {
                string v = parameter.Substring(2);
                string[] floats = v.Split(';');
                return new Vector3(
                    (float)Deserialize(floats[0]),
                    (float)Deserialize(floats[1]),
                    (float)Deserialize(floats[2])
                );
            }
            if (parameter.StartsWith("go"))
            {
                return null;
            }
            
            return null;
        }

        public static string SerializeDefault(Type fieldType)
        {
            return Serialize(Default(fieldType));
        }

        public static object Default(Type fieldType)
        {
            if (fieldType == typeof(int))
                return 0;
            if (fieldType == typeof(string))
                return "";
            if (fieldType == typeof(float))
                return 0f;
            if (fieldType == typeof(bool))
                return false;
            if (fieldType == typeof(Vector2))
                return Vector2.zero;
            if (fieldType == typeof(Vector3))
                return Vector3.zero;
            
            return null;
        }

        public static bool IsSupported(Type type)
        {
            return type == typeof(int) ||
                   type == typeof(string) ||
                   type == typeof(float) ||
                   type == typeof(bool) ||
                   type == typeof(Vector2) ||
                   type == typeof(Vector3) ||
                   type == typeof(GameObject);
        }
    }
}