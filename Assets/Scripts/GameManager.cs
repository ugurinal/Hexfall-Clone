using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HexfallClone.Hexagon;
using HexfallClone.UISystem;

namespace HexfallClone.GameController
{
    public enum HexagonColor
    {
        Blue,
        Green,
        Red,
        Yellow,
        White,
    }

    public enum GameState
    {
        Filling,
        Checking,
        Exploading,
        Rotating,
        Idle
    }

    public class GameManager : MonoBehaviour
    {
        private static GameManager _instance;
        public static GameManager Instance { get => _instance; }

        [SerializeField] private GameVariables _gameVariables;

        [SerializeField] private GameObject _hexagoneParent;

        private float _hexagoneWidth;
        private float _hexagoneHeight;

        private GameObject[,] _hexagones;

        public GameObject[,] Hexagones { get => _hexagones; }

        private int[] missingHexagonCounter;

        private bool matchFound;

        private Vector3 startPos;

        private GameState gameState;
        public GameState GameState { get => gameState; set => gameState = value; }

        private int _score;
        public int Score { get => _score; }

        private int _highScore;
        public int HighScore { get => _highScore; }

        private int _moveCounter;
        public int MoveCounter { get => _moveCounter; }

        private int _matchCounter;

        private MainUIManager _UIManager;

        public bool IsPlayable;

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
            // Row = Height
            // Column = Width

            _UIManager = MainUIManager.Instance;

            gameState = GameState.Idle;
            Debug.Log(gameState);
            IsPlayable = true;

            _matchCounter = 0;
            _score = 0;
            _moveCounter = 0;
            _highScore = PlayerPrefs.GetInt("Highscore", 0);

            _hexagones = new GameObject[_gameVariables.GridWidth, _gameVariables.GridHeight];
            missingHexagonCounter = new int[_gameVariables.GridWidth];

            matchFound = false;

            AddGap();
            CalculateStartPosition();
            StartCoroutine(InitGame());
        }

        private void AddGap()
        {
            _hexagoneHeight += _gameVariables.HexagonHeight + _gameVariables.GapBetweenHexagones;
            _hexagoneWidth += _gameVariables.HexagonWidth + _gameVariables.GapBetweenHexagones;
        }

        private IEnumerator InitGame()
        {
            IsPlayable = false;

            gameState = GameState.Filling;
            Debug.Log(gameState);

            CalculateStartPosition();

            for (int column = 0; column < _gameVariables.GridWidth; column++)
            {
                for (int row = 0; row < _gameVariables.GridHeight; row++)
                {
                    Vector3 spawnPos = CalculatePosition(_gameVariables.GridHeight + 4, column);
                    Vector3 targetPos = CalculatePosition(row, column);

                    int hexNum = Random.Range(0, _gameVariables.HexagonPrefabs.Length);

                    GameObject hexagon = Instantiate(_gameVariables.HexagonPrefabs[hexNum], spawnPos, Quaternion.identity, _hexagoneParent.transform);
                    _hexagones[column, row] = hexagon;
                    hexagon.GetComponent<HexagonPiece>().Row = row;
                    hexagon.GetComponent<HexagonPiece>().Column = column;
                    hexagon.GetComponent<HexagonPiece>().TargetPosition = targetPos;

                    yield return new WaitForSeconds(0.05f);
                }
            }
            CheckMatches();
        }

        private void CalculateStartPosition()
        {
            float x = -_hexagoneWidth * (_gameVariables.GridWidth / 2.0f) + _hexagoneWidth;
            float y = -_hexagoneHeight * (_gameVariables.GridHeight / 2.0f);
            startPos = new Vector3(x, y, 0);
        }

        private Vector3 CalculatePosition(int column, int row)
        {
            float offset = 0;

            if (row % 2 != 0)
            {
                offset = _hexagoneHeight / 2.0f;
            }

            float x = startPos.x + (row * _hexagoneWidth) * 0.9f;
            float y = startPos.y + (column * _hexagoneHeight) + offset;

            return new Vector3(x, y, 0);
        }

        public bool CheckMatches()
        {
            List<GameObject> explodedHexagones = new List<GameObject>();
            gameState = GameState.Checking;
            Debug.Log(gameState);

            IsPlayable = false;

            for (int column = 0; column < _gameVariables.GridWidth; column++)
            {
                for (int row = 0; row < _gameVariables.GridHeight; row++)
                {
                    //int left = column - 1;
                    int right = column + 1;
                    int top = row + 1;
                    int bottom = row - 1;

                    GameObject currentHexagon = _hexagones[column, row];

                    if (column % 2 == 0)
                    {
                        // right then top left
                        if (right < _gameVariables.GridWidth && top < _gameVariables.GridHeight)
                        {
                            if (currentHexagon.GetComponent<HexagonPiece>().HexagonColor == _hexagones[right, row].GetComponent<HexagonPiece>().HexagonColor &&
                                currentHexagon.GetComponent<HexagonPiece>().HexagonColor == _hexagones[column, top].GetComponent<HexagonPiece>().HexagonColor)
                            {
                                explodedHexagones.Add(_hexagones[column, row]);
                                explodedHexagones.Add(_hexagones[right, row]);
                                explodedHexagones.Add(_hexagones[column, top]);

                                _matchCounter = 3;

                                matchFound = true;

                                break;
                            }
                        }

                        // right bottom then top
                        if (right < _gameVariables.GridWidth && bottom >= 0)
                        {
                            if (currentHexagon.GetComponent<HexagonPiece>().HexagonColor == _hexagones[right, bottom].GetComponent<HexagonPiece>().HexagonColor &&
                                currentHexagon.GetComponent<HexagonPiece>().HexagonColor == _hexagones[right, row].GetComponent<HexagonPiece>().HexagonColor)
                            {
                                explodedHexagones.Add(_hexagones[column, row]);
                                explodedHexagones.Add(_hexagones[right, bottom]);
                                explodedHexagones.Add(_hexagones[right, row]);

                                _matchCounter = 3;

                                matchFound = true;

                                break;
                            }
                        }
                    }
                    else
                    {
                        // right then top left
                        if (right < _gameVariables.GridWidth && top < _gameVariables.GridHeight)
                        {
                            if (currentHexagon.GetComponent<HexagonPiece>().HexagonColor == _hexagones[right, top].GetComponent<HexagonPiece>().HexagonColor &&
                                currentHexagon.GetComponent<HexagonPiece>().HexagonColor == _hexagones[column, top].GetComponent<HexagonPiece>().HexagonColor)
                            {
                                explodedHexagones.Add(_hexagones[column, row]);
                                explodedHexagones.Add(_hexagones[right, top]);
                                explodedHexagones.Add(_hexagones[column, top]);
                                matchFound = true;

                                _matchCounter = 3;

                                break;
                            }
                        }

                        // right bottom then top
                        if (right < _gameVariables.GridWidth && top < _gameVariables.GridHeight)
                        {
                            if (currentHexagon.GetComponent<HexagonPiece>().HexagonColor == _hexagones[right, row].GetComponent<HexagonPiece>().HexagonColor &&
                                currentHexagon.GetComponent<HexagonPiece>().HexagonColor == _hexagones[right, top].GetComponent<HexagonPiece>().HexagonColor)
                            {
                                explodedHexagones.Add(_hexagones[column, row]);
                                explodedHexagones.Add(_hexagones[right, row]);
                                explodedHexagones.Add(_hexagones[right, top]);

                                _matchCounter = 3;

                                matchFound = true;

                                break;
                            }
                        }
                    }
                }
                if (matchFound)
                    break;
            }

            if (matchFound)
            {
                StartCoroutine(ExplodeMatches(explodedHexagones));
                IsPlayable = false;

                return true;
            }
            else
            {
                Debug.Log("IN ELSE");

                gameState = GameState.Idle;
                Debug.Log(gameState);

                // IsPlayable = true;

                return false;
            }

            //Debug.Log(explodedHexagones.Count);
            //gameState = GameState.Idle;
        }

        private void FillEmptyPlaces()
        {
            gameState = GameState.Filling;
            Debug.Log(gameState);

            IsPlayable = false;

            for (int column = 0; column < _gameVariables.GridWidth; column++)
            {
                for (int row = 0; row < missingHexagonCounter[column]; row++)
                {
                    Vector3 initPos = CalculatePosition(_gameVariables.GridWidth + 4, column);
                    Vector3 targetPos = CalculatePosition(_gameVariables.GridWidth - row, column);

                    int hexNum = Random.Range(0, _gameVariables.HexagonPrefabs.Length);

                    GameObject hexagon = Instantiate(_gameVariables.HexagonPrefabs[hexNum], initPos, Quaternion.identity, _hexagoneParent.transform);

                    _hexagones[column, _gameVariables.GridWidth - row] = hexagon;
                    hexagon.GetComponent<HexagonPiece>().Row = _gameVariables.GridWidth - row;
                    hexagon.GetComponent<HexagonPiece>().Column = column;
                    hexagon.GetComponent<HexagonPiece>().TargetPosition = targetPos;
                }
                missingHexagonCounter[column] = 0;
            }

            //gameState = GameState.Idle;
        }

        private IEnumerator ExplodeMatches(List<GameObject> explodedObject)
        {
            gameState = GameState.Exploading;
            Debug.Log(gameState);

            IsPlayable = false;

            for (int i = 0; i < explodedObject.Count; i++)
            {
                StartCoroutine(explodedObject[i].GetComponent<HexagonPiece>().Explode(_gameVariables.ExplosionTime));

                missingHexagonCounter[explodedObject[i].GetComponent<HexagonPiece>().Column]++;
            }

            yield return new WaitForSeconds(_gameVariables.ExplosionTime);

            for (int i = 0; i < explodedObject.Count; i++)
            {
                int row = explodedObject[i].GetComponent<HexagonPiece>().Row;
                int column = explodedObject[i].GetComponent<HexagonPiece>().Column;

                for (int j = row; j < _gameVariables.GridHeight - 1; j++)
                {
                    GameObject current = _hexagones[column, j];
                    GameObject top = _hexagones[column, j + 1];

                    int currentRow = current.GetComponent<HexagonPiece>().Row;
                    int topRow = top.GetComponent<HexagonPiece>().Row;

                    _hexagones[column, j] = top;
                    _hexagones[column, j + 1] = current;

                    _hexagones[column, j].GetComponent<HexagonPiece>().Row = currentRow;
                    _hexagones[column, j + 1].GetComponent<HexagonPiece>().Row = topRow;

                    _hexagones[column, j].GetComponent<HexagonPiece>().TargetPosition = CalculatePosition(j, column);
                    _hexagones[column, j + 1].GetComponent<HexagonPiece>().TargetPosition = CalculatePosition(j + 1, column);
                }
            }

            matchFound = false;
            FillEmptyPlaces();

            yield return new WaitForSeconds(_gameVariables.ExplosionTime);
            CheckMatches();
        }

        public void UpdateScoreAndMove()
        {
            _score += _matchCounter * _gameVariables.ScorePerHexagon;
            _moveCounter++;
            _matchCounter = 0;
            _UIManager.UpdateUI();
        }
    }   // gamemanager
}   // namespace