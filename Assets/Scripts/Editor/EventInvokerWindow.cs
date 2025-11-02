using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Editor
{
    public class EventInvokerWindow : EditorWindow
    {
        private Type[] eventTypes;
        private Type selectedType;
        private MethodInfo[] eventMethods;
        private MethodInfo selectedMethod;
        
        private VisualElement leftPanel;
        private VisualElement rightPanel;
        private ScrollView eventListContainer;
        private VisualElement paramContainer;
        private Button invokeButton;
        
        private Func<object>[] currentGetters;

        private bool isPlaying;
        
        [MenuItem("Editor/EventInvokerWindow")]
        public static void OpenWindow()
        {
            var window = GetWindow<EventInvokerWindow>();
            window.titleContent = new GUIContent("Event Invoker");
            
            window.minSize = new Vector2(450, 200);
            window.maxSize = new Vector2(1920, 720);
        }

        private void OnEnable()
        {
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
        }

        private void OnPlayModeStateChanged(PlayModeStateChange state)
        {
            isPlaying = EditorApplication.isPlaying;
        }

        public void CreateGUI()
        {
            //Structure
            var splitView = new TwoPaneSplitView(0, 250, TwoPaneSplitViewOrientation.Horizontal);
            rootVisualElement.Add(splitView);

            //Left Panel
            leftPanel = new VisualElement();
            leftPanel.style.flexDirection = FlexDirection.Column;
            splitView.Add(leftPanel);
            
            var dropdown = new DropdownField(new List<string> {"Loading..."}, 0);
            leftPanel.Add(dropdown);

            eventListContainer = new ScrollView();
            eventListContainer.style.flexGrow = 1;
            leftPanel.Add(eventListContainer);

            //Right panel
            rightPanel = new VisualElement();
            rightPanel.style.flexDirection = FlexDirection.Column;
            splitView.Add(rightPanel);
            
            paramContainer = new VisualElement();
            paramContainer.style.flexGrow = 1;
            rightPanel.Add(paramContainer);
            
            //Data loading
            LoadEventTypes();

            if (eventTypes.Length == 0)
            {
                leftPanel.Add(new HelpBox("No event types found", HelpBoxMessageType.Info));
                return;
            }

            dropdown.choices = eventTypes.Select(t => t.Name).ToList();
            dropdown.value = eventTypes.First().Name;
            selectedType = eventTypes.First();
            RefreshEventMethods();

            dropdown.RegisterValueChangedCallback(changeEvent =>
            {
                selectedType = eventTypes.FirstOrDefault(t => t.Name == changeEvent.newValue);
                selectedMethod = null;
                RefreshEventMethods();
                paramContainer.Clear();
            });
        }

        private void LoadEventTypes()
        {
            eventTypes = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(a => a.GetTypes())
                .Where(t => t.IsClass && t.IsAbstract && t.IsSealed)
                .Where(t => t.Namespace == "Events")
                .ToArray();
        }

        private void RefreshEventMethods()
        {
            eventListContainer.Clear();
            
            if (selectedType == null) return;
            
            eventMethods = selectedType
                .GetMethods(BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly)
                .Where(m => m.ReturnType == typeof(void))
                .Where(m => !m.Name.StartsWith("add_") && !m.Name.StartsWith("remove_"))
                .Where(m => !m.Name.StartsWith("On"))
                .ToArray();

            if (eventMethods.Length == 0)
            {
                eventListContainer.Add(new Label("Invokable methods not found"));
                return;
            }
            
            foreach (var method in eventMethods)
            {
                var button = new Button(() =>
                {
                    selectedMethod = method;
                    BuildParameterUI(method);
                });
                button.text = method.Name;
                
                eventListContainer.Add(button);
            }
        }

        private void BuildParameterUI(MethodInfo method)
        {
            paramContainer.Clear();
            
            var methodTitle = new Label(method.Name);
            methodTitle.style.unityFontStyleAndWeight = FontStyle.Bold;
            methodTitle.style.fontSize = 16;
            paramContainer.Add(methodTitle);

            var parameters = method.GetParameters();
            currentGetters = new Func<object>[parameters.Length];

            if (parameters.Length == 0)
            {
                paramContainer.Add(new Label("Event doesn't have any parameters"));
            }
            else
            {
                foreach (var (p, i) in parameters.Select((p, i) => (p, i)))
                {
                    var row = new VisualElement { style = { flexDirection = FlexDirection.Row } };
                    row.Add(new Label($"{p.Name} ({p.ParameterType.Name})") { style = { minWidth = 160 } });
                    var input = CreateInputField(p.ParameterType, out var getter);
                    row.Add(input);
                    paramContainer.Add(row);
                    currentGetters[i] = getter;
                }
            }

            invokeButton = new Button(() =>
            {
                var args = currentGetters.Select(g => g()).ToArray();
                method.Invoke(null, args);
            })
            {
                text = "🔹 Invoke Event",
                style =
                {
                    unityFontStyleAndWeight = FontStyle.Bold,
                    backgroundColor = new Color(0.25f, 0.5f, 1f),
                    color = Color.white
                }
            };

            paramContainer.Add(invokeButton);
            invokeButton.SetEnabled(isPlaying);
        }
        
        private VisualElement CreateInputField(Type type, out Func<object> getter)
        {
            if (type == typeof(int))
            {
                var field = new IntegerField();
                getter = () => field.value;
                return field;
            }
            if (type == typeof(float))
            {
                var field = new FloatField();
                getter = () => field.value;
                return field;
            }
            if (type == typeof(string))
            {
                var field = new TextField();
                getter = () => field.value;
                return field;
            }
            if (type == typeof(bool))
            {
                var field = new Toggle();
                getter = () => field.value;
                return field;
            }
            if (typeof(UnityEngine.Object).IsAssignableFrom(type))
            {
                var field = new ObjectField { objectType = type };
                getter = () => field.value;
                return field;
            }

            var unsupported = new Label($"(Tipo {type.Name} no soportado)");
            getter = () => null;
            return unsupported;
        }
    }
}