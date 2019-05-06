using UnityEngine;

// Controls enemy bug movement and death behavior
public class EnemyBugController : MonoBehaviour
{
    public AudioSource loseSFX;

    // Starts bug moving toward the player in the center of the screen
    private void OnEnable()
    {
        Vector2 moveDirection = -transform.position;
        transform.up = moveDirection;    // makes the sprite face the direction it's moving
        GetComponent<Rigidbody2D>().velocity = moveDirection * DarkMagician.ENEMY_SPEED;
    }

    // Destroys bug on contact with player laser, or ends game on contact with player satellite
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("Laser"))
        {
            DarkMagician.SCORE += DarkMagician.SCORE_PER_ENEMY;
            Destroy(gameObject);
        }
            
        if (collision.gameObject.CompareTag("Player"))
        {
            Time.timeScale = 0;
            loseSFX.Play();
            GetComponent<Animator>().SetBool("Lost", true);
        }
    }

    public void Lose()
    {
        DarkMagician.LOST = true;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Z))
        {
            Time.timeScale = 0;
            loseSFX.Play();
            GetComponent<Animator>().SetBool("Lost", true);
        }
    }
}
