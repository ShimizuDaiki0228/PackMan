using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ReadScore : MonoBehaviour
{
    [SerializeField]
    private Text _highScoreText;
    [SerializeField]
    private Text _levelText;

    private void Start()
    {
        _highScoreText.text = "HighScore: " + ScoreController.Score;
        _levelText.text = "Level: " + ScoreController.Level;
    }
}
