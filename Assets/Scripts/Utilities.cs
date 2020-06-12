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
