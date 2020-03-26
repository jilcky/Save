using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;


/// <summary>
/// KingsLevelUp computes level ups from xp.
/// It allows for different xp-costs for different levels.
/// It handles the UI elements to show and fill the xp bar and shows the level numbers.
/// It generates events if something changes.
/// 
/// It saves/loads the actual state and allows for one level-up.
/// If it should handle multible level ups, make the 'prefsKey' public and adjust it for each instance.
/// </summary>

public class KingsLevelUp : MonoBehaviour {
    #region definitions
    [System.Serializable] public class mEvent : UnityEvent { }

    [System.Serializable]
    public class C_LevelUpStats
    {
        [Tooltip("What is the max-level?")]
        public float maxLevel = 999;
        [Tooltip("Define the costs per level for levelling up. The list should start from level 0. Not mentioned levelnumbers are filled:\n" +
"Level 0 cost 100\n" +
"Level 5 cost 500\n" +
"Level 10 cost 1000\n" +
"\n" +
"results in the level cost:\n" +
"Level 1: 100\n" +
"Level 2: 100\n" +
"...\n" +
"Level 4: 100\n" +
"Level 5: 500\n" +
"...\n" +
"Level 10: 1000\n" +
"Level >10: 1000")]
        public List<C_XpPerLevel> xpCosts = new List<C_XpPerLevel>();
        [Tooltip("What is the actual player level, starting with 1.")]
        [ReadOnlyInspector] public int actualLevel;
        [Tooltip("How much xp is needed until the next level.")]
        [ReadOnlyInspector] public float XpNextLevel;
        [Tooltip("How much xp was awarded the player in total.")]
        [ReadOnlyInspector] public float XpAll;
        [Tooltip("How much the player has actual for the actual level.")]
        [ReadOnlyInspector] public float XpActual;
    }

    [Tooltip("The actual level up stats.")]
    public C_LevelUpStats stats;

    //The key to identify the save/load for this levelling.
    string prefsKey = "levelUp";

    /// <summary>
    /// Configuration entry for one xp cost per level.
    /// </summary>
    [System.Serializable]
    public class C_XpPerLevel
    {
        [Tooltip("From which level is the cost applicable?")]
        public int fromLevel = 0;
        [Tooltip("How much xp does it cost to increase to the next level starting from this level?")]
        public int xpCost = 1000;
    }


       
    /// <summary>
    /// Group for the configuration of the UI elements.
    /// </summary>
    [System.Serializable]
    public class C_LevelUpUiConfig
    {
        public Slider xpBar;
        public Image imageFill;
        [Tooltip("How fast shall the xp bar be filled. Value is in percentage/second.")]
        [Range(0.01f, 10f)] public float xpBarFillSpeed = 0.5f;
        [Tooltip("If the ui actualizes a level, how long shall it wait to fill the next bar?")]
        public float levelIncreaseFillDelay = 0.1f;

        //For compability reasons the texts are a little bit strange.
        //Can't solve this with a clean solution without the destruction of links of all the other cards. 
        
        public EventScript.C_Texts actualLevelNumber;
        public EventScript.C_Texts delayedUILevelNumber;
    }

    [Tooltip("Configuration of the UI elements and adjusting of the bahaviour.")]
    public C_LevelUpUiConfig uiConfig;

    /// <summary>
    /// Group for the events which can be triggered by this script.
    /// </summary>
    [System.Serializable]
    public class C_LevelUpEvents {
        public mEvent OnXpIncrease;
        public mEvent OnLevelUp;
        public mEvent OnUiStartBarFilling;
        public mEvent OnUiStopBarFilling;
        public mEvent OnUiLevelUp;
    }

    public C_LevelUpEvents events;

    //save the level to an kings-value
    [System.Serializable]
    public class C_ValueSave
    {
        public bool saveToValue = false;
        public valueDefinitions.values value;
    }

    public C_ValueSave levelSave;

    #endregion

    public static KingsLevelUp instance;

    private void Awake()
    {
        load();
        TestXpCostList();
        stats.XpNextLevel = GetXpForNextLevel(stats.actualLevel);
        uiLevelNumber = stats.actualLevel;
        StartCoroutine(cyclicFillingComputations());
        instance = this;
    }

    private void OnDestroy()
    {
        instance = null;
    }

    #region internal methods

    /// <summary>
    /// Test the 'xpPerLevel' cost list for ambigous or missing informations.
    /// </summary>
    void TestXpCostList() {
        //test if the list has elements
        if (stats.xpCosts.Count <= 0) {
            Debug.LogError("The xp cost list for levelling up contains no elements ("+gameObject.name+").");
            return;
        }
        //test if the list starts from level 0
        if (stats.xpCosts[0].fromLevel != 0) {
            Debug.LogError("The xp cost list for levelling up doesn't contain a cost definition from level 0 on. Please supply xp costs from level 0 in the first element (" + gameObject.name + ").");
            return;
        }
        //test if the list is in good order (definition for levels increase by element to element)
        int lastLevel = -1;
        int lastCost = -1;
        for (int i = 0; i< stats.xpCosts.Count; i++)
        {
            //test the increas of levels
            if (stats.xpCosts[i].fromLevel > lastLevel)
            {
                //level increases nicely
            }
            else {
                Debug.LogWarning("The xp cost list is not sorted nicely. It decreases/stays from " + lastLevel.ToString() + " to " + stats.xpCosts[i].fromLevel.ToString() + " at index " + i.ToString()+ " (" + gameObject.name + ").");
            }

            //test the value of costs, should be >0
            if (stats.xpCosts[i].xpCost <= 0)
            {
                Debug.LogWarning("The xp cost is <= 0. At index " + i.ToString() + " (" + gameObject.name + ").");
            }

            //test the increase of costs, if it stays it's no problem
            if (stats.xpCosts[i].xpCost < lastCost) {
                Debug.LogWarning("The xp cost list is configured strange. The xp cost decreases from " + lastCost.ToString() + " to " + stats.xpCosts[i].xpCost.ToString() + " at index " + i.ToString() + " (" + gameObject.name + ").");
            }

            lastLevel = stats.xpCosts[i].fromLevel;
            lastCost = stats.xpCosts[i].xpCost;
        }
    }

    /// <summary>
    /// Get the xp cost for the next level.
    /// Information is taken from the 'xpPerLevel' cost list.
    /// </summary>
    /// <param name="actLevel"></param>
    /// <returns></returns>
    float GetXpForNextLevel(int actLevel) {
        float nextXp = 0f;

        int selection = 0;
        for(int i = 0; i<stats.xpCosts.Count; i++) {
            if (actLevel >= stats.xpCosts[i].fromLevel)
            {
                selection = i;
            }
            else {
                //level in list is higher than actual level
                break;
            }
        }

        nextXp = stats.xpCosts[selection].xpCost;

        if (nextXp <= 0) {
            Debug.LogError("XP cost for level up <= 0.");
        }

        return nextXp;
    }

    /// <summary>
    /// Compute instant level increases and generates the targets for the ui levling up process.
    /// Called recursively until the xp is consumed.
    /// Doesn't save the progress.
    /// </summary>
    void computeLevelChange()
    {
        if (stats.XpActual >= stats.XpNextLevel)
        {

            if (stats.actualLevel == stats.maxLevel) {
                return;
            }

            stats.actualLevel++;

            stats.XpActual -= stats.XpNextLevel;

            //event is fired for each level increase
            events.OnLevelUp.Invoke();

            //check if the cost increased
            stats.XpNextLevel = GetXpForNextLevel(stats.actualLevel);

            //recursively, if xp is enough for multible level changes
            computeLevelChange();
        }
    }

    public float getXpBarFilling()
    {
        return fillingPercentage;
    }
    public int getUiIncreasingLevelNumber() {
        return uiLevelNumber;
    }

     float fillingPercentage = 0f;
    int uiLevelNumber = 0;
    IEnumerator cyclicFillingComputations() {
        //initialize
        uiLevelNumber = stats.actualLevel;
        float uiDisplayedXp = stats.XpActual;
        float uiTargetXpAll = stats.XpAll;
        fillingPercentage = stats.XpActual / stats.XpNextLevel;
        float targetFillingPercentage = fillingPercentage;
        bool isFilling = false;
        float fillSpeed = uiConfig.xpBarFillSpeed * Time.deltaTime;

        while (true) {
            //if the overall xp increased
            if (stats.XpAll > uiTargetXpAll)
            {

                if (isFilling == false)
                {
                    isFilling = true;
                    events.OnUiStartBarFilling.Invoke();
                }

                //if the displayed level is lower than the actual level:
                //fill the percentage bar until maximum (1) is reached
                if (uiLevelNumber < stats.actualLevel)
                {
                    fillingPercentage += fillSpeed;

                    if (fillingPercentage >= 1) {
                        uiLevelNumber++;
                        events.OnUiLevelUp.Invoke();
                        fillingPercentage = 1f;
                        actualizeUI();
                        yield return new WaitForSeconds(uiConfig.levelIncreaseFillDelay);
                        fillingPercentage = 0f;
                    }
                }
                else
                {
                    //if the levels are already the same, fill the xp bar until the target
                    //recompute the target filling, if it increases while filling
                    targetFillingPercentage = stats.XpActual / stats.XpNextLevel;

                    fillingPercentage += fillSpeed;

                    if (fillingPercentage >= targetFillingPercentage)
                    {
                        //filling is finished (the level number and the target filling is reached)
                        fillingPercentage = targetFillingPercentage;
                        uiTargetXpAll = stats.XpAll;
                    }
                }
            }
            else {
                if (isFilling == true) {
                    isFilling = false;
                    events.OnUiStopBarFilling.Invoke();
                }
            }

            actualizeUI();

            yield return null;
        }
    }

    void actualizeUI()
    {
        if (uiConfig.xpBar != null)
        {
            uiConfig.xpBar.value = fillingPercentage;
        }

        if (uiConfig.imageFill != null)
        {
            uiConfig.imageFill.fillAmount = fillingPercentage;
        }

        uiConfig.actualLevelNumber.text = stats.actualLevel.ToString();
        uiConfig.delayedUILevelNumber.text = uiLevelNumber.ToString();
    }

    #endregion

    /// <summary>
    /// Add xp to the levelUp.
    /// Level increases are automatically computed and events generated.
    /// </summary>
    /// <param name="xp"></param>
    public void AddXp(float xp)
    {
        if (xp > 0f)
        {
            stats.XpAll += xp;
            stats.XpActual += xp;
            computeLevelChange();
            save();

            events.OnXpIncrease.Invoke();
        }
    }


    #region save & load
    void save() {
        SecurePlayerPrefs.SetInt(prefsKey + "_actualLevel", stats.actualLevel);
        SecurePlayerPrefs.SetFloat(prefsKey + "_actualXp", stats.XpActual);
        SecurePlayerPrefs.SetFloat(prefsKey + "_allXp", stats.XpAll);

        if (levelSave.saveToValue == true)
        {
            ValueScript saveVs = valueManager.instance.getFirstFittingValue(levelSave.value);
            if (saveVs != null)
            {
                saveVs.setValue(stats.actualLevel);
            }
        }
    }

    void load() {
        stats.actualLevel = SecurePlayerPrefs.GetInt(prefsKey + "_actualLevel");
        stats.XpActual = SecurePlayerPrefs.GetFloat(prefsKey + "_actualXp");
        stats.XpAll = SecurePlayerPrefs.GetFloat(prefsKey + "_allXp");

        if (stats.actualLevel < 1)
        {
            stats.actualLevel = 1;
            SecurePlayerPrefs.SetInt(prefsKey + "_actualLevel", stats.actualLevel);
        }
    }
    #endregion

}
