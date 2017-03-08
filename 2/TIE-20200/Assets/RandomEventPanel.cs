using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class RandomEventPanel : MonoBehaviour {
	public Text panelName;
	public Button acceptAction;
	private Text acceptButtonText;
	public Button declineAction;
	private Text declineButtonText;
	// Use this for initialization
	void Awake () {
		acceptButtonText = acceptAction.GetComponentInChildren<Text> ();
		declineButtonText = declineAction.GetComponentInChildren<Text> ();
	}
	void OnEnable(){
		acceptAction.onClick.RemoveAllListeners ();
		declineAction.onClick.RemoveAllListeners ();

		if (GameManager.instance.currentRandomEvent==null) {
			panelName.text = "Quest";
			acceptButtonText.text = "Continue quest";
			declineButtonText.text = "Cancel quest";
			acceptAction.onClick.AddListener (delegate {
				GameManager.instance.ContinueQuest();
			});
			declineAction.onClick.AddListener (delegate {
				GameManager.instance.CancelQuest();
				gameObject.SetActive(false);
			});
		}
		else {
			panelName.text = "Event";
			acceptButtonText.text = "Figth!";
			acceptAction.onClick.AddListener (delegate {
				GameManager.instance.StartRandomEvent();
			});

			if (GameManager.instance.currentRandomEvent.type == RandomEventType.AGGRESSIVE){
				declineAction.onClick.AddListener (delegate {
					GameManager.instance.CancelQuest();
					gameObject.SetActive(false);
					// LoseUnits?
				});
				declineButtonText.text = "Try to flee";
			}
			else if (GameManager.instance.currentRandomEvent.type == RandomEventType.OPTIONAL){
				declineAction.onClick.AddListener (delegate {
					GameManager.instance.CancelQuest();
					gameObject.SetActive(false);
				});
				declineButtonText.text = "Ingore";
			}
		}
	}
}
