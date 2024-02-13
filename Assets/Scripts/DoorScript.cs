using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorScript : MonoBehaviour
{
    public Animator DoorAnim;
    bool Opened;
    bool Interactable = true;
    public AudioSource DoorOpen;
    public string KeyNeeded;
    public bool Locked;
    // Start is called before the first frame update
    void Start()
    {
        Locked = true;
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
        if(Locked){
            PlayerMovement player = GameObject.Find("Player").GetComponent<PlayerMovement>();
            if(player.HasKey(KeyNeeded)){
                Locked = false;
                Opened = !Opened;
                player.RemoveKey(KeyNeeded);
                StartCoroutine(Delay());
            }else{
                player.SendMessage("You Need a Key to Open This Door");
            }
        }
        if(Interactable && !Locked){
            Opened = !Opened;
            StartCoroutine(Delay());
        }
    }
    IEnumerator Delay(){
        Interactable = false;
        DoorOpen.Play();
        yield return new WaitForSeconds(1);
        Interactable = true;
    }
}
