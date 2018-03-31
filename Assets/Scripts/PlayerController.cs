﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour {

	public float speed = 20.0f;
	public int health = 100;
	private Rigidbody2D rigidBody;
	private bool right = true;
	[SerializeField]
	private Animator animator;
	public float horizontal;
	public float vertical;
	[SerializeField]
	private Text healthText;
	public EnumBreadType breadType = EnumBreadType.NORMAL;
	private bool infected = false;

	public GameObject defaultBread;
	public GameObject infectedBread;

	public GameObject gameOverPanel;

	void Start () {
		rigidBody = GetComponent<Rigidbody2D> ();
	}

	void Update () {
		horizontal = Input.GetAxis ("Horizontal");
		vertical = Input.GetAxis ("Vertical");

		if (GameObject.Find ("Events").GetComponent<Events> ().weatherType == Events.EnumWeatherType.FOGGY) {
			horizontal *= -1;
			vertical *= -1;
		}

		if (vertical != 0f || horizontal != 0f) {
			Move ();
			animator.SetBool ("walking", true);
		} else if (animator.GetBool ("walking")) {
			animator.SetBool ("walking", false);
		}
	}

	void Move () {
		Vector2 force = rigidBody.transform.position;

		force.x += horizontal * Time.deltaTime * speed;
		force.y += vertical * Time.deltaTime * speed;
		rigidBody.MovePosition (force);

		if (horizontal > 0f && !right) {
			Flip ();
			right = true;
		} else if (horizontal < 0f && right) {
			Flip ();
			right = false;
		}
	}

	void Flip () {
		Vector3 theScale = transform.localScale;
		theScale.x *= -1;
		transform.localScale = theScale;
	}

	public void Damage (int damage) {
		health -= damage;
		if (health <= 50) {
			if (breadType == EnumBreadType.EATED) {
				breadType = EnumBreadType.EATED_INFECTED;
			} else {
				breadType = EnumBreadType.EATED;
			}
		} 
		if (health <= 0f) {
			gameOverPanel.SetActive (true);
			Destroy (gameObject);
			Destroy (GameObject.Find ("ScoreText"));
			//Time.timeScale = 0;
		}
		healthText.text = "Health: " + health + "%";
	}

	public void Infect () {
		if (breadType == EnumBreadType.EATED) {
			breadType = EnumBreadType.EATED_INFECTED;
		} else {
			breadType = EnumBreadType.INFECTED;
		}
		speed -= 2f;
		Invoke ("Pure", 15f);
	}

	void Pure () {
		if (breadType == EnumBreadType.EATED_INFECTED) {
			breadType = EnumBreadType.EATED;
		}
		breadType = EnumBreadType.NORMAL;
		speed = 10f;
	}

	public enum EnumBreadType {
		NORMAL,
		EATED,
		INFECTED,
		EATED_INFECTED,
		DRYED,
		DRYED_EATED
	}
}
