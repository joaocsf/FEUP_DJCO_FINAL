using UnityEngine;
using UnityEngine.UI;
using System.Collections;
public class UIManager : MonoBehaviour
{
    public GameObject mainPanel;
    public Button btnExit;
    public Button btnContinue;
    public Button btnNewGame;
    public Button btnSettings;
    private bool active = true;

    void Start()
    {
        Button bExit = btnExit.GetComponent<Button>();
        Button bNewGame = btnNewGame.GetComponent<Button>();
        Button bSettings = btnSettings.GetComponent<Button>();
        Button bContinue = btnContinue.GetComponent<Button>();

        bExit.onClick.AddListener(ClickExit);
        bContinue.onClick.AddListener(ClickContinue);
        bNewGame.onClick.AddListener(ClickNewgame);
        bSettings.onClick.AddListener(ClickSettings);
    }
    public void ClickContinue()
    {
        mainPanel.SetActive(false);
    }
    public void ClickNewgame()
    {
    }
    public void ClickExit()
    {
        Debug.Log("Application Closing");
        Application.Quit();
    }
    public void ClickSettings()
    {
    }

    public void ToggleMenu(){
        active = !active;
        mainPanel.SetActive(active);
    }
}