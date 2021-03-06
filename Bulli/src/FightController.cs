﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Player controls for the boss fight minigame
/// </summary>
public class FightController : MonoBehaviour
{
	/// <summary>
	/// Instance variablse
	/// </summary>
	public static int DeathsInTheGym = 0;
	public Sprite ryuSpriteIdle;
	public Sprite ryuSpriteKick;
	public Sprite ryuSpriteDead;
	private MorssiController mC;
	private bool bullIsDead = false;
	private SpriteRenderer spriteRendererBull;
	public SpriteRenderer youDiedSign;
	public GameObject lifeBarMorssi;
	public bool attackingAllowed;
	public float hitpoint = 45.0f;
	public float damage = 0.60f;

	void Start ()
	{
		///Get object references
		lifeBarMorssi = GameObject.Find ("lifeBarMorssiHealth");
		mC = FindObjectOfType (typeof(MorssiController)) as MorssiController;
		youDiedSign = GameObject.Find ("youDiedSign").GetComponent<SpriteRenderer> ();
		HideYouDied (); // hide the you died-sign on start
		spriteRendererBull = GetComponent<SpriteRenderer> ();
		if (spriteRendererBull.sprite == null) // if the sprite on spriteRenderer is null then
			spriteRendererBull.sprite = ryuSpriteIdle; // set the sprite to ryuSpriteIdle
	}

	void Update ()
	{
		BullAttackSystem (); // summon for bullAttackSystem
		///Checks if the boss is alive
		if (hitpoint <= 0) {
			bullKnockOutsMorssi ();
		}
	}

	/// <summary>
	/// Method for the player's attack system and changing sprites to reflect the state of the attacks
	/// </summary>
	public void BullAttackSystem ()
	{ 
		if (attackingAllowed) {
			if (Input.GetKey (KeyCode.Space) && hitpoint >= 0 && !bullIsDead) { 
				spriteRendererBull.sprite = ryuSpriteKick; // idle to kick
				healthReduceMorssi ();
			} else {
				spriteRendererBull.sprite = ryuSpriteIdle; // kick to idle
			}
		}
	}

	/// <summary>
	/// Method for allowing the player controls
	/// </summary>
	public void AllowControls ()
	{
		attackingAllowed = true;
	}

	/// <summary>
	/// Method for disabling the player controls
	/// </summary>
	public void DisableControls ()
	{
		attackingAllowed = false;
	}

	/// <summary>
	/// Method for doing damage to the boss
	/// </summary>
	public void healthReduceMorssi ()
	{
		if (Input.GetKeyDown (KeyCode.Space)) {
			lifeBarMorssi.gameObject.transform.localScale -= new Vector3 (damage, 0, 0);
			hitpoint -= damage;
		}
	}

	/// <summary>
	/// Method for running the boss' death script if the player wins
	/// </summary>
	public void bullKnockOutsMorssi ()
	{
		mC.morssiDeathScript ();
	}

	/// <summary>
	/// Method for the player to die and reload the scene
	/// </summary>
	public void bullDeathScript ()
	{
		spriteRendererBull.sprite = ryuSpriteDead; // idle to kick
		bullIsDead = true;
		BullController.amountOfDeaths++;
		SceneManager.LoadScene (10);
	}

	/// <summary>
	/// Shows the you died-sign.
	/// </summary>
	public void ShowYouDied ()
	{
		youDiedSign.enabled = true;
	}

	/// <summary>
	/// Hides the you died-sign.
	/// </summary>
	public void HideYouDied ()
	{
		youDiedSign.enabled = false;
	}

	/// <summary>
	/// Waiting coroutine between rounds
	/// </summary>

	public IEnumerator SecondRound ()
	{
		ShowYouDied ();
		DisableControls ();
		yield return new WaitForSeconds (3.0f);
		AllowControls ();
		HideYouDied ();
	}
}