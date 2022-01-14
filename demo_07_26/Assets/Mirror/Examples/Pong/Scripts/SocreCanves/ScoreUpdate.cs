using Mirror;
using pong;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreUpdate : NetworkBehaviour
{
    public int LeftScore;
    public int RightScore;

    public int LeftMiss;
    public int RightMiss;

    public override void OnStartServer()
    {
        base.OnStartServer();
        if (isServer)
        {
            Gamemanager.instance.GE.onScore += Set_Score;
            Gamemanager.instance.GE.onMiss += Set_Miss;
        }
    }

    public override void OnStopServer()
    {
        base.OnStopServer();
        if (isServer)
        {
            Gamemanager.instance.GE.onScore -= Set_Score;
            Gamemanager.instance.GE.onMiss -= Set_Miss;
        }
    }

    [SyncVar(hook = nameof(SetScoreLeft))]
    public int Left = 0;

    [SyncVar(hook = nameof(SetScoreRight))]
    public int Right = 0;

    [SyncVar(hook = nameof(SetMiss))]
    public int Miss = 10;

    [SyncVar(hook = nameof(Start_Game_bool))]
    public bool Gamestart = false;

    [SyncVar]
    public float time;

    public float Highscore
        {
             set
                {
                    PlayerPrefs.SetFloat("Highscore", value);        
                }
               get
                {
                  return  PlayerPrefs.GetFloat("Highscore",0);
                }
        }

    public Text ScoreText;
    public Text MissText;
    public void SetScoreLeft(int oldscore , int newscore)
    {
        LeftScore = newscore;
        Debug.Log("Change" + oldscore + " " + newscore);
        ScoreText.text = "Score " + LeftScore + ":" + RightScore;
    }

    public void SetScoreRight(int oldscore, int newscore)
    {
       /* RightScore = newscore;
        Debug.Log("Change" + oldscore + " " + newscore);
        ScoreText.text = "Score " + LeftScore + ":" + RightScore;*/
    }

    public void SetMiss(int oldscore, int newscore)
    {
        Miss = newscore;
        Debug.Log("Change" + oldscore + " " + newscore);
        MissText.text = "Miss " + Miss;
    }

    /*public void SetMissRight(int oldscore, int newscore)
    {
        RightMiss = newscore;
        Debug.Log("Change" + oldscore + " " + newscore);
        MissText.text = "Miss " + Left_Miss + ":" + Right_Miss;
    }*/

    public void Set_Score(int Score, Side side)
    {
        if(side == Side.Left)
        {
            Left += 1;
        }
        else
        {
            Right += 1;
        }
    }

    public void Set_Miss(int Score, Side side)
    {
        Miss += 1;
        SetMiss(Miss, Miss);
        if (Miss == 10)
        {
            Gamemanager.instance.manager.GameOver();
            if (Highscore < time)
            {
                Highscore = time;
                Gamemanager.instance.HighScoreText.text = "HighScorce " + Highscore;
            }
           
            Miss = 0;
            time = 0;
            
            Gamemanager.instance.Enable_Start_Button(true);
        }
        
    }

    public void Update()
    {
        if(Gamestart)
        {
            time += Time.deltaTime;
            ScoreText.text = "Time :" + time;
        }
    }

    public void Start_Game_bool(bool oldvalue , bool newvalue)
    {
        Debug.Log("Gamestart" + Gamestart);
        if(newvalue)
        {
            Gamemanager.instance.manager.start_game();
        }
    }
    public void setting_Text_Racket(string Racket)
    {
        if (!isServer)
        {
            Debug.Log("Racket");
            Gamemanager.instance.Main_Planel.SetActive(false);
            Gamemanager.instance.Connect_Planel.SetActive(false);
            Gamemanager.instance.AfterConnection.SetActive(true);
            Gamemanager.instance.AfterConnection.transform.GetChild(0).GetComponent<Text>().text = Racket;
        }
    }

    //[Command]
    [Command(ignoreAuthority =true)]
    public void Start_game_from_client()
    {
        Debug.Log("Commandcalled");
        StartCoroutine(Gamemanager.instance.Start_game_timer());
        Gamemanager.instance.SU.Enable_Start_ButtonRPC(false);
    }

    [Command(ignoreAuthority = true)]
    public void Stop_game_from_client()
    {
        Debug.Log("Stop the Server");
        Gamemanager.instance.Stop_Server_Button_CLick();
        
        
    }
    
    [ClientRpc]
    public void Enable_Start_ButtonRPC(bool Active)
    {
        Gamemanager.instance.Enable_Start_Button(Active);
    }

    [ClientRpc]
    public void ClosePlanel()
    {
        
    }


}
