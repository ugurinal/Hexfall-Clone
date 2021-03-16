﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

    public class GameManager : MonoBehaviour
    {
        private static GameManager _instance;
        public static GameManager Instance { get => _instance; }

        [SerializeField] private GameVariables _gameVariables;

        [SerializeField] private GameObject _hexagoneParent;

        private float _hexagoneWidth;
        private float _hexagoneHeight;

        private GameObject[,] _hexagones;
        private List<GameObject> _bombs;

        public GameObject[,] Hexagones { get => _hexagones; }

        private bool matchFound;

        private Vector3 startPos;

        private GameState gameState;
        public GameState GameState { get => gameState; set => gameState = value; }

        private int _score;
        public int Score { get => _score; }

        private int _bombScore;

        private int _highScore;
        public int HighScore { get => _highScore; }

        private int _moveCounter;
        public int MoveCounter { get => _moveCounter; }

        private int _matchCounter;

        private MainUIManager _UIManager;

        public bool IsGameStarted;

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

            _bombs = new List<GameObject>();

            IsGameStarted = false; ;

            _matchCounter = 0;
            _bombScore = 0;
            _score = 0;
            _moveCounter = 0;
            _highScore = PlayerPrefs.GetInt("Highscore", 0);

            _hexagones = new GameObject[_gameVariables.GridWidth, _gameVariables.GridHeight];

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
            gameState = GameState.Filling;

            CalculateStartPosition();

            StartCoroutine(FillEmptyPlaces());

            yield return new WaitForSeconds(2f);
        }

        private void CalculateStartPosition()
        {
            float x = -_hexagoneWidth * ((_gameVariables.GridWidth - 1) / 2.0f) + 0.25f;
            float y = -_hexagoneHeight * (_gameVariables.GridHeight / 2.0f) - 0.25f;
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
                        // right top hexagon and top hexagon
                        if (right < _gameVariables.GridWidth && top < _gameVariables.GridHeight)
                        {
                            if (currentHexagon.GetComponent<HexagonPiece>().HexagonColor == _hexagones[right, row].GetComponent<HexagonPiece>().HexagonColor &&
                                currentHexagon.GetComponent<HexagonPiece>().HexagonColor == _hexagones[column, top].GetComponent<HexagonPiece>().HexagonColor)
                            {
                                explodedHexagones.Add(_hexagones[column, row]);
                                explodedHexagones.Add(_hexagones[right, row]);
                                explodedHexagones.Add(_hexagones[column, top]);

                                //check 4th one
                                if (currentHexagon.GetComponent<HexagonPiece>().HexagonColor == _hexagones[right, top].GetComponent<HexagonPiece>().HexagonColor)
                                {
                                    explodedHexagones.Add(_hexagones[right, top]);
                                }

                                _matchCounter = explodedHexagones.Count;

                                matchFound = true;
                            }
                        }

                        // right bottom hexagon and top hexagon
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
                        // right top hexagon and top hexagon
                        if (right < _gameVariables.GridWidth && top < _gameVariables.GridHeight)
                        {
                            if (currentHexagon.GetComponent<HexagonPiece>().HexagonColor == _hexagones[right, top].GetComponent<HexagonPiece>().HexagonColor &&
                                currentHexagon.GetComponent<HexagonPiece>().HexagonColor == _hexagones[column, top].GetComponent<HexagonPiece>().HexagonColor)
                            {
                                explodedHexagones.Add(_hexagones[column, row]);
                                explodedHexagones.Add(_hexagones[right, top]);
                                explodedHexagones.Add(_hexagones[column, top]);
                                matchFound = true;

                                if (top + 1 < _gameVariables.GridHeight)
                                {
                                    if (currentHexagon.GetComponent<HexagonPiece>().HexagonColor == _hexagones[right, top + 1].GetComponent<HexagonPiece>().HexagonColor)
                                    {
                                        explodedHexagones.Add(_hexagones[right, top + 1]);
                                    }
                                }

                                _matchCounter = explodedHexagones.Count;
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
                //_matchCounter = explodedHexagones.Count;
                // if bomb is going to explode then remove it from bomb list
                for (int i = 0; i < explodedHexagones.Count; i++)
                {
                    if (_bombs.Contains(explodedHexagones[i]))
                    {
                        _bombs.Remove(explodedHexagones[i]);
                    }
                }

                // if they contain duplicate objects remove them
                explodedHexagones = explodedHexagones.Distinct().ToList();

                StartCoroutine(ExplodeMatches(explodedHexagones));

                if (IsGameStarted)
                {
                    //Debug.Log(explodedHexagones.Count);
                    _score += explodedHexagones.Count * _gameVariables.ScorePerHexagon;
                    _bombScore += explodedHexagones.Count * _gameVariables.ScorePerHexagon;
                    _UIManager.UpdateUI();
                    //explodedHexagones.Clear();
                }

                return true;
            }
            else
            {
                gameState = GameState.Idle;
                //if (!CheckPossibleMatchesWithRaycast())
                //{
                //    Debug.Log("In match list !");
                //}
                // check possible matches

                return false;
            }
        }

        private IEnumerator FillEmptyPlaces()
        {
            //Debug.Log("FILL EMPTY PLACES LOG!");
            gameState = GameState.Filling;
            List<int> missingHexagonList = new List<int>();

            // find empty places
            for (int column = 0; column < _gameVariables.GridWidth; column++)
            {
                int missingHexagonCounter = 0;
                for (int row = 0; row < _gameVariables.GridHeight; row++)
                {
                    if (_hexagones[column, row] == null)
                    {
                        //Debug.Log("Column: " + column + " Row: " + row + " is NULL!");
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
                    Vector3 initPos = CalculatePosition(_gameVariables.GridWidth + 4, column);
                    Vector3 targetPos = CalculatePosition(_gameVariables.GridHeight - row - 1, column);

                    GameObject hexagon;

                    if (_bombScore >= _gameVariables.ScoreForBomb)
                    {
                        int bombNum = Random.Range(0, _gameVariables.BombPrefabs.Length);
                        _bombScore = 0;
                        hexagon = Instantiate(_gameVariables.BombPrefabs[bombNum], initPos, Quaternion.identity, _hexagoneParent.transform);
                        hexagon.GetComponent<BombHexagon>().BombCounter = _gameVariables.BombLife;
                        _bombs.Add(hexagon);
                    }
                    else
                    {
                        int hexNum = Random.Range(0, _gameVariables.HexagonPrefabs.Length);
                        hexagon = Instantiate(_gameVariables.HexagonPrefabs[hexNum], initPos, Quaternion.identity, _hexagoneParent.transform);
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

                //missingHexagonCounter[explodedObject[i].GetComponent<HexagonPiece>().Column]++;
            }

            yield return new WaitForSeconds(_gameVariables.ExplosionTime);

            //Debug.Log("Column: " + explodedObject[0].GetComponent<HexagonPiece>().Column + " Row: " + explodedObject[0].GetComponent<HexagonPiece>().Row + " is giont to Explode!");
            //Debug.Log("Column: " + explodedObject[1].GetComponent<HexagonPiece>().Column + " Row: " + explodedObject[1].GetComponent<HexagonPiece>().Row + " is giont to Explode!");
            //Debug.Log("Column: " + explodedObject[2].GetComponent<HexagonPiece>().Column + " Row: " + explodedObject[2].GetComponent<HexagonPiece>().Row + " is giont to Explode!");

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