using pong;
using UnityEngine;

namespace Mirror.Examples.Pong
{
    public class Player : NetworkBehaviour
    {

        public Side side;
        public float speed = 30;
        public Rigidbody2D rigidbody2d;

        private void Start()
        {
            if(SystemInfo.supportsGyroscope)
            {
                Input.gyro.enabled = true;
            }
        }

        // need to use FixedUpdate for rigidbody
        void FixedUpdate()
        {
            // only let the local player control the racket.
            // don't control other player's rackets
            if (isLocalPlayer && Gamemanager.instance.SU.Gamestart)
                rigidbody2d.velocity = new Vector2(0, Input.GetAxisRaw("Vertical")) * speed * Time.fixedDeltaTime;


            if (SystemInfo.supportsGyroscope && isLocalPlayer && Gamemanager.instance.SU.Gamestart)
            {
                Gamemanager.instance.playerside = side;
                float pos = Mathf.Clamp(11.46f * Input.acceleration.y,-11.46f, 11.46f);
                transform.position =new Vector3 (transform.position.x, pos, transform.position.z);
                Debug.Log(Input.acceleration);
            }
        }

        private Quaternion GyroToUnity(Quaternion q)
        {
            return new Quaternion(q.x, q.y, -q.z, -q.w);
        }

        [ServerCallback]
        //this will call only when the server will change the data
        void OnCollisionEnter2D(Collision2D col)
        {
            Ball ball = col.gameObject.GetComponent<Ball>();
            if (col.gameObject.tag == "Ball")
            {
                Debug.Log("Ball collision player");

                if (ball.lasttouch != side)
                {
                    ball.lasttouch = side;
                }
                Gamemanager.instance.GE.ScoreChange(1, side);
            }

        }

        [ClientRpc]
        public void Setting_text(Side sidesent)
        {
            if(isLocalPlayer)

            Debug.Log("Side" + sidesent.ToString());
            Gamemanager.instance.SU.setting_Text_Racket("Connected");
        }
    }
}
