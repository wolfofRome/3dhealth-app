using UnityEngine;
using UnityEngine.UI;

public class VersionTextManager : MonoBehaviour {

	// Use this for initialization
	void Start () {
        Text versionText = gameObject.GetComponent<Text>();
        versionText.text = "Version " + Application.version;
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
