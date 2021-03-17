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

        [Header("Top Panel")]
        [SerializeField] private TextMeshProUGUI _scoreText;
        [SerializeField] private TextMeshProUGUI _moveText;

        [Header("Left Menu Panel")]
        [Space(10f)]
        [SerializeField] private Animator _leftMenuAnim;
        [SerializeField] private Button _menuButton;
        [SerializeField] private Button _newGameButton;
        [SerializeField] private Button _backButton;
        [SerializeField] private Button _exitButton;

        [Header("Game Over Panel")]
        [Space(10f)]
        [SerializeField] private GameObject _gameoverPanel;
        [SerializeField] private TextMeshProUGUI _gameoverInfo;
        [SerializeField] private Animator _gameoverAnim;
        [SerializeField] private Button _restartButton;
        [SerializeField] private Button _gameOverExitButton;

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

            // remove listener
            _menuButton.onClick.RemoveAllListeners();
            _newGameButton.onClick.RemoveAllListeners();
            _backButton.onClick.RemoveAllListeners();
            _exitButton.onClick.RemoveAllListeners();
            _restartButton.onClick.RemoveAllListeners();
            _gameOverExitButton.onClick.RemoveAllListeners();

            // add listener
            _menuButton.onClick.AddListener(OpenMenu);
            _newGameButton.onClick.AddListener(LoadNewGame);
            _backButton.onClick.AddListener(CloseMenu);
            _exitButton.onClick.AddListener(ExitGame);
            _restartButton.onClick.AddListener(LoadNewGame);
            _gameOverExitButton.onClick.AddListener(ExitGame);
        }

        public void UpdateUI()
        {
            _scoreText.text = "" + _gameManager.Score;
            _moveText.text = "" + _gameManager.MoveCounter;
        }

        public void OpenMenu()
        {
            _menuButton.interactable = false;   // player can not press more than one time
            _leftMenuAnim.SetTrigger("FadeIn");
        }

        public void CloseMenu()
        {
            _menuButton.interactable = true;
            _leftMenuAnim.SetTrigger("FadeOut");
        }

        private void LoadNewGame()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        private void ExitGame()
        {
            Application.Quit();
        }

        public void LoadGameOverScreen(string message)
        {
            _gameoverInfo.text = message;
            _gameoverPanel.SetActive(true);
            _gameoverAnim.SetTrigger("Gameover");
        }
    }   // mainuimanager
}   // namespace