using System;
using System.Collections.Generic;

namespace Core.Resource
{
    [Serializable]
    public class AssetBundleCollectorGroup
    {
        /// <summary>
        /// 组名
        /// </summary>
        public string GroupName;

        /// <summary>
        /// 组描述
        /// </summary>
        public string GroupDesc;

        /// <summary>
        /// 组标签
        /// </summary>
        public string GroupAssetTags;

        /// <summary>
        /// 激活状态
        /// </summary>
        public bool Enabled = true;

        /// <summary>
        /// 收集器列表
        /// </summary>
        public List<AssetBundleCollector> Collectors = new();

        /// <summary>
        /// 创建一个收集器
        /// </summary>
        public void CreateCollector()
        {
            AssetBundleCollector collector = new();
            Collectors.Add(collector);

            AssetBundleCollectorSetting.SetDirtyStatus();
        }

        /// <summary>
        /// 移除一个收集器
        /// </summary>
        public void RemoveCollector(AssetBundleCollector collector)
        {
            Collectors.Remove(collector);

            AssetBundleCollectorSetting.SetDirtyStatus();
        }
    }
}