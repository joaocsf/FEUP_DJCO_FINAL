using UnityEngine;
using UnityEngine.UI;
using System.Collections;
public class ClickButtonExit : MonoBehaviour

{
    public void onClick(){

        Debug.Log ("Application Closing");
        Application.Quit ();
    }
}