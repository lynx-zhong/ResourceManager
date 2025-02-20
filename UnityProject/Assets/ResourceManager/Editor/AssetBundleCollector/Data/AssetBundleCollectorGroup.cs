using System;

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
        /// 激活状态
        /// </summary>
        public bool Enabled { get; private set; } = true;
    }
}