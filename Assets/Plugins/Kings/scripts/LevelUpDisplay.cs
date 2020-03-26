using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelUpDisplay : MonoBehaviour {


    public Slider xpBar;
    public Text delayedUILevelNumber;


    private void Update()
    {
        if (KingsLevelUp.instance != null ) {
            if (xpBar != null)
            {
                xpBar.value = KingsLevelUp.instance.getXpBarFilling();
            }
            if (delayedUILevelNumber != null)
            {
                delayedUILevelNumber.text = KingsLevelUp.instance.getUiIncreasingLevelNumber().ToString();
            }
        }
    }
}
