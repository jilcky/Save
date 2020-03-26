using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class CardStyle : MonoBehaviour
{
    //Card Style. Not relevant for ingame behavior but für import-Export purposes.
    //Importing cards of a specific style allows depending imagages/color.

    //public string styleName = "default";
    public KingsCardStyle style;
    private KingsCardStyle _oldStyle;

    public void SetStyle(KingsCardStyle newStyle) {
        style = newStyle;
    }

    //public void SetStyleName(string newName) {
    // styleName = newName;
    //}
    public string GetStyleName() {
        if (style != null) {
            return style.name;
        }
        else
        {
            return "default";
        }

    }

    //Refresh style dependant images, colors etc.
    public void Refresh() {
        if (style != null)
        {
            actualizeColoring();
            actualizeImages();
        }
    }

    //specific overwrites

    private void Awake()
    {
        Refresh();
    }

    private void Update()
    {
        if (style != _oldStyle ) {
            Refresh();
        }
    }

    //Coloring for images
    [System.Serializable]
    public class C_CardImages
    {
        [HideInInspector]public Color color = Color.white;
        public Image front;
        public Image back;
    }

    public C_CardImages cardImages;
    public Image iconImage;

    void actualizeColoring()
    {
        cardImages.color = style.cardColor;
        actualizeImageColor(cardImages.front);
        actualizeImageColor(cardImages.back);
    }

    void actualizeImageColor(Image img)
    {
        if (img != null)
        {
            img.color = cardImages.color;
        }
    }

    void actualizeImages() {
        if (iconImage != null && style.icon != null) {
            iconImage.sprite = style.icon;
        }
        if (cardImages.front != null && style.cardFront != null) {

            cardImages.front.sprite = style.cardFront;
        }
        if(cardImages.back != null && style.cardBack != null)
        {
            cardImages.back.sprite = style.cardBack;
        }
    }
}
