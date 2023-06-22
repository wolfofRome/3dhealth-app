using UnityEngine.UI;

namespace QuickResponseCode
{
	public class QuickResponseCodeManager : QuickResponseCodeGenerator {

		// Use this for initialization
		void Start () {
			this.GetComponent<Image>().sprite = QrSprite;
		}
	
		// Update is called once per frame
		void Update () {
		
		}
	}
}
