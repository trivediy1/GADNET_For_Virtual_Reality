using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using pong;
using Mirror.Examples.Pong;

public class WallScripts : NetworkBehaviour
{
    public Side side;
    [ServerCallback]
    private void OnCollisionEnter2D(Collision2D col)
    {
        Debug.Log("Detect");
        if (col.gameObject.tag == "Ball")
        {
            Ball ball = col.gameObject.GetComponent<Ball>();
            if (ball.lasttouch != side)
            {
                ball.lasttouch = side;
                Gamemanager.instance.GE.misschange(1, side);
            }
        }
        
    }
}
