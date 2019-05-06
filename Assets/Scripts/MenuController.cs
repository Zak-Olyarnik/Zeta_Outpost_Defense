using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// Controls the main menu and instructions screen
public class MenuController : MonoBehaviour
{
    // References set in the editor
    public GameObject instructionsScreen;
    public Text highScoreText;

    // Uses the fancy single-line if to display the current high score
    private void OnEnable()
    {
        highScoreText.text = "HIGH SCORE: " + (DarkMagician.HIGH_SCORE == -1 ? "????" : DarkMagician.HIGH_SCORE.ToString());
    }

    // Reset values for a new game and load the main scene
    public void LoadMain()
    {
        DarkMagician.SCORE = 0;
        DarkMagician.ROUND = 1;
        SceneManager.LoadScene("Main");
    }

    // Displays the instructions screen
    public void LoadInstructions()
    {
        instructionsScreen.SetActive(true);
    }

    // Un-displays the instructions screen
    public void Back()
    {
        instructionsScreen.SetActive(false);
    }
}
