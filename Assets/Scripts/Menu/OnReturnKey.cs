using UnityEngine;
using UnityEngine.UI;

public class OnReturnKey : MonoBehaviour
{
    private Button button;

    void Start(){
        button = GetComponent<Button>();
    }
    void Update()
    {
        //Detect when the Return key is pressed down
        if (Input.GetKeyDown(KeyCode.Return))
        {
            button.onClick.Invoke();
        }

        // //Detect when the Return key has been released
        // if (Input.GetKeyUp(KeyCode.Return))
        // {
        //     Debug.Log("Return key was released.");
        // }
    }
}