using System.Collections;
using System.Collections.Generic;
//using System.Diagnostics;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerMovement : MonoBehaviour
{
    /// <summary>
    /// Contains tunable parameters to tweak the player's basic movement.
    /// </summary>
    [System.Serializable] // allows the values to be seen in the editor without it, it will not show up
    public struct Stats
    {
    

        [Tooltip("How fast the player runs.")]
        public float speed;

        [Tooltip("How high the player jumps.")]
        public float jumpForce;

        [Tooltip("Whether the player is allowed to move or not.")]
        public bool canMove;

        [Tooltip("When the player is allowed to jump or not.")]
        public bool canJump;

    }

    [SerializeField]
    private float maxHealth = 100f;
    private float currentHealth;

    private float heartValue = 25f;
    public Slider healthBar;

    public void ModifyHealth(float amount, string source)
    {
        currentHealth += amount;
        currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);
        Debug.Log("Current health: " + currentHealth + " Health modified by " + amount + " by: " + source);
    }
    
    public Stats playerStats;

    [Tooltip("The script that will play the player's sound effects.")]
    public SoundManager soundManager;

    [Tooltip("Which layer allows the player to jump.")]
    public LayerMask groundLayer;

    [Tooltip("The transform that detects what layer the player is on.")]
    public Transform groundCheckL, groundCheckR;

    [Tooltip("The transform that the player's directional movement will be based upon.")]
    public Transform mainCamera;

    private float moveX, moveY;
    private float facing;
    private Rigidbody rb;
    
    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        currentHealth = maxHealth;
    }

    private void Update()
    {
        if (playerStats.canMove == true)
        {

            // maps movement onto WASD keys and arrow keys
            moveX = Input.GetAxis("Horizontal");
            moveY = Input.GetAxis("Vertical");  

            // creates linecasts that check for the ground layer, allowing the player to jump
            bool hitL = Physics.Linecast(new Vector3(groundCheckL.position.x, transform.position.y + 1, transform.position.z), groundCheckL.position, groundLayer);
            bool hitR = Physics.Linecast(new Vector3(groundCheckR.position.x, transform.position.y + 1, transform.position.z), groundCheckR.position, groundLayer);
            Debug.DrawLine(new Vector3(groundCheckL.position.x, transform.position.y + 1, transform.position.z), groundCheckL.position, Color.red);
            Debug.DrawLine(new Vector3(groundCheckR.position.x, transform.position.y + 1, transform.position.z), groundCheckR.position, Color.red);

            if (hitL || hitR)
            {
                playerStats.canJump = true;
            }
            else
            {
                playerStats.canJump = false;
            }

            if (playerStats.canJump)
            {
                if (Input.GetButtonDown("Jump"))
                {
                    Jump();
                }
            }
        }

        if (currentHealth < heartValue)
        {
            Reset();
        }

        healthBar.value = currentHealth;
    }

    private void FixedUpdate()
    {
        if (playerStats.canMove == true)
        {
            
            // directional movement is wrapped around which way the camera is facing
            Vector3 movement = ((mainCamera.right * moveX) * playerStats.speed) + ((mainCamera.forward * moveY) * playerStats.speed);
            rb.velocity = new Vector3(movement.x, rb.velocity.y, movement.z);

            // player faces the direction they are moving towards
            if (movement.x != 0 && movement.z != 0)
            {
                facing = Mathf.Atan2(movement.x, movement.z) * Mathf.Rad2Deg;
            }
            rb.rotation = Quaternion.Euler(0, facing, 0);  
        }
    }
    
    private void Jump()
    {
        playerStats.canJump = false;
        soundManager.PlayJumpSound();
        rb.AddForce(Vector3.up * playerStats.jumpForce);
    }

    private void Reset() //for if the player health is less then on heart
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(currentSceneIndex);
    }
}