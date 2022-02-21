using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board
{
    public List<Task> Tasks;
    public string Name;

    public Board(string name)
    {
        this.Name = name;
        this.Tasks = new List<Task>();
    }

    public void AddTask(Task task)
    {
        Tasks.Add(task);
    }

    public void RemoveTask(Task task)
    {
        Tasks.Remove(task);
    }

    public List<Task> GetTasks()
    {
        return Tasks;
    }
}
