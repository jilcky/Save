using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

//Math on kings values. Not too complex, but should fulfill the most needs.
public class ValueMath : MonoBehaviour {

    #region definitions

    [System.Serializable] public class mEvent : UnityEvent { }

    public valueDefinitions.values result;

    [System.Serializable]
    public enum T_MathValueType
    {
        Value,
        Const
    }

    [System.Serializable]
    public enum T_ValueMathOperations {
        none,
        add,
        substract,
        multiply,
        divideBy,
        power
    }

    [System.Serializable]
    public class C_MathOperation {
        public T_MathValueType valueType;
        public T_ValueMathOperations mathOperator;
        public float valueFactor = 1f;
        public valueDefinitions.values valueName;
        public float constant = 1f;
    }

    public List<C_MathOperation> operations;

    [System.Serializable]
    public class C_Mathf_rArgs
    {
        public int index;
        public valueDefinitions.values value;
    }

    [Tooltip("If cascaded calcualtions are needed you can trigger another valueMath.Calculation() here.")]
    public mEvent AfterCalculation;

    #endregion

    #region methods

    float getOperationValue(C_MathOperation op) {
        float result = 0f;

        switch (op.valueType) {
            case T_MathValueType.Const:
                result = op.constant;
                break;
            case T_MathValueType.Value:
                result = valueManager.instance.getFirstFittingValue(op.valueName).value * op.valueFactor;
                break;
            default:
                break;
        }

        return result;
    }

    public void Calculate()
    {

        float mem = 0f;
        float nextVal = 0f;
        if (operations.Count > 0)
        {
            if (operations[0].mathOperator != T_ValueMathOperations.none)
            {
                Debug.LogWarning("The operation of '" + operations[0].mathOperator.ToString() + "' has no effect on the first element (ValueMath.cs)(" + gameObject.name + ").");
            }
            mem = getOperationValue(operations[0]);

            for (int i = 1; i < operations.Count; i++)
            {
                nextVal = getOperationValue(operations[i]);

                switch (operations[i].mathOperator)
                {
                    case T_ValueMathOperations.none:
                        Debug.LogError("The operator 'none' is illegal for index " + i.ToString() + " (ValueMath.cs)(" + gameObject.name + ").");
                        break;
                    case T_ValueMathOperations.add:
                        mem += nextVal;
                        break;
                    case T_ValueMathOperations.substract:
                        mem -= nextVal;
                        break;
                    case T_ValueMathOperations.multiply:
                        mem *= nextVal;
                        break;
                    case T_ValueMathOperations.divideBy:
                        mem /= nextVal;
                        break;
                    case T_ValueMathOperations.power:
                        mem = Mathf.Pow(mem, nextVal);
                        break;
                    default:
                        //?
                        break;
                }
            }

            valueManager.instance.setValue(result, mem);

            AfterCalculation.Invoke();
        }
        else {
            Debug.LogWarning("No operations defined (ValueMath.cs)(" + gameObject.name + ").");
        }

    }

    #endregion

}


