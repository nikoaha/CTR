﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Threading;
using System;


public class BullTrainController : MonoBehaviour {
	//instanssimuuttujat
	private Rigidbody2D bull;
	private static int amountOfDeaths;
	private Animator anim;
	public SignTrainController signs;
	private bool facingLeft = false;
	private bool playerMoving;
	private float maxSpeed = 700f;
	private Vector2 vertical = new Vector2 (600f,0f);
	//hyppiminen
	private Vector2 jumppi = new Vector2 (0f, 750f);
	private bool isJumping = false;
	public bool movementAllowed;

	void Start(){
		//Haetaan objektiviittaukset
		signs = FindObjectOfType(typeof(SignTrainController)) as SignTrainController;	
		bull = GetComponent<Rigidbody2D>();
		anim = GetComponent<Animator> ();
	}


	void Update(){
		//bullin maksiminopeuden valvonta ja säätö	
		if (bull.velocity.magnitude > maxSpeed) {
			bull.velocity = bull.velocity.normalized * maxSpeed;
		}
		if (movementAllowed) {
			//liikkuminen
			if (Input.GetKey (KeyCode.LeftArrow)) {
				bull.AddForce (-30 * vertical, ForceMode2D.Force);
				anim.SetFloat ("speed", Mathf.Abs (bull.velocity.x));
			}
			if (Input.GetKey (KeyCode.RightArrow)) {
				anim.SetFloat ("speed", Mathf.Abs (bull.velocity.x));
				bull.AddForce (30 * vertical, ForceMode2D.Force);
			}
			if (Input.GetKeyDown (KeyCode.Space) && isJumping == false) {
				Pomppaus ();
			}
			if (Input.GetKey (KeyCode.DownArrow)) {
				bull.AddForce (-20 * jumppi, ForceMode2D.Force);
			}
		}
	}
	void FixedUpdate(){
		//Tässä pyöritetään animaatiota kulkusuunnasta riippuen
		float h = Input.GetAxis ("Horizontal");
		if (h < 0 && !facingLeft)
			reverseImage ();
		else if (h > 0 && facingLeft)
			reverseImage ();
	}

	//Hyppymekaniikka
	public void Pomppaus ()
	{

		if (Input.GetKeyDown (KeyCode.Space) && isJumping == false) {
			bull.AddForce (29 * jumppi, ForceMode2D.Impulse);
			isJumping = true; 
		} 
	}
	//Metodi, jolla Bullin juoksuanimaatio käännetään y-akselin suhteen peilikuvaksi
	void reverseImage()
	{
		// Get and store the local scale of the RigidBody2D
		Vector2 theScale = bull.transform.localScale;
		facingLeft = !facingLeft;
		// Flip it around the other way
		theScale.x *= -1;
		bull.transform.localScale = theScale;
	}

	//kuolemismekaniikka
	public void Die(){	

		Vector3 deathVector = new Vector3 (100, 10, 0);
		bull.transform.position = deathVector;
		amountOfDeaths++;
		StartCoroutine(signs.DeathSign());
	}

	//hypyn nollaaminen ja kuoleminen osuttaessa viholliseen


	void OnCollisionEnter2D(Collision2D collider) {
		if (collider.gameObject.tag == "roof") {
			isJumping = true;
		}
		isJumping = false;
		if (collider.gameObject.tag == "police") {
			DieOnTheTrain ();
		}

	}

	public void DieOnTheTrain (){
		SceneManager.LoadScene (6);
		signs.DisableSignCoroutines ();
		EnableMovement ();

	}

	public void DisableMovement(){
		movementAllowed = false;
	}

	public void EnableMovement(){
		movementAllowed = true;
	}

}