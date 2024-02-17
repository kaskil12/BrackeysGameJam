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
                List<int> availablePositions = new List<int>();
                for (int i = 0; i < FurniturePositions.Length; i++)
                {
                    availablePositions.Add(i);
                }
                foreach(GameObject furniture in Furniture){
                    if (availablePositions.Count == 0)
                    {
                        break; // No more available positions
                    }
                    int randomIndex = Random.Range(0, availablePositions.Count);
                    int positionIndex = availablePositions[randomIndex];
                    GameObject furniturePosition = FurniturePositions[positionIndex];
                    Instantiate(furniture, furniturePosition.transform.position, furniturePosition.transform.rotation);
                    availablePositions.RemoveAt(randomIndex);
                }
            }
        }
    }
    public GameObject[] walls; // 0 - Up 1 - Down 2 - Right 3 - Left
    public GameObject[] doors;
    public void UpdateRoom(bool[] status){
        for(int i = 0; i < status.Length; i++){
            doors[i].SetActive(status[i]);
            walls[i].SetActive(!status[i]);
        }
    }
}
