using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimulateGame : MonoBehaviour
{
    public int numberOfPlayers = 2;
    public GameObject gameController;

    // Start is called before the first frame update
    private void Start()
    {
        for (int i = 0; i < numberOfPlayers; i++)
        {
            PlayerController newPlayer = gameController.AddComponent<PlayerController>();
            newPlayer.PlayerName = "Player " + i;
        }
    }

    // Update is called once per frame
    private void Update()
    {
    }
}