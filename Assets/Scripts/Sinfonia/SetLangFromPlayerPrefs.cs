using System.Collections;
using UnityEngine;
using I2.Loc;

public class SetLangFromPlayerPrefs : MonoBehaviour {

    void Awake() {
        if (PlayerPrefs.HasKey("Lang")) LocalizationManager.CurrentLanguage = PlayerPrefs.GetString("Lang");
        else FirstLanguageSetup();
    }

    void FirstLanguageSetup() {
        string s = "";
        switch (Application.systemLanguage) {
            case SystemLanguage.Japanese:
                s = "Japanese";
                break;
            case SystemLanguage.Chinese:
            case SystemLanguage.ChineseSimplified:
            case SystemLanguage.ChineseTraditional:
                s = "Chinese";
                break;
            default:
                s = "English";
                break;
        }
        PlayerPrefs.SetString("Lang", s);
        LocalizationManager.CurrentLanguage = PlayerPrefs.GetString("Lang");
    }
}