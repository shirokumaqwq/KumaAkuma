using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;  // ���ڼ��س���
using UnityEngine.UI;  // ���ڲ��� UI Ԫ��

public class GameEndManager : MonoBehaviour
{
    // ���� UI Ԫ��
    public GameObject endWindow;
    public GameObject successPanel;  // �ɹ�����ʾ��
    public GameObject failurePanel;  // ʧ�ܵ���ʾ��
    public GameObject messageTextObject;
    private TextMeshProUGUI messageText;

    private void Start()
    {
        messageText = messageTextObject.GetComponent<TextMeshProUGUI>();
        successPanel.SetActive(false);
        failurePanel.SetActive(false);
        endWindow.SetActive(false);
    }

    // ���ø÷����������ؿ�����ʾ��ʾ��
    public void EndGame(bool isSuccess)
    {
        endWindow.SetActive(true);
        if (isSuccess)
        {
            // ��ʾ�ɹ���ʾ��
            successPanel.SetActive(true);
            messageText.text = "Congratulations! \n You Win!";
        }
        else
        {
            // ��ʾʧ����ʾ��
            failurePanel.SetActive(true);
            messageText.text = "Game Over! \n You Lose!";
        }
    }

    // ���¿�ʼ��Ϸ
    public void RestartGame()
    {
        // ���ص�ǰ���������¿�ʼ
        SceneManager.LoadScene("MainMenu");
    }

    // �˳���Ϸ
    public void QuitGame()
    {
        Application.Quit();
    }
}
