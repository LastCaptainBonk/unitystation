﻿using UnityEngine;
using System.Collections;

public class SpaceTrigger : MonoBehaviour {

	private bool isSpaced = false;

	//TEMP MOCK UP

	void OnTriggerEnter2D (Collider2D coll){
	
	
		if (PlayerScript.playerControl != null) {
		
			if (coll.gameObject == PlayerScript.playerControl.gameObject && !isSpaced) {
			
				isSpaced = true;
				PlayerScript.playerControl.isSpaced = true;
			
			}
		
		
		}
	
	}
}
