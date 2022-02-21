using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class WorkerController : MonoBehaviour, IDropHandler, IPointerEnterHandler, IPointerExitHandler
{
    public Board WorkerBoard;

    public GameObject GameObjectController;
    GameController gameController;

    public int OverloadedAmount = 5;
    bool isIdle = true;
    public bool isOverloaded = false;
    Task.Type specialist;
    bool isFirstUpdate = true;

    float timer = 0;
    int seconds = 0;

    bool isMouseOver = false;

    public TextMeshProUGUI WorkingInText;
    public TextMeshProUGUI WorkingType;
    public TextMeshProUGUI Status;
    public TextMeshProUGUI CountTask;
    public TextMeshProUGUI WorkerName;
    public Image Icon;
    public Sprite FirePcSprite;

    Image WorkerPc;

    AudioSource audioMain;
    public AudioClip TaskReadySound;

    void Start()
    {
        SetBoard(new Board("Worker Board"));
        specialist = GetRandomSpecialization();
        gameController = GameObjectController.GetComponent<GameController>();
        WorkerPc = gameObject.GetComponent<Image>();
        audioMain = GameObjectController.GetComponent<AudioSource>();
    }

    void Update()
    {
        if (isMouseOver)
        {
            WorkingInText.text = "working in: " + WorkerBoard.GetTasks().Count + " tasks";
            WorkingType.text = specialist.ToString();
            Status.text = "status: " + (isOverloaded ? "overloaded" : isIdle ? "idle" : "working");
            WorkerName.text = gameObject.name;
        }

        if (isOverloaded) {
            WorkerPc.sprite = FirePcSprite;
            return;
        }

        List<Task> tasks = WorkerBoard.GetTasks();
        int tasksCount = tasks.Count;
        CountTask.text = "" + tasksCount;

        if (tasksCount > OverloadedAmount)
        {
            isOverloaded = true;
        }

        if (tasksCount < 1)
        {
            isIdle = true;
        } else
        {
            isIdle = false;

            Task currentTask = tasks[0];

            if (currentTask.TaskType == specialist && isFirstUpdate)
            {
                timer = SpecialistBonus();
                isFirstUpdate = false;
            }

            timer += Time.deltaTime;
            seconds = (int) System.Math.Truncate(timer);

            currentTask.DurationTime = seconds;

            if (currentTask.DurationTime >= currentTask.DeadlineTime)
            {
                seconds = 0;
                timer = 0;
                isFirstUpdate = true;

                if (Random.value > 0.5f) specialist = GetRandomSpecialization();

                audioMain.PlayOneShot(TaskReadySound);
                gameController.score++;
                WorkerBoard.RemoveTask(currentTask);
            }
        }
    }

    void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData)
    {
        isMouseOver = true;
    }

    void IPointerExitHandler.OnPointerExit(PointerEventData eventData)
    {
        isMouseOver = false;
    }

    public void OnDrop(PointerEventData eventData)
    {
        if (eventData.pointerDrag == null) return;

        if (isOverloaded) return;

        TaskController taskController = eventData.pointerDrag.gameObject.GetComponent<TaskController>();

        WorkerBoard.AddTask(taskController.task);
        taskController.Designated();
    }

    void SetBoard(Board board)
    {
        this.WorkerBoard = board;
    }

    int SpecialistBonus() {
        return Random.Range(1, 11);
    }

    Task.Type GetRandomSpecialization()
    {
        int index = Random.Range(0, 3);

        switch(index)
        {
            default:
            case 0:
                {
                    Icon.sprite = TaskAssets.Instance.BugSprite;
                    return Task.Type.Bug;
                }
            case 1:
                {

                    Icon.sprite = TaskAssets.Instance.FeatSprite;
                    return Task.Type.Feat;
                }
            case 2:
                {
                    Icon.sprite = TaskAssets.Instance.ImprovementSprite;
                    return Task.Type.Improvement;
                }
        }
    }
}
