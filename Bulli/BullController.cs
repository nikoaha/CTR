﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Threading;
using System;


public class BullController : MonoBehaviour {
	//instanssimuuttujat
	private Rigidbody2D bull;
	private static int amountOfDeaths;
	private Animator anim;
	public AudioClip jumpSound;
	public AudioClip deathSound;
	public SignController signs;
	private bool facingLeft = false;
	private bool playerMoving;
	private float maxSpeed = 140f;
	private AudioSource audioSystem;
	private Vector2 vertical = new Vector2 (120f,0f);
	//hyppiminen

	private bool isJumping = false;
	public bool movementAllowed;

	void Start(){
		//Haetaan objektiviittaukset
		signs = FindObjectOfType(typeof(SignController)) as SignController;	
		bull = GetComponent<Rigidbody2D>();
		anim = GetComponent<Animator> ();
		audioSystem = GetComponent<AudioSource> ();


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
				Jump ();
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

	//Jumping mechanics
	private Vector2 jump = new Vector2 (0f, 150f);
	public void Jump ()
	{
		audioSystem.clip = jumpSound;

		if (Input.GetKeyDown (KeyCode.Space) && isJumping == false) {
			audioSystem.Play ();
			bull.AddForce (29 * jump, ForceMode2D.Impulse);
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
		audioSystem.clip = deathSound;
		audioSystem.Play ();
		Vector3 deathVector = new Vector3 (100, 10, 0);
		bull.transform.position = deathVector;
		amountOfDeaths++;
		StartCoroutine(signs.DeathSign());
	}
		
	//hypyn nollaaminen ja kuoleminen osuttaessa viholliseen
	void OnCollisionEnter2D(Collision2D vaga) {
		isJumping = false;

		if (vaga.gameObject.tag == "vagabond") {
			Die ();
		}
	}

	public void DisableMovement(){
		movementAllowed = false;
	}

	public void EnableMovement(){
		movementAllowed = true;
	}

}