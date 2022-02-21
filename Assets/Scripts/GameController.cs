using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    public Board MainBoard;
    public TextMeshProUGUI GameTitle;

    public Image Background;
    public Transform UIBoard;
    public Transform UITask;

    public int score = 0; 
    public TextMeshProUGUI ScoreText;
    public TextMeshProUGUI SecondsText;
    public TextMeshProUGUI TotalTaskText;

    public TextMeshProUGUI ScoreGameOver;
    public TextMeshProUGUI BestScoreText;

    public int TaskDeadlineMinRange = 15;
    public int TaskDeadlineMaxRange = 30;

    float timer;
    int seconds;

    public int NextRoundMinRange = 5;
    public int NextRoundMaxRange = 8;
    public int NextTasksRoundTime = 5;

    public int TaskQuantityMinRange = 2;
    public int TaskQuantityMaxRange = 5;

    public int OverloadedAmount = 5;
    bool isOverloaded = false;

    public Image PlayGameOverUI;
    public Button PlayButton;
    public TextMeshProUGUI PlayGameOverText;
    public TextMeshProUGUI PlayButtonText;

    GameObject[] allWorkers;
    List<WorkerController> allWorkersControllers = new List<WorkerController>();

    public bool IsInChaos = false;
    public int ChaosLastMax = 15;
    int chaosStartedSecond = 0;
    public int ChaosTime = 15;
    public int ChaosModeMinRange = 10;
    public int ChaosModeMaxRange = 30;
    public Color ChaosColor;
    public Color NormalColor;

    AudioSource audioMain;
    public AudioClip NewTasksArrive;
    public AudioClip Siren;

    void Start()
    {
        SetBoard(new Board("Main Board"));

        Time.timeScale = 0;

        Button btn = PlayButton.GetComponent<Button>();
        btn.onClick.AddListener(PlayGame);

        allWorkers = GameObject.FindGameObjectsWithTag("Worker"); 
        
        foreach (GameObject worker in allWorkers)
        {
            allWorkersControllers.Add(worker.GetComponent<WorkerController>());
        }

        audioMain = gameObject.GetComponent<AudioSource>();
        BestScoreText.text = "your best is: " + PlayerPrefs.GetInt("score");
    }

    private void Update()
    {
        int notOverloadedWorkerIndex = allWorkersControllers.FindIndex(workerController => workerController.isOverloaded == false);

        if (notOverloadedWorkerIndex == -1)
        {
            GameOver("you overloaded everyone");
        }

        if (isOverloaded)
        {
            GameOver("you are overloaded");
        };

        int tasksCount = MainBoard.GetTasks().Count;

        timer += Time.deltaTime;
        seconds = (int)Math.Truncate(timer);

        ScoreText.text = "task score: " + score;
        SecondsText.text = "seconds working today: " + seconds;
        TotalTaskText.text = "total todo tasks: " + tasksCount;

        if (tasksCount > OverloadedAmount)
        {
            isOverloaded = true;
            return;
        }

        if (seconds >= NextTasksRoundTime)
        {
            NextTasksRoundTime = seconds + UnityEngine.Random.Range(NextRoundMinRange, NextRoundMaxRange);
            NewTasks(UnityEngine.Random.Range(TaskQuantityMinRange, TaskQuantityMaxRange));
        }

        if (seconds >= ChaosTime && !IsInChaos)
        {
            ChaosTime = seconds + UnityEngine.Random.Range(ChaosModeMinRange, ChaosModeMaxRange);
            chaosStartedSecond = seconds;
            ChaosMode(true);
        }

        if (IsInChaos && seconds > chaosStartedSecond + ChaosLastMax)
        {
            ChaosMode(false);
        }
    }

    void PlayGame()
    {
        PlayGameOverUI.gameObject.SetActive(false);
        Time.timeScale = 1;
        NewTasks(3);
    }

    void RestartGame()
    {
        SceneManager.LoadScene("SampleScene");
    }

    void ChaosMode(bool active)
    {
        if (active)
        {
            IsInChaos = true;
            Time.timeScale = 2;
            Background.color = ChaosColor;
            audioMain.PlayOneShot(Siren);
            GameTitle.text = "chaos is a problem, i think";
        } else
        {
            IsInChaos = false;
            Time.timeScale = 1;
            Background.color = NormalColor;
            GameTitle.text = "work overload is a problem, probably";
        }
    }

    public void GameOver(string message)
    {
        int scoreSaved = PlayerPrefs.GetInt("score");

        if (this.score > scoreSaved)
        {
            PlayerPrefs.SetInt("score", this.score);
            PlayerPrefs.Save();
        }

        var bestScore = this.score > scoreSaved ? this.score : scoreSaved;

        ScoreGameOver.text = "you did " + this.score + " tasks";
        BestScoreText.text = "your best is: " + bestScore;

        bool val = PlayerPrefs.GetInt("PropName") == 1 ? true : false;

        PlayGameOverText.text = message;
        PlayButtonText.text = "try again";

        Button btn = PlayButton.GetComponent<Button>();
        btn.onClick.RemoveAllListeners();
        btn.onClick.AddListener(RestartGame);

        PlayGameOverUI.gameObject.SetActive(true);
        Time.timeScale = 0;
    }

    void NewTasks(int taskQuantity)
    {
        for (int i = 0; i < taskQuantity; i++)
        {
            int taskDeadline = UnityEngine.Random.Range(TaskDeadlineMinRange, TaskDeadlineMaxRange);
            Task.Type taskType = GetTaskTypeByIndex(UnityEngine.Random.Range(0, 3));

            MainBoard.AddTask(new Task { TaskType = taskType, DeadlineTime = taskDeadline });
        }

        RefreshBoardTasks();

        audioMain.PlayOneShot(NewTasksArrive);
    }

    public void SetBoard(Board board)
    {
        this.MainBoard = board;
    }

    public void RefreshBoardTasks()
    {
        foreach (Transform child in UIBoard)
        {
            if (child == UITask) continue;

            Destroy(child.gameObject);
        }

        int y = 1;
        float taskSize = 80f;

        foreach (Task task in MainBoard.GetTasks())
        {
            RectTransform UITaskRectTransform = Instantiate(UITask, UIBoard).GetComponent<RectTransform>();
            TaskController taskController = UITaskRectTransform.gameObject.GetComponent<TaskController>();

            UITaskRectTransform.gameObject.SetActive(true);
            UITaskRectTransform.anchoredPosition = new Vector2(-11f, - y * (taskSize + 5f) + 180f);
            y++;

            Image image = UITaskRectTransform.Find("ColorImage").GetComponent<Image>();
            Image icon = UITaskRectTransform.Find("Icon").GetComponent<Image>();
            image.color = task.GetColor();
            icon.sprite = task.GetSprite();
            taskController.task = task;
        }
    }

    Task.Type GetTaskTypeByIndex(int taskTypeIndex)
    {
        switch (taskTypeIndex)
        {
            default:
            case 0: return Task.Type.Bug;
            case 1: return Task.Type.Feat;
            case 2: return Task.Type.Improvement;
        }
    }
}
