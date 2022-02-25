using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerManager : MonoBehaviour
{
    private bool oxygen = true;
    private bool isAlive = true;

    [SerializeField] private int healthPointValue;
    [SerializeField] private int coinValue;
    [SerializeField] private int treasureValue;
    [SerializeField] private int enemyValue;

    [SerializeField] private float gaspValue;
    [SerializeField] private float breathValue;

    [SerializeField] private Text coinText;

    [SerializeField] private Text scoreText;
    [SerializeField] private Text scoreValueText;

    [SerializeField] private Slider healthBar;

    [SerializeField] private GameObject startScreen;
    [SerializeField] private GameObject endScreen;

    private int coin = 0;

    static private bool firstStart = true;
    static private int coinMax = 0;

    private void Start()
    {
        if (firstStart)
        {
            firstStart = false;
        }
        else
        {
            startScreen.SetActive(false);
        }
    }

    private void Update()
    {
        if(isAlive) Breath();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "HollEnter")
        {
            oxygen = false;
        }
        else if (other.gameObject.tag == "HollExit")
        {
            oxygen = true;
        }
        else if (other.gameObject.tag == "Health")
        {
            healthBar.value += 1000;
            Destroy(other.gameObject);
            CoinRecount(0);
        }
        else if (other.gameObject.tag == "Coin")
        {
            Destroy(other.gameObject);
            CoinRecount(1);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "enemy")
        {
            Death();
        }
        else if (collision.gameObject.tag == "Treasure")
        {
            Destroy(collision.gameObject);
            CoinRecount(2);
        }
    }

    private void Breath()
    {
        if (oxygen)
        {
            healthBar.value += breathValue;
        }
        else
        {
            healthBar.value -= gaspValue;
        }

        if (healthBar.value <= 0)
        {
            Death();
        }
    }

    private void Death()
    {
        isAlive = false;
        if (coin > coinMax)
        {
            coinMax = coin;
            scoreText.text = "New Record";
            scoreValueText.text = "" + coinMax;
        }
        else
        {
            scoreText.text = "Score";
            scoreValueText.text = "" + coin;
        }
        
        endScreen.SetActive(true);
    }

    public void CoinRecount(int value) 
    {
        if (value == 0) 
        {
            coin += healthPointValue;
        }
        else if (value == 1)
        {
            coin += coinValue;
        }
        else if (value == 2)
        {
            coin += treasureValue;
        }
        else if (value == 3)
        {
            coin += enemyValue;
        }
        coinText.text = "" + coin;
    }

    public void StartGame()
    {
        startScreen.SetActive(false);
    }

    public void HomeButton()
    {
        Application.Quit();
    }

    public void RestartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
