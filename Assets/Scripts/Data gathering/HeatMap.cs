using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

//using System.Drawing.Bitmap;
public class HeatMap : MonoBehaviour
{
    public static HeatMap singleton;
    public GameObject Floor;
    public static Vector2 topLeft;
    public static Vector2 bottomRight;
    public static int[,] intGrid;
    public static int gridX = 300; //thease values represent that actual size of the heatmap that is being created (in Pixels) 
    public static int gridY = 300;
    private GameObject _gameManager;
    private GameObject _player;
    public static string path = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Desktop) + "\\App"; // this is the folder that will hold the heatmap data
    public static Color startColor = new Color(0,0,255);
    public static Color endColor = new Color(255, 0, 0);

    // Start is called before the first frame update
    void Start()
    {
        singleton = this; //only one of its kind change if you are using more floors
        intGrid = new int[gridX, gridY];
        createGrid();
    }

    // Update is called once per frame
    void Update()
    {
       
    }

    private void FixedUpdate()
    {
        if (!_gameManager)
        {
            _gameManager = GameObject.FindGameObjectWithTag("Management");
        }
        foreach (PlayerData player in _gameManager.GetComponent<LobbyManager>().players)
        {
            playerToIntGrid(player.gameObject);
        }
    }

    public void createGrid() {
        float MinX = Floor.GetComponent<Collider>().bounds.min.x; // needed for top left
        float MaxX = Floor.GetComponent<Collider>().bounds.max.x; // needed for bottom right
        float MinY = Floor.GetComponent<Collider>().bounds.min.z; // needed for bottom right
        float MaxY = Floor.GetComponent<Collider>().bounds.max.z; // needed for top left
        topLeft = new Vector2(MinX, MinY);
        bottomRight = new Vector2(MaxX, MaxY);
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(Floor.transform.position, new Vector3(topLeft.x*2, 20, topLeft.y*2));
    }

    public static void StoreAndSave()
    {
        //check if the singleton exists 
        //if not then the heatmap is not set up
        if (!singleton)
        {
            return;
        }
        else
        {
            int maxInt = -1;
            for (int i = 0; i < intGrid.GetLength(0); i++)
            {
                for (int j = 0; j < intGrid.GetLength(1); j++)
                {
                    if (intGrid[i,j] > maxInt)
                    {
                        maxInt = intGrid[i, j];
                    }
                }
            }

            Texture2D tex = new Texture2D(intGrid.GetLength(0), intGrid.GetLength(1));
            
            for (int i = 0; i < tex.width; i++)
            {
                for(int j = 0; j < tex.height; j++)
                {
                    float value = Mathf.InverseLerp(0, maxInt, intGrid[i,j]); //(float) intGrid[i, j] / maxInt;
                    tex.SetPixel(i, j, Color.Lerp(startColor, endColor, value));
                    Debug.Log(value);
                }
            }
            byte[] bytes = tex.EncodeToPNG();

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            File.WriteAllBytes( path + "\\HeatMap.png" , bytes); //this is the file that will be created when using the heatmap
        }
    }

    public static void playerToIntGrid(GameObject player)
    {
        Vector3 playerPos = player.transform.position;
        Vector2 relativePosition;
        relativePosition.x = (playerPos.x - topLeft.x) / (bottomRight.x - topLeft.x);
        relativePosition.y = (playerPos.z - topLeft.y) / (bottomRight.y - topLeft.y);
        
        intGrid[Mathf.RoundToInt(relativePosition.x * gridX), Mathf.RoundToInt(relativePosition.y * gridY)] += 1; //values of gridx and grid y can be interchanged to be more precise(more detailed) lore less precise
                                                                                                                  //the breater the number of those two values the greater the number of pixels used in the out image
    }
}