using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonsScript : MonoBehaviour
{
    public string buttonName;
    public void OnButtonPress(){
        SceneManager.LoadScene(buttonName);
    }
}
