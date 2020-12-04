using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SocialPlatforms;

public class WallMover : MonoBehaviour
{
    public float speed = 7f;
    public int maxWait = 12;

    // Start is called before the first frame update
    void Start()
    {
        foreach (Transform child in transform)
        {
            Holder.Add(new WallInfo(child.localPosition, child.name));
        }
        foreach (Transform child in transform)
        {
            StartCoroutine(MoveWall(child));
        }
    }

    //Holds walls original info
    public class WallInfo
    {
        public Vector3 Pos;
        public String Name;
        public WallInfo(Vector3 POSITION, String NAMETOUSE)
        {
            Pos = POSITION;
            Name = NAMETOUSE;
        }
    }
    //Holds object and position of where it wants to move
    public class MovingWallInfo
    {
        public Vector3 Pos;
        public Transform obj;
        public MovingWallInfo(Vector3 POSITION, Transform TRANSFORMTOUSE)
        {
            Pos = POSITION;
            obj = TRANSFORMTOUSE;
        }
    }

    List<WallInfo> Holder = new List<WallInfo>();
    List<MovingWallInfo> MovingWalls = new List<MovingWallInfo>();

    //Return the objects original position from a name given
    private Vector3 ReturnOriginalPos(String inputName)
    {
        WallInfo WallInList = Holder.Find(x => x.Name == inputName);
        return WallInList.Pos;
    }

    IEnumerator MoveWall(Transform go)
    {
        Vector3 startPos = ReturnOriginalPos(go.name);
        Vector3 endPos = new Vector3(0, 0, 0);
        bool IsDown = false;
        //Decide the positions based off the table of original positions
        if (startPos.y > go.localPosition.y)
        {
            endPos = startPos;
            IsDown = true;
        }
        else {
            endPos = startPos - new Vector3(0, 25, 0);
        }

        //Do the waiting (if down, wait less)
        if (IsDown)
        {
            int WallCount = 1;
            yield return new WaitForSeconds(UnityEngine.Random.Range(WallCount, WallCount + Mathf.Ceil(maxWait/6)));
        }
        else
        {
            var WallCount = ToRemoveWalls.Count + 7;
            yield return new WaitForSeconds(UnityEngine.Random.Range(WallCount, WallCount + maxWait));
        }
        //Add to the moving wall list
        MovingWalls.Add(new MovingWallInfo(endPos, go));
        //Wait till the movement is done
        yield return new WaitForSeconds(speed);
        //Start the movement again
        StartCoroutine(MoveWall(go));
    }

    List<MovingWallInfo> ToRemoveWalls = new List<MovingWallInfo>();

    // Update is called once per frame
    void Update()
    {
        //Loop through the moving walls and actually move them based on their position
        foreach (MovingWallInfo MW in MovingWalls)
        {
            if (MW != null)
            {
                float step = speed * Time.deltaTime; // calculate distance to move
                MW.obj.localPosition = Vector3.MoveTowards(MW.obj.localPosition, MW.Pos, step);

                //If done add to a seperate list to remove (prevents errors from removing directly from the list)
                if (MW.obj.localPosition == MW.Pos)
                {
                    ToRemoveWalls.Add(MW);
                }
            }
        }

        //Remove everything and then clear the list for more (fancy trash can)
        foreach (MovingWallInfo MW in ToRemoveWalls)
        {
            MovingWalls.Remove(MW);
        }
        ToRemoveWalls.Clear();
    }
}
