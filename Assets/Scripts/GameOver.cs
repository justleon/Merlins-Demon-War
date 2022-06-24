using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameOver : MonoBehaviour
{
    public TMP_Text demonsKilled = null;
    public TMP_Text score = null;

    private void Awake() {
        demonsKilled.text = "Demons killed: " + GameController.instance.playerKills.ToString();
        score.text = "Score: " + GameController.instance.playerScore.ToString();
    }

    public void MainMenu()
    {
        SceneManager.LoadScene(0);
    }
}
