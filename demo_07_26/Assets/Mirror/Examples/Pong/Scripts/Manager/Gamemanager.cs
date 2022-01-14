using Mirror;
using Mirror.Examples.Pong;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.Management;

public enum Side
{
    Left,Right
}
namespace pong
{
    public class Gamemanager : MonoBehaviour
    {
        [Header("Camera")] 
        public GameObject Simplecamera;
        public GameObject rotationcamera;
        
        [Header("Planel")]
        public GameObject Main_Planel;
        public GameObject Connect_Planel;
        public GameObject AfterConnection;

        public GameObject ClientPlanel;
        public GameObject StartPlanel;

        public Side playerside;
        

        [Header("Sync Data")]
        public ScoreUpdate SU;

        [Header("Start_Button")]
        public Button Start_Game_Button;

        [Header("StopServer")]
        public Button Stop_Server;

        [Header("HighScore")]
        public Text HighScoreText;


        public static Gamemanager instance;
        public NetworkManagerPong manager;

        public GameEvent GE;

        public Text IPServer;
        public Text IPClients;

        public InputField ipaddress;

        public IEnumerator enumerator;
        private void Awake()
        {
            Screen.sleepTimeout = SleepTimeout.NeverSleep;
            if (instance == null)
            {
                instance = this;
                manager = GetComponentInParent<NetworkManagerPong>();
            }
            ClientPlanel.SetActive(true);
            StartPlanel.SetActive(false);
            Enable_Start_Button(false);
            Main_Planel.SetActive(true);
            AfterConnection.SetActive(false);
            Connect_Planel.SetActive(false);

        }

        private void OnEnable()
        {

        }

        private void OnDisable()
        {

        }

        private void Start()
        {
            print("IP!@#" + IPManager.GetLocalIPAddress());
            //IPServer.text = IPManager.GetIP(IPManager.ADDRESSFAM.IPv4);
            IPServer.text = IPManager.GetLocalIPAddress();
            
            HighScoreText.text = "HighScroce " + SU.Highscore;
           
        }
        

        public void StartHosting()
        {
            manager.StartServer();
            StartCoroutine(ServerStarted());
        }

        public void StartClient()
        {

            if (ipaddress.text == null || ipaddress.text == "")
            {
                ipaddress.transform.GetChild(1).GetComponent<Text>().text = "Plz add ip address";
                return;
            }
            manager.networkAddress = ipaddress.text;
            manager.StartClient();
            StartCoroutine(ClientStarted());
        }

        public void Disconnect_Action()
        {
            StartPlanel.SetActive(false);
            ClientPlanel.SetActive(true);
            Main_Planel.SetActive(true);
            Connect_Planel.SetActive(false);
            AfterConnection.SetActive(false);
        }

        IEnumerator ServerStarted()
        {
            yield return new WaitUntil(() => NetworkServer.active);
            StartCoroutine(StartXR());
            Debug.Log("Server Started");
           // Simplecamera.SetActive(false);
          //  rotationcamera.SetActive(true);
            rotationcamera.transform.rotation = Quaternion.identity;
            Server_Started();          
        }

        IEnumerator ServerStop()
        {
            yield return new WaitUntil(() => !NetworkServer.active);
            StopXR();
            Debug.Log("Server Stop");
          //  Simplecamera.SetActive(false);
          //  rotationcamera.SetActive(true);
            Disconnect_Action();
        }


        public void Connection_Attempt()
        {
            Main_Planel.SetActive(false);
            Connect_Planel.SetActive(true);
            enumerator = ClientConnected();
            StartCoroutine(enumerator);
            StartCoroutine(ClientStoped());
            
        }

        IEnumerator ClientStarted()
        {
            yield return new WaitUntil(() => NetworkClient.active);
            Connection_Attempt();
        }

        IEnumerator ClientStoped()
        {  

            yield return new WaitUntil(() => !NetworkClient.active);
            Debug.Log("ClientStoped" + NetworkClient.isConnected);
            StopCoroutine(enumerator);
            Disconnect_Action();
        }

        IEnumerator ClientConnected()
        {
            Debug.Log("Client connected properly" + NetworkClient.isConnected);
            yield return new WaitUntil(() => NetworkClient.isConnected);
            //Debug.Log("Client connected properly");
            Client_Started();
        }
        
        public void Server_Started()
        {
            ClientPlanel.SetActive(false);
            StartPlanel.SetActive(true);
            Main_Planel.SetActive(false);
            Connect_Planel.SetActive(false);
            StartCoroutine(ServerStop());
        }

        public void Client_Started()
        {
            ClientPlanel.SetActive(true);
            // StartPlanel.SetActive(true);
            AfterConnection.SetActive(true);
            Main_Planel.SetActive(false);
            Connect_Planel.SetActive(false);
            //StartCoroutine(ServerStop());
        }

        public void Started_Game()
        {
            ClientPlanel.SetActive(false);
            StartPlanel.SetActive(true);
        }

        
        public void Start_game()
        {
            Debug.Log("Start_game");
            SU.Start_game_from_client();
        }

        public void StopServer()
        {
            Debug.Log("StopServer");
            SU.Stop_game_from_client();
        }

        public void Stop_Server_Button_CLick()
        {
            rotationcamera.transform.GetChild(0).transform.rotation = Quaternion.identity;
            manager.StopServer();
        }

        public IEnumerator Start_game_timer()
        {
            SU.Miss = 0;
            SU.time = 0;
            yield return new WaitForSeconds(2);
            SU.Gamestart = true;
            manager.start_game();
        }

        public void Enable_Start_Button(bool active)
        {
            Start_Game_Button.gameObject.SetActive(active);
        }

        public void Enable_StopServer_Button(bool active)
        {
            Stop_Server.gameObject.SetActive(active);
        }
        
        private IEnumerator StartXR()
        {
            Debug.Log("Initializing XR...");
                yield return XRGeneralSettings.Instance.Manager.InitializeLoader();

            if (XRGeneralSettings.Instance.Manager.activeLoader == null)
            {
                Debug.LogError("Initializing XR Failed.");
            }
            else
            {
                Debug.Log("XR initialized.");

                Debug.Log("Starting XR...");
                XRGeneralSettings.Instance.Manager.StartSubsystems();
                
                Debug.Log("XR started.");
            }
        }

        private void StopXR()
        {
            Debug.Log("Stopping XR...");
            XRGeneralSettings.Instance.Manager.StopSubsystems();
            Debug.Log("XR stopped.");

            Debug.Log("Deinitializing XR...");
            XRGeneralSettings.Instance.Manager.DeinitializeLoader();
            Debug.Log("XR deinitialized.");
        }

       
        


    }
}
