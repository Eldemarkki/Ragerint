using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spike : MonoBehaviour {
	
	void OnCollisionEnter2D(Collision2D col){
        if (col.gameObject.tag == "Player")
        {
            if(!col.gameObject.GetComponent<BallController>().isWon)
                col.gameObject.GetComponent<BallController>().Die();
        }
	}
}
