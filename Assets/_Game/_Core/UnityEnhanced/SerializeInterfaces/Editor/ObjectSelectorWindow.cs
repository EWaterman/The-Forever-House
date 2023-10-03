using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

namespace UnityEnhanced.SerializeInterfaces.Editor
{
    internal class ObjectSelectorWindow : EditorWindow
    {
        public class ItemInfo
        {
            public Texture Icon;
            public int? InstanceID;
            public string Label;
        }

        public static ObjectSelectorWindow Instance { get; private set; }

        Action<Object> _selectionChangedCallback;
        Action<Object, bool> _selectorClosedCallback;
        ObjectSelectorFilter _filter;
        SerializedProperty _editingProperty;
        List<ItemInfo> _allItems;
        List<ItemInfo> _filteredItems;
        ItemInfo _currentItem;
        string _searchText;
        bool _userCanceled = false;
        bool _showSceneObjects = true;
        int _undoGroup;
        ToolbarSearchField _searchbox;
        ListView _listView;
        Label _detailsLabel;
        Label _detailsIndexLabel;
        Label _detailsTypeLabel;
        Tab _sceneTab;
        Tab _assetsTab;

        static ItemInfo _nullItem = new() { InstanceID = null, Label = "None" };

        public bool initialized { get; set; } = false;

        public string SearchText
        {
            get => _searchText;
            set
            {
                _searchText = value;
                FilterItems();
            }
        }

        public static void Show(SerializedProperty property, Action<Object> onSelectionChanged, Action<Object, bool> onSelectorClosed, ObjectSelectorFilter filter)
        {
            if (Instance == null)
                Instance = ScriptableObject.CreateInstance<ObjectSelectorWindow>();
            Instance._editingProperty = property;
            Instance._selectionChangedCallback = onSelectionChanged;
            Instance._selectorClosedCallback = onSelectorClosed;
            Instance._filter = filter;
            Instance.Init();
            Instance.ShowAuxWindow();
            //Instance.Show();
        }

        public void SetSearchFilter(string query)
        {
            _searchbox.value = query;
        }

        void Init()
        {
            InitData();
            InitVisualElements();
            BindVisualElements();
            FinishInit();
        }

        void InitData()
        {
            _undoGroup = Undo.GetCurrentGroup();
            _searchText = "";
            _allItems = new List<ItemInfo>();
            _filteredItems = new List<ItemInfo>();

            _showSceneObjects = true;
            var target = _editingProperty.objectReferenceValue;
            if (target != null)
                _showSceneObjects = !AssetDatabase.Contains(target);

            PopulateItems();
            FilterItems();
        }

        void InitVisualElements()
        {
            var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/SerializeInterfaces/Assets/USS/ObjectSelectorWindow.uss");
            rootVisualElement.styleSheets.Add(styleSheet);

            _searchbox = new ToolbarSearchField();
            _searchbox.RegisterValueChangedCallback(SearchFilterChanged);
            rootVisualElement.Add(_searchbox);

            var tabContainer = new VisualElement();
            tabContainer.style.flexDirection = FlexDirection.Row;
            _assetsTab = new Tab("Assets");
            _sceneTab = new Tab("Scene");
            tabContainer.Add(_assetsTab);
            tabContainer.Add(_sceneTab);
            rootVisualElement.Add(tabContainer);

            _listView = new ListView(_filteredItems, 16, MakeItem, BindItem);
            _listView.selectionChanged += ItemSelectionChanged;
            _listView.itemsChosen += ItemsChosen;
            rootVisualElement.Add(_listView);

            _detailsLabel = new Label();
            _detailsTypeLabel = new Label();
            _detailsIndexLabel = new Label();

            var details = new VisualElement();
            details.AddToClassList("details");
            details.Add(_detailsLabel);
            details.Add(_detailsIndexLabel);
            details.Add(_detailsTypeLabel);
            rootVisualElement.Add(details);
        }

        void BindVisualElements()
        {
            Tab activeTab = _showSceneObjects ? _sceneTab : _assetsTab;
            activeTab.SetValueWithoutNotify(true);

            var toggleGroup = new ToggleGroup();
            toggleGroup.RegisterToggle(_assetsTab);
            toggleGroup.RegisterToggle(_sceneTab);
            toggleGroup.OnToggleChanged += HandleGroupChanged;

            if (GetIndexOfEditingPropertyValue(out var index))
                _listView.selectedIndex = index;
        }

        void FinishInit()
        {
            EditorApplication.delayCall += () =>
            {
                _listView.Focus();
                initialized = true;
            };
        }

        bool GetIndexOfEditingPropertyValue(out int index)
        {
            index = -1;
            var targetObject = _editingProperty.objectReferenceValue;
            if (targetObject)
            {
                int instanceID = targetObject.GetInstanceID();
                index = _filteredItems.FindIndex(x => x.InstanceID == instanceID);
            }
            return index >= 0;
        }

        bool GetIndexOfCurrentItem(out int index)
        {
            index = -1;
            if (_currentItem != null)
                index = _filteredItems.FindIndex(0, x => x.InstanceID == _currentItem.InstanceID);
            return index >= 0;
        }

        void HandleGroupChanged(object sender, Toggle toggle)
        {
            if (_showSceneObjects && toggle == this._sceneTab) return;
            _showSceneObjects = !_showSceneObjects;
            PopulateItems();
            FilterItems();
            var list = new List<int>();
            if (GetIndexOfCurrentItem(out var index))
                list.Add(index);
            _listView.SetSelectionWithoutNotify(list);
            _listView.Focus();
        }

        void OnDisable()
        {
            _selectorClosedCallback?.Invoke(GetCurrentObject(), _userCanceled);
            if (_userCanceled)
                Undo.RevertAllDownToGroup(_undoGroup);
            else
                Undo.CollapseUndoOperations(_undoGroup);
            Instance = null;
        }

        void PopulateItems()
        {
            _allItems.Clear();
            _filteredItems.Clear();
            if (_showSceneObjects)
                _allItems.AddRange(FetchAllComponents());
            else
                _allItems.AddRange(FetchAllAssets());
            _allItems.Sort((item, other) => item.Label.CompareTo(other.Label));
        }

        void SearchFilterChanged(ChangeEvent<string> evt)
        {
            SearchText = evt.newValue;
        }

        void FilterItems()
        {
            _filteredItems.Clear();
            _filteredItems.Add(_nullItem);
            _filteredItems.AddRange(_allItems.Where(item => string.IsNullOrEmpty(SearchText) || item.Label.IndexOf(SearchText, StringComparison.InvariantCultureIgnoreCase) >= 0));

            _listView?.Rebuild();
        }

        void BindItem(VisualElement listItem, int index)
        {
            if (index < 0 || index >= _filteredItems.Count)
                return;

            var label = listItem.Q<Label>();
            if (label != null)
                label.text = _filteredItems[index].Label;
            var image = listItem.Q<Image>();
            image.image = _filteredItems[index].Icon;
        }

        static VisualElement MakeItem()
        {
            var ve = new VisualElement();
            var image = new Image();
            var label = new Label();
            ve.Add(image);
            ve.Add(label);

            ve.AddToClassList("list-item");
            label.AddToClassList("list-item__text");
            image.AddToClassList("list-item__icon");

            return ve;
        }

        void ItemSelectionChanged(IEnumerable<object> selectedItems)
        {
            _currentItem = selectedItems.FirstOrDefault() as ItemInfo;
            UpdateDetails();
            _selectionChangedCallback?.Invoke(GetCurrentObject());
        }

        void ItemsChosen(IEnumerable<object> selectedItems)
        {
            _currentItem = selectedItems.FirstOrDefault() as ItemInfo;
            _userCanceled = false;
            Close();
        }

        void UpdateDetails()
        {
            GetText(_currentItem, out var infoText, out var indexText, out var typeText);

            static void SetText(Label label, string text)
            {
                label.text = string.IsNullOrEmpty(text) ? "" : text;
            }

            SetText(_detailsLabel, infoText);
            SetText(_detailsIndexLabel, indexText);
            SetText(_detailsTypeLabel, typeText);
        }

        static void GetText(ItemInfo itemInfo, out string text, out string indexText, out string typeText)
        {
            text = null;
            indexText = null;
            typeText = null;

            if (itemInfo == null) return;
            if (itemInfo.InstanceID == null)
            {
                text = itemInfo.Label;
                return;
            }
            var obj = EditorUtility.InstanceIDToObject((int)itemInfo.InstanceID);
            if (AssetDatabase.Contains(obj))
            {
                text = AssetDatabase.GetAssetPath(obj);
            }
            else
            {
                var transform = (obj is GameObject go) ? go.transform : (obj as Component).transform;
                int compIndex = Array.IndexOf(transform.gameObject.GetComponents(typeof(Component)), obj);
                text = $"{GetTransformPath(transform)}";
                indexText = $"[{compIndex}]";
            }
            typeText = $"({obj.GetType().Name})";
        }

        static string GetTransformPath(Transform transform)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(transform.name);
            while (transform.parent != null)
            {
                sb.Insert(0, transform.parent.name + "/");
                transform = transform.parent;
            }
            return sb.ToString();
        }

        IEnumerable<ItemInfo> FetchAllAssets()
        {
            var property = new HierarchyProperty(HierarchyType.Assets, false);
            property.SetSearchFilter(_filter.AssetSearchFilter, 0);

            while (property.Next(null))
            {
                yield return new ItemInfo { Icon = property.icon, InstanceID = property.instanceID, Label = property.name };
            }
            yield break;
        }

        IEnumerable<ItemInfo> FetchAllComponents()
        {
            var property = new HierarchyProperty(HierarchyType.GameObjects, false);

            while (property.Next(null))
            {
                var go = property.pptrValue as GameObject;
                if (go == null) continue;

                if (CheckFilter(go))
                    yield return new ItemInfo { Icon = property.icon, InstanceID = property.instanceID, Label = property.name };

                foreach (var comp in go.GetComponents(typeof(Component)))
                {
                    if (CheckFilter(comp))
                        yield return new ItemInfo { Icon = EditorGUIUtility.ObjectContent(comp, comp.GetType()).image, InstanceID = comp.GetInstanceID(), Label = property.name };
                }
            }
        }

        bool CheckFilter(Object obj)
        {
            var matchFilterConstraint = _filter.SceneFilterCallback?.Invoke(obj);
            return (!matchFilterConstraint.HasValue || matchFilterConstraint.Value);
        }

        Object GetCurrentObject()
        {
            return _currentItem == null || _currentItem.InstanceID == null
                ? null
                : EditorUtility.InstanceIDToObject((int)_currentItem.InstanceID);
        }
    }

    public class ObjectSelectorFilter
    {
        public string AssetSearchFilter;
        public Func<Object, bool> SceneFilterCallback;

        public ObjectSelectorFilter() : this("", x => true) { }

        public ObjectSelectorFilter(string assetSearchFilter, Func<Object, bool> sceneFilterCallback)
        {
            AssetSearchFilter = assetSearchFilter;
            SceneFilterCallback = sceneFilterCallback;
        }
    }
}