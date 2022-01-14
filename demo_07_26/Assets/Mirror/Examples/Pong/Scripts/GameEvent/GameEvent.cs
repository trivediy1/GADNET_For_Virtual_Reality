using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;


public class GameEvent : MonoBehaviour
{
    public event Action<int,Side> onScore = null;

    public void ScoreChange(int currentIndex ,Side side)
    {
        onScore?.Invoke(currentIndex , side);
    }

    public event Action<int, Side> onMiss = null;

    public void misschange(int currentIndex, Side side)
    {
        onMiss?.Invoke(currentIndex, side);
    }

    private void Start()
    {
        GetLocalIPAddress();
    }

    public void GetLocalIPAddress()
    {
       // print("IP!@#" + IPManager.GetIP(IPManager.ADDRESSFAM.IPv4));
    }

}
