using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomBehaviour : MonoBehaviour
{
    public GameObject[] Furniture;
    public GameObject[] FurniturePositions;
    void Start(){
        Furniture = Resources.LoadAll<GameObject>("Furniture");
                if(Furniture.Length > 0){
            //if the furniture is not spawned
            if(FurniturePositions.Length > 0){
                //spawn the furniture
                foreach(GameObject furniture in Furniture){
                    int randomIndex = Random.Range(0, FurniturePositions.Length);
                    GameObject furniturePosition = FurniturePositions[randomIndex];
                    Instantiate(furniture, furniturePosition.transform.position, furniturePosition.transform.rotation);
                }
            }
        }
    }
    public GameObject[] walls; // 0 - Up 1 - Down 2 - Right 3 - Left
    public GameObject[] doors;
    public bool[] testStaus;
    public void UpdateRoom(bool[] status){
        for(int i = 0; i < status.Length; i++){
            doors[i].SetActive(status[i]);
            walls[i].SetActive(!status[i]);
        }
    }
}
