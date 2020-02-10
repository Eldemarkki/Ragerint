using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpikeWall : MonoBehaviour {

	Animation anim;
	BoxCollider2D col;

	// Use this for initialization
	void Start () {
		anim = GetComponent<Animation>();
		col = GetComponentInChildren<BoxCollider2D>();
		StartCoroutine(OpenAndClose());

	}
	
	IEnumerator OpenAndClose(){
		anim.Play("SpikeWallAnimationClose");
		col.enabled = true;
		yield return new WaitForSecondsRealtime(5f);
		
		anim.Play("SpikeWallAnimationOpen");
		col.enabled=false;
		yield return new WaitForSecondsRealtime(10f);
		
		StartCoroutine(OpenAndClose());
		

	}
}
