// AlmostLogical Software - http://www.almostlogical.com - support@almostlogical.com
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

public class PPEditorWindow : EditorWindow
{
    private Vector2 scrollPosition = new Vector2();

    private static List<PPCurrentContext> allContexts = new List<PPCurrentContext>();
    private static List<PPCurrentContext> currentContexts = new List<PPCurrentContext>();

    public static PPEditorWindow currentWindow;

    public static void AutoInit()
    {
            EditorWindow[] allWindows = Resources.FindObjectsOfTypeAll(typeof(EditorWindow)) as EditorWindow[];
            bool doesHavePPEditorWindow = false;
            foreach (EditorWindow editorWin in allWindows)
            {
                if (editorWin is PPEditorWindow)
                {
                    doesHavePPEditorWindow = true;
                    break;
                }
            }
            if (!doesHavePPEditorWindow)
            {
                RemoveAllFailedToLoadWindows(allWindows);
                Init(true);
            }
    }

    [MenuItem("Window/PlayModePersist")]
    static void ManualInit()
    {
        Init(false);
    }

    static void Init(bool manualFocus)
    {
            EditorWindow inspectorWindow = getEditorWindowOfTypeStr("UnityEditor.InspectorWindow");
            if (inspectorWindow == null)
            {
                PPEditorWindow.currentWindow = (PPEditorWindow)EditorWindow.GetWindow<PPEditorWindow>("PM-Persist", false);
            }
            else
            {
                System.Type[] dockNextTo = new System.Type[1];
                dockNextTo[0] = inspectorWindow.GetType();
                PPEditorWindow.currentWindow = (PPEditorWindow)EditorWindow.GetWindow<PPEditorWindow>("PM-Persist", false, dockNextTo);
                if (manualFocus)
                {
                    inspectorWindow.Focus();
                }
            }
    }

    private static void RemoveAllFailedToLoadWindows(EditorWindow[] allWindows)
    {
        foreach (EditorWindow editorWin in allWindows)
        {
            if (editorWin.GetType().ToString() == "UnityEditor.FallbackEditorWindow") //cleans up old unused windows to deal with Unity layout bug
            {
                editorWin.Close();
            }
        }
    }

    private static void RemoveFailedToLoadWindowDockedWithInspector(EditorWindow[] allWindows)
    {
        EditorWindow inspectorWindow = null;
        foreach (EditorWindow editorWin in allWindows)
        {
            if (editorWin.GetType().ToString() == "UnityEditor.InspectorWindow")
            {
                inspectorWindow = editorWin;
                break;
            }
        }
      
        if (inspectorWindow != null)
        {
            foreach (EditorWindow editorWin in allWindows)
            {
                if (editorWin.GetType().ToString() == "UnityEditor.FallbackEditorWindow") //cleans up old unused windows to deal with Unity layout bug
                {
                    if (editorWin.position == inspectorWindow.position) //if docked
                    {
                        editorWin.Close();
                         break;
                    }
                }
            }
        }
    }

    public void OnSelectionChange()
    {
        Repaint(); 
    }

    public void OnEnable()
    {
        EditorApplication.playmodeStateChanged+= Application_PlaymodeStateChanged;
        if (currentWindow == null)
        {
            currentWindow = this;
        }
    }

    public void OnDestroy()
    {
        currentWindow = null;
    }

    public void OnGUI()
    {
        if (EditorApplication.isPlaying || EditorApplication.isPaused)
        {
            ReconcileContextsBasedOnSelection();

            if (currentContexts.Count > 0)
            {
                if (!currentContexts.Any(c =>
                {
                    PrefabAssetType contextPrefabType = GetPrefabTypeFromGameObject(c.GameObj);
                    return contextPrefabType == PrefabAssetType.Regular || contextPrefabType == PrefabAssetType.Model;
                }))
                {
                    DrawControls();
                }
                else
                {
                    GUILayout.Label("Prefabs cannot be persisted");
                }
            }
            else
            {
                GUILayout.Label("No GameObject(s) selected");
            }
        }
        else
        {
            GUILayout.Label("Only enabled in play mode");
        }
    }

    private void DrawControls()
    {
        PPGOSetting firstSetting = currentContexts.First().GameObjectSetting;

        EditorGUILayout.BeginVertical("box", GUILayout.ExpandWidth(true));
        GUILayout.Label("PlayModePersist");
        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("", GUILayout.Width(150f));

        if (firstSetting.ComponentSettings.Any(s => !s.IsSavingSettings))
        {
            currentContexts.ForEach(c => c.GameObjectSetting.SaveAll = false);
        }

        bool wasSaveAll = firstSetting.SaveAll;

        bool saveAll = GUILayout.Toggle(wasSaveAll, "Save All");

        currentContexts.ForEach(c => c.GameObjectSetting.SaveAll = saveAll);

        EditorGUILayout.EndHorizontal();

        GUILayout.Box("", GUILayout.Height(1f), GUILayout.ExpandWidth(true));
        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

        if (firstSetting.SaveAll && !wasSaveAll)
        {
            currentContexts.ForEach(c => c.GameObjectSetting.ComponentSettings.ForEach(cs => cs.IsSavingSettings = true));
        }
        else if (!firstSetting.SaveAll && wasSaveAll)
        {
            currentContexts.ForEach(c => c.GameObjectSetting.ComponentSettings.ForEach(cs => cs.IsSavingSettings = false));
        }

        IEnumerable<IGrouping<string, PPComponentSetting>> componentGroups
            = currentContexts
                .SelectMany(c => c.GameObjectSetting.ComponentSettings)
                .GroupBy(cs => cs.ComponentName);

        foreach (IGrouping<string, PPComponentSetting> componentGroup in componentGroups)
        {
            PPComponentSetting firstComponentSetting = componentGroup.FirstOrDefault(c => !c.IsSavingSettings) ?? componentGroup.First();

            EditorGUILayout.BeginHorizontal();
            
            int count = componentGroup.Count();

            if (count > 1)
            {
                GUILayout.Label(componentGroup.Key + " (" + count + ")", GUILayout.Width(150f));
            }
            else
            {
                GUILayout.Label(componentGroup.Key, GUILayout.Width(150f));
            }

            EditorGUI.showMixedValue = !componentGroup.All(c => c.IsSavingSettings == firstComponentSetting.IsSavingSettings);
            bool isSavingSettings = GUILayout.Toggle(firstComponentSetting.IsSavingSettings, "Save");

            if (isSavingSettings != firstComponentSetting.IsSavingSettings)
            {
                foreach (PPComponentSetting setting in componentGroup)
                {
                    setting.IsSavingSettings = isSavingSettings;
                }
            }

            EditorGUILayout.EndHorizontal();
        }

        EditorGUILayout.EndScrollView();
        EditorGUILayout.EndVertical();
    }

    public static void SetPersistanceForAllExistingContextObjects(string typeName, bool isSaveSetting)
    {
        foreach (PPCurrentContext context in allContexts.Where(c => c.GameObj != null))
        {
            PPComponentSetting componentSetting = context.GameObjectSetting.ComponentSettings.FirstOrDefault(cs => cs.ComponentName == typeName);

            if (componentSetting != null)
            {
                componentSetting.IsSavingSettings = isSaveSetting;

                PPCurrentContext currentContext = currentContexts.FirstOrDefault(c => c.GameObj == context.GameObj);

                // If current context is changed and the PlayModePersist panel is open, update the checkboxes
                if (currentContext != null && currentContext.GameObjectSetting.Expanded)
                {
                    EditorUtility.SetDirty(context.GameObj);
                }
            }
        }
    }

    public static void PersistSelectedGameObject()
    {
        ReconcileContextsBasedOnSelection();

        if (currentContexts.Count > 0)
        {
            bool saveAll = !currentContexts.First().GameObjectSetting.SaveAll;

            currentContexts.ForEach(c =>
            {
                c.GameObjectSetting.SaveAll = saveAll;
                c.GameObjectSetting.ComponentSettings.ForEach(s => s.IsSavingSettings = saveAll);
                c.GameObjectSetting.Expanded = true;
                EditorUtility.SetDirty(c.GameObj);
            });
        }
    }

    private static void Application_PlaymodeStateChanged()
    {
        if (EditorApplication.isPlaying || EditorApplication.isPaused)
        {
            AddAllContextsWithDefaults();
            for (int n = 0; n < allContexts.Count; n++)
            {
                allContexts[n].GameObjectSetting.StoreAllSelectedSettings();
            }
            if (PPEditorWindow.currentWindow != null)
            {
                currentWindow.Repaint();
            }
        }
        else
        {
            List<Component> listOfComponentsChanged;
            PrefabInstanceStatus prefabInstanceStatus;
            for (int n = 0; n < allContexts.Count; n++)
            {
                listOfComponentsChanged = allContexts[n].GameObjectSetting.RestoreAllSelectedSettings();

                if (listOfComponentsChanged.Count>0)
                {
                    prefabInstanceStatus = PrefabUtility.GetPrefabInstanceStatus(allContexts[n].GameObj);
                    if (prefabInstanceStatus == PrefabInstanceStatus.Disconnected || prefabInstanceStatus == PrefabInstanceStatus.Connected)
                    {
                        foreach (Component changedComp in listOfComponentsChanged)
                        {
                            EditorUtility.SetDirty(changedComp);
                        }
                    }
                }
            }

            allContexts.Clear();
            if (PPEditorWindow.currentWindow != null)
            {
                currentWindow.Repaint();
            }
        }
    }

    private static void ReconcileContextsBasedOnSelection()
    {
        currentContexts.Clear();

#if (UNITY_2_6 || UNITY_2_6_1 || UNITY_3_0 || UNITY_3_0_0 || UNITY_3_1 || UNITY_3_2 || UNITY_3_3 || UNITY_3_4)
        List<GameObject> selectedGameObjects = new List<GameObject>();
        if (Selection.activeGameObject != null)
        {
            selectedGameObjects.Add(Selection.activeGameObject);
        }
#else
        List<GameObject> selectedGameObjects = Selection.gameObjects.Where(g => !currentContexts.Any(c => c.GameObj == g)).ToList();
#endif

        foreach (GameObject gameObject in selectedGameObjects)
        {
            PPCurrentContext context = allContexts.SingleOrDefault(c => c.GameObj == gameObject);

            if (context == null)
            {
                context = CreateContext(gameObject.transform);
            }

            currentContexts.Add(context);
        }
    }

    private PrefabAssetType GetPrefabTypeFromGameObject(GameObject gameObject)
    {
        return PrefabUtility.GetPrefabAssetType(gameObject);
    }

    private static PPCurrentContext CreateContext(Transform transform)
    {
        PPCurrentContext context = new PPCurrentContext();
        allContexts.Add(context);
        context.SetContext(transform);
        return context;
    }

    private static List<Assembly> GetActiveAssemblies()
    {
        List<Assembly> assemblyList = new List<Assembly>();
        Assembly[] currDomainAssemblies = System.AppDomain.CurrentDomain.GetAssemblies();
        string asmPath;
        foreach (Assembly currDomainAsm in currDomainAssemblies)
        {
            asmPath = currDomainAsm.CodeBase;
            if (asmPath.EndsWith("Assembly-UnityScript.dll") || asmPath.EndsWith("Assembly-CSharp.dll"))
            {
                assemblyList.Add(currDomainAsm);
            }
        }
        return assemblyList;
    }

    private static void AddAllContextsWithDefaults()
    {
        if (PPLocalStorageManager.Defaults.Count > 0)
        {
            List<Assembly> assemblyList = null;
          
            foreach (string defaultTypeStr in PPLocalStorageManager.Defaults)
            {
                if (!defaultTypeStr.Contains("."))
                {
                    if (assemblyList == null) { assemblyList = GetActiveAssemblies(); }
   
                    foreach (Assembly asm in assemblyList)
                    {
                        TryAddMissingGameObject(asm,defaultTypeStr);
                    }
                }
                else
                {
                    TryAddMissingGameObject(typeof(Component).Assembly, defaultTypeStr);
                }
            }
        }
    }

    private static bool TryAddMissingGameObject(Assembly assembly, string typeName)
    {
        bool success = false;

        if (assembly != null)
        {
            System.Type type = assembly.GetType(typeName);
            if (type != null)
            {
                AddMissingGameObjectsByType(type);
                success = true;
            }
        }

        return success;
    }

    private static void AddMissingGameObjectsByType(System.Type type)
    {
        Object[] objects = Object.FindObjectsOfType(type);

        List<GameObject> gameObjects = 
            objects.Select(o => o as Component)
            .Where(c => c != null && c.gameObject != null)
            .Select(c => c.gameObject)
            .ToList();

        foreach (GameObject gameObject in gameObjects)
        {
            if (!allContexts.Any(c => c.GameObj == gameObject))
            {
                CreateContext(gameObject.transform);
            }
        }
    }

    private static EditorWindow getEditorWindowOfTypeStr(string type)
    {
        EditorWindow[] allWindows = Resources.FindObjectsOfTypeAll(typeof(EditorWindow)) as EditorWindow[];
        EditorWindow requestedWindow = null;
        foreach (EditorWindow window in allWindows)
        {
            if (window.GetType().ToString() == type)
            {
                requestedWindow = window;
                break;
            }
        }
        return requestedWindow;
    }
}