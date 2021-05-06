using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ClickToPlayPage : MonoBehaviour
{
    // Start is called before the first frame update
    public Button clickToPlay;
    [SerializeField] private NameInputPage nameInputPage;
    void Start()
    {
        clickToPlay.onClick.AddListener(ClickToPlay);
    }

    void ClickToPlay(){
        gameObject.SetActive(false);
        nameInputPage.Open();

    }

    // Update is called once per frame

}
