using UnityEngine;
using UnityEngine.UI;

public class BeforeAfterButtonController : MonoBehaviour {

    public Sprite[]
        buttonImgs;
        // 0:default  1:chinese

    void Update() {
        int i = 0;
        switch (PlayerPrefs.GetString("Lang")) {
            default:
            case "Japanese":
                i = 0;
                break;
            case "Chinese":
                i = 1;
                break;
        }
        GetComponent<Image>().sprite = buttonImgs[i];
    }
}