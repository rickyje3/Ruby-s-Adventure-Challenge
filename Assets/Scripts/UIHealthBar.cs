using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIHealthBar : MonoBehaviour
{
    //Allows you to write UIhealthbar.instance in any script and it will call that get property
    //Set is private because we dont want that changed outside the script
    public static UIHealthBar instance { get; private set; }
    public UnityEngine.UI.Image mask;
    float originalSize;


    void Awake()
    {
        //this = the object that currently runs that function
        //calling uihealthbar.instance in another script will return the value to that script
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        originalSize = mask.rectTransform.rect.width;
    }

    // Update is called once per frame
    void Update()
    {

    }

    //Calls setvalue when the health changes to a value between 0 and 1 and this changes the size of the mask
    public void SetValue(float value)
    {
        mask.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, originalSize * value);
    }

}
