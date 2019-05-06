using System.Collections;
using UnityEngine;
using UnityEngine.UI;

// Controls displaying the Round End Menu and player upgrades
public class RoundEndMenuController : MonoBehaviour
{
    // Starting levels of upgrades
    private static int SWEEP_SPEED_LEVEL = 0;
    private static int LASER_RECHARGE_LEVEL = 0;
    private static int VISIBILITY_LEVEL = 0;

    private static bool UPGRADE_TAKEN = false;

    // References set in the editor
    public Text sweepSpeedText;
    public Text laserRechargeText;
    public Text visibilityText;
    public Button sweepSpeedButton;
    public Button laserRechargeButton;
    public Button visibilityButton;
    public GameObject continueButton;


    private void OnEnable()
    {
        // Display current upgrade levels
        sweepSpeedText.text = "LV. " + SWEEP_SPEED_LEVEL;
        laserRechargeText.text = "LV. " + LASER_RECHARGE_LEVEL;
        visibilityText.text = "LV. " + VISIBILITY_LEVEL;

        // Disable upgrade buttons at max level
        if (SWEEP_SPEED_LEVEL == 10)
            sweepSpeedButton.interactable = false;
        if (LASER_RECHARGE_LEVEL == 10)
            laserRechargeButton.interactable = false;
        if (VISIBILITY_LEVEL == 10)
            visibilityButton.interactable = false;

        // Provide a separate continue button once all upgrades have been recieved
        if (SWEEP_SPEED_LEVEL == 10 && LASER_RECHARGE_LEVEL == 10 && VISIBILITY_LEVEL == 10)
            continueButton.SetActive(true);

        // Reset the ability to take an upgrade
        UPGRADE_TAKEN = false;
    }

    // Applies the player's chosen upgrade and advances the round, called when the player clicks an upgrade button on the Round End Menu
    public void ApplyUpgrade(int in_upgrade)
    {
        // Ensure that only one upgrade can be taken per round.  This is necessary because of the delay for SFX introduced below.
        if (UPGRADE_TAKEN) return;
        UPGRADE_TAKEN = true;

        // Apply chosen upgrade
        switch (in_upgrade)
        {
            case 0:
                SWEEP_SPEED_LEVEL++;
                DarkMagician.SWEEP_SPEED += DarkMagician.SWEEP_SPEED_UPGRADE;
                break;
            case 1:
                LASER_RECHARGE_LEVEL++;
                DarkMagician.LASER_RECHARGE += DarkMagician.LASER_RECHARGE_UPGRADE;
                break;
            case 2:
                VISIBILITY_LEVEL++;
                DarkMagician.VISIBILITY_SCALE += DarkMagician.VISIBILITY_UPGRADE;
                break;
        }

        // Wait for upgrade SFX to finish before loading the next scene
        StartCoroutine(WaitForSFX());
    }

    // Pass control back to the DarkMagician
    private IEnumerator WaitForSFX()
    {
        yield return new WaitForSecondsRealtime(0.5f);
        DarkMagician.AdvanceRound();
    }

    // Pass control back to the DarkMagician (duplicate functionality needed to work within Unity's UI system)
    public void Continue()
    {
        DarkMagician.AdvanceRound();
    }
}
