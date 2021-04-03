using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LoadManager : MonoBehaviour
{
    public GameObject loadScreeen;
    public Slider slider;


    public Text text;


   public void LoadNextLevel()
    {
        StartCoroutine((string)Loadlevel());
    }

    IEnumerable Loadlevel()
    {
        loadScreeen.SetActive(true);

        AsyncOperation operation = SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex + 1);

        operation.allowSceneActivation = false;

        while (!operation.isDone)
        {
            slider.value = operation.progress;

            text.text = operation.progress * 100 + "%";

            if (operation.progress >= 0.9)
            {
                slider.value = 1;


                text.text = "P A T C";

                if (Input.anyKeyDown)
                {
                    operation.allowSceneActivation = true;
                }
            }
            yield return null;
        }
    }
}
