using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class PersistentImages : MonoBehaviour {
    [ReadOnlyInspector] public string saveKey = "PersistentSpriteIndex";
    [ReadOnlyInspector] public int spriteIndex = 0;
    //public Sprite[] sprites;

    [System.Serializable]
    public class C_ImageConfig
    {
        public Sprite sprite;
        public Color color = Color.white;
    }

    public C_ImageConfig[] imageSets;

    private Image mImage;

    private void Awake()
    {
        mImage = gameObject.GetComponent<Image>();
    }

    void Start () {
        load();
        ActualizeImage();
	}

    public bool testNextImage = false;

    private void Update()
    {
        if(testNextImage == true)
        {
            spriteIndex++;
            if(spriteIndex >= imageSets.Length)
            {
                spriteIndex = 0;
            }
            ActualizeImage();
            testNextImage = false;
        }
    }

    public void SetSpriteIndex(int index)
    {
        if (index >= imageSets.Length || index < 0) {
            Debug.LogWarning("Kings warning (PersistentImages): There is no element "+index.ToString()+" in the sprite list at '" + gameObject.name+"'. Possible values range from 0 to "+(imageSets.Length-1).ToString()+". Index is limited to allowed values.");
        }

        spriteIndex = index;
        limitActSpriteIndex();
        ActualizeImage();
        save();
    }

    void ActualizeImage() {
        if(imageSets.Length > 0)
        {
                mImage.overrideSprite = imageSets[spriteIndex].sprite;
                mImage.color = imageSets[spriteIndex].color;
        }
        else
        {
            Debug.LogWarning("Kings warning (PersistentImages): The list of sprites at '" + gameObject.name + "' has no elements to select from.");
        }

    }

    void limitActSpriteIndex()
    {
        if (spriteIndex >= imageSets.Length)
        {
            spriteIndex = imageSets.Length - 1;
        }

        if(spriteIndex < 0)
        {
            spriteIndex = 0;
        }
    }

    private void OnDestroy()
    {
        save();
    }

    void save()
    {
        SecurePlayerPrefs.SetInt(saveKey, spriteIndex);
    }

    void load()
    {
        spriteIndex = SecurePlayerPrefs.GetInt(saveKey);
        SetSpriteIndex(spriteIndex);
    }
}
