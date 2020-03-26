using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class scoreCalculator : MonoBehaviour {

    [System.Serializable] public class mEvent : UnityEvent { }

    public float score;

	public static scoreCalculator instance;

    public Action<float> onNewScore;

	[System.Serializable]
	public class scoreRelevantPair{
		public valueDefinitions.values valueType;
		public float multiplier;
	}

	public scoreRelevantPair[] scoreValues;
	public scoreCounter[] extraScores;

	public scoreCounter highScore;
	public scoreCounter maxHighScore;

    [System.Serializable]
    public class C_ScoreSave {
        public bool saveScoreToValue = false;
        public valueDefinitions.values saveToValue;
    }

    //KingsLevelUp levelUp;

	public void calculateScore(){

        score = 0f;
		ValueScript vs;
		foreach (scoreRelevantPair srp in scoreValues) {
			vs = valueManager.instance.getFirstFittingValue (srp.valueType);
			score += vs.value * srp.multiplier;
		}

		foreach (scoreCounter sc in extraScores) {
			score += sc.getScore ();
		}

		if (highScore != null) {
			highScore.setScore(Mathf.RoundToInt(score));
		}

		if (maxHighScore != null) {
			maxHighScore.setMaxScore(Mathf.RoundToInt(score));
		}

        addLevelUpXp(score);

        if (onNewScore != null)
        {
            onNewScore(score);
        }
	}

	void Awake() {
		instance = this;
        //levelUp = gameObject.GetComponent<KingsLevelUp>();
	}

    void addLevelUpXp(float xp) {
        //if (levelUp != null) {
        if (KingsLevelUp.instance != null)
        {
            KingsLevelUp.instance.AddXp(xp);
        }
        //}
    }

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}


}

#if UNITY_EDITOR

[CustomPropertyDrawer(typeof(scoreCalculator.scoreRelevantPair))]
public class scoreRelevantPairDrawer : PropertyDrawer
{
	// Draw the property inside the given rect
	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
	{
		//don't alter
		EditorGUI.BeginProperty(position, label, property);
		position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);
		var indent = EditorGUI.indentLevel;
		EditorGUI.indentLevel = 0;

		// Calculate rects
		var modRect = new Rect(position.x, position.y, position.width * 0.50f, position.height);
		var valRect = new Rect(position.x + position.width * 0.52f  , position.y, position.width * 0.48f , position.height);

		// Draw fields - passs GUIContent.none to each so they are drawn without labels
		EditorGUI.PropertyField(modRect, property.FindPropertyRelative("valueType"), GUIContent.none);
		EditorGUI.PropertyField(valRect, property.FindPropertyRelative("multiplier"), GUIContent.none);


		//don't alter
		EditorGUI.indentLevel = indent;
		EditorGUI.EndProperty();
	}
}
#endif
