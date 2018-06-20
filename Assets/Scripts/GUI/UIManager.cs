using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Search_Shell.Game;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class UIManager : MonoBehaviour
{
    public FadeController mainPanel;
    public FadeController gamePanel;

    public FadeController optionsPanel;
    public FadeController endingPanelMenu;

    public GameObject menuSelect;
    public GameObject optionsSelect;
    public GameObject endSelect;

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
        Destroy(GameObject.Find("WwiseGlobal"));
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

    Vector3 lastMouse;
    void EnableMouse(bool b){
        lastMouse = Input.mousePosition;
        Cursor.visible = b;
        cameraFollow.useMouse = b;
    }
    void Update () {
        if(lastMouse!=Input.mousePosition)
            EnableMouse(true);

        lastMouse = Input.mousePosition;
        if (Input.GetButtonDown("Escape"))
        {
            ToggleMenu();
        }

        if (Input.GetButtonDown("Select"))
            ClickNewgame();

        if(Input.GetAxis("Vertical1") != 0){
            ActivatePanel(gamePanel, false);
            EnableMouse(false);
        }
    }
    public void ToggleMenu()
    {
        if(end)return;
        DisableUI();
        Debug.Log(end);
        ActivatePanel(gamePanel,active);
        gameControl.SetCanControl(active);        
        active = !active;

        if(active){
            EventSystem.current.SetSelectedGameObject(menuSelect);
        }
        ActivatePanel(mainPanel,active);
        gameControl.PlayState(active? stateMenu: stateGame);
    }

    public void ActivatePanel(FadeController anim, bool state = true){
        anim.SetActive(state);
    }

    public void ClickOpenMenu(bool open){
        DisableUI();
        ActivatePanel(mainPanel, open);
        if (open) EventSystem.current.SetSelectedGameObject(menuSelect);
    }

    public void ClickOpenOptions(bool options){
        DisableUI();
        ActivatePanel(optionsPanel, options);
        EventSystem.current.SetSelectedGameObject(optionsSelect);
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
        EventSystem.current.SetSelectedGameObject(endSelect);
    }
}