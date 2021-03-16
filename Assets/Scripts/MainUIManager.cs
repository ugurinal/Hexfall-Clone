using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
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
        [SerializeField] private Animator _anim;
        [SerializeField] private Button _menuButton;
        [SerializeField] private Button _newGameButton;
        [SerializeField] private Button _backButton;
        [SerializeField] private Button _exitButton;

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

            _menuButton.onClick.RemoveAllListeners();
            _newGameButton.onClick.RemoveAllListeners();
            _backButton.onClick.RemoveAllListeners();
            _exitButton.onClick.RemoveAllListeners();

            _menuButton.onClick.AddListener(OpenMenu);
            _newGameButton.onClick.AddListener(LoadNewGame);
            _backButton.onClick.AddListener(CloseMenu);
            _exitButton.onClick.AddListener(ExitGame);
        }

        public void UpdateUI()
        {
            _scoreText.text = "" + _gameManager.Score;
            _moveText.text = "" + _gameManager.MoveCounter;
        }

        private void OpenMenu()
        {
            _anim.SetTrigger("FadeIn");
        }

        private void CloseMenu()
        {
            _anim.SetTrigger("FadeOut");
        }

        private void LoadNewGame()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        private void ExitGame()
        {
            Application.Quit();
        }
    }
}