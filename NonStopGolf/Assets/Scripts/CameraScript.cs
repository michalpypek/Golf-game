using UnityEngine;
using System.Collections;

public class CameraScript : MonoBehaviour
{
    GameObject ball;
    GameObject hole;

    Vector3 mid;

	void Start ()
    {
        ball = GameObject.Find("Ball");
        hole = GameObject.Find("Hole");
	}

    void FixedUpdate()
    {
        float distance = Vector2.Distance(ball.transform.position, hole.transform.position);
        
        mid.x = (ball.transform.position.x + hole.transform.position.x) /2;
        mid.y = (ball.transform.position.y + hole.transform.position.y) / 2;
        mid.z = -1;
        transform.position = mid;
        //Mathf.Clamp(distance, 5, Mathf.Infinity);

        Camera.main.orthographicSize = Mathf.Clamp(distance + 2, 7, Mathf.Infinity);
    }
	
}
