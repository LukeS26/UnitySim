using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Ball : MonoBehaviour
{
	GameObject ball;
	public float speed, angle;
	public double n;
	float t = 0;

	// Start is called before the first frame update
	void Start() {
		ball = this.gameObject;
		double dist = RobotController.dist;
        //double n = 45 / (dist < 1 ? 1 : dist * dist);

        //if(n < 1) { n = 1; }

		Vector3 velocity = GameObject.Find("Bot").GetComponent<RobotController>().vl;
		double d1 = dist-velocity.z;
        if( (dist - 0.381) >= (2 + velocity.x)) {
            angle = (float) Math.Atan( ((Math.Tan(-0.698131701) * (d1)) - (2 * (2.64 - 0.6))) / -(d1) );
        } else {
            angle = (float) Math.Atan( ((Math.Tan(-1.21) * (d1)) - (2 * (2.64 - 0.6))) / -(d1) );
        }
        speed = (float) GameObject.Find("Bot").GetComponent<RobotController>().mult;
		if( Single.IsNaN(speed) || Single.IsInfinity(speed)) {
			speed = 10;
		}

		double yV = velocity.z;
		double xV = velocity.x;
		double turn = 0;
		double sign = Math.Sign(xV);
		xV = Math.Abs(xV);
		
		if(Math.Abs(turn) > Math.PI / 2) {
			Debug.Log(Math.Abs(turn));
		}

		double result = (2.64 - 0.6);

		double error = result - eq(speed, angle, velocity, dist, turn);
		double i = 0;

		//speed = (float) Math.Sqrt(-(9.8 * dist * dist * (1 + (Math.Tan(angle) * Math.Tan(angle)))) / (2 * (result) - (2 * dist * Math.Tan(angle))));

		while (Math.Abs(error) > 0.01 && i < 10000) {
			i++;

			if (error > 0) {
				speed += speed / 2;
			}
			else {
				speed -= speed / 2;
			}

			error = result - eq(speed, angle, velocity, dist, turn);
		}

		GameObject.Find("Bot").GetComponent<RobotController>().mult = (float) speed;

        
		yV = Math.Abs(yV);
		xV = Math.Abs(xV);
		
		double vX = yV + (Math.Cos(angle) * speed);
        double initDrag = 0.2 * 1.225 * 0.0145564225 * Math.PI * vX * vX / 0.27;
        double time = dist / ( yV + ( speed * Math.Cos(angle) * Math.Cos(turn) ) );
		
		turn = Math.Acos(  Math.Abs(dist - (yV * time) ) / Math.Sqrt( (dist - (yV * time))*(dist - (yV * time)) + (xV*xV*time*time)) );
		Debug.Log( dist + "-" + yV + "*" + time);
		if(sign > 0) {
			turn *= -1;
		}

		error = result - eq(speed, angle, velocity, dist, turn);
		i = 0;
		while (Math.Abs(error) > 0.01 && i < 10000000) {
			i++;

			if (error > 0) {
				speed += speed / 2;
			}
			else {
				speed -= speed / 2;
			}

			error = result - eq(speed, angle, velocity, dist, Math.Abs(turn) );
		}

		// vX = velocity.z + Math.Cos(angle) * speed;
        // initDrag = 0.2 * 1.225 * 0.0145564225 * Math.PI * vX * vX / 0.27;
        // time = dist / (yV + (speed * Math.Cos(angle) * Math.Cos(turn)) );
		

		//speed *= (float) ( Math.Sqrt( (dist*dist) + (xV*xV*time*time)) / dist ) ;
		GameObject.Find("Bot").GetComponent<RobotController>().updateAngle(turn);
		
        speed += (float) (initDrag * time * time * 0.5 );

		Shoot(speed, angle, turn);
		Destroy(ball, 7.5f);
	}

	void FixedUpdate() {
		t += Time.deltaTime;
		double vX = ball.GetComponent<Rigidbody>().velocity.x;
		double vZ = ball.GetComponent<Rigidbody>().velocity.z;
		double vY = ball.GetComponent<Rigidbody>().velocity.y;
	
    	double dragX = 0.2 * 1.225 * 0.0145564225 * Math.PI * vX * vX / 0.27;
		double dragZ = 0.2 * 1.225 * 0.0145564225 * Math.PI * vX * vX / 0.27;
    	double dragY = 0.2 * 1.225 * 0.0145564225 * Math.PI * vY * vY / 0.27;
	
    	// double p = 1.225;
    	// double r = 0.12065;
    	// double w = angularVelo.value();
    	// double G = 2 * Math.PI * r * r * w;

    	// double F = p * vX * G * (0.2413);

		ball.GetComponent<Rigidbody>().velocity -= new Vector3(  (float) (dragX * Time.deltaTime), (float) (9.8 + (vY > 0 ? dragY: -dragY)) * Time.deltaTime, (float) (dragZ * Time.deltaTime) );
  	}

	public void Shoot(double speed, double angle, double turn) {
		//RobotController.angle += (float) turn;
		double a = Math.PI + RobotController.angle + turn;

		ball.GetComponent<Rigidbody>().velocity = new Vector3((float)(Math.Cos(angle) * Math.Sin(a) * speed), (float)(Math.Sin(angle) * speed), (float)(Math.Cos(angle) * Math.Cos(a) * speed));
		ball.GetComponent<Rigidbody>().velocity += new Vector3(RobotController.actualVelo.x, 0, RobotController.actualVelo.z);
	}

	double eq(double speed, double angle, Vector3 velo, double xDist, double turn) {
		if (xDist == 0) {
			xDist = 0.01;
		}

		return (speed * xDist * Math.Sin(angle) / (velo.z + (speed * Math.Cos(turn) * Math.Cos(angle) )) ) -  9.80665/2 * xDist * xDist / ((velo.z * velo.z) + (2*velo.z * speed * Math.Cos(turn) * Math.Cos(angle)) + (speed*Math.Cos(turn)*Math.Cos(angle)*speed*Math.Cos(turn)*Math.Cos(angle)) );
	}
}
