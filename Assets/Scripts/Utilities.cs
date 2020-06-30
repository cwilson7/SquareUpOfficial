using System.Collections.Generic;
using UnityEngine;

namespace CustomUtilities
{
	public static class Utils
	{
        public static void PopulateList<T>(List<T> list, string prefabFolderPath)
        {
            if (list == null) list = new List<T>();
            UnityEngine.Object[] prefabs = Resources.LoadAll(prefabFolderPath);
            foreach (UnityEngine.Object prefab in prefabs)
            {
                T prefabGO = (T)(object)prefab;
                list.Add(prefabGO);
            }
        }

        public static T CopyComponent<T>(T original, GameObject destination) where T : Component
        {
            System.Type type = original.GetType();
            Component copy = destination.AddComponent(type);
            System.Reflection.FieldInfo[] fields = type.GetFields();
            foreach (System.Reflection.FieldInfo field in fields)
            {
                field.SetValue(copy, field.GetValue(original));
            }
            return copy as T;
        }

        public static T FindParentWithClass<T>(Transform child)
        {
            T classScript = child.GetComponentInParent<T>();
            if (classScript == null)
            {
                return FindParentWithClass<T>(child.parent);
            }
            else return classScript;
        }


    }
}
