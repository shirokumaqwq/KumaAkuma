using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    public void OnStartButtonClick()
    {
        Debug.Log("Start!");
        SceneManager.LoadScene("TestLevel");
    }

    public void OnEndButtonClick()
    {
        Debug.Log("End");
        Application.Quit();
    }
}
