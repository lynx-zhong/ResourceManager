using System.Collections.Generic;
using UnityEngine.UIElements;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;

namespace Core.Resource
{
    public class AssetBundleCollectorWindow : EditorWindow
    {
        public VisualTreeAsset visualTreeAsset;

        //
        public AssetBundleCollectorPackage selectedPackage;

        // package list
        private ListView packageListView;

        // groups
        private TextField packageNameText;
        private TextField packageDescText;
        private ListView groupListView;

        // collectors
        private ListView collectorListView;


        [MenuItem("Tools/AssetBundle Collector Window")]
        public static void OpenWindow()
        {
            AssetBundleCollectorWindow window = GetWindow<AssetBundleCollectorWindow>(typeof(AssetBundleCollectorWindow));
            window.minSize = new Vector2(800, 600);
        }

        public void CreateGUI()
        {
            visualTreeAsset.CloneTree(rootVisualElement);

            // 包
            {
                packageListView = rootVisualElement.Q<ListView>("packageListView");
                packageListView.makeItem = MakePackageListViewItem;
                packageListView.bindItem = BindPackageListViewItem;
                packageListView.onSelectionChange += OnPackageListViewSelectionChange;

                // 添加包
                var packageAddContainer = rootVisualElement.Q<VisualElement>("PackageAddContainer");
                var addPackageButton = packageAddContainer.Q<Button>("AddBtn");
                addPackageButton.clicked += OnAddPackageBtnClick;

                // 移除包
                var removePackageButton = packageAddContainer.Q<Button>("RemoveBtn");
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
                var groupNameText = collectorContainer.Q<TextField>("GroupName");
                groupNameText.RegisterValueChangedCallback(OnGroupNameChange);

                // 组描述
                var groupDescText = collectorContainer.Q<TextField>("GroupDesc");
                groupDescText.RegisterValueChangedCallback(OnGroupDescChange);

                // 组标签
                var groupTagsText = collectorContainer.Q<TextField>("GroupTags");
                groupTagsText.RegisterValueChangedCallback(OnGroupTagsChange);

                // 添加收集器
                var addCollectorBtn = collectorContainer.Q<Button>("AddBtn");
                addCollectorBtn.clicked += OnAddCollectorsBtnClick;

                // list
                collectorListView = collectorContainer.Q<ListView>("CollectorScrollView");
                collectorListView.makeItem = MakeCollectorListViewItem;
                collectorListView.bindItem = BindCollectorListViewItem;
                collectorListView.onSelectionChange += OnCollectorViewSelectionChange;
            }
        }

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
            if (packageListView.selectedItem is AssetBundleCollectorPackage selectPackage)
            {
                packageNameText.SetValueWithoutNotify(selectPackage.PackageName);
                packageDescText.SetValueWithoutNotify(selectPackage.PackageDesc);
            }
        }

        private void OnRemovePackageBtnClick()
        {
            if (packageListView.selectedItem is not AssetBundleCollectorPackage selectPackage)
                return;

            Undo.RecordObject(AssetBundleCollectorSetting.Instance, "ResourceManager RemovePackage");
            AssetBundleCollectorSetting.Instance.RemovePackage(selectPackage);
            FillPackageViewData();
        }

        private void OnAddPackageBtnClick()
        {
            Undo.RecordObject(AssetBundleCollectorSetting.Instance, "ResourceManager CreatePackage");
            AssetBundleCollectorSetting.Instance.CreatePackage();
            FillPackageViewData();
        }

        private void OnPackageNameChange(ChangeEvent<string> evt)
        {
            if (packageListView.selectedItem is not AssetBundleCollectorPackage selectPackage)
                return;

            Undo.RecordObject(AssetBundleCollectorSetting.Instance, "ResourceManager OnPackageNameChange");

            selectPackage.PackageName = evt.newValue;
            packageNameText.SetValueWithoutNotify(evt.newValue);
            AssetBundleCollectorSetting.SetDirtyStatus();
            FillPackageViewData();
        }

        private void OnPackageDescChange(ChangeEvent<string> evt)
        {
            if (packageListView.selectedItem is not AssetBundleCollectorPackage selectPackage)
                return;

            Undo.RecordObject(AssetBundleCollectorSetting.Instance, "ResourceManager OnPackageDescChange");

            selectPackage.PackageDesc = evt.newValue;
            packageDescText.SetValueWithoutNotify(evt.newValue);
            AssetBundleCollectorSetting.SetDirtyStatus();
            FillPackageViewData();
        }


        private VisualElement MakeGroupListViewItem()
        {
            VisualElement element = new VisualElement();
            {
                var label = new Label();
                label.name = "Label1";
                label.style.unityTextAlign = TextAnchor.MiddleLeft;
                label.style.flexGrow = 1f;
                label.style.height = 20f;
                element.Add(label);
            }

            return element;
        }

        private void BindGroupListViewItem(VisualElement element, int index)
        {
            if (packageListView.selectedItem is not AssetBundleCollectorPackage selectPackage)
                return;

            var group = selectPackage.Groups[index];

            var textField1 = element.Q<Label>("Label1");
            if (string.IsNullOrEmpty(group.GroupDesc))
                textField1.text = group.GroupName;
            else
                textField1.text = $"{group.GroupName} ({group.GroupDesc})";

            textField1.SetEnabled(group.Enabled);
        }

        private void OnGroupListViewSelectionChange(IEnumerable<object> obj)
        {
            FillCollectorViewData();
        }

        private void OnAddGroupBtnClick()
        {
            if (packageListView.selectedItem is not AssetBundleCollectorPackage selectPackage)
                return;

            Undo.RecordObject(AssetBundleCollectorSetting.Instance, "ResourceManager OnAddGroupBtnClick");
            selectPackage.CreateGroup();
            FillCollectorViewData();
        }

        private void OnRemoveGroupBtnClick()
        {
            if (packageListView.selectedItem is not AssetBundleCollectorPackage selectPackage)
                return;

            if (groupListView.selectedItem is not AssetBundleCollectorGroup selectGroup)
                return;

            Undo.RecordObject(AssetBundleCollectorSetting.Instance, "ResourceManager OnAddGroupBtnClick");
            selectPackage.RemoveGroup(selectGroup);
            FillCollectorViewData();
        }

        private void FillCollectorViewData()
        {
            groupListView.Clear();
            groupListView.ClearSelection();

            if (packageListView.selectedItem is not AssetBundleCollectorPackage selectPackage)
                return;

            groupListView.itemsSource = selectPackage.Groups;
            groupListView.Rebuild();
        }

        private void OnGroupNameChange(ChangeEvent<string> evt)
        {
            throw new System.NotImplementedException();
        }

        private void OnGroupDescChange(ChangeEvent<string> evt)
        {
            throw new System.NotImplementedException();
        }

        private void OnGroupTagsChange(ChangeEvent<string> evt)
        {
            throw new System.NotImplementedException();
        }

        private void OnAddCollectorsBtnClick()
        {
            throw new System.NotImplementedException();
        }

        private VisualElement MakeCollectorListViewItem()
        {
            VisualElement element = new VisualElement();

            VisualElement elementTop = new VisualElement();
            elementTop.style.flexDirection = FlexDirection.Row;
            element.Add(elementTop);

            VisualElement elementBottom = new VisualElement();
            elementBottom.style.flexDirection = FlexDirection.Row;
            element.Add(elementBottom);

            VisualElement elementFoldout = new VisualElement();
            elementFoldout.style.flexDirection = FlexDirection.Row;
            element.Add(elementFoldout);

            VisualElement elementSpace = new VisualElement();
            elementSpace.style.flexDirection = FlexDirection.Column;
            element.Add(elementSpace);

            // Top VisualElement
            {
                var button = new Button();
                button.name = "Button1";
                button.text = "-";
                button.style.unityTextAlign = TextAnchor.MiddleCenter;
                button.style.flexGrow = 0f;
                elementTop.Add(button);
            }
            {
                var objectField = new ObjectField();
                objectField.name = "ObjectField1";
                objectField.label = "Collector";
                objectField.objectType = typeof(UnityEngine.Object);
                objectField.style.unityTextAlign = TextAnchor.MiddleLeft;
                objectField.style.flexGrow = 1f;
                elementTop.Add(objectField);
                var label = objectField.Q<Label>();
                label.style.minWidth = 63;
            }

            // Bottom VisualElement
            {
                var label = new Label();
                label.style.width = 90;
                elementBottom.Add(label);
            }
            {
                var popupField = new PopupField<string>(_collectorTypeList, 0);
                popupField.name = "PopupField0";
                popupField.style.unityTextAlign = TextAnchor.MiddleLeft;
                popupField.style.width = 150;
                elementBottom.Add(popupField);
            }
            if (_enableAddressableToogle.value)
            {
                var popupField = new PopupField<RuleDisplayName>(_addressRuleList, 0);
                popupField.name = "PopupField1";
                popupField.style.unityTextAlign = TextAnchor.MiddleLeft;
                popupField.style.width = 220;
                elementBottom.Add(popupField);
            }

            {
                var popupField = new PopupField<RuleDisplayName>(_packRuleList, 0);
                popupField.name = "PopupField2";
                popupField.style.unityTextAlign = TextAnchor.MiddleLeft;
                popupField.style.width = 220;
                elementBottom.Add(popupField);
            }
            {
                var popupField = new PopupField<RuleDisplayName>(_filterRuleList, 0);
                popupField.name = "PopupField3";
                popupField.style.unityTextAlign = TextAnchor.MiddleLeft;
                popupField.style.width = 150;
                elementBottom.Add(popupField);
            }
            {
                var textField = new TextField();
                textField.name = "TextField0";
                textField.label = "User Data";
                textField.style.width = 200;
                elementBottom.Add(textField);
                var label = textField.Q<Label>();
                label.style.minWidth = 63;
            }
            {
                var textField = new TextField();
                textField.name = "TextField1";
                textField.label = "Asset Tags";
                textField.style.width = 100;
                textField.style.marginLeft = 20;
                textField.style.flexGrow = 1;
                elementBottom.Add(textField);
                var label = textField.Q<Label>();
                label.style.minWidth = 40;
            }

            // Foldout VisualElement
            {
                var label = new Label();
                label.style.width = 90;
                elementFoldout.Add(label);
            }
            {
                var foldout = new Foldout();
                foldout.name = "Foldout1";
                foldout.value = false;
                foldout.text = "Main Assets";
                elementFoldout.Add(foldout);
            }

            // Space VisualElement
            {
                var label = new Label();
                label.style.height = 10;
                elementSpace.Add(label);
            }

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