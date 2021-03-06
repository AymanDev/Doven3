﻿using System;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    public float speed = 20.0f;
    public int health = 100;
    private Rigidbody2D rigidBody;
    private bool right = true;
    [SerializeField] private Animator animator;
    public float horizontal;
    public float vertical;
    [SerializeField] private Text healthText;
    public EnumBreadType breadType = EnumBreadType.NORMAL;
    private bool infected = false;

    public GameObject gameOverPanel;

    public int resistCharges;

    [SerializeField] private GameObject helmetObjectArmor;

    private void Start()
    {
        rigidBody = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        var joystickObject = GameObject.Find("Joystick");
        horizontal = Input.GetAxis("Horizontal");
        vertical = Input.GetAxis("Vertical");

        if (joystickObject != null)
        {
            var joystick = joystickObject.GetComponent<Joystick>();
            if (Math.Abs(joystick.horizontal) > 0f)
            {
                horizontal = joystick.horizontal;
            }

            if (Math.Abs(joystick.vertical) > 0f)
            {
                vertical = joystick.vertical;
            }
        } /*else {
			horizontal = Input.GetAxis ("Horizontal");
			vertical = Input.GetAxis ("Vertical");
		}*/

        if (GameObject.Find("Events").GetComponent<Events>().weatherType == Events.EnumWeatherType.FOGGY)
        {
            horizontal *= -1;
            vertical *= -1;
        }

        helmetObjectArmor.SetActive(resistCharges > 0);

        if (Math.Abs(vertical) > 0f || Math.Abs(horizontal) > 0f)
        {
            Move();
            animator.SetBool("walking", true);
            transform.FindChild("Particle System").gameObject.SetActive(true);
        }
        else if (animator.GetBool("walking"))
        {
            animator.SetBool("walking", false);
            transform.FindChild("Particle System").gameObject.SetActive(false);
        }
    }

    private void Move()
    {
        var force = rigidBody.transform.position;

        force.x += horizontal * Time.deltaTime * speed;
        force.y += vertical * Time.deltaTime * speed;
        rigidBody.MovePosition(force);
        if (horizontal > 0f && !right)
        {
            Flip();
            right = true;
        }
        else if (horizontal < 0f && right)
        {
            Flip();
            right = false;
        }
    }

    private void Flip()
    {
        var theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
    }

    public void Damage(int damage, bool resistable)
    {
        if (damage > 0)
        {
            if (resistable && resistCharges > 0)
            {
                resistCharges--;
            }
            else
            {
                health -= damage;
            }
         
            if (health <= 0f)
            {
                gameOverPanel.SetActive(true);
                Destroy(gameObject);
                Destroy(GameObject.Find("HealthPanel"));
                Destroy(GameObject.Find("TimePanel"));
            }
            Camera.main.GetComponent<CameraShake>().shakeDuration = 0.4f;
        }

        if (health <= 50)
        {
            breadType = breadType == EnumBreadType.INFECTED ? EnumBreadType.EATED_INFECTED : EnumBreadType.EATED;
        }
        else if (breadType != EnumBreadType.INFECTED)
        {
            if (breadType == EnumBreadType.EATED_INFECTED)
            {
                breadType = EnumBreadType.INFECTED;
            }

            if (breadType == EnumBreadType.EATED)
            {
                breadType = EnumBreadType.NORMAL;
            }
        }
        
        healthText.text = "Health: " + health + "%";
    }

    public void Infect()
    {
        if (breadType == EnumBreadType.INFECTED && breadType == EnumBreadType.EATED_INFECTED) return;
        breadType = breadType == EnumBreadType.EATED ? EnumBreadType.EATED_INFECTED : EnumBreadType.INFECTED;

        speed = 7f;
        Damage(0, false);
        Invoke("Pure", 16f);
    }

    private void Pure()
    {
        if (breadType == EnumBreadType.EATED_INFECTED)
        {
            breadType = EnumBreadType.EATED;
        }

        breadType = EnumBreadType.NORMAL;
        speed = 10f;
        Damage(0, false);
    }

    public enum EnumBreadType
    {
        NORMAL,
        EATED,
        INFECTED,
        EATED_INFECTED,
        DRYED,
        DRYED_EATED
    }
}