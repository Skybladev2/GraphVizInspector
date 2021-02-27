using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Subtegral.DialogueSystem.Editor
{
    public class GraphEditor : EditorWindow
    {
        private string _fileName = "New Narrative";

        private Graph _graphView;

        [MenuItem("Graph/Graph")]
        public static void CreateGraphViewWindow()
        {
            var window = GetWindow<GraphEditor>();
            window.titleContent = new GUIContent("Graph");
        }

        private void ConstructGraphView()
        {
            _graphView = new Graph(this)
            {
                name = "Graph",
            };
            _graphView.StretchToParentSize();
            rootVisualElement.Add(_graphView);
        }

        private void GenerateToolbar()
        {
            var toolbar = new Toolbar();

            var fileNameTextField = new TextField("File Name:");
            fileNameTextField.SetValueWithoutNotify(_fileName);
            fileNameTextField.MarkDirtyRepaint();
            fileNameTextField.RegisterValueChangedCallback(evt => _fileName = evt.newValue);
            toolbar.Add(fileNameTextField);
            toolbar.Add(new Button(() => RequestDataOperation(true)) {text = "Save Data"});
            toolbar.Add(new Button(() => RequestDataOperation(false)) {text = "Load Data"});
            rootVisualElement.Add(toolbar);
        }

        private void RequestDataOperation(bool save)
        {
            if (!string.IsNullOrEmpty(_fileName))
            {
                var saveUtility = GraphSaveUtility.GetInstance(_graphView);
                if (save)
                    saveUtility.SaveGraph(_fileName);
                else
                    saveUtility.LoadGraph(_fileName);
            }
            else
            {
                EditorUtility.DisplayDialog("Invalid File name", "Please Enter a valid filename", "OK");
            }
        }

        private void OnEnable()
        {
            ConstructGraphView();
            GenerateToolbar();
        }

        private void OnDisable()
        {
            rootVisualElement.Remove(_graphView);
        }
    }
}