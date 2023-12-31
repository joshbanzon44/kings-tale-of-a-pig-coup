using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using TMPro.EditorUtilities;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    public float movementSpeed = 0.5f;
    public float jumpPower = 150f;

    public GameObject liveBar;
    public TMP_Text diamondText;
    public GameObject whiteScreen;
    public TMP_Text deadText;

    private float lateralMovement = 0;
    private bool isJumping = false;
    private bool canJump = true;
    private bool firing = false;
    private bool isFiring = false;
    private bool canFire = true;
    private bool cantMove = false;
    private bool dead = false;

    //References
    Rigidbody2D rb;
    SpriteRenderer spriteRenderer;
    Animator animator;
    AudioSource audioSource;
    GameObject collisionObj;
    GameObject doorObj;

    private int hearts = 3;
    public int Heart
    {
        get { return hearts; }
        set 
        { 
            hearts = value;
            SpriteRenderer heart = liveBar.transform.GetChild(value).GetComponent<SpriteRenderer>();
            heart.color = Color.clear;
            if (value == 0)
            {
                animator.SetTrigger("Die");
                dead = true;
                liveBar.SetActive(false);
                Coroutine c = StartCoroutine(FadeToWhite());
                Invoke("Die", 4);
            }
        }
    }

    private int diamonds = 0;
    public int Diamond
    {
        get { return diamonds; }
        set 
        { 
            diamonds = value;
            diamondText.text = $"{diamonds}";
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        //Get components
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = rb.GetComponent<SpriteRenderer>();
        animator = transform.GetComponentInChildren<Animator>();
        audioSource = GetComponent<AudioSource>();

        Diamond = PlayerPrefs.GetInt("Diamond", 0);
        jumpPower = 150 + Diamond*10;
        Color color = whiteScreen.GetComponent<SpriteRenderer>().color;
        color.a = 0;
        whiteScreen.GetComponent<SpriteRenderer>().color = color;
        color = deadText.color;
        color.a = 0;
        deadText.color = color;

        liveBar.SetActive(true);
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
        if (dead || cantMove)
        {
            rb.velocity = Vector3.zero;
            return;
        }

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
            Vector3 feetPosition = transform.GetChild(0).transform.GetChild(0).position;
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
            firing = true;
            Invoke("canMove", 0.3f);
            Invoke("stopFiring", 0.5f);
            animator.SetTrigger("Fire");

            Invoke("allowFire", 0.75f);
        }
        
        if (firing)
        {
            Vector3 swingPosition = transform.GetChild(0).transform.GetChild(1).position;
            Collider2D[] colliders = Physics2D.OverlapCircleAll(swingPosition, 0.1f);
            for (int i = 0; i < colliders.Length; i++)
            {
                if (colliders[i].gameObject.tag != "Enemy")
                    continue; //Move to next loop iteration
                collisionObj = colliders[i].gameObject;
                Animator an1 = collisionObj.gameObject.GetComponent<Animator>();
                an1.SetTrigger("Die");
                CapsuleCollider2D cc = collisionObj.gameObject.GetComponent<CapsuleCollider2D>();
                cc.enabled = false;
                Invoke("Delete", 1);
                break;  //Stop loop after hit
            }
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

        if (rb.velocity.magnitude > 0.001f && rb.velocity.y > 0.001f)
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
    private void stopFiring()
    {
        firing = false;
    }
    private void canMove()
    {
        cantMove = false;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        //Switch statement depending on tag of object touched
        switch (collision.gameObject.tag)
        {
            case "Enemy":
                if (Heart == 0 || cantMove)
                {
                    return;
                }
                cantMove = true;
                Invoke("canMove", 0.5f);
                Heart--;
                Animator an2 = collision.gameObject.GetComponent<Animator>();
                an2.SetTrigger("Attack");
                Debug.Log("Enemy collision");
                animator.SetTrigger("Hit");
                rb.velocity = new Vector2(0, 0);

                break;
            default:
                break;
        }
    }

    void Delete()
    {
        Destroy(collisionObj.gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //Switch statement depending on tag of object touched
        switch (collision.gameObject.tag)
        {
            case "Powerup":
                Debug.Log("Powerup collision");
                Diamond++;
                Invoke("Delete", 1);
                //Play particle system
                ParticleSystem ps = collision.gameObject.GetComponent<ParticleSystem>();
                ps.Play();
                Animator an = collision.gameObject.GetComponent<Animator>();
                an.SetTrigger("Hit");
                //Clear sprite
                SpriteRenderer sr = collision.gameObject.GetComponent<SpriteRenderer>();
                sr.color = Color.clear;
                //Disable collider
                CircleCollider2D cc = collision.gameObject.GetComponent<CircleCollider2D>();
                cc.enabled = false;
                //Play audio
                AudioSource collisionAudio = collision.gameObject.GetComponent<AudioSource>();
                collisionAudio.Play();
                jumpPower = 150 + Diamond*10;
                break;
            case "ExitDoor":
                rb.velocity = Vector3.zero;
                doorObj = collision.gameObject;
                Invoke("NextLevel", 2);
                cantMove = true;
                animator.SetTrigger("Leave");
                Animator an3 = doorObj.GetComponent<Animator>();
                an3.SetTrigger("Open");

                break;
            default:
                break;
        }

        

    }

    void NextLevel()
    {
        PlayerPrefs.SetInt("Diamond", Diamond);
        PlayerPrefs.SetInt("LevelNum", doorObj.GetComponent<ChangeLevel>().nextLevelNum);
        PlayerPrefs.SetString("Level", doorObj.GetComponent<ChangeLevel>().nextLevel);
        PlayerPrefs.Save();
        SceneManager.LoadScene(doorObj.GetComponent<ChangeLevel>().nextLevel);
    }

    //Player dies
    void Die()
    {
        SceneManager.LoadScene("Menu");
    }

    //Coroutine to slowly turn screen white when player dies
    IEnumerator FadeToWhite()
    {
        SpriteRenderer screenRenderer = whiteScreen.GetComponent<SpriteRenderer>();
        
        Color c = screenRenderer.color;
        Color c2 = new Color (1, 0, 0, 0);

        float opacity = 0f;
        for (opacity = 0f; opacity <= 1; opacity += 0.01f)
        {
            c.a = opacity;
            c2.a = opacity;
            screenRenderer.color = c;
            deadText.color = c2;
           
            yield return null;            
        }
    }
}
