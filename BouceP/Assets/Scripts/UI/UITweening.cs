using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class UITweening : MonoBehaviour
{
    [SerializeField] private RectTransform title;
    [SerializeField] private Vector3 titleTargetScale = new Vector3(2f, 2f, 1f); 
    [SerializeField] private float titleScaleDuration;
    [SerializeField] private LeanTweenType titleTweenType;
    
    [SerializeField] private RectTransform buttons;
    [SerializeField] private Vector3 buttonsTargetScale = new Vector3(2f, 2f, 1f);
    [SerializeField] private float buttonsScaleDuration;
    [SerializeField] private LeanTweenType buttonsTweenType;

    private void Start()
    {
       
        
    }

    private void Awake()
    {
        // Scale title
        if (title != null)
            LeanTween.scale(title, titleTargetScale, titleScaleDuration).setEase(titleTweenType);

        if (buttons != null)
            LeanTween.scale(buttons, buttonsTargetScale, buttonsScaleDuration).setEase(buttonsTweenType);
    }

    public void PopupTweenScale(GameObject gameobject)
    {

        LeanTween.scale(gameobject, new Vector3(0, 0, 0), 0).setOnComplete(
           delegate () 
           { 
               LeanTween.scale(gameobject, new Vector3(1, 1, 1), 0.2f).setEase(LeanTweenType.linear); 
           });
    }

    public void ClosePopupTweenScale(GameObject gameobject)
    {
        
        LeanTween.scale(gameobject, new Vector3(1, 1, 1), 0).setOnComplete(
           delegate () { LeanTween.scale(gameobject, new Vector3(0, 0, 0), 0.2f).setEase(LeanTweenType.linear).setOnComplete(
           delegate () { GameObject.FindGameObjectWithTag("Popup").SetActive(false); });
           });
    }

    public void MovePopupDown()
    {
        GameObject go = GameObject.FindGameObjectWithTag("Popup");
        go.transform.LeanSetPosY(5000);
        LeanTween.moveLocalY(go, 0, 0.2f).setEase(LeanTweenType.linear);
    }
}
