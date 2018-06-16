using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Search_Shell.Game;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public GameObject mainPanel;
    public GameObject gamePanel;
    private GameController gameControl;
    private CameraFollow cameraFollow;
    public AK.Wwise.State stateMenu;
    public AK.Wwise.State stateGame;
    
    private bool active = true;

    void Start()
    {
        gameControl=FindObjectOfType<GameController>();
        cameraFollow =FindObjectOfType<CameraFollow>();
    
    }
    public void clickRotate(int i) 
    {
        cameraFollow.CameraRotate(i);
    }
    public void clickUndo()
    {
        gameControl.Undo();
    }
    public void ClickContinue()
    {
        ToggleMenu();
    }
    public void ClickNewgame()
    {
        SceneManager.LoadScene(0);
    }
    public void ClickExit()
    {
        Debug.Log("Application Closing");
        Application.Quit();
    }
    public void ClickSettings()
    {
    }
    void Update () {
        if (Input.GetKeyDown("escape"))
        {
            ToggleMenu();
        }
    }
    public void ToggleMenu()
    {
        gamePanel.SetActive(active);
        gameControl.SetCanControl(active);        
        active = !active;

        mainPanel.SetActive(active);
        gameControl.PlayState(active? stateMenu: stateGame);
    }
}