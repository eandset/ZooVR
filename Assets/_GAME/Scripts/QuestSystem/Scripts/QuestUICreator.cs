using TMPro;
using UnityEditor;
using UnityEditor.Events;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
#if ENABLE_INPUT_SYSTEM && !ENABLE_LEGACY_INPUT_MANAGER
using UnityEngine.InputSystem.UI;
#endif

namespace QuestSystem.EditorTools
{
    /// <summary>
    /// Пункты контекстного меню Hierarchy: GameObject → UI → Quests.
    /// Создают готовую иерархию QuestListView и QuestItemView со всеми ссылками.
    /// Положите файл в любую папку Editor, например Assets/QuestSystem/Scripts/Editor.
    /// </summary>
    public static class QuestUICreator
    {
        private const string PrefabFolder = "Assets/_GAME/QuestSystem/Prefabs";
        private const string ItemPrefabPath = PrefabFolder + "/QuestItem.prefab";

        // ================================================================= menu

        [MenuItem("GameObject/UI/Quests/Quest List View", false, 10)]
        private static void CreateListView(MenuCommand cmd)
        {
            var parent = ResolveParent(cmd);
            var itemPrefab = GetOrCreateItemPrefab();

            var view = BuildList(parent, itemPrefab);
            Finish(view.gameObject, "Create Quest List View");
        }

        [MenuItem("GameObject/UI/Quests/Quest Item View", false, 11)]
        private static void CreateItemView(MenuCommand cmd)
        {
            var parent = ResolveParent(cmd);

            var item = BuildItem(parent);
            var rt = (RectTransform)item.transform;
            rt.anchorMin = rt.anchorMax = rt.pivot = new Vector2(0.5f, 0.5f);
            rt.sizeDelta = new Vector2(480f, 170f);
            rt.anchoredPosition = Vector2.zero;

            Finish(item.gameObject, "Create Quest Item View");
        }

        private static void Finish(GameObject go, string undoName)
        {
            GameObjectUtility.EnsureUniqueNameForSibling(go);
            Undo.RegisterCreatedObjectUndo(go, undoName);
            Selection.activeGameObject = go;
        }

        // ================================================================= item

        private static QuestItemView GetOrCreateItemPrefab()
        {
            var existing = AssetDatabase.LoadAssetAtPath<QuestItemView>(ItemPrefabPath);
            if (existing != null) return existing;

            EnsureFolder("Assets/QuestSystem");
            EnsureFolder(PrefabFolder);

            var temp = BuildItem(null);
            var saved = PrefabUtility.SaveAsPrefabAsset(temp.gameObject, ItemPrefabPath);
            Object.DestroyImmediate(temp.gameObject);

            Debug.Log($"[Quests] Создан префаб карточки: {ItemPrefabPath}", saved);
            return saved.GetComponent<QuestItemView>();
        }

        /// <summary>
        /// QuestItem
        ///  ├ StateStripe       (Image, цвет состояния)
        ///  ├ Header            (Icon, Title, State, Timer)
        ///  ├ Description
        ///  ├ Objectives
        ///  ├ Rewards
        ///  └ Footer            (ProgressBar/Fill, ProgressText, ActionButton)
        /// </summary>
        private static QuestItemView BuildItem(Transform parent)
        {
            var root = NewRect("QuestItem", parent);
            var rootBg = root.gameObject.AddComponent<Image>();
            StyleSliced(rootBg, new Color(0.16f, 0.17f, 0.21f, 0.95f));

            var selectButton = root.gameObject.AddComponent<Button>();
            selectButton.targetGraphic = rootBg;

            AddLayout<VerticalLayoutGroup>(root.gameObject, 6f, new RectOffset(12, 12, 10, 12),
                TextAnchor.UpperLeft, true, true, true, false);

            // --- stripe
            var stripe = NewRect("StateStripe", root);
            var stripeImg = stripe.gameObject.AddComponent<Image>();
            stripeImg.color = Color.gray;
            Le(stripe.gameObject, prefH: 4f, flexH: 0f);

            // --- header
            var header = NewRect("Header", root);
            AddLayout<HorizontalLayoutGroup>(header.gameObject, 8f, new RectOffset(),
                TextAnchor.MiddleLeft, true, true, false, false);
            Le(header.gameObject, minH: 36f);

            var icon = NewRect("Icon", header);
            var iconImg = icon.gameObject.AddComponent<Image>();
            iconImg.preserveAspect = true;
            Le(icon.gameObject, prefW: 40f, prefH: 40f, flexW: 0f);

            var title = NewText("Title", header, "Quest title", 22f, FontStyles.Bold, TextAlignmentOptions.MidlineLeft);
            Le(title.gameObject, flexW: 1f);

            var state = NewText("State", header, "State", 16f, FontStyles.Normal, TextAlignmentOptions.MidlineRight);
            Le(state.gameObject, minW: 90f);

            var timer = NewText("Timer", header, "00:00", 16f, FontStyles.Normal, TextAlignmentOptions.MidlineRight);
            Le(timer.gameObject, minW: 60f);

            // --- body
            var description = NewText("Description", root, "Description", 16f, FontStyles.Normal,
                TextAlignmentOptions.TopLeft, new Color(0.8f, 0.8f, 0.85f));
            var objectives = NewText("Objectives", root, "[ ] Objective (0/1)", 16f, FontStyles.Normal,
                TextAlignmentOptions.TopLeft);
            var rewards = NewText("Rewards", root, "Reward x1", 14f, FontStyles.Italic,
                TextAlignmentOptions.TopLeft, new Color(1f, 0.85f, 0.4f));

            // --- footer
            var footer = NewRect("Footer", root);
            AddLayout<HorizontalLayoutGroup>(footer.gameObject, 10f, new RectOffset(),
                TextAnchor.MiddleLeft, true, true, false, false);
            Le(footer.gameObject, minH: 32f);

            var bar = NewRect("ProgressBar", footer);
            var barBg = bar.gameObject.AddComponent<Image>();
            StyleSliced(barBg, new Color(0.07f, 0.07f, 0.09f, 1f));
            Le(bar.gameObject, prefH: 14f, flexW: 1f);

            var fill = NewRect("Fill", bar);
            Stretch(fill, 2f);
            var fillImg = fill.gameObject.AddComponent<Image>();
            fillImg.sprite = UISprite;
            fillImg.color = new Color(0.4f, 0.9f, 0.4f);
            fillImg.type = Image.Type.Filled;
            fillImg.fillMethod = Image.FillMethod.Horizontal;
            fillImg.fillOrigin = (int)Image.OriginHorizontal.Left;
            fillImg.fillAmount = 0f;

            var progressText = NewText("ProgressText", footer, "0%", 14f, FontStyles.Normal,
                TextAlignmentOptions.MidlineRight);
            Le(progressText.gameObject, minW: 48f);

            var actionButton = NewButton("ActionButton", footer, "Начать", out var actionLabel);
            Le(actionButton.gameObject, prefW: 110f, prefH: 32f, flexW: 0f);

            // --- component + wiring
            var view = root.gameObject.AddComponent<QuestItemView>();
            var so = new SerializedObject(view);
            Ref(so, "titleText", title);
            Ref(so, "descriptionText", description);
            Ref(so, "objectivesText", objectives);
            Ref(so, "rewardsText", rewards);
            Ref(so, "timerText", timer);
            Ref(so, "stateText", state);
            Ref(so, "iconImage", iconImg);
            Ref(so, "progressFill", fillImg);
            Ref(so, "progressText", progressText);
            Ref(so, "stateTint", stripeImg);
            Ref(so, "selectButton", selectButton);
            Ref(so, "actionButton", actionButton);
            Ref(so, "actionLabel", actionLabel);

            var styles = so.FindProperty("stateStyles");
            styles.arraySize = 5;
            SetStyle(styles, 0, QuestState.Locked, "Закрыт", new Color(0.5f, 0.5f, 0.5f));
            SetStyle(styles, 1, QuestState.Available, "Доступен", new Color(0.5f, 0.8f, 1f));
            SetStyle(styles, 2, QuestState.Active, "В процессе", new Color(1f, 0.85f, 0.3f));
            SetStyle(styles, 3, QuestState.Completed, "Выполнен", new Color(0.4f, 0.9f, 0.4f));
            SetStyle(styles, 4, QuestState.Failed, "Провален", new Color(0.9f, 0.35f, 0.35f));
            so.ApplyModifiedPropertiesWithoutUndo();

            return view;
        }

        private static void SetStyle(SerializedProperty array, int index, QuestState state, string label, Color color)
        {
            var e = array.GetArrayElementAtIndex(index);
            e.FindPropertyRelative("state").enumValueIndex = (int)state;
            e.FindPropertyRelative("label").stringValue = label;
            e.FindPropertyRelative("color").colorValue = color;
        }

        // ================================================================= list

        /// <summary>
        /// QuestListView
        ///  ├ Header            (Title, Counter)
        ///  ├ Filters           (Все / Доступные / Активные / Выполненные / Проваленные)
        ///  └ Scroll View
        ///     └ Viewport
        ///        ├ Content     (сюда спавнятся карточки)
        ///        └ EmptyState
        /// </summary>
        private static QuestListView BuildList(Transform parent, QuestItemView itemPrefab)
        {
            var root = NewRect("QuestListView", parent);
            root.anchorMin = root.anchorMax = root.pivot = new Vector2(0.5f, 0.5f);
            root.sizeDelta = new Vector2(520f, 640f);
            root.anchoredPosition = Vector2.zero;

            var bg = root.gameObject.AddComponent<Image>();
            StyleSliced(bg, new Color(0.1f, 0.1f, 0.13f, 0.95f));
            AddLayout<VerticalLayoutGroup>(root.gameObject, 8f, new RectOffset(10, 10, 10, 10),
                TextAnchor.UpperLeft, true, true, true, false);

            var view = root.gameObject.AddComponent<QuestListView>();

            // --- header
            var header = NewRect("Header", root);
            AddLayout<HorizontalLayoutGroup>(header.gameObject, 8f, new RectOffset(),
                TextAnchor.MiddleLeft, true, true, false, false);
            Le(header.gameObject, minH: 36f);

            var title = NewText("Title", header, "Квесты", 26f, FontStyles.Bold, TextAlignmentOptions.MidlineLeft);
            Le(title.gameObject, flexW: 1f);
            var counter = NewText("Counter", header, "0/0", 18f, FontStyles.Normal, TextAlignmentOptions.MidlineRight);
            Le(counter.gameObject, minW: 70f);

            // --- filters
            var filters = NewRect("Filters", root);
            AddLayout<HorizontalLayoutGroup>(filters.gameObject, 6f, new RectOffset(),
                TextAnchor.MiddleCenter, true, true, true, false);
            Le(filters.gameObject, minH: 34f);

            AddFilterButton(filters, "Все", view.ShowAll);
            AddFilterButton(filters, "Доступные", view.ShowAvailable);
            AddFilterButton(filters, "Активные", view.ShowActive);
            AddFilterButton(filters, "Выполненные", view.ShowCompleted);
            AddFilterButton(filters, "Проваленные", view.ShowFailed);

            // --- scroll view
            var scrollRt = NewRect("Scroll View", root);
            var scrollImg = scrollRt.gameObject.AddComponent<Image>();
            scrollImg.color = new Color(0f, 0f, 0f, 0.25f);
            Le(scrollRt.gameObject, minH: 200f, flexH: 1f);

            var viewport = NewRect("Viewport", scrollRt);
            Stretch(viewport, 0f);
            viewport.gameObject.AddComponent<RectMask2D>();

            var content = NewRect("Content", viewport);
            content.anchorMin = new Vector2(0f, 1f);
            content.anchorMax = new Vector2(1f, 1f);
            content.pivot = new Vector2(0.5f, 1f);
            content.sizeDelta = Vector2.zero;
            content.anchoredPosition = Vector2.zero;
            AddLayout<VerticalLayoutGroup>(content.gameObject, 8f, new RectOffset(4, 4, 4, 4),
                TextAnchor.UpperLeft, true, true, true, false);
            var fitter = content.gameObject.AddComponent<ContentSizeFitter>();
            fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
            fitter.horizontalFit = ContentSizeFitter.FitMode.Unconstrained;

            var empty = NewText("EmptyState", viewport, "Квестов нет", 20f, FontStyles.Italic,
                TextAlignmentOptions.Center, new Color(0.6f, 0.6f, 0.65f));
            Stretch(empty.rectTransform, 0f);

            var scroll = scrollRt.gameObject.AddComponent<ScrollRect>();
            scroll.viewport = viewport;
            scroll.content = content;
            scroll.horizontal = false;
            scroll.vertical = true;
            scroll.movementType = ScrollRect.MovementType.Elastic;
            scroll.scrollSensitivity = 25f;

            // --- wiring
            var so = new SerializedObject(view);
            Ref(so, "itemPrefab", itemPrefab);
            Ref(so, "container", content);
            Ref(so, "emptyState", empty.gameObject);
            Ref(so, "counterText", counter);
            so.ApplyModifiedPropertiesWithoutUndo();

            return view;
        }

        private static void AddFilterButton(Transform parent, string label, UnityEngine.Events.UnityAction action)
        {
            var btn = NewButton("Filter_" + label, parent, label, out var text);
            text.fontSize = 14f;
            Le(btn.gameObject, minH: 34f, flexW: 1f);
            UnityEventTools.AddPersistentListener(btn.onClick, action);
        }

        // ================================================================= canvas / parent

        private static Transform ResolveParent(MenuCommand cmd)
        {
            var go = cmd.context as GameObject;
            if (go == null) go = Selection.activeGameObject;

            if (go != null && go.scene.IsValid() && go.GetComponentInParent<Canvas>() != null)
            {
                EnsureEventSystem();
                return go.transform;
            }

            var canvas = Object.FindAnyObjectByType<Canvas>();
            if (canvas == null) canvas = CreateCanvas();
            EnsureEventSystem();
            return canvas.transform;
        }

        private static Canvas CreateCanvas()
        {
            var go = new GameObject("Canvas", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
            go.layer = LayerMask.NameToLayer("UI");

            var canvas = go.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;

            var scaler = go.GetComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920f, 1080f);
            scaler.matchWidthOrHeight = 0.5f;

            Undo.RegisterCreatedObjectUndo(go, "Create Canvas");
            return canvas;
        }

        private static void EnsureEventSystem()
        {
            if (Object.FindAnyObjectByType<EventSystem>() != null) return;

            var go = new GameObject("EventSystem", typeof(EventSystem));
#if ENABLE_INPUT_SYSTEM && !ENABLE_LEGACY_INPUT_MANAGER
            go.AddComponent<InputSystemUIInputModule>();
#else
            go.AddComponent<StandaloneInputModule>();
#endif
            Undo.RegisterCreatedObjectUndo(go, "Create EventSystem");
        }

        // ================================================================= helpers

        private static Sprite UISprite => AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/UISprite.psd");

        private static RectTransform NewRect(string name, Transform parent)
        {
            var go = new GameObject(name, typeof(RectTransform));
            go.layer = LayerMask.NameToLayer("UI");
            go.transform.SetParent(parent, false);
            return (RectTransform)go.transform;
        }

        private static TextMeshProUGUI NewText(string name, Transform parent, string text, float size,
            FontStyles style, TextAlignmentOptions align, Color? color = null)
        {
            var rt = NewRect(name, parent);
            var t = rt.gameObject.AddComponent<TextMeshProUGUI>();
            t.text = text;
            t.fontSize = size;
            t.fontStyle = style;
            t.alignment = align;
            t.color = color ?? Color.white;
            t.raycastTarget = false;
            return t;
        }

        private static Button NewButton(string name, Transform parent, string label, out TextMeshProUGUI labelText)
        {
            var rt = NewRect(name, parent);
            var img = rt.gameObject.AddComponent<Image>();
            StyleSliced(img, new Color(0.25f, 0.28f, 0.35f, 1f));

            var btn = rt.gameObject.AddComponent<Button>();
            btn.targetGraphic = img;

            labelText = NewText("Label", rt, label, 16f, FontStyles.Normal, TextAlignmentOptions.Center);
            Stretch(labelText.rectTransform, 4f);
            return btn;
        }

        private static void StyleSliced(Image img, Color color)
        {
            img.sprite = UISprite;
            img.type = Image.Type.Sliced;
            img.color = color;
        }

        private static void Stretch(RectTransform rt, float inset)
        {
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            rt.offsetMin = new Vector2(inset, inset);
            rt.offsetMax = new Vector2(-inset, -inset);
        }

        private static T AddLayout<T>(GameObject go, float spacing, RectOffset padding, TextAnchor align,
            bool controlW, bool controlH, bool expandW, bool expandH) where T : HorizontalOrVerticalLayoutGroup
        {
            var g = go.AddComponent<T>();
            g.spacing = spacing;
            g.padding = padding;
            g.childAlignment = align;
            g.childControlWidth = controlW;
            g.childControlHeight = controlH;
            g.childForceExpandWidth = expandW;
            g.childForceExpandHeight = expandH;
            return g;
        }

        private static LayoutElement Le(GameObject go, float minW = -1f, float minH = -1f,
            float prefW = -1f, float prefH = -1f, float flexW = -1f, float flexH = -1f)
        {
            var le = go.GetComponent<LayoutElement>();
            if (le == null) le = go.AddComponent<LayoutElement>();
            le.minWidth = minW;
            le.minHeight = minH;
            le.preferredWidth = prefW;
            le.preferredHeight = prefH;
            le.flexibleWidth = flexW;
            le.flexibleHeight = flexH;
            return le;
        }

        private static void Ref(SerializedObject so, string field, Object value)
        {
            var p = so.FindProperty(field);
            if (p == null)
            {
                Debug.LogWarning($"[Quests] Поле '{field}' не найдено в {so.targetObject.GetType().Name}");
                return;
            }
            p.objectReferenceValue = value;
        }

        private static void EnsureFolder(string path)
        {
            if (AssetDatabase.IsValidFolder(path)) return;
            int slash = path.LastIndexOf('/');
            AssetDatabase.CreateFolder(path.Substring(0, slash), path.Substring(slash + 1));
        }
    }
}
