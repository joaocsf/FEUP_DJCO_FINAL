using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Search_Shell.Game;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public FadeController mainPanel;
    public FadeController gamePanel;

    public FadeController optionsPanel;
    public FadeController endingPanelMenu;
    private GameController gameControl;
    private CameraFollow cameraFollow;
    public AK.Wwise.State stateMenu;
    public AK.Wwise.State stateGame;

    public Text endingText;
    
    private bool active = true;

    public bool end = false;

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
        ClickOpenOptions(true);
    }
    void Update () {
        if (Input.GetKeyDown("escape"))
        {
            ToggleMenu();
        }
    }
    public void ToggleMenu()
    {
        DisableUI();
        Debug.Log(end);
        if(end)return;
        ActivatePanel(gamePanel,active);
        gameControl.SetCanControl(active);        
        active = !active;

        ActivatePanel(mainPanel,active);
        gameControl.PlayState(active? stateMenu: stateGame);
    }

    public void ActivatePanel(FadeController anim, bool state = true){
        anim.SetActive(state);
    }

    public void ClickOpenMenu(bool open){
        DisableUI();
        ActivatePanel(mainPanel, open);
    }

    public void ClickOpenOptions(bool options){
        DisableUI();
        ActivatePanel(optionsPanel, options);
    }
    public void DisableUI(){
        ActivatePanel(mainPanel,false);
        ActivatePanel(gamePanel,false);
        ActivatePanel(endingPanelMenu, false);
        ActivatePanel(optionsPanel, false);
    }

    public void ActivateEnding(bool setMenu, string endingText = ""){
        end=true;
        ActivatePanel(mainPanel,false);
        ActivatePanel(gamePanel,false);
        this.endingText.text = endingText;
        ActivatePanel(endingPanelMenu, setMenu);
    }
}