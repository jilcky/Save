

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


/*
 * Logger to give a summary of the played game at the end of the game.
 */

public class GameLogger : TranslatableContent {



	public static GameLogger instance;

    private string logKey = "gameLog";
    private string splitLogKey = "splitGameLog";
    private int subLogLimit = 100;

    [System.Serializable]
    public enum E_SubLogTarget
    {
        _default = 0,
        subLog1,
        subLog2,
        subLog3,
        subLog4,
        subLog5,
        subLog6,
        subLog7,
        subLog8,
        subLog9,
        subLog10
    }

    [System.Serializable]
    public class C_InspectorGameLogEntry
    {
        public E_SubLogTarget subLogSelection = E_SubLogTarget._default;
        public string text = "";
        public void addGameLogText(string txt)
        {
            instance.addGameLog(txt, subLogSelection);
        }


        public List<string> GetTranslatableContent()
        {
            List<string> tContent = new List<string>();
            tContent.Add(text);
            return tContent;
        }
    }

	/*
	 * Returns the game log as text.
	 */
	public string getGameLog(int subLogSelection = -1){
		return buildResultText (subLogSelection);	
	}
    public string getGameLog(E_SubLogTarget subLogSelection )
    {
        return getGameLog(subLogEnumToInt(subLogSelection));
    }

    private int subLogEnumToInt(E_SubLogTarget subLogSelection)
    {
        switch (subLogSelection)
        {
            case E_SubLogTarget._default:
                return -1;
            default:
                return (int)subLogSelection;
        }
    }
    

    /*
	 * Shows the gamelog at the defined text box.
	 */
    [HideInInspector]public Text gameLogText;
	public void showGameLogUI(int subLogSelection = -1){
		if (gameLogText != null) {
			gameLogText.text = buildResultText (subLogSelection);
		}
	}

	[System.Serializable]
	public class strList
	{
		public bool locked = false;
		public List <string> gameLogs;
	}

	[Tooltip("Gamelog of the actual game.")]
	[ReadOnlyInspector]public strList logs;

    [System.Serializable]
    public class C_encapsulatedStringList
    {
        public List<string> gameLogs = new List<string>();
    }

    [System.Serializable]
    public class C_SplitLogs
    {
        public bool locked = false;
        public List<C_encapsulatedStringList> subLogs = new List<C_encapsulatedStringList>();
    }
    [Tooltip("Split logs are categorized logs.")]
    [ReadOnlyInspector]public C_SplitLogs splitLogs;

	[Tooltip("'textBreakEvery' generates a linebreak ever x logs to format the output string in a more readable text.")]
	public int textBreakEvery = 1;

	string buildResultText(int subLogSelection)
    {
		string result = "";
		int lineCnt = 0;

        //subLogSelection -1: use old log-system
        if (subLogSelection < 0)
        {
            foreach (string s in logs.gameLogs)
            {
                result = result + TextReplacement.TranslateAndReplace(s) + " ";

                lineCnt++;
                if (lineCnt >= textBreakEvery)
                {
                    result = result + "\n\n";
                    lineCnt = 0;
                }
            }
            // Debug.Log("Sub log " + subLogSelection.ToString() + ":" + result);
            return result;
        }else if(subLogSelection >= 0 && subLogSelection < splitLogs.subLogs.Count)
        {
            foreach (string s in splitLogs.subLogs[subLogSelection].gameLogs)
            {
                result = result + TextReplacement.TranslateAndReplace(s) + " ";

                lineCnt++;
                if (lineCnt >= textBreakEvery)
                {
                    result = result + "\n\n";
                    lineCnt = 0;
                }
            }
           // Debug.Log("Sub log " + subLogSelection.ToString() + ":" + result);
            return result;
        }
        else
        {
            result = "";
            Debug.LogWarning("There is no subLog '" + subLogSelection.ToString() + "'. Resulting string is empty");
            return result;
        }
        
	}


	void Awake(){
		instance = this;
	}

	void Start(){
		logs.gameLogs = new List<string> ();
		logs.gameLogs.Clear();
        splitLogs.subLogs = new List<C_encapsulatedStringList>();
        splitLogs.subLogs.Clear();
		loadGameLogs ();
        loadSplitGameLogs();
		TranslationManager.instance.registerTranslateableContentScript (this);
	}

	/*
	 * Force to load the gamelogs. This is autmatically done at start of the script.
	 */
	public bool loadGameLogs(){
		string json = PlayerPrefs.GetString (logKey);
		if (string.IsNullOrEmpty (json)) {
			return false;
		} else {
			JsonUtility.FromJsonOverwrite (json, logs);
			return true;
		}
	}
    public bool loadSplitGameLogs()
    {
        string json = PlayerPrefs.GetString(splitLogKey);
        if (string.IsNullOrEmpty(json))
        {
            return false;
        }
        else
        {
            JsonUtility.FromJsonOverwrite(json, splitLogs);
            return true;
        }
    }

    /*
	 * Locking the logs prevents the script from adding new logs.
	 * This is needed, because the loading of a running game can cause cards to add already logged logs.
	 */
    public void lockOutput(bool doLock){
		logs.locked = doLock;
        splitLogs.locked = doLock;
	}

	public void saveGameLogs(){
        //save logs
		string json = JsonUtility.ToJson (logs);
		PlayerPrefs.SetString (logKey, json);
        //save split logs
        json = JsonUtility.ToJson(splitLogs);
        PlayerPrefs.SetString(splitLogKey, json);
    }


    /*
	 * Add a new log-entry for this game, if the logger is not locked.
     * 'subLogSelection' lets the designer select the target sub log. -1 for using default log system.
	 */
    public void addGameLog(string log, E_SubLogTarget subLogSelection)
    {
        //Debug.Log("Adding to '" + subLogSelection.ToString() + "' (" + ((int)subLogSelection).ToString() + "):" + log);
        addGameLog(log, subLogEnumToInt(subLogSelection));
    }
    public void addGameLog(string log, int subLogSelection = -1){
		if (!string.IsNullOrEmpty (log) && logs.locked == false) {
            if (subLogSelection < 0)
            {
                string txt = log;
                logs.gameLogs.Add(txt);

            }
            else
            {
                if (subLogSelection > subLogLimit)
                {
                    Debug.LogError("The limit for the max. index of sub logs is " + subLogLimit.ToString() + ". Logging to selection " + subLogSelection.ToString() + " is discarded.");
                    return;
                }

                //fill sub logs until selected log becomes available
                while (subLogSelection >= splitLogs.subLogs.Count)
                {
                    C_encapsulatedStringList subLog = new C_encapsulatedStringList();
                    splitLogs.subLogs.Add(subLog);
                }

                string txt = log;
                splitLogs.subLogs[subLogSelection].gameLogs.Add(txt);
            }

            saveGameLogs();
        }
	}

	/*
	 * By calling 'clearGameLog' the gamelog is cleared and the lock is removed.
	 */
	public void clearGameLog(){
		logs.gameLogs.Clear ();
		logs.locked = false;
        splitLogs.subLogs.Clear();
        splitLogs.locked = false;
		saveGameLogs ();
	}

	/*
	 * Return all possible translatable terms
	 */
	public override List<string> getTranslatableTerms ()
	{
		List<string> terms = new List<string> ();
		terms.Clear ();

		Debug.LogWarning ("Strings of 'GameLogger' are not directly listed and therefore can't be completely added to translation term list. ");

		return terms;
	}
}
