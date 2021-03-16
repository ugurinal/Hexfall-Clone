﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using HexfallClone.Hexagon;
using HexfallClone.UISystem;

namespace HexfallClone.GameController
{
    #region ENUMS

    public enum HexagonColor
    {
        Blue,
        Green,
        Red,
        Yellow,
        Purple
    }

    public enum GameState
    {
        Filling,
        Checking,
        Exploading,
        Rotating,
        Idle
    }

    #endregion ENUMS

    public class GameManager : MonoBehaviour
    {
        #region Private Fields

        private static GameManager _instance;   // for singleton pattern

        private GameState gameState;

        private float _hexagoneWidth;   // gap size will be added to these variables
        private float _hexagoneHeight;
        private Vector3 _startPos;   // first hexagon will be instantiated from here just to make sure we instantiate game board in the center regardless of the grid width and height

        private GameObject[,] _hexagones;   // keeps all hexagones and bombs in the game board
        private List<GameObject> _bombs;    // keeps all bombs in the game board

        private int _score;         // player score
        private int _bombScore;     // score to instantiate bombs
        private int _moveCounter;   // player move counter

        private bool matchFound;    // if there is match
        private int _matchCounter;  // how many hexagons are matched

        private bool _isGameStarted;    // if game is not started then first hexagon matches will not be added to the score

        private MainUIManager _UIManager;

        #endregion Private Fields

        #region Public Fields

        public static GameManager Instance { get => _instance; }

        [SerializeField] private GameVariables _gameVariables;
        [SerializeField] private Transform _hexagoneParent;    // to keep all hexagons under one parent (not necessary)

        public GameObject[,] Hexagones { get => _hexagones; }
        public GameState GameState { get => gameState; set => gameState = value; }
        public int Score { get => _score; }
        public int MoveCounter { get => _moveCounter; }

        public bool IsGameStarted { get => _isGameStarted; set => _isGameStarted = value; }

        #endregion Public Fields

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
            SetOtherInstances();
            StartCoroutine(InitGame());
        }

        /// <summary>
        /// get other instances from other singleton pattern classes
        /// </summary>
        private void SetOtherInstances()
        {
            _UIManager = MainUIManager.Instance;
        }

        /// <summary>
        /// add gap to the hexagon width and height
        /// </summary>
        private void AddGap()
        {
            _hexagoneHeight += _gameVariables.HexagonHeight + _gameVariables.GapBetweenHexagones;
            _hexagoneWidth += _gameVariables.HexagonWidth + _gameVariables.GapBetweenHexagones;
        }

        /// <summary>
        /// initialize game then waits for 2 seconds
        /// </summary>
        /// <returns></returns>
        private IEnumerator InitGame()
        {
            gameState = GameState.Filling;

            _isGameStarted = false;
            _bombs = new List<GameObject>();

            _matchCounter = 0;
            matchFound = false;

            _score = 0;
            _bombScore = 0;
            _moveCounter = 0;

            _hexagones = new GameObject[_gameVariables.GridWidth, _gameVariables.GridHeight];

            AddGap();
            CalculateStartPosition();
            StartCoroutine(FillEmptyPlaces());

            yield return new WaitForSeconds(2f);
        }

        /// <summary>
        /// calculates start positions of first hexagon
        /// </summary>
        private void CalculateStartPosition()
        {
            float x = -_hexagoneWidth * ((_gameVariables.GridWidth - 1) / 2.0f) + 0.25f;
            float y = -_hexagoneHeight * (_gameVariables.GridHeight / 2.0f) - 0.25f;
            _startPos = new Vector3(x, y, 0);
        }

        /// <summary>
        /// calculates hexagon positions based on their column and row
        /// if column is row it will be instantiated higher
        /// </summary>
        /// <param name="column"></param>
        /// <param name="row"></param>
        /// <returns></returns>
        private Vector3 CalculatePosition(int row, int column)
        {
            float offset = 0;

            if (column % 2 != 0)
            {
                offset = _hexagoneHeight / 2.0f;
            }

            float x = _startPos.x + (column * _hexagoneWidth) * 0.9f;
            float y = _startPos.y + (row * _hexagoneHeight) + offset;

            return new Vector3(x, y, 0);
        }

        /// <summary>
        /// this is the method that checks if there is match
        /// </summary>
        /// <returns></returns>
        public bool CheckMatches()
        {
            gameState = GameState.Checking; // update game state to checking
            List<GameObject> explodedHexagones = new List<GameObject>();        // keeps the hexagons that will be exploaded

            // we search every index to check if there is match
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
                        // right top hexagon and current top hexagon
                        if (right < _gameVariables.GridWidth && top < _gameVariables.GridHeight)
                        {
                            if (currentHexagon.GetComponent<HexagonPiece>().HexagonColor == _hexagones[right, row].GetComponent<HexagonPiece>().HexagonColor &&
                                currentHexagon.GetComponent<HexagonPiece>().HexagonColor == _hexagones[column, top].GetComponent<HexagonPiece>().HexagonColor)
                            {
                                explodedHexagones.Add(_hexagones[column, row]);
                                explodedHexagones.Add(_hexagones[right, row]);
                                explodedHexagones.Add(_hexagones[column, top]);

                                _matchCounter = explodedHexagones.Count;

                                matchFound = true;
                            }
                        }

                        // right bottom hexagon and right top hexagon
                        if (right < _gameVariables.GridWidth && bottom >= 0)
                        {
                            if (currentHexagon.GetComponent<HexagonPiece>().HexagonColor == _hexagones[right, bottom].GetComponent<HexagonPiece>().HexagonColor &&
                                currentHexagon.GetComponent<HexagonPiece>().HexagonColor == _hexagones[right, row].GetComponent<HexagonPiece>().HexagonColor)
                            {
                                explodedHexagones.Add(_hexagones[column, row]);
                                explodedHexagones.Add(_hexagones[right, bottom]);
                                explodedHexagones.Add(_hexagones[right, row]);

                                _matchCounter = explodedHexagones.Count;

                                matchFound = true;
                            }
                        }
                    }
                    else
                    {
                        // right top hexagon and  current top hexagon
                        if (right < _gameVariables.GridWidth && top < _gameVariables.GridHeight)
                        {
                            if (currentHexagon.GetComponent<HexagonPiece>().HexagonColor == _hexagones[right, top].GetComponent<HexagonPiece>().HexagonColor &&
                                currentHexagon.GetComponent<HexagonPiece>().HexagonColor == _hexagones[column, top].GetComponent<HexagonPiece>().HexagonColor)
                            {
                                explodedHexagones.Add(_hexagones[column, row]);
                                explodedHexagones.Add(_hexagones[right, top]);
                                explodedHexagones.Add(_hexagones[column, top]);

                                // if column is odd this condition checks the fourth  or more matches
                                if (top + 1 < _gameVariables.GridHeight)
                                {
                                    if (currentHexagon.GetComponent<HexagonPiece>().HexagonColor == _hexagones[right, top + 1].GetComponent<HexagonPiece>().HexagonColor)
                                    {
                                        explodedHexagones.Add(_hexagones[right, top + 1]);
                                    }
                                }

                                _matchCounter = explodedHexagones.Count;
                                matchFound = true;
                            }
                        }

                        // right bottom hexagon and right top hexagon
                        if (right < _gameVariables.GridWidth && top < _gameVariables.GridHeight)
                        {
                            if (currentHexagon.GetComponent<HexagonPiece>().HexagonColor == _hexagones[right, row].GetComponent<HexagonPiece>().HexagonColor &&
                                currentHexagon.GetComponent<HexagonPiece>().HexagonColor == _hexagones[right, top].GetComponent<HexagonPiece>().HexagonColor)
                            {
                                explodedHexagones.Add(_hexagones[column, row]);
                                explodedHexagones.Add(_hexagones[right, row]);
                                explodedHexagones.Add(_hexagones[right, top]);

                                // if column is odd this condition checks the fourth  or more matches
                                if (currentHexagon.GetComponent<HexagonPiece>().HexagonColor == _hexagones[column, top].GetComponent<HexagonPiece>().HexagonColor)
                                {
                                    explodedHexagones.Add(_hexagones[column, top]);
                                }

                                _matchCounter = explodedHexagones.Count;
                                matchFound = true;
                            }
                        }
                    }
                }
            }

            if (matchFound)
            {
                // if there is bomb in the game board and it is going to explode then remove it from bomb list
                if (_bombs.Count > 0)
                {
                    for (int i = 0; i < explodedHexagones.Count; i++)
                    {
                        if (_bombs.Contains(explodedHexagones[i]))
                        {
                            _bombs.Remove(explodedHexagones[i]);
                        }
                    }
                }

                // if they contain duplicate objects then remove them
                // if we don't apply this we may increase score more than necessary
                explodedHexagones = explodedHexagones.Distinct().ToList();

                // start exploading them
                StartCoroutine(ExplodeMatches(explodedHexagones));

                // in the beginning if there is match we don't increase score
                if (_isGameStarted)
                {
                    _score += explodedHexagones.Count * _gameVariables.ScorePerHexagon;
                    _bombScore += explodedHexagones.Count * _gameVariables.ScorePerHexagon;
                    _UIManager.UpdateUI();
                }

                return true;
            }
            else
            {
                gameState = GameState.Idle;
                return false;
            }
        }

        /// <summary>
        /// this method checks every index of game board if there is a missing hexagon
        /// </summary>
        /// <returns></returns>
        private IEnumerator FillEmptyPlaces()
        {
            gameState = GameState.Filling;
            List<int> missingHexagonList = new List<int>(); // this will keep how many hexagon is missing in each index
                                                            // if first index is zero then it menas there is no missing hexagon in column zero
                                                            // if third index (2) is 2 then it menas there are 2 missing hexagon in column 2

            // find empty indexes
            for (int column = 0; column < _gameVariables.GridWidth; column++)
            {
                int missingHexagonCounter = 0;
                for (int row = 0; row < _gameVariables.GridHeight; row++)
                {
                    if (_hexagones[column, row] == null)
                    {
                        missingHexagonCounter++;
                    }
                }
                missingHexagonList.Add(missingHexagonCounter);
            }

            // fill them
            for (int column = 0; column < _gameVariables.GridWidth; column++)
            {
                for (int row = 0; row < missingHexagonList[column]; row++)
                {
                    Vector3 initPos = CalculatePosition(_gameVariables.GridWidth + 4, column);      // hexagon will be instantiated from this position just to make them come from above
                    Vector3 targetPos = CalculatePosition(_gameVariables.GridHeight - row - 1, column); // hexagons destination
                                                                                                        // meaning of -row -1 => if height == 7 and missingHexagon[2] == 1
                                                                                                        // this means its destination is column 2 row 5

                    GameObject hexagon;

                    // if we reached bomb score then instantiate a bomb then reset bomb score
                    if (_bombScore >= _gameVariables.ScoreForBomb)
                    {
                        int bombNum = Random.Range(0, _gameVariables.BombPrefabs.Length);
                        _bombScore = 0;
                        hexagon = Instantiate(_gameVariables.BombPrefabs[bombNum], initPos, Quaternion.identity, _hexagoneParent);
                        hexagon.GetComponent<BombHexagon>().BombCounter = _gameVariables.BombLife;
                        _bombs.Add(hexagon);
                    }
                    else
                    {
                        int hexNum = Random.Range(0, _gameVariables.HexagonPrefabs.Length);
                        hexagon = Instantiate(_gameVariables.HexagonPrefabs[hexNum], initPos, Quaternion.identity, _hexagoneParent);
                    }

                    _hexagones[column, _gameVariables.GridHeight - row - 1] = hexagon;
                    hexagon.GetComponent<HexagonPiece>().Row = _gameVariables.GridHeight - row - 1;
                    hexagon.GetComponent<HexagonPiece>().Column = column;
                    hexagon.GetComponent<HexagonPiece>().MovementSpeed = _gameVariables.HexagonMovementSpeed;
                    hexagon.GetComponent<HexagonPiece>().TargetPosition = targetPos;

                    yield return new WaitForSeconds(0.05f);
                }
            }

            yield return new WaitForSeconds(0.2f);

            CheckMatches();

            yield return new WaitForSeconds(0.2f);

            if (!CheckPossibleMatchesWithRaycast())
            {
                _UIManager.LoadGameOverScreen("THERE IS NO AVAILABLE MATCHES!");
                Debug.Log("THERE IS NO AVAILABLE MATCHES!");
                Debug.Log("GAME OVER!");
            }
        }

        private IEnumerator ExplodeMatches(List<GameObject> explodedObject)
        {
            gameState = GameState.Exploading;
            //Debug.Log(gameState);

            for (int i = 0; i < explodedObject.Count; i++)
            {
                StartCoroutine(explodedObject[i].GetComponent<HexagonPiece>().Explode(_gameVariables.ExplosionTime));
            }

            yield return new WaitForSeconds(_gameVariables.ExplosionTime);

            // move other hexagones down
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
            //FillEmptyPlaces();

            yield return new WaitForSeconds(0.25f);
            explodedObject.Clear();
            StartCoroutine(FillEmptyPlaces());

            //CheckMatches();
        }

        public void UpdateMoveCounter()
        {
            _moveCounter++;

            if (_bombs.Count > 0)
            {
                foreach (GameObject bomb in _bombs)
                {
                    bomb.GetComponent<BombHexagon>().DecreaseCounter();
                }
            }

            _UIManager.UpdateUI();
        }

        public bool CheckPossibleMatchesWithRaycast()
        {
            bool foundMatch = false;

            for (int column = 0; column < _gameVariables.GridWidth; column++)
            {
                for (int row = 0; row < _gameVariables.GridHeight; row++)
                {
                    GameObject currentHexagon = _hexagones[column, row];

                    currentHexagon.GetComponent<PolygonCollider2D>().enabled = false;

                    List<RaycastHit2D> hits = new List<RaycastHit2D>();

                    for (int i = 0; i < 6; i++)
                    {
                        RaycastHit2D hit = Physics2D.Raycast(currentHexagon.transform.position, _gameVariables.RaycastDirections[i], 10f, LayerMask.GetMask("Hexagon"));

                        if (hit.collider != null)
                        {
                            hits.Add(hit);
                        }
                    }

                    int blueCounter = 0;
                    int greenCounter = 0;
                    int purpleCounter = 0;
                    int redCounter = 0;
                    int yellowCounter = 0;

                    for (int i = 0; i < hits.Count; i++)
                    {
                        string color = hits[i].transform.GetComponent<HexagonPiece>().HexagonColor;
                        switch (color)
                        {
                            case "Blue":
                                blueCounter++;
                                break;

                            case "Green":
                                greenCounter++;
                                break;

                            case "Purple":
                                purpleCounter++;
                                break;

                            case "Red":
                                redCounter++;
                                break;

                            case "Yellow":
                                yellowCounter++;
                                break;

                            default:
                                Debug.Log("No Color!");
                                break;
                        }
                    }

                    if (blueCounter >= 3 || greenCounter >= 3 || purpleCounter >= 3 || redCounter >= 3 || yellowCounter >= 3)
                    {
                        foundMatch = true;
                        Debug.Log("There are potential matches.");
                        currentHexagon.GetComponent<PolygonCollider2D>().enabled = true;
                        return foundMatch;
                    }
                    currentHexagon.GetComponent<PolygonCollider2D>().enabled = true;
                }
            }

            Debug.Log("There is no potential matches.");
            return foundMatch;
        }
    }   // gamemanager
}   // namespace