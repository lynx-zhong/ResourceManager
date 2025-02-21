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
        public bool EnableAddressable = true;

        /// <summary>
        /// 包含资源GUID数据
        /// </summary>
        public bool IncludeAssetGUID = false;

        /// <summary>
        /// 自动收集所有着色器（所有着色器存储在一个资源包内）
        /// </summary>
        public bool AutoCollectShaders = true;

        /// <summary>
        /// 分组列表
        /// </summary>
        public List<AssetBundleCollectorGroup> Groups = new();

        /// <summary>
        /// 创建组
        /// </summary>
        public AssetBundleCollectorGroup CreateGroup()
        {
            AssetBundleCollectorGroup group = new AssetBundleCollectorGroup();
            group.GroupName = $"DefaultGroup_{Groups.Count}";
            Groups.Add(group);
            AssetBundleCollectorSetting.SetDirtyStatus();
            return group;
        }

        /// <summary>
        /// 移除组
        /// </summary>
        public void RemoveGroup(AssetBundleCollectorGroup group)
        {
            Groups.Remove(group);
            AssetBundleCollectorSetting.SetDirtyStatus();
        }

        /// <summary>
        /// 获取第一个组
        /// </summary>
        public AssetBundleCollectorGroup GetFirstGroup()
        {
            if (Groups.Count is 0)
                return null;

            return Groups[0];
        }
    }
}