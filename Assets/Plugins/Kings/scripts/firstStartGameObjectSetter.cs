using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class firstStartGameObjectSetter : MonoBehaviour {

	public GameObject[] gameobjectToDeactivate;
	public GameObject gameobjectToActivate;

	// Use this for initialization
	void Start () {
		int firststartup = PlayerPrefs.GetInt ("firstStartUp");

		if (firststartup == 0) {
			if(gameobjectToDeactivate!=null){
				foreach(GameObject g in gameobjectToDeactivate){
					g.SetActive(false);
				}
			}

			if(gameobjectToActivate !=null){
				gameobjectToActivate.SetActive(true);
			}

			PlayerPrefs.SetInt("firstStartUp",1);
		}
	}
}
