using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

/*
 * 'DelayedEvent' invokes an unity event after an adjustable time.
 * The start of the event can also be triggered by an event or at the
 * activation of the gameobject of the script.
 */

public class DelayedEvent : MonoBehaviour {
    [System.Serializable] public class mEvent : UnityEvent { }

    [Tooltip("Can the event be retriggered while the timer already counts down and the until the event gets invoked extends (retriggerable = true)," +
		" or will it be discarded (retriggerable = false).")]
	public bool retriggerable = false;
	[Tooltip("Use the time scale of the game (unscaledTime = false) or ignore times scale (unscaledTime = true)")]
	public bool unscaledTime = false;
	[Tooltip("Start the delay to the event at activation of the gameobject (startOnStart = true) or only by triggering it.")]
	public bool startOnStart = false;
	public float delay = 1f;

    public C_UIDisplays countdownDisplay;
	[Tooltip("Event after time count down.")]
	public mEvent AfterDelay;

    [System.Serializable]
    public class C_UIDisplays
    {
        public Text countdownText;
        public string countdownFormat = "#";
        public Slider countdownSlider;
    }



	/*
	 * Call 'startEventdelay()' by a script or an event to start the count down.
	 * The exact timing and behavior of the count down is dependent on the configuration of the 'DelayedEvent' script.
	 */
	public void startEventdelay(){
		if (retriggerable == true) {
			StopAllCoroutines ();
			StartCoroutine (_delay ());
		} else {
			if (timerActive == false) {
				StartCoroutine (_delay ());
			} else {
				//do nothing, the routine is already running
			}
		}
	}

	void Start(){
		if (startOnStart == true) {
			StartCoroutine (_delay ());
		}
	}

	[ReadOnlyInspector]bool timerActive = false;

	//the actual delay to the invoke of the event
	IEnumerator _delay(){
		timerActive = true;

        float tmpDelay = delay;

		if (unscaledTime == true) {
            while (tmpDelay > 0f)
            {
                tmpDelay -= Time.unscaledDeltaTime;
                actualizeCountdowns(delay, tmpDelay);
                yield return null;
            }
			//yield return new WaitForSecondsRealtime (delay);
		} else {
            while (tmpDelay > 0f)
            {
                tmpDelay -= Time.deltaTime;
                actualizeCountdowns(delay, tmpDelay);
                yield return null;
            }
            //yield return new WaitForSeconds (delay);
		}

		yield return null;

		AfterDelay.Invoke ();
		timerActive = false;
	}

    void actualizeCountdowns(float maxTime, float actTime)
    {
        if(countdownDisplay.countdownSlider!= null)
        {
            countdownDisplay.countdownSlider.maxValue = maxTime;
            countdownDisplay.countdownSlider.value = actTime;
        }

        if(countdownDisplay.countdownText != null)
        {
            countdownDisplay.countdownText.text = actTime.ToString(countdownDisplay.countdownFormat);
        }
    }
}
