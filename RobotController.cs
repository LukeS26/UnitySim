using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class RobotController : MonoBehaviour
{
    GameObject robot;
    public GameObject cargo;
    public static float angle, x, y, z, dist;
    public float turn = 0;
    public static Vector3 velo, actualVelo;
    public Vector3 vl, vl2;
    public float speed;
    public double mult;
    float t = 0;
    
    // Start is called before the first frame update
    void Start() {
        robot = this.gameObject;
    }

    void FixedUpdate() {
        
        //angle += (float) turn;

        //robot.transform.localRotation = Quaternion.Euler(0, (float) (angle * (180 / Math.PI)), 0);
    }

    public void Update() {
        t += Time.deltaTime;

        robot.GetComponent<Rigidbody>().velocity = new Vector3(Input.GetAxis("Horizontal") * speed, 0, Input.GetAxis("Vertical") * speed);        

        x = robot.transform.position.x;
        z = robot.transform.position.z;
        y = robot.transform.position.y;

        dist = (float) Math.Sqrt(x*x + z*z);
        
        angle = (float) (Math.Atan2(x, z));

        velo = robot.GetComponent<Rigidbody>().GetRelativePointVelocity(new Vector3(0, 0, 0));
        actualVelo = new Vector3(velo.x, velo.y, velo.z);
        
        //velo = new Vector3( x, y, z );

        velo = new Vector3 ( (float) -( Math.Abs( Math.Sin(angle) * velo.z) + Math.Cos(angle) * velo.x), 0, (float) -(Math.Cos(angle) * velo.z + (Math.Sin(angle) * velo.x) ) );

        vl = velo;
        vl2 = actualVelo;
        
        if(Input.GetKeyDown("space") && t > 0.3) {
            t = 0;
            GameObject ball = Instantiate(cargo, new Vector3(x, 0.6f, z), Quaternion.identity);
        }
    }

    public void updateAngle(double t) {
        //angle -= (float) turn;
        turn = (float) t;

        robot.transform.localRotation = Quaternion.Euler(0, (float) ( ( (angle + (float) t) * (180 / Math.PI))), 0);
    }
}
