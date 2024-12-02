using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;  // 用于加载场景
using UnityEngine.UI;  // 用于操作 UI 元素

public class GameEndManager : MonoBehaviour
{
    // 引用 UI 元素
    public GameObject endWindow;
    public GameObject successPanel;  // 成功的提示框
    public GameObject failurePanel;  // 失败的提示框
    public GameObject messageTextObject;
    private TextMeshProUGUI messageText;

    private void Start()
    {
        messageText = messageTextObject.GetComponent<TextMeshProUGUI>();
        successPanel.SetActive(false);
        failurePanel.SetActive(false);
        endWindow.SetActive(false);
    }

    // 调用该方法来结束关卡并显示提示框
    public void EndGame(bool isSuccess)
    {
        endWindow.SetActive(true);
        if (isSuccess)
        {
            // 显示成功提示框
            successPanel.SetActive(true);
            messageText.text = "Congratulations! \n You Win!";
        }
        else
        {
            // 显示失败提示框
            failurePanel.SetActive(true);
            messageText.text = "Game Over! \n You Lose!";
        }
    }

    // 重新开始游戏
    public void RestartGame()
    {
        // 加载当前场景，重新开始
        SceneManager.LoadScene("MainMenu");
    }

    // 退出游戏
    public void QuitGame()
    {
        Application.Quit();
    }
}
