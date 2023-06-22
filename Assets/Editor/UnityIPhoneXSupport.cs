#if UNITY_IOS

using UnityEditor;
using UnityEditor.Callbacks;
using System.IO;
using UnityEditor.iOS.Xcode;
using UnityEngine;

public class UnityIPhoneXSupport
{
    [PostProcessBuild]
    public static void OnPostProcessBuild(BuildTarget buildTarget, string path)
    {

        if (UnityEditor.PlayerSettings.statusBarHidden)
        {
            var plistPath = Path.Combine(path, "Info.plist");
            var plist = new PlistDocument();
            plist.ReadFromFile(plistPath);
            plist.root.SetBoolean("UIStatusBarHidden", false);
            plist.WriteToFile(plistPath);

            string viewControllerPath = Path.Combine(path, "Classes/UI/UnityViewControllerBaseiOS.mm");
            if (!File.Exists(viewControllerPath))
            {
                viewControllerPath = Path.Combine(path, "Classes/UI/UnityViewControllerBase+iOS.mm");
            }
            string viewControllerContent = File.ReadAllText(viewControllerPath);
            string vcOldText = "    return _PrefersStatusBarHidden;";
            string vcNewText = "    CGSize size = [UIScreen mainScreen].bounds.size;\n" +
                               "    if (fmax(size.height, size.width) / fmin(size.height, size.width) > 2.15f) {\n" +
                               "        return NO;\n" +
                               "    } else {\n" +
                               "        return YES;\n" +
                               "    }";
            viewControllerContent = viewControllerContent.Replace(vcOldText, vcNewText);
            File.WriteAllText(viewControllerPath, viewControllerContent);
        }
    }
}

#endif
