using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TennisBallScript : MonoBehaviour
{
    public bool isEquipped = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(isEquipped)
        {
            if(Input.GetMouseButtonDown(0))
            {
                PlayerMovement player = GameObject.Find("Player").GetComponent<PlayerMovement>();
                player.HandDrop();
                GetComponent<Rigidbody>().AddForce(-1000, 0, 0);
            }
        }
    }
    public void Equip()
    {
        isEquipped = true;
    }
    public void Unequip()
    {
        isEquipped = false;
    }
}
