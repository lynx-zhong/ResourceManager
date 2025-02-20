using UnityEngine.UIElements;
using UnityEditor;
using UnityEngine;

namespace Core.Resource
{
    public class BundleCollectorWindow : EditorWindow
    {
        public VisualTreeAsset visualTreeAsset;

        [MenuItem("Tools/AssetBundle Collector Window")]
        public static void OpenWindow()
        {
            BundleCollectorWindow window = GetWindow<BundleCollectorWindow>(typeof(BundleCollectorWindow));
            window.minSize = new Vector2(800, 600);
        }

        public void CreateGUI()
        {
            visualTreeAsset.CloneTree(rootVisualElement);
        }
    }
}