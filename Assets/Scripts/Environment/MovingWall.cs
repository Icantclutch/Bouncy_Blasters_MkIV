using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MovingWall : MonoBehaviour
{

    float clock;
    public float maxTime;
    public bool isUp;
    public float moveDistance;
    public float speed;
    Vector3 upMove;
    Vector3 downMove;
    // Start is called before the first frame update
    void Start()
    {
        clock = 0;
        //if the wall is already up, then downMove will send the wall down, and upMove will return it to its original position
        if(isUp)
        {
            downMove = new Vector3(this.transform.position.x, this.transform.position.y - moveDistance, this.transform.position.z);
            upMove = this.transform.position;
        }
        //if the wall starts down, upMove will make the wall go up, and downMove returns it to its original position
        else if (!isUp)
        {
            downMove = this.transform.position;
            upMove = new Vector3(this.transform.position.x, this.transform.position.y + moveDistance, this.transform.position.z);
        }


    }

    // Update is called once per frame
    void Update()
    {
        //clock is a timer going up
        clock += Time.deltaTime;
        Debug.Log("up position: " + upMove);
        Debug.Log("down position: " + downMove);
        
        MoveWall();

    }



    public void MoveWall()
    { 
    if (clock >= maxTime)
    {
            //if the wall is up, move wall down, change isUp to false and reset the timer
        if(isUp)
        {
                this.transform.position = Vector3.MoveTowards(this.transform.position, downMove , speed*Time.deltaTime);
            isUp = false;
            clock = 0;
        }
        //if wall is down then move the wall up, change isUp to true and reset the timer
        else if (!isUp)
        {
                this.transform.position = Vector3.MoveTowards(this.transform.position, upMove, speed * Time.deltaTime);
            isUp = true;
            clock = 0;

        }
    }
}
}
