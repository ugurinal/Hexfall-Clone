using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using HexfallClone.GameController;

namespace HexfallClone.UISystem
{
    public class MainUIManager : MonoBehaviour
    {
        private static MainUIManager _instance;
        public static MainUIManager Instance { get => _instance; }

        [SerializeField] private TextMeshProUGUI _scoreText;
        [SerializeField] private TextMeshProUGUI _highScore;
        [SerializeField] private TextMeshProUGUI _moveText;

        private GameManager _gameManager;

        private void Awake()
        {
            MakeSingleton();
        }

        private void MakeSingleton()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
            }
            else
            {
                _instance = this;
            }
        }

        private void Start()
        {
            _gameManager = GameManager.Instance;

            UpdateUI();
            _highScore.text = "Highscore: " + _gameManager.HighScore;
        }

        public void UpdateUI()
        {
            _scoreText.text = "" + _gameManager.Score;
            _moveText.text = "" + _gameManager.MoveCounter;
        }
    }
}