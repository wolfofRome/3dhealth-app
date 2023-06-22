using Assets.Scripts.Common;
using Assets.Scripts.Compare;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.ThreeDView
{
	[RequireComponent(typeof(Button))]
	[RequireComponent(typeof(CompareLoadParam))]
	public class BeforeAfterController : MonoBehaviour
	{
        private void OnEnable()
        {
            Button button = GetComponent<Button>();
            button.interactable = DataManager.Instance.ContentsList.Count >= 2;
        }

        public void OnClick()
        {
            var contentsList = DataManager.Instance.ContentsList.OrderByDescending(v => v.CreateTimeAsDateTime).ToList();
            
            CompareLoadParam compareLoadParam = GetComponent<CompareLoadParam>();
            compareLoadParam.argment.leftContents = contentsList[0];
            compareLoadParam.argment.rightContents = contentsList[1];
            
            SceneLoader.Instance.LoadSceneWithParams(compareLoadParam);
        }
	}
}
