using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ClickToPlayPage : MenuPage
{
    // Start is called before the first frame update
    public Button clickToPlay;
    public Camera camera;
    [SerializeField] private NameInputPage nameInputPage;
    void Start()
    {
        clickToPlay.onClick.AddListener(ClickToPlay);
    }

    void ClickToPlay(){
        gameObject.SetActive(false);
        nameInputPage.Open();
        camera.GetComponent<Animator>().enabled = true;

    }

    // Update is called once per frame

}
