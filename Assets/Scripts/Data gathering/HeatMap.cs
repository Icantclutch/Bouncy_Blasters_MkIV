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
    public static int gridX = 600;
    public static int gridY = 600;
    private GameObject _gameManager;
    private GameObject _player;
    public static string path = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Desktop) + "\\App";
    public static Color startColor = new Color(0,0,255);
    public static Color endColor = new Color(255, 0, 0);

    // Start is called before the first frame update
    void Start()
    {
        singleton = this;
        intGrid = new int[gridX, gridY];
    
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
        float MinX = Floor.transform.position.x + Floor.GetComponent<RectTransform>().rect.xMin; // needed for top left
        float MaxX = Floor.transform.position.x + Floor.GetComponent<RectTransform>().rect.xMax; // needed for bottom right
        float MinY = Floor.transform.position.y + Floor.GetComponent<RectTransform>().rect.yMin; // needed for bottom right
        float MaxY = Floor.transform.position.y + Floor.GetComponent<RectTransform>().rect.yMax; // needed for top left
        topLeft = new Vector2(MinX, MaxY);
        bottomRight = new Vector2(MaxX, MinY);
    }

    public static void StoreAndSave()
    {
        //check if the singleton exists 
        //if not then the heatmap is not set up
        //
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
                    float value = (float) intGrid[i, j] / maxInt;
                    tex.SetPixel(i, j, Color.Lerp(startColor, endColor, value));
                }
            }
            byte[] bytes = tex.EncodeToPNG();

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            File.WriteAllBytes( path + "\\HeatMap.png" , bytes);
        }
    }

    public static void playerToIntGrid(GameObject player)
    {
        Vector3 playerPos = player.transform.position;
        Vector2 relativePosition;
        relativePosition.x = (playerPos.x - topLeft.x) / (bottomRight.x - topLeft.x);
        relativePosition.y = (playerPos.z - topLeft.y) / (bottomRight.y - topLeft.y);
        
        //intGrid[Mathf.RoundToInt(relativePosition.x * gridX), Mathf.RoundToInt(relativePosition.y * gridY)] += 1;
    }
}