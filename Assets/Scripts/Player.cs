using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    public float movementSpeed = 0.5f;
    public float jumpPower = 10f;

    private float lateralMovement = 0;
    private bool isJumping = false;
    private bool canJump = true;
    private bool isFiring = false;
    private bool canFire = true;

    //References
    Rigidbody2D rb;
    SpriteRenderer spriteRenderer;
    Animator animator;
    AudioSource audioSource;



    // Start is called before the first frame update
    void Start()
    {
        //Get components
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = rb.GetComponent<SpriteRenderer>();
        animator = rb.GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();

    }

    // Update is called once per frame
    void Update()
    {
        //Exit game if cancel button pushed
        if (Input.GetAxis("Cancel") == 1)
        {
            SceneManager.LoadScene("Menu");
        }

        //Get firing input
        isFiring = Input.GetAxis("Fire1") > 0 ? true : false;

        //Get lateral input as fast as you can
        lateralMovement = Input.GetAxis("Horizontal");

        //Get jump movement
        isJumping = Input.GetAxis("Jump") > 0 ? true : false;
    }

    private void FixedUpdate()
    {
        //Update location based on input
        rb.velocity = new Vector2(lateralMovement * movementSpeed * Time.fixedDeltaTime, rb.velocity.y);

        //Flip the sprite based on direction of movement
        if (rb.velocity.x > 0)
        {
        
            
            transform.localScale = new Vector3(1,1,1);
        }
        else if (rb.velocity.x < 0)
        {
            transform.localScale = new Vector3(-1, 1, 1);
        }

        //Jumping mechanic
        //Only jump if feet on ground and after delay
        if (isJumping && canJump)
        {
            //Check if our feet on ground
            canJump = false;
            Invoke("allowJump", 0.5f);
            Vector3 feetPosition = transform.GetChild(0).position;
            Collider2D[] colliders = Physics2D.OverlapCircleAll(feetPosition, 0.02f);
            for (int i = 0; i < colliders.Length; i++)
            {
                if (colliders[i].gameObject == gameObject)
                    continue; //Move to next loop iteration
                Debug.Log("Jumping");
                rb.velocity = new Vector2(rb.velocity.x, 0);
                rb.AddForce(Vector2.up * jumpPower);
                audioSource.Play();
                break;  //Stop loop after movement
            }
        }

        //Swing mechanic
        //Only swing every second and on click
        if (isFiring && canFire)
        {
            canFire = false;
            animator.SetTrigger("Fire");

            Invoke("allowFire", 0.75f);
        }

        //Track if we are moving or not in order to update the animation
        if (rb.velocity.magnitude > 0.0001f)
        {
            animator.SetBool("isMoving", true);
        }
        else
        {
            animator.SetBool("isMoving", false);
        }

        if (rb.velocity.magnitude > 0.0001f && rb.velocity.y > 0.0001f)
        {
            animator.SetBool("isJumping", true);
        }
        else
        {
            animator.SetBool("isJumping", false);
        }

    }


    private void allowJump()
    {
        canJump = true;
    }
    private void allowFire()
    {
        canFire = true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {

        //Switch statement depending on tag of object touched
        switch (collision.gameObject.tag)
        {
            case "Powerup":
                Debug.Log("Powerup collision");
            
                break;
            default:
                break;
        }

        void Die()
        {
            Destroy(collision.gameObject);
            Debug.Log("object destroyed");
        }
    }
}
