using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SSQPanelController : MonoBehaviour
{
    public GameObject SSQEndingPanel;
    public TextMeshProUGUI EndingText;
    public TextMeshProUGUI EndingTextTitle;
    public GameObject Nexbutton;
    private int _SSQLog;
    public GameObject SSQPanel;

    private void Start()
    {
        _SSQLog = CoreManager.Instance.GetSSQLog();
    }

    public void EnableSSQEnding()
    {
        SSQPanel.SetActive(false);
        if (_SSQLog == 1)
        {
            EndingTextTitle.text = "Thank you for participating!";
            EndingText.text = "You have completed all required VR tasks and questionnaires. You will now be assisted to conclude the session.";
            Nexbutton.SetActive(true);
        }
        SSQEndingPanel.SetActive(true);
    }

    public void loadLobby()
    {
        SceneManager.LoadScene("LobbyScene");
    }
}
