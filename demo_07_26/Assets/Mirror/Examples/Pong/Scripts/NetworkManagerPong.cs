using pong;
using UnityEngine;
using UnityEngine.UI;

namespace Mirror.Examples.Pong
{
    // Custom NetworkManager that simply assigns the correct racket positions when
    // spawning players. The built in RoundRobin spawn method wouldn't work after
    // someone reconnects (both players would be on the same side).
    [AddComponentMenu("")]
    public class NetworkManagerPong : NetworkManager
    {
        public Transform leftRacketSpawn;
        public Transform rightRacketSpawn;
        GameObject ball;

        public GameObject Leftpong;
        public GameObject RightPong;

        public override void OnServerAddPlayer(NetworkConnection conn)
        {

            //old code
            // add player at correct spawn position
            //Transform start = numPlayers == 0 ? leftRacketSpawn : rightRacketSpawn;
            //GameObject player = Instantiate(playerPrefab, start.position, start.rotation);
            //NetworkServer.AddPlayerForConnection(conn, player);
            //// conn.Send()
            //// spawn ball if two players
            //if (numPlayers == 2)
            //{
            //    ball = Instantiate(spawnPrefabs.Find(prefab => prefab.name == "Ball"));
            //    NetworkServer.Spawn(ball);
            //}

            //New Code
            if (numPlayers == 0)
            {
                GameObject player = Instantiate(playerPrefab, leftRacketSpawn.position, leftRacketSpawn.rotation);
                player.GetComponent<Player>().side = Side.Left;
                Leftpong = player;
                NetworkServer.AddPlayerForConnection(conn, player);
                player.GetComponent<Player>().Setting_text(Side.Left);
                //Gamemanager.instance.SU.setting_Text_Racket("LeftRacket");
            }
            else
            {
                GameObject player = Instantiate(playerPrefab, rightRacketSpawn.position, rightRacketSpawn.rotation);
                player.GetComponent<Player>().side = Side.Right;
                RightPong = player;
                NetworkServer.AddPlayerForConnection(conn, player);
                player.GetComponent<Player>().Setting_text(Side.Right);
               // Gamemanager.instance.SU.setting_Text_Racket("RightRacket");
            }

            if (numPlayers == 2 )
            {
                Gamemanager.instance.SU.Enable_Start_ButtonRPC(true);
            }
            // conn.Send()
            // spawn ball if two players

        }

        public override void OnStartServer()
        {
            base.OnStartServer();
            Gamemanager.instance.Enable_Start_Button(false);
        }

        public override void OnServerDisconnect(NetworkConnection conn)
        {
            // destroy ball
            if (ball != null)
                NetworkServer.Destroy(ball);

            // call base functionality (actually destroys the player)
            base.OnServerDisconnect(conn);

            if(numPlayers <2)
            {
                Gamemanager.instance.SU.Gamestart = false;
                Reset();
            }
        }

        public void start_game()
        {
            if (numPlayers == 2)
            {
                ball = Instantiate(spawnPrefabs.Find(prefab => prefab.name == "Ball"));
                NetworkServer.Spawn(ball);
                Gamemanager.instance.SU.Enable_Start_ButtonRPC(false);
            }
        }

        public void Reset()
        {
            Gamemanager.instance.SU.Gamestart = false;
            Leftpong.transform.position = leftRacketSpawn.position;
            RightPong.transform.position = rightRacketSpawn.position;
        }

        public void GameOver()
        {
            Gamemanager.instance.SU.Gamestart = false;
            if (ball != null)
            {
                NetworkServer.Destroy(ball);
                ball = null;
                Reset();
            }
            Gamemanager.instance.SU.Enable_Start_ButtonRPC(true);
            
        }
    }
}
