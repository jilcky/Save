using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class conditionalActionList : TranslatableContent
{
    [System.Serializable] public class mEvent : UnityEvent { }
    [Tooltip("The changes are only executed, if all pre conditions are true.")]
    public ConditionsAndEffects.C_Conditions preConditions;
    [Tooltip("Please define the time when the tests and invokes are computed:\n" +
        "onCardDestroy: After a card is destroyed. At this point all changes from the card are done.\n" +
        "cyclic: The test is done all the time. Should only be used with the execution behavior 'onStateChange', else the changes and events are invoked each frame and can lead to some unwanted behavior.\n" +
        "manual: The test is only done from an external source. Please call the method 'ExecuteCheck()' from an event or another script to do so.")]
    public E_InvokeTimingTypes testTiming;
    [Tooltip("Please define the execution repetition logic:\n" +
        "always: Every time a check is done and the conditions are met the changes and the events are invoked.\n" +
        "onStateChange: Execute if an element state changes (condition result transition false -> true).")]
    public E_ExectionBehaviorTypes executionBehavior; 

    [System.Serializable]
    public enum E_InvokeTimingTypes
    {
        onCardDestroy,
        cyclic,
        manual
    }

    [System.Serializable]
    public enum E_ExectionBehaviorTypes
    {
        always,                 /*Execute everytime the List is tested, except the preconditions are not met.*/
        onStateChange         /*Execute if an element state changes (condition result transition false -> true). I prefere the term from elctronics: on rising edge.*/
    }

    [System.Serializable]
    public class C_ValueContentToAction
    {
        [Tooltip("This element is only executed, if the conditions for it are met.")]
        public ConditionsAndEffects.C_Conditions elementCondition;
        public ConditionsAndEffects.C_Changes changes;
        public mEvent _event;

        [HideInInspector]public bool evaluationResult = false;
        [HideInInspector]public bool lastEvaluationResult = false;
    }

    public List<C_ValueContentToAction> actionList;
    private CardStack cardStack;

    private void Awake()
    {

    }
    private void Start()
    {
        TranslationManager.instance.registerTranslateableContentScript(this);

        if (CardStack.instance != null)
        {
            CardStack.instance.OnCardDestroy += OnCardDestroy;  //add callback
        }
        StartCoroutine(eachFrame());
    }
    private void OnDestroy()
    {
        if (CardStack.instance != null)
        {
            CardStack.instance.OnCardDestroy -= OnCardDestroy; //remove callback
        }
    }

    //Callback if a card was destroyed. At this time all the value-, item- etc. changes are done, the new card is not spawned yet.
    void OnCardDestroy()
    {
        if(testTiming == E_InvokeTimingTypes.onCardDestroy)
        {
            ExecuteCheck();
        }
    }

    //Cyclic method for testing the conditions. 
    IEnumerator eachFrame()
    {
        yield return null;
        initialize();           //have to be called once for testing the initial state of the elements.
        while (true)
        {
            if (testTiming == E_InvokeTimingTypes.cyclic)
            {
                ExecuteCheck();
            }
            yield return null;
        }
    }

    private void initialize()
    {
        //Test each element of the list
        for (int i = 0; i < actionList.Count; i++)
        {
            //Test for each element condition
            actionList[i].evaluationResult = actionList[i].elementCondition.AreConditionsMet();
            actionList[i].lastEvaluationResult = actionList[i].evaluationResult; //memorize last result for each element
        }
    }

    public void ExecuteCheck()
    {
        executionLogic();
    }

    void executionLogic()
    {

        //test for preconditions. Abort if not met.
        if(preConditions.AreConditionsMet() == false)
        {
            return;
        }

        bool execute = false; //temporary memory, should the result be executed?

        //Test each element of the list
        for (int i = 0; i < actionList.Count; i++)
        {
            //Test for each element condition
            actionList[i].evaluationResult = actionList[i].elementCondition.AreConditionsMet();

            switch (executionBehavior)
            {
                case E_ExectionBehaviorTypes.always:
                    execute = actionList[i].evaluationResult;
                    break;
                case E_ExectionBehaviorTypes.onStateChange:
                    if(actionList[i].evaluationResult == true && actionList[i].lastEvaluationResult == false)
                    {
                        execute = true;
                    }
                    else
                    {
                        execute = false;
                    }
                    break;
            }

            if (execute == true)
            {
                actionList[i].changes.ExecuteEffect();
                actionList[i]._event.Invoke();
            }

            actionList[i].lastEvaluationResult = actionList[i].evaluationResult; //memorize last result for each element
        }
    }

    //Return all possible translatable terms
    public override List<string> getTranslatableTerms()
    {
        List<string> terms = new List<string>();
        for (int i = 0; i < actionList.Count; i++)
        {
            terms.AddRange(actionList[i].changes.GetTranslatableContent());
        }
        return terms;
    }
}
