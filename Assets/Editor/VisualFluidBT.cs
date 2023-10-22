using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using CleverCrow.Fluid.BTs.Trees;
using System.Collections.Generic;
using CleverCrow.Fluid.BTs.Tasks;

[InitializeOnLoadAttribute]
public class VisualFluidBT : EditorWindow
{
    private static VisualFluidBTView staticVisualBT;

    [SerializeField]
    private VisualTreeAsset m_VisualTreeAsset = default;

    VisualFluidBTView visualFuildBTView;
    InspectorView inspectorView;

    [MenuItem("Window/VisualFluidBT")]
    public static void OpenWindow()
    {
        VisualFluidBT wnd = GetWindow<VisualFluidBT>();
        wnd.titleContent = new GUIContent("VisualFluidBT");
    }

    void OnEnterPlayMode(PlayModeStateChange state)
    {
        visualFuildBTView.SaveTree();
    }

    public void CreateGUI()
    {
        EditorApplication.playModeStateChanged += OnEnterPlayMode;

        // Each editor window contains a root VisualElement object
        VisualElement root = rootVisualElement;

        // Instantiate UXML
        VisualElement labelFromUXML = m_VisualTreeAsset.Instantiate();
        root.Add(labelFromUXML);

        visualFuildBTView = root.Q<VisualFluidBTView>();
        inspectorView =  root.Q<InspectorView>();

        staticVisualBT = visualFuildBTView;

        OnSelectionChange();
        visualFuildBTView.OnNodeSelected = OnNodeSelectionChanged;
        SetupButtonHandler();
    }

    void SetupButtonHandler()
    {
        VisualElement root = rootVisualElement;

        var buttons = root.Query<Button>();
        buttons.ForEach(RegisterHandler);
    }

    void RegisterHandler(Button button)
    {
        button.RegisterCallback<ClickEvent>(SaveTree);
    }

    void SaveTree(ClickEvent evt)
    {
        visualFuildBTView.SaveTree();
    }

    private void OnSelectionChange()
    {
        BehaviorTree tree = Selection.activeObject as BehaviorTree;

        if (tree)
        {
            visualFuildBTView.PopulateView(tree);
        }
    }

    void OnNodeSelectionChanged(NodeView node){
        inspectorView.UpdateSelection(node);
    }

    [UnityEditor.Callbacks.DidReloadScripts]
    private static void OnReloadScripts()
    {
        BehaviorTree tree = Selection.activeObject as BehaviorTree;

        if (tree && staticVisualBT != null)
        {
            staticVisualBT.PopulateView(tree);
        }
        Debug.Log("Scripts foram recompilados...");
    }
}
