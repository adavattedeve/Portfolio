using UnityEngine;
using System.Collections;

public class PlaceHolderMusicSystem : MonoBehaviour {

	public AudioClip menu;
	public AudioClip battle;

	private AudioSource source;

	void Awake(){
		source = GetComponent<AudioSource> ();
		source.clip = menu;
		source.Play();
	}
	void OnLevelWasLoaded(int level){
		if (level == GameManager.instance.mainMenuSceneIndex || level == GameManager.instance.gameSceneIndex) {
			if (source.clip == null || source.clip != menu) {
				source.clip = menu;
				source.Play();
			}
		}
		else {
			if (source.clip ==null || source.clip !=battle){
				source.clip=battle;
				source.Play();
			}
		}
	}
}
