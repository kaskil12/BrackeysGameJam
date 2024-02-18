using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PerkDetector : MonoBehaviour
{
    public PlayerMovement player;
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Player").GetComponent<PlayerMovement>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void OnTriggerEnter(Collider other){
        if(other.gameObject.tag == "Player"){
            player.PerkVoid();
            Destroy(gameObject);
        }
    }
}
