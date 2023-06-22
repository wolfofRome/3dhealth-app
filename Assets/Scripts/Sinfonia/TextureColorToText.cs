using UnityEngine;
using UnityEngine.UI;

public class TextureColorToText : MonoBehaviour {
    public Image
        colorSampleImage;
    void Update() {
        GetComponent<Text>().color = colorSampleImage.color;
    }
}
