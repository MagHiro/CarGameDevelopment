using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuBehaviour : MonoBehaviour
{

    public GameObject MainMenu; //this is the main menu container
	public GameObject Credits; //credits container
	public GameObject Options; //options container
	[SerializeField] Slider VolSlider;
    [SerializeField] private TMP_Text highScoreText;
    [SerializeField] private TMP_Text energyText;
    [SerializeField] private AndroidNotificationHandler _androidNotificationHandler;
    [SerializeField] private IOSNotificationHandler _iosNotificationHandler;
    [SerializeField] private Button playButton;
    [SerializeField] private int maxEnergy;
    [SerializeField] private int energyRechargeMinutes;

    private int energy;

    private const string EnergyKey = "Energy";
    private const string EnergyReadyKey = "EnergyReady";

    private void Start()
    {
        OnApplicationFocus(true);
        MainMenu.SetActive(true); //set to active (you can see it)
		Credits.SetActive(false); //set to inactive (you cannot see it!)
		Options.SetActive(false);
		AudioListener.volume = VolSlider.value;
    }

    private void OnApplicationFocus(bool hasFocus)
    {
        if (!hasFocus)
        {
            return;
        }

        CancelInvoke();

        int highScore = PlayerPrefs.GetInt(ScoreSystem.HighScoreKey, 0);
        highScoreText.text = "High Score : " + highScore.ToString(); //or $"High Score {highScore}";
        energy = PlayerPrefs.GetInt(EnergyKey, maxEnergy);

        if (energy == 0)
        {
            string energyReadyString = PlayerPrefs.GetString(EnergyReadyKey, string.Empty);
            if (energyReadyString == String.Empty)
            {
                return;
            }

            DateTime energyReady = DateTime.Parse(energyReadyString);
            if (DateTime.Now > energyReady)
            {
                energy = maxEnergy;
                PlayerPrefs.SetInt(EnergyKey, energy);
            }
            else
            {
                playButton.interactable = false;
                Invoke(nameof(EnergyRecharge), (energyReady - DateTime.Now).Seconds);
            }
        }

        energyText.text = "Play :" + energy;
    }

    public void OptButton() {
		MainMenu.SetActive(false);
		Credits.SetActive(false);
		Options.SetActive(true);
	}
	
	public void CreditButton() {
		Credits.SetActive(true);
		MainMenu.SetActive(false);
		Options.SetActive(false);
	}
	
	public void BackToMainMenu() {
		MainMenu.SetActive(true);
		Credits.SetActive(false);
		Options.SetActive(false);
	}
	
	public void QuitButton() {
		Application.Quit(); //get the **** out of this app
		
	}
	
	public void AdjustVol() {
		AudioListener.volume = VolSlider.value;
		//you need slider to adjust volumes
	}

    private void EnergyRecharge()
    {
        playButton.interactable = true;
        energy = maxEnergy;
        PlayerPrefs.SetInt(EnergyKey, energy);
        energyText.text = "Play :" + energy;
    }

    public void PlayButton()
    {
        if (energy < 1)
        {
            return;
        }

        energy--;

        PlayerPrefs.SetInt(EnergyKey, energy);

        if (energy == 0)
        {
            DateTime energyReady = DateTime.Now.AddMinutes(energyRechargeMinutes);
            PlayerPrefs.SetString(EnergyReadyKey, energyReady.ToString());
#if UNITY_ANDROID
            _androidNotificationHandler.ScheduleNotification(energyReady);
#elif UNITY_IOS
            _iosNotificationHandler.ScheduleNotification(energyRechargeMinutes);
#endif
        }

        SceneManager.LoadScene(1);
    }
}