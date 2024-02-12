using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorScript : MonoBehaviour
{
    public Animator DoorAnim;
    bool Opened;
    bool Interactable = true;
    // Start is called before the first frame update
    void Start()
    {
        Opened = false;
    }

    // Update is called once per frame
    void Update()
    {
        if(Opened){
            DoorAnim.SetBool("Open", true);
        }else{
            DoorAnim.SetBool("Open", false);
        }
    }
    public void Open(){
        if(Interactable){
            Opened = !Opened;
            StartCoroutine(Delay());
        }
    }
    IEnumerator Delay(){
        Interactable = false;
        yield return new WaitForSeconds(1);
        Interactable = true;
    }
}
