using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

namespace Tests.Editor {
    public class TaskTest {
        // [Test]
        // public void StealTaskToJson() {
        //     StealingTask task = new StealingTask("Hello");
        //     string jsonString = JsonUtility.ToJson(task);
        //     string exptectedString = "{\"isCompleted\":false,\"description\":\"Hello\",\"type\":\"\",\"objectToSteal\":{\"instanceID\":0}}";
        //     Assert.AreEqual(jsonString, exptectedString);
        // }
        // 
        // [Test]
        // public void StealTaskFromJson() {
        //     StealingTask exptectedTask = new StealingTask("Hello");
        //     string jsonString = "{\"isCompleted\":false,\"description\":\"Hello\",\"type\":\"\",\"objectToSteal\":{\"instanceID\":0}}";
        //     StealingTask task = JsonUtility.FromJson<StealingTask>(jsonString);
        //     Assert.AreEqual(task.description, exptectedTask.description);
        // }
        // 
        // [Test]
        // public void TasksToJson() {
        //     List<Task> tasksList = new List<Task>();
        //     tasksList.Add(new StealingTask("task0"));
        //     tasksList.Add(new StealingTask("task1"));

        //     Tasks tasks = new Tasks(tasksList);

        //     List<string> jsonList = tasks.ToJson();
        //     List<string> exptectedString = new List<string>{
        //       @"{""isCompleted"":false,""description"":""task0"",""type"":""StealingTask"",""objectToSteal"":{""instanceID"":0}}",
        //       @"{""isCompleted"":false,""description"":""task1"",""type"":""StealingTask"",""objectToSteal"":{""instanceID"":0}}",
        //     };
        //     
        //     Assert.AreEqual(jsonList, exptectedString);
        // }
        // 
        // [Test]
        // public void TasksFromJson() {
        // }
    }
}
