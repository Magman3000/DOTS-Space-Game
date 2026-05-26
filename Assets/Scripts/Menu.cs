using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using Unity.Entities;
using TMPro;
using UnityEngine.InputSystem;
using InputActions;

public class Menu : MonoBehaviour
{
    [SerializeField] private TMP_Text scoreText, scoreboard, pauseResume, health;
	[SerializeField] private GameObject startPanel;
    private PlayerController pC;
    private SpawnSystem spS;
    private bool paused, active, pauseToggle = false;
    private List<int> pastScores = new List<int>();
    private InputSystem_Actions inputActions;
    void Start()
    {
		Time.timeScale = 0;
        pC = World.DefaultGameObjectInjectionWorld.GetExistingSystemManaged<PlayerController>();
        spS =  World.DefaultGameObjectInjectionWorld.GetExistingSystemManaged<SpawnSystem>();
        pC.Death+= OnDeath;
    }

    void Update()
    {
        if (!active && pC.active)
        {
            inputActions = pC.inputActions;
            active = true;
        }
        else if (active)
        {
            if (inputActions.Player.Pause.ReadValue<float>() > 0)
            {
                if (!pauseToggle) TogglePause();
                pauseToggle = true;
            }
            else if (inputActions.Player.Pause.ReadValue<float>() <= 0) pauseToggle = false;
        }
        scoreText.text = "Score: " + spS.score;
        string temp = "";
        foreach (int i in pastScores)
        {
            temp += i + "\n";
        }
        scoreboard.text = temp;
        health.text = pC.health + ":HP";
    }

	public void StartGame()
	{
		startPanel.SetActive(false);
		Time.timeScale = 1;
	}
    public void TogglePause()
    {
        paused = !paused;
        string temp = "";
        if (paused)
        {
            temp = "Resume";
            Time.timeScale = 0;
        }
        else
        {
            temp = "Pause";
            
            Time.timeScale = 1;
        }
        pauseResume.text = temp;
    }
    public void Reset()
    {
        if (active) pC.Reset(this);
    }

    public void Quit()
    {
        Application.Quit();
    }
    private void OnDeath(object sender, System.EventArgs a)
    {
        pastScores.Add(spS.score);
        spS.score = -1;
    }
}
