using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TaskController : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    public GameController GameController;
    public Canvas canvas;

    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;

    [HideInInspector]
    public Task task;

    Vector2 originalPosition;

    void Awake()
    {
        rectTransform = gameObject.GetComponent<RectTransform>();
        canvasGroup = gameObject.GetComponent<CanvasGroup>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        originalPosition = rectTransform.anchoredPosition;
        canvasGroup.blocksRaycasts = false;
        canvasGroup.alpha = 0.5f;
    }

    public void OnDrag(PointerEventData eventData)
    {
        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.blocksRaycasts = true;
        canvasGroup.alpha = 1f;
        rectTransform.anchoredPosition = originalPosition;
    }

    public void Designated() {
        Destroy(gameObject);
        GameController.MainBoard.RemoveTask(task);
        GameController.RefreshBoardTasks();
    }
}
