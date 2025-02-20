using System;
using System.Collections.Generic;

namespace Core.Resource
{
    [Serializable]
    public class AssetBundleCollectorPackage
    {
        /// <summary>
        /// 包裹名称
        /// </summary>
        public string PackageName = string.Empty;

        /// <summary>
        /// 包裹描述
        /// </summary>
        public string PackageDesc = string.Empty;

        /// <summary>
        /// 启用可寻址资源定位
        /// </summary>
        public bool EnableAddressable;

        /// <summary>
        /// 分组列表
        /// </summary>
        public List<AssetBundleCollectorGroup> Groups { get; private set; } = new();

        /// <summary>
        /// 创建组
        /// </summary>
        public void CreateGroup()
        {
            AssetBundleCollectorGroup group = new AssetBundleCollectorGroup();
            Groups.Add(group);
            AssetBundleCollectorSetting.SetDirtyStatus();
        }

        /// <summary>
        /// 移除组
        /// </summary>
        public void RemoveGroup(AssetBundleCollectorGroup group)
        {
            Groups.Remove(group);
            AssetBundleCollectorSetting.SetDirtyStatus();
        }
    }
}