using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShowGameLog : MonoBehaviour {

	public Text logText;

    public GameLogger.E_SubLogTarget subLogSelection = GameLogger.E_SubLogTarget._default;

	// Use this for initialization
	void Start () {
		logText.text = GameLogger.instance.getGameLog (subLogSelection);
	}

}
