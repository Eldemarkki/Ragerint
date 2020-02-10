using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class FollowPlayer : MonoBehaviour {

	public Transform player;
	public float speed;
	public float zoomSpeed;

	public bool follow = true;

	public Vector3 mapMiddle;

	public static FollowPlayer instance;

	Camera cam;

	void Awake(){
		instance = this;
		cam = GetComponent<Camera>();
		cam.orthographicSize = 7;
	}

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {

		if(Input.GetKeyDown(KeyCode.E))
			follow = !follow;

		if(follow){
			Vector3 l = Vector3.Lerp(transform.position, player.position, Time.deltaTime*speed);
			transform.position = new Vector3(l.x,l.y,-10);
			cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, 7, Time.deltaTime*zoomSpeed);

		}
		else{
			transform.position = Vector3.Lerp(transform.position, mapMiddle, Time.deltaTime*speed);
			cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, 13, Time.deltaTime*zoomSpeed);
		}
	}
}
