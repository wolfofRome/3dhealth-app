using System.Collections;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using I2.Loc;

public class LangSettingBtn : MonoBehaviour {

    public GameObject
        titleText,
        rebootText,
        langSettingWindow;

    public Button
        ok,
        close,
        japanese,
        english,
        chinese;

    private bool
        langSettingWindowSW = false;

    void Start() {
        rebootText.SetActive(false);
        ok.gameObject.SetActive(false);
        langSettingWindow.SetActive(langSettingWindowSW);
        gameObject.GetComponent<Button>().onClick.AddListener(() => ShowMenu());
        close.onClick.AddListener(() => ShowMenu());
        ok.onClick.AddListener(() => CloseApp());
        japanese.onClick.AddListener(() => SetLanguage("Japanese"));
        english.onClick.AddListener(() => SetLanguage("English"));
        chinese.onClick.AddListener(() => SetLanguage("Chinese"));
    }

    void ShowMenu() {
        langSettingWindowSW = !langSettingWindowSW;
        langSettingWindow.SetActive(langSettingWindowSW);
    }

    void SetLanguage(string s) {
        if(PlayerPrefs.GetString("Lang") != s) {
            PlayerPrefs.SetString("Lang", s);
            LocalizationManager.CurrentLanguage = PlayerPrefs.GetString("Lang");

            titleText.SetActive(false);
            close.gameObject.SetActive(false);
            japanese.gameObject.SetActive(false);
            english.gameObject.SetActive(false);
            chinese.gameObject.SetActive(false);

            rebootText.SetActive(true);
            ok.gameObject.SetActive(true);
        } else {
            ShowMenu();
        }
    }

    void CloseApp() {
#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}