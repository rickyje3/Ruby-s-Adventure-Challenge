using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class RubyController : MonoBehaviour
{
    public float speed = 3.0f;
    public float timeInvincible = 2.0f;
    //true or false statement
    bool isInvincible;
    //stores amount of time ruby has left before reverting to vincible
    float invincibleTimer;
    public int maxHealth = 5;
    int currentHealth;
    //Allows you to only GET the health int from other scripts without making currentHealth public
    public int health { get { return currentHealth; } }
    Rigidbody2D rigidbody2d;
    float horizontal;
    float vertical;
    Animator animator;
    //stores the direction ruby's looking so it can provide a direction
    Vector2 lookDirection = new Vector2(1, 0);
    public GameObject projectilePrefab;
    Projectile projectile;
    AudioSource audioSource;
    public AudioClip throwSound;
    public AudioClip collectedClip;
    public AudioClip hitSound;
    public AudioClip winMusic;
    public AudioClip loseMusic;
    public GameObject backgroundMusic;
    public GameObject healthEffect;
    public GameObject hitEffect;
    public ParticleSystem healthIncrease;
    public ParticleSystem healthDecrease;
    public TextMeshProUGUI FixedRobots;
    public TextMeshProUGUI CogText;
    public GameObject gameOverText;
    public GameObject winText;
    public GameObject loseText;
    int score = 0;
    int cogs = 4;
    private TextMeshProUGUI scoreText;
    bool gameOver;
    public static int level;


    // Start is called before the first frame update
    void Start()
    {
        //Tells Unity to give the rigidbody to whoever it's attached to in unity
        rigidbody2d = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        //Sets health to max at game start
        currentHealth = maxHealth;
        SetScoreText();
        SetCogText();
        audioSource = GetComponent<AudioSource>();
        winText.SetActive(false);
        loseText.SetActive(false);
        gameOverText.SetActive(false);
        backgroundMusic.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        //Stores a variable when you press the key linked to horizontal
        horizontal = Input.GetAxis("Horizontal");
        vertical = Input.GetAxis("Vertical");

        //stores the movement in a vector2 called move, handles x amd y at the same time
        Vector2 move = new Vector2(horizontal, vertical);

        //checks to see whether move.x or move.y isn't equal to 0
        if (!Mathf.Approximately(move.x, 0.0f) || !Mathf.Approximately(move.y, 0.0f))
        {
            lookDirection.Set(move.x, move.y);
            //Normalize makes the length = 1
            lookDirection.Normalize();
        }

        animator.SetFloat("Look X", lookDirection.x);
        animator.SetFloat("Look Y", lookDirection.y);
        animator.SetFloat("Speed", move.magnitude);

        if (isInvincible)
        {
            //if invincible you remove deltatime from the timer and when that reaches 0 ruby is no longer invincible
            invincibleTimer -= Time.deltaTime;
            if (invincibleTimer < 0)
                isInvincible = false;
        }

        if (Input.GetKeyDown(KeyCode.C))
        {
            if (cogs >= 1)
            {
                Launch();
                cogs -= 1;
                SetCogText();
                audioSource.PlayOneShot(throwSound);
            }
        }

        if (Input.GetKeyDown(KeyCode.X))
        {
            //tests if ruby is facing up, max distance away from subject is 1.5, only tests with the layer npc
            RaycastHit2D hit = Physics2D.Raycast(rigidbody2d.position + Vector2.up * 0.2f, lookDirection, 1.5f, LayerMask.GetMask("NPC"));
            //if you hit a collider, does nothing
            if (hit.collider != null)
            {
                if (score == 4)
                {
                    SceneManager.LoadScene("Scene2");
                    level = 2;
                }
                //checks if we have a hit then tries to find the nonplayercharacter script on the object the raycast hit
                //if the script exists on that object the dialog will display
                NonPlayerCharacter character = hit.collider.GetComponent<NonPlayerCharacter>();
                if (character != null)
                {
                    character.DisplayDialog();
                }
            }
        }

        if (Input.GetKey(KeyCode.R))
        {
            if (gameOver == true)
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex); // this loads the currently active scene
            }
        }
    }

    void FixedUpdate()
    {
        //Storing the position in the rigidbody component inside a Vector2 variable.
        Vector2 position = rigidbody2d.position;
        //Multiplies the amount the gameobject moves by the value of the axis. Left key moves it left right key right
        position.x = position.x + speed * horizontal * Time.deltaTime;
        position.y = position.y + speed * vertical * Time.deltaTime;
        //Relays the info back to the rigidbody component so it can execute the top 2 lines
        rigidbody2d.MovePosition(position);
    }

    public void ChangeHealth(int amount)
    {
        if (amount < 0)
        {
            animator.SetTrigger("Hit");
            audioSource.PlayOneShot(hitSound);
            GameObject healthDecrease = Instantiate(hitEffect, rigidbody2d.position + Vector2.up * 0.5f, Quaternion.identity);
            if (isInvincible)
                return;
            //if invincible is true the timer turns on 
            isInvincible = true;
            invincibleTimer = timeInvincible;
        }

        if (amount > 0)
        {
            GameObject healthIncrease = Instantiate(healthEffect, rigidbody2d.position + Vector2.up * 0.5f, Quaternion.identity);
        }

        //clamp ensures that health doesn't go below 0 or the parameter for maxHealth (5)
        currentHealth = Mathf.Clamp(currentHealth + amount, 0, maxHealth);
        //Gives the ratio between current and max health (float) makes C# think maxhealth is a floating point value
        UIHealthBar.instance.SetValue(currentHealth / (float)maxHealth);

        if (currentHealth == 0)
        {
            loseText.SetActive(true);
            gameOver = true;
            speed = 0;
            backgroundMusic.SetActive(false);
            PlaySound(loseMusic);
        }
    }

    void Launch()
    {
        //instantiate takes an object as the first parameter and creates a copy at the position in the second parameter, then rotate in the third.
        //the prefab is being copied, placed at the position of the rigidbody, and rotated of quaternion.identity
        //quaternions are math operators that express rotation. quaternion.identity means no rotation
        GameObject projectileObject = Instantiate(projectilePrefab, rigidbody2d.position + Vector2.up * 0.5f, Quaternion.identity);

        Projectile projectile = projectileObject.GetComponent<Projectile>();
        projectile.Launch(lookDirection, 300);

        animator.SetTrigger("Launch");
    }

    void OnTriggerEnter2D(Collider2D cog)
    {
        if (GameObject.FindWithTag("Cog"))
        {
            cogs += 4;
            SetCogText();
            Destroy(cog.gameObject);
            PlaySound(collectedClip);
        }
    }

    void SetCogText()
    {
        CogText.text = "Cogs: " + cogs.ToString();
    }

    public void PlaySound(AudioClip clip)
    {
        audioSource.PlayOneShot(clip);
    }

    void SetScoreText()
    {
        FixedRobots.text = "Robots Fixed: " + score.ToString() + "/4";

        if (score == 4)
        {
            gameOverText.SetActive(true);
            {
                if (level == 2)
                {

                    if (score == 4)
                    {
                        winText.SetActive(true);
                        gameOver = true;
                        gameOverText.SetActive(false);
                        speed = 0;
                        backgroundMusic.SetActive(false);
                        audioSource.Stop();
                        PlaySound(winMusic);
                    }

                }
            }
        }
    }

    public void ChangeScore(int amount)
    {
        score += amount;
        SetScoreText();
    }
}
