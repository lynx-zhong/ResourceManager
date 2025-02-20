using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Core.Resource
{
    public class AssetBundleCollectorSetting : ScriptableObject
    {
        private static AssetBundleCollectorSetting instance;

        public static AssetBundleCollectorSetting Instance
        {
            get
            {
                if (instance)
                    return instance;

                var guids = AssetDatabase.FindAssets($"t:{nameof(AssetBundleCollectorSetting)}");
                if (guids.Length > 0)
                {
                    string filePath = AssetDatabase.GUIDToAssetPath(guids[0]);
                    instance = AssetDatabase.LoadAssetAtPath<AssetBundleCollectorSetting>(filePath);
                }


                instance = CreateInstance<AssetBundleCollectorSetting>();
                AssetDatabase.CreateAsset(instance, "Assets/AssetBundleCollectorSetting.asset");
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();

                return instance;
            }
        }

        /// <summary>
        /// 所有的包
        /// </summary>
        public List<AssetBundleCollectorPackage> Packages { get; private set; } = new();

        /// <summary>
        /// 标脏
        /// </summary>
        public static bool IsDirty { get; private set; } = false;


        /// <summary>
        /// 创建一个包
        /// </summary>
        public void CreatePackage(string packageName = "NewPackage")
        {
            AssetBundleCollectorPackage collectorPackage = new();
            collectorPackage.PackageName = packageName;
            Packages.Add(collectorPackage);
            IsDirty = true;
        }

        /// <summary>
        /// 移除一个包
        /// </summary>
        public void RemovePackage(AssetBundleCollectorPackage package)
        {
            Packages.Remove(package);
            IsDirty = true;
        }

        /// <summary>
        /// 标脏
        /// </summary>
        public static void SetDirtyStatus()
        {
            IsDirty = true;
        }
    }
}