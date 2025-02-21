using System;
using System.Collections.Generic;
using UnityEngine.UIElements;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;

namespace Core.Resource
{
    public class AssetBundleCollectorWindow : EditorWindow
    {
        // root
        public VisualTreeAsset visualTreeAsset;

        // toolbar
        private Button saveButton;

        // global settings
        private VisualElement globalSettingContainer;

        // package settings
        private Button packageSettingBtn;
        private VisualElement packageSettingContainer;
        private Toggle enableAddressable;
        private Toggle includeAssetGUID;
        private Toggle autoCollectShaders;

        // package
        private VisualElement packageContainer;
        private ListView packageListView;

        // groups
        private TextField packageNameText;
        private TextField packageDescText;
        private ListView groupListView;

        // collectors
        private TextField groupNameText;
        private TextField groupDescText;
        private ScrollView collectorListView;

        // data
        private AssetBundleCollectorPackage selectedPackage;
        private AssetBundleCollectorGroup selectedGroup;


        [MenuItem("Tools/AssetBundle Collector Window")]
        public static void OpenWindow()
        {
            AssetBundleCollectorWindow window = GetWindow<AssetBundleCollectorWindow>(typeof(AssetBundleCollectorWindow));
            window?.Close();

            window = GetWindow<AssetBundleCollectorWindow>(typeof(AssetBundleCollectorWindow));
            window.minSize = new Vector2(800, 600);
        }

        public void CreateGUI()
        {
            visualTreeAsset.CloneTree(rootVisualElement);

            // toolbar
            {
                saveButton = rootVisualElement.Q<Button>("SaveBtn");
                saveButton.clicked += OnSaveBtnClick;
            }

            // global 设置
            {
                var globalSettingBtn = rootVisualElement.Q<Button>("GlobalSettingsButton");
                globalSettingBtn.clicked += OnGlobalSettingBtnClick;

                globalSettingContainer = rootVisualElement.Q<VisualElement>("PublicContainer1");
                globalSettingContainer.style.display = DisplayStyle.None;

                // show package
                var showPackage = rootVisualElement.Q<Toggle>("ShowPackages");
                showPackage.SetValueWithoutNotify(AssetBundleCollectorSetting.Instance.ShowPackage);
                showPackage.RegisterValueChangedCallback(OnShowPackageToggleValueChanged);
            }

            // package 设置
            {
                packageSettingContainer = rootVisualElement.Q<VisualElement>("PublicContainer2");
                packageSettingContainer.style.display = DisplayStyle.None;

                // 激活寻址
                enableAddressable = packageSettingContainer.Q<Toggle>("EnableAddressable");
                enableAddressable.RegisterValueChangedCallback(OnEnableAddressableChanged);

                // 资源 GUID
                includeAssetGUID = packageSettingContainer.Q<Toggle>("IncludeAssetGUID");
                includeAssetGUID.RegisterValueChangedCallback(OnIncludeAssetGUIDChanged);

                // 收集 shaders
                autoCollectShaders = packageSettingContainer.Q<Toggle>("AutoCollectShaders");
                autoCollectShaders.RegisterValueChangedCallback(OnAutoCollectShadersChanged);

                //
                packageSettingBtn = rootVisualElement.Q<Button>("PackageSettingsButton");
                packageSettingBtn.clicked += OnPackageSettingChanged;
            }

            // 包
            {
                packageContainer = rootVisualElement.Q<VisualElement>("PackageContainer");

                //
                packageListView = packageContainer.Q<ListView>("PackageListView");
                packageListView.makeItem = MakePackageListViewItem;
                packageListView.bindItem = BindPackageListViewItem;
                packageListView.onSelectionChange += OnPackageListViewSelectionChange;

                // 添加包
                var addPackageButton = packageContainer.Q<Button>("AddBtn");
                addPackageButton.clicked += OnAddPackageBtnClick;

                // 移除包
                var removePackageButton = packageContainer.Q<Button>("RemoveBtn");
                removePackageButton.clicked += OnRemovePackageBtnClick;
            }

            // 组
            {
                var groupContainer = rootVisualElement.Q<VisualElement>("GroupContainer");
                packageNameText = groupContainer.Q<TextField>("PackageName");
                packageNameText.RegisterValueChangedCallback(OnPackageNameChange);

                packageDescText = groupContainer.Q<TextField>("PackageDesc");
                packageDescText.RegisterValueChangedCallback(OnPackageDescChange);

                // list
                groupListView = groupContainer.Q<ListView>("GroupListView");
                groupListView.makeItem = MakeGroupListViewItem;
                groupListView.bindItem = BindGroupListViewItem;
                groupListView.onSelectionChange += OnGroupListViewSelectionChange;

                // 添加包
                var addGroupBtn = groupContainer.Q<Button>("AddBtn");
                addGroupBtn.clicked += OnAddGroupBtnClick;

                // 移除包
                var removeGroupBtn = groupContainer.Q<Button>("RemoveBtn");
                removeGroupBtn.clicked += OnRemoveGroupBtnClick;
            }

            // 收集器
            {
                // 组名
                var collectorContainer = rootVisualElement.Q<VisualElement>("CollectorContainer");
                groupNameText = collectorContainer.Q<TextField>("GroupName");
                groupNameText.RegisterValueChangedCallback(OnGroupNameChange);

                // 组描述
                groupDescText = collectorContainer.Q<TextField>("GroupDesc");
                groupDescText.RegisterValueChangedCallback(OnGroupDescChange);

                // 组标签
                var groupTagsText = collectorContainer.Q<TextField>("GroupTags");
                groupTagsText.RegisterValueChangedCallback(OnGroupTagsChange);

                // 添加收集器
                var addCollectorBtn = collectorContainer.Q<Button>("AddBtn");
                addCollectorBtn.clicked += OnAddCollectorsBtnClick;

                // list
                collectorListView = collectorContainer.Q<ScrollView>("CollectorScrollView");
                collectorListView.style.height = new Length(100, LengthUnit.Percent);
                collectorListView.viewDataKey = "scrollView";
            }

            InitializeWindow();
        }

        private void Update()
        {
            if (saveButton != null)
            {
                if (AssetBundleCollectorSetting.IsDirty)
                {
                    if (saveButton.enabledSelf == false)
                        saveButton.SetEnabled(true);
                }
                else
                {
                    if (saveButton.enabledSelf)
                        saveButton.SetEnabled(false);
                }
            }
        }

        private void OnSaveBtnClick()
        {
            AssetBundleCollectorSetting.Instance.Save();
        }

        private void OnGlobalSettingBtnClick()
        {
            var displayStyle = globalSettingContainer.style.display;
            globalSettingContainer.style.display = displayStyle != DisplayStyle.Flex ? DisplayStyle.Flex : DisplayStyle.None;
        }

        private void OnShowPackageToggleValueChanged(ChangeEvent<bool> evt)
        {
            AssetBundleCollectorSetting.Instance.ShowPackage = evt.newValue;
            packageContainer.style.display = evt.newValue ? DisplayStyle.Flex : DisplayStyle.None;
        }

        private void OnPackageSettingChanged()
        {
            var displayStyle = packageSettingContainer.style.display;
            packageSettingContainer.style.display = displayStyle != DisplayStyle.Flex ? DisplayStyle.Flex : DisplayStyle.None;
        }

        private void OnEnableAddressableChanged(ChangeEvent<bool> evt)
        {
            if (selectedPackage == null)
                return;

            selectedPackage.EnableAddressable = evt.newValue;
        }

        private void OnIncludeAssetGUIDChanged(ChangeEvent<bool> evt)
        {
            if (selectedPackage == null)
                return;

            selectedPackage.IncludeAssetGUID = evt.newValue;
        }

        private void OnAutoCollectShadersChanged(ChangeEvent<bool> evt)
        {
            if (selectedPackage == null)
                return;

            selectedPackage.AutoCollectShaders = evt.newValue;
        }

        private void ShowPackageSetting()
        {
            if (selectedPackage == null)
            {
                enableAddressable.SetValueWithoutNotify(false);
                includeAssetGUID.SetValueWithoutNotify(false);
                autoCollectShaders.SetValueWithoutNotify(false);
            }
            else
            {
                enableAddressable.SetValueWithoutNotify(selectedPackage.EnableAddressable);
                includeAssetGUID.SetValueWithoutNotify(selectedPackage.IncludeAssetGUID);
                autoCollectShaders.SetValueWithoutNotify(selectedPackage.AutoCollectShaders);
            }
        }

        private void InitializeWindow()
        {
            selectedPackage = AssetBundleCollectorSetting.Instance.GetFirstPackage();
            selectedGroup = selectedPackage?.GetFirstGroup();

            //
            packageContainer.style.display = AssetBundleCollectorSetting.Instance.ShowPackage ? DisplayStyle.Flex : DisplayStyle.None;

            //
            ShowPackageSetting();
            FillPackageViewData();
            FillGroupViewData();
            FillCollectorViewData();
        }

        #region Package 展示

        private void FillPackageViewData()
        {
            packageListView.Clear();
            packageListView.ClearSelection();
            packageListView.itemsSource = AssetBundleCollectorSetting.Instance.Packages;
            packageListView.Rebuild();
        }

        private VisualElement MakePackageListViewItem()
        {
            VisualElement element = new VisualElement();

            var label = new Label();
            label.name = "Label1";
            label.style.unityTextAlign = TextAnchor.MiddleLeft;
            label.style.flexGrow = 1f;
            label.style.height = 20f;
            element.Add(label);
            return element;
        }

        private void BindPackageListViewItem(VisualElement element, int index)
        {
            var package = AssetBundleCollectorSetting.Instance.Packages[index];

            var textField1 = element.Q<Label>("Label1");
            if (string.IsNullOrEmpty(package.PackageDesc))
                textField1.text = package.PackageName;
            else
                textField1.text = $"{package.PackageName} ({package.PackageDesc})";
        }

        private void OnPackageListViewSelectionChange(IEnumerable<object> obj)
        {
            if (packageListView.selectedItem is not AssetBundleCollectorPackage selectPackage)
                return;

            selectedPackage = selectPackage;
            packageNameText.SetValueWithoutNotify(selectPackage.PackageName);
            packageDescText.SetValueWithoutNotify(selectPackage.PackageDesc);
        }

        private void OnRemovePackageBtnClick()
        {
            if (selectedPackage == null)
                return;

            AssetBundleCollectorSetting.Instance.RemovePackage(selectedPackage);
            selectedPackage = AssetBundleCollectorSetting.Instance.GetFirstPackage();

            FillPackageViewData();
            FillGroupViewData();
            FillCollectorViewData();
        }

        private void OnAddPackageBtnClick()
        {
            selectedPackage = AssetBundleCollectorSetting.Instance.CreatePackage();

            FillPackageViewData();
            FillGroupViewData();
            FillCollectorViewData();
        }

        #endregion

        #region Group 展示

        private void OnPackageNameChange(ChangeEvent<string> evt)
        {
            if (selectedPackage == null)
                return;

            selectedPackage.PackageName = evt.newValue;
            packageNameText.SetValueWithoutNotify(evt.newValue);
            FillPackageViewData();
        }

        private void OnPackageDescChange(ChangeEvent<string> evt)
        {
            if (selectedPackage == null)
                return;

            selectedPackage.PackageDesc = evt.newValue;
            packageDescText.SetValueWithoutNotify(evt.newValue);
            FillPackageViewData();
        }

        private void FillGroupViewData()
        {
            groupListView.Clear();
            groupListView.ClearSelection();
            if (selectedPackage != null)
            {
                groupListView.itemsSource = selectedPackage.Groups;

                //
                packageNameText.SetValueWithoutNotify(selectedPackage.PackageName);
                packageDescText.SetValueWithoutNotify(selectedPackage.PackageDesc);
            }

            groupListView.Rebuild();
        }

        private VisualElement MakeGroupListViewItem()
        {
            VisualElement element = new VisualElement();
            var label = new Label();
            label.name = "Label1";
            label.style.unityTextAlign = TextAnchor.MiddleLeft;
            label.style.flexGrow = 1f;
            label.style.height = 20f;
            element.Add(label);
            return element;
        }

        private void BindGroupListViewItem(VisualElement element, int index)
        {
            if (selectedPackage == null)
                return;

            var group = selectedPackage.Groups[index];
            var textField1 = element.Q<Label>("Label1");
            if (string.IsNullOrEmpty(group.GroupDesc))
                textField1.text = group.GroupName;
            else
                textField1.text = $"{group.GroupName} ({group.GroupDesc})";

            textField1.SetEnabled(group.Enabled);
        }

        private void OnGroupListViewSelectionChange(IEnumerable<object> obj)
        {
            if (groupListView.selectedItem is not AssetBundleCollectorGroup selectGroup)
                return;

            selectedGroup = selectGroup;
            FillCollectorViewData();
        }

        private void OnRemoveGroupBtnClick()
        {
            if (selectedPackage == null || selectedGroup == null)
                return;

            selectedPackage.RemoveGroup(selectedGroup);
            FillGroupViewData();
            FillCollectorViewData();
        }

        private void OnAddGroupBtnClick()
        {
            if (selectedPackage == null)
                return;

            selectedGroup = selectedPackage.CreateGroup();
            FillGroupViewData();
            FillCollectorViewData();
        }

        #endregion

        private void FillCollectorViewData()
        {
            if (selectedGroup == null)
                return;

            groupNameText.SetValueWithoutNotify(selectedGroup.GroupName);
            groupDescText.SetValueWithoutNotify(selectedGroup.GroupDesc);

            collectorListView.Clear();
            for (int i = 0; i < selectedGroup.Collectors.Count; i++)
            {
                VisualElement element = MakeCollectorListViewItem();
            }
        }

        private void OnGroupNameChange(ChangeEvent<string> evt)
        {
            if (selectedGroup == null)
                return;

            selectedGroup.GroupName = evt.newValue;
            FillGroupViewData();
        }

        private void OnGroupDescChange(ChangeEvent<string> evt)
        {
            if (selectedGroup == null)
                return;

            selectedGroup.GroupDesc = evt.newValue;
            FillGroupViewData();
        }

        private void OnGroupTagsChange(ChangeEvent<string> evt)
        {
            if (selectedGroup == null)
                return;

            selectedGroup.GroupAssetTags = evt.newValue;
        }

        private void OnAddCollectorsBtnClick()
        {
            if (selectedGroup == null)
                return;

            selectedGroup.CreateCollector();
            FillCollectorViewData();
        }

        private VisualElement MakeCollectorListViewItem()
        {
            VisualElement element = new VisualElement();

            // VisualElement elementTop = new VisualElement();
            // elementTop.style.flexDirection = FlexDirection.Row;
            // element.Add(elementTop);
            //
            // VisualElement elementBottom = new VisualElement();
            // elementBottom.style.flexDirection = FlexDirection.Row;
            // element.Add(elementBottom);
            //
            // VisualElement elementFoldout = new VisualElement();
            // elementFoldout.style.flexDirection = FlexDirection.Row;
            // element.Add(elementFoldout);
            //
            // VisualElement elementSpace = new VisualElement();
            // elementSpace.style.flexDirection = FlexDirection.Column;
            // element.Add(elementSpace);
            //
            // // Top VisualElement
            // {
            //     var button = new Button();
            //     button.name = "Button1";
            //     button.text = "-";
            //     button.style.unityTextAlign = TextAnchor.MiddleCenter;
            //     button.style.flexGrow = 0f;
            //     elementTop.Add(button);
            // }
            // {
            //     var objectField = new ObjectField();
            //     objectField.name = "ObjectField1";
            //     objectField.label = "Collector";
            //     objectField.objectType = typeof(UnityEngine.Object);
            //     objectField.style.unityTextAlign = TextAnchor.MiddleLeft;
            //     objectField.style.flexGrow = 1f;
            //     elementTop.Add(objectField);
            //     var label = objectField.Q<Label>();
            //     label.style.minWidth = 63;
            // }
            //
            // // Bottom VisualElement
            // {
            //     var label = new Label();
            //     label.style.width = 90;
            //     elementBottom.Add(label);
            // }
            // {
            //     var popupField = new PopupField<string>(_collectorTypeList, 0);
            //     popupField.name = "PopupField0";
            //     popupField.style.unityTextAlign = TextAnchor.MiddleLeft;
            //     popupField.style.width = 150;
            //     elementBottom.Add(popupField);
            // }
            // if (_enableAddressableToogle.value)
            // {
            //     var popupField = new PopupField<RuleDisplayName>(_addressRuleList, 0);
            //     popupField.name = "PopupField1";
            //     popupField.style.unityTextAlign = TextAnchor.MiddleLeft;
            //     popupField.style.width = 220;
            //     elementBottom.Add(popupField);
            // }
            //
            // {
            //     var popupField = new PopupField<RuleDisplayName>(_packRuleList, 0);
            //     popupField.name = "PopupField2";
            //     popupField.style.unityTextAlign = TextAnchor.MiddleLeft;
            //     popupField.style.width = 220;
            //     elementBottom.Add(popupField);
            // }
            // {
            //     var popupField = new PopupField<RuleDisplayName>(_filterRuleList, 0);
            //     popupField.name = "PopupField3";
            //     popupField.style.unityTextAlign = TextAnchor.MiddleLeft;
            //     popupField.style.width = 150;
            //     elementBottom.Add(popupField);
            // }
            // {
            //     var textField = new TextField();
            //     textField.name = "TextField0";
            //     textField.label = "User Data";
            //     textField.style.width = 200;
            //     elementBottom.Add(textField);
            //     var label = textField.Q<Label>();
            //     label.style.minWidth = 63;
            // }
            // {
            //     var textField = new TextField();
            //     textField.name = "TextField1";
            //     textField.label = "Asset Tags";
            //     textField.style.width = 100;
            //     textField.style.marginLeft = 20;
            //     textField.style.flexGrow = 1;
            //     elementBottom.Add(textField);
            //     var label = textField.Q<Label>();
            //     label.style.minWidth = 40;
            // }
            //
            // // Foldout VisualElement
            // {
            //     var label = new Label();
            //     label.style.width = 90;
            //     elementFoldout.Add(label);
            // }
            // {
            //     var foldout = new Foldout();
            //     foldout.name = "Foldout1";
            //     foldout.value = false;
            //     foldout.text = "Main Assets";
            //     elementFoldout.Add(foldout);
            // }
            //
            // // Space VisualElement
            // {
            //     var label = new Label();
            //     label.style.height = 10;
            //     elementSpace.Add(label);
            // }

            return element;
        }

        private void BindCollectorListViewItem(VisualElement arg1, int arg2)
        {
            throw new System.NotImplementedException();
        }

        private void OnCollectorViewSelectionChange(IEnumerable<object> obj)
        {
            throw new System.NotImplementedException();
        }
    }
}