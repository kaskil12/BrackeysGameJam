using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomGenerator : MonoBehaviour
{
    public class Cell{
        public bool visited = false;
        public bool[] status = new bool[4];
    }
    public Vector2 size;
    public int startPos = 0;
    public GameObject room;
    public Vector2 offset;

    List<Cell> board;
    // Start is called before the first frame update
    void Start()
    {
        int width = PlayerPrefs.GetInt("Width", 10); // 10 is a default value
        int height = PlayerPrefs.GetInt("Height", 10); // 10 is a default value
        size = new Vector2(width, height);
        MazeGenerator();
    }

    // Update is called once per frame
    void Update()
    {
        int width = PlayerPrefs.GetInt("Width", 10); // 10 is a default value
        int height = PlayerPrefs.GetInt("Height", 10); // 10 is a default value
        size = new Vector2(width, height);
    }

    void GenerateDungeon(){
        for(int i = 0; i < size.x; i++){
            for(int j = 0; j < size.y; j++){
                Cell currentCell = board[Mathf.FloorToInt(i+j*size.x)];
                if(currentCell.visited){
                    var newRoom = Instantiate(room, new Vector3(i * offset.x, 0, -j * offset.y), Quaternion.identity, transform).GetComponent<RoomBehaviour>();
                    newRoom.UpdateRoom(currentCell.status);
                    newRoom.name += " "+i+"-" + j;
                }
            }
        }
    }
    
    void MazeGenerator(){
        // 0 - Up 1 - Down 2 - Right 3 - Left
        board = new List<Cell>();
        for (int i = 0; i < size.x; i++){
            for (int j = 0; j < size.y; j++){
                board.Add(new Cell());
            }
        }
        int currentCell = startPos;

        Stack<int> path = new Stack<int>();

        int k = 0;

        while (k < 1000){
            k++;

            board[currentCell].visited = true;

            if(currentCell == board.Count-1){
                break;
            }
            List<int> neighbours = CheckNeighbours(currentCell);
            if(neighbours.Count == 0){
                if(path.Count == 0){
                    break;
                }else{
                    currentCell = path.Pop();
                }
            }else{
                path.Push(currentCell);
                int newCell = neighbours[Random.Range(0, neighbours.Count)];
                if(newCell > currentCell){
                    if(newCell - 1 == currentCell){
                        board[currentCell].status[2] = true;
                        currentCell = newCell;
                        board[currentCell].status[3] = true;
                    }else{
                        board[currentCell].status[1] = true;
                        currentCell = newCell;
                        board[currentCell].status[0] = true;
                    }
                }else{
                    if(newCell + 1 == currentCell){
                        board[currentCell].status[3] = true;
                        currentCell = newCell;
                        board[currentCell].status[2] = true;
                    }else{
                        board[currentCell].status[0] = true;
                        currentCell = newCell;
                        board[currentCell].status[1] = true;
                    }
                }
            }
        }
        GenerateDungeon();
    }
    List<int> CheckNeighbours(int cell){
        List<int> neighbours = new List<int>();
        //Check up neighbour
        if(cell - size.x >= 0 && !board[Mathf.FloorToInt(cell - size.x)].visited){
            neighbours.Add(Mathf.FloorToInt(cell - size.x));
        }
        //Check down neighbour
        if(cell + size.x < board.Count && !board[Mathf.FloorToInt(cell + size.x)].visited){
            neighbours.Add(Mathf.FloorToInt(cell + size.x));
        }
        //Check right neighbour
        if((cell+1) % size.x != 0 && !board[Mathf.FloorToInt(cell + 1)].visited){
            neighbours.Add(Mathf.FloorToInt(cell + 1));
        }
        //Check left neighbour
        if(cell % size.x != 0 && !board[Mathf.FloorToInt(cell - 1)].visited){
            neighbours.Add(Mathf.FloorToInt(cell - 1));
        }
        return neighbours;
    }
}
