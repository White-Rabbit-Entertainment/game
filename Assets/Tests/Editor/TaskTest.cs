using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

namespace Tests.Editor {
    public class TaskTest {
        [Test]
        public void StealTaskToJson() {
            GameObject go = new GameObject();
            go.AddComponent<StealableItem>();

            StealingTask task = new StealingTask("Hello", go.transform);
            string jsonString = JsonUtility.ToJson(task);
            string exptectedString = "{\"isCompleted\":false,\"description\":\"Hello\",\"type\":\"\",\"objectToSteal\":{\"instanceID\":0}}";
            Assert.AreEqual(jsonString, exptectedString);
        }
        
        [Test]
        public void StealTaskFromJson() {
            GameObject go = new GameObject();
            go.AddComponent<StealableItem>();

            StealingTask exptectedTask = new StealingTask("Hello", go.transform);
            string jsonString = "{\"isCompleted\":false,\"description\":\"Hello\",\"type\":\"\",\"objectToSteal\":{\"instanceID\":0}}";
            StealingTask task = JsonUtility.FromJson<StealingTask>(jsonString);
            Assert.AreEqual(task.description, exptectedTask.description);
        }
        
        [Test]
        public void TasksToJson() {
            GameObject go = new GameObject();
            go.AddComponent<StealableItem>();

            List<Task> tasksList = new List<Task>();
            tasksList.Add(new StealingTask("task0", go.transform));
            tasksList.Add(new StealingTask("task1", go.transform));

            Tasks tasks = new Tasks(tasksList);

            List<string> jsonList = tasks.ToJson();
            List<string> exptectedString = new List<string>{
              @"{""task"": ""{""isCompleted"":false,""description"":""task0"",""type"":""StealingTask"",""objectToSteal"":{""instanceID"":{go.GetInstanceID()}}}"", ""type"": ""StealingTask""}",
              @"{""task"": ""{""isCompleted"":false,""description"":""task1"",""type"":""StealingTask"",""objectToSteal"":{""instanceID"":0}}"", ""type"": ""StealingTask""}",
            };
            
            Debug.Log(jsonList);
            Debug.Log(exptectedString);
            Assert.AreEqual(jsonList, exptectedString);
        }
        
        [Test]
        public void TasksFromJson() {
        }
    }
}
