using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SwipeController : MonoBehaviour, IEndDragHandler
{
    [SerializeField] int currentPage;
    [SerializeField] int maxPage;
    Vector3 targetPosition;

    [SerializeField] Vector3 pageStep;
    [SerializeField] RectTransform levelPagesRect;

    [SerializeField] float tweenTime;
    [SerializeField] LeanTweenType tweenType;
    public float dragThreshold;

    [SerializeField] Image[] barImage;
    [SerializeField] Sprite barClosed, barOpen;
    [SerializeField] Button previousBtn, nextBtn;

    private void Awake()
    {
        currentPage = 1;
        targetPosition = levelPagesRect.localPosition;
        dragThreshold = Screen.width / 15;

        UpdateBar();
        UpdateArrowButton();
    }

    public void Next()
    {
        if (currentPage < maxPage)
        {
            currentPage++;
            targetPosition += pageStep;
            MovePage();
        }
    }

    public void Previous() 
    {
        if (currentPage > 1)
        {
            currentPage--;
            targetPosition -= pageStep;
            MovePage();
        }
    }

    void MovePage()
    {
        levelPagesRect.LeanMoveLocal(targetPosition, tweenTime).setEase(tweenType);
        UpdateBar();
        UpdateArrowButton();
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (Mathf.Abs(eventData.position.x - eventData.pressPosition.x) > dragThreshold)
        {
            if (eventData.position.x > eventData.pressPosition.x)
                Previous();
            else
                Next();
        }
        //else
        //    MovePage();
    }

    void UpdateBar()
    {
        foreach (var item in barImage)
        {
            item.sprite = barClosed;
        }
        barImage[currentPage - 1].sprite = barOpen;
    }

    void UpdateArrowButton()
    {
        nextBtn.interactable = true;
        previousBtn.interactable = true;

        if (currentPage == 1) 
            previousBtn.interactable = false;
        
        else if (currentPage == maxPage) 
            nextBtn.interactable = false;
    }
}
