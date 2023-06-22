using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
using System.Collections;
using System.Collections.Generic;
using System.IO;
#if UNITY_IOS
using UnityEditor.iOS.Xcode;
#endif

public class PostBuildProcess
{
    [PostProcessBuild]
    public static void OnPostProcessBuild (BuildTarget buildTarget, string path)
    {
        if (buildTarget == BuildTarget.iOS) {
#if UNITY_IOS
            ProcessForiOS (path);
#endif
        }
    }
#if UNITY_IOS
    private static void ProcessForiOS (string path)
    {
        string pjPath = PBXProject.GetPBXProjectPath (path);
        PBXProject pj = new PBXProject ();
        pj.ReadFromString (File.ReadAllText (pjPath));
        string target = pj.TargetGuidByName ("Unity-iPhone");

        // Enable BitCode -> NO
        pj.SetBuildProperty (target, "ENABLE_BITCODE", "NO");
        
        List<string> frameworks = new List<string> () {
            //"AdSupport.framework",
            "CoreData.framework",
            "SystemConfiguration.framework",
            "libz.tbd",
            "libsqlite3.tbd"
        };

        foreach (var framework in frameworks) {
            pj.AddFrameworkToProject (target, framework, false);
        }

        File.WriteAllText (pjPath, pj.WriteToString ());
    }
#endif
}