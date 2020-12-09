using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SocialPlatforms;
using UnityEngine.SocialPlatforms.GameCenter;

public class BigWallMover : MonoBehaviour
{
    public float speed = 7f;
    public int maxWait = 12;
    private int upWalls = 0;

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
    List<String> UpWalls = new List<String>();

    //Return the objects original position from a name given
    private Vector3 ReturnOriginalPos(String inputName)
    {
        WallInfo WallInList = Holder.Find(x => x.Name == inputName);
        return WallInList.Pos;
    }

    //A check to see if the called name is in the UpWalls list
    //Used in canbringup
    private bool IsInUpWalls(String WallName)
    {
        if (UpWalls.Contains(WallName))
        {
            return false;
        }
        return true;
    }

    //Checks the wall name with the UpWall list
    //If the wall that prevents it from coming up is up, it will return false otherwise
    //It will default to true allowing the MoveWall function to use it
    private bool CanBringUp(String WallName)
    {
        if (WallName == "Box room 1" || WallName == "Box room 2")
        {
            if (UpWalls.Contains("center octagon")) { return false; }
            if (UpWalls.Contains("perpendicular 1")) { return false; }
        } 
        else if (WallName == "Isle")
        {
            if (UpWalls.Contains("trapazoid 1")) { return false; }
            if (UpWalls.Contains("trapazoid 2")) { return false; }
        } 
        else if (WallName == "center octagon")
        {
            if (UpWalls.Contains("perpendicular 1")) { return false; }
        } 
        else if (WallName == "trapazoid 1" || WallName == "trapazoid 2")
        {
            if (UpWalls.Contains("perpendicular 1")) { return false; }
            if (UpWalls.Contains("Isle")) { return false; }
        } 
        else if (WallName == "perpendicular 1")
        {
            if (UpWalls.Contains("trapazoid 1")) { return false; }
            if (UpWalls.Contains("trapazoid 2")) { return false; }
            if (UpWalls.Contains("center octagon")) { return false; }
            if (UpWalls.Contains("Box room 1")) { return false; }
            if (UpWalls.Contains("Box room 2")) { return false; }
        }
        //Box room 1
        //Box room 2
        //Isle
        //center octagon
        //trapazoid 1
        //trapazoid 2
        //perpendicular 1

        return true;
    }

    IEnumerator MoveWall(Transform go)
    {
        Vector3 startPos = ReturnOriginalPos(go.name);
        Vector3 endPos = new Vector3(0, 0, 0);
        bool IsDown = false;
        //Decide the positions based off the table of original positions
        if (startPos.y < go.localPosition.y)
        {
            endPos = startPos;
            IsDown = true;
        }
        else
        {
            endPos = startPos + new Vector3(0, 50, 0);
        }

        //Do the waiting (if down, wait less)
        if (IsDown)
        {
            int WallCount = 1;
            yield return new WaitForSeconds(UnityEngine.Random.Range(WallCount, WallCount + maxWait));
        }
        else
        {
            var WallCount = ToRemoveWalls.Count + 7;
            yield return new WaitForSeconds(UnityEngine.Random.Range(WallCount, WallCount + maxWait));
            //Check if wall is up and if the max wall count is already reached
            while (UpWalls.Count >= 2 || !CanBringUp(go.name))
            {
                yield return new WaitForSeconds(UnityEngine.Random.Range(5, 10));
            }
            UpWalls.Add(go.name);
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
                    if (UpWalls.Contains(MW.obj.name))
                    {
                        UpWalls.Remove(MW.obj.name);
                    }
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
