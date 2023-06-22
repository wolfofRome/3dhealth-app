using UnityEngine;
using UnityEngine.UI;

public class HistoryTitles : MonoBehaviour {
    public Text[]
        _bc,
        _size;
    public static string[]
        bc,
        size;

    private void Awake() {
        bc = new string[_bc.Length];
        size = new string[_size.Length];
        for(int i = 0; i < _bc.Length; i++) {
            bc[i] = _bc[i].text;
        }
        for (int i = 0; i < _size.Length; i++) {
            size[i] = _size[i].text;
        }
    }
}
