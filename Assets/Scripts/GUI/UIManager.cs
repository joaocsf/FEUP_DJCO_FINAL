using UnityEngine;
using UnityEngine.UI;
using System.Collections;
public class UIManager : MonoBehaviour
{
    public GameObject mainPanel;
    public GameObject gamePanel;

    public Button btnExit;
    public Button btnContinue;
    public Button btnNewGame;
    public Button btnSettings;
    public Button btnMenu;
    public Button btnBack;
    public Button btnLeft;
    public Button btnRight;
    private bool active = true;

    void Start()
    {
        Button bExit = btnExit.GetComponent<Button>();
        Button bNewGame = btnNewGame.GetComponent<Button>();
        Button bSettings = btnSettings.GetComponent<Button>();
        Button bContinue = btnContinue.GetComponent<Button>();
        Button bMenu = btnMenu.GetComponent<Button>();
        Button bLeft = btnLeft.GetComponent<Button>();
        Button bRight = btnRight.GetComponent<Button>();
        // Button bBack = btnBack.GetComponent<Button>();

        bExit.onClick.AddListener(ClickExit);
        bContinue.onClick.AddListener(ClickContinue);
        bNewGame.onClick.AddListener(ClickNewgame);
        bSettings.onClick.AddListener(ClickSettings);
        bMenu.onClick.AddListener(ToggleMenu);
    }
    public void ClickContinue()
    {
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
    public void ToggleMenu()
    {
        gamePanel.SetActive(active);
        active = !active;
        mainPanel.SetActive(active);
    }
}