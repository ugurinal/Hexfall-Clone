﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HexfallClone.Hexagon;

namespace HexfallClone.GameController
{
    public enum HexagonColor
    {
        Blue,
        Green,
        Red,
        Yellow,
        White
    }

    public enum GameState
    {
        Filling,
        Checking,
        Updating,
        Idle
    }

    public class GameManager : MonoBehaviour
    {
        private static GameManager _instance;
        public static GameManager Instance { get => _instance; }

        [SerializeField] private GameObject _hexagoneParent;
        [SerializeField] private GameObject[] _hexagonePrefabs;

        [SerializeField] private int _gridWidth;
        [SerializeField] private int _gridHeight;
        [SerializeField] private float _gridGap;

        [SerializeField] private float _hexagoneWidth;
        [SerializeField] private float _hexagoneHeight;

        private GameObject[,] _hexagones;

        public GameObject[,] Hexagones { get => _hexagones; }

        private int[] missingHexagonCounter;

        private bool matchFound;

        private Vector3 startPos;

        private GameState gameState;
        public GameState GameState { get => gameState; }

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

            gameState = GameState.Idle;

            _hexagones = new GameObject[_gridWidth, _gridHeight];
            missingHexagonCounter = new int[_gridWidth];

            matchFound = false;

            AddGap();
            CalculateStartPosition();
            StartCoroutine(InitGame());
        }

        private void AddGap()
        {
            _hexagoneHeight += _gridGap;
            _hexagoneWidth += _gridGap;
        }

        private IEnumerator InitGame()
        {
            gameState = GameState.Filling;
            CalculateStartPosition();

            for (int column = 0; column < _gridWidth; column++)
            {
                for (int row = 0; row < _gridHeight; row++)
                {
                    Vector3 spawnPos = CalculatePosition(_gridHeight + 4, column);
                    Vector3 targetPos = CalculatePosition(row, column);

                    int hexNum = Random.Range(0, _hexagonePrefabs.Length);

                    GameObject hexagon = Instantiate(_hexagonePrefabs[hexNum], spawnPos, Quaternion.identity, _hexagoneParent.transform);
                    _hexagones[column, row] = hexagon;
                    hexagon.GetComponent<HexagonPiece>().Row = row;
                    hexagon.GetComponent<HexagonPiece>().Column = column;
                    hexagon.GetComponent<HexagonPiece>().TargetPosition = targetPos;

                    yield return new WaitForSeconds(0.005f);
                }
            }
            CheckMatches();
        }

        private void CalculateStartPosition()
        {
            float x = -_hexagoneWidth * (_gridWidth / 2.0f) + _hexagoneWidth;
            float y = -_hexagoneHeight * (_gridHeight / 2.0f);
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

        private void CheckMatches()
        {
            List<GameObject> explodedHexagones = new List<GameObject>();
            gameState = GameState.Checking;

            for (int column = 0; column < _gridWidth; column++)
            {
                if (matchFound)
                    break;

                for (int row = 0; row < _gridHeight; row++)
                {
                    int left = column - 1;
                    int right = column + 1;
                    int top = row + 1;
                    int bottom = row - 1;

                    GameObject currentHexagon = _hexagones[column, row];

                    if (column % 2 == 0)
                    {
                        // right then top left
                        if (right < _gridWidth && top < _gridHeight)
                        {
                            if (currentHexagon.GetComponent<HexagonPiece>().HexagonColor == _hexagones[right, row].GetComponent<HexagonPiece>().HexagonColor &&
                                currentHexagon.GetComponent<HexagonPiece>().HexagonColor == _hexagones[column, top].GetComponent<HexagonPiece>().HexagonColor)
                            {
                                // UpdateBoard(_hexagones[column, row], _hexagones[column, top], _hexagones[right, row]);

                                _hexagones[column, row].transform.GetChild(1).gameObject.SetActive(true);
                                _hexagones[right, row].transform.GetChild(1).gameObject.SetActive(true);
                                _hexagones[column, top].transform.GetChild(1).gameObject.SetActive(true);

                                explodedHexagones.Add(_hexagones[column, row]);
                                explodedHexagones.Add(_hexagones[right, row]);
                                explodedHexagones.Add(_hexagones[column, top]);

                                matchFound = true;

                                break;

                                //UpdateBoard(column, row);
                                //UpdateBoard(right, row);
                                //UpdateBoard(column, top);

                                //matchFound = true;
                            }
                        }

                        // right bottom then top
                        if (right < _gridWidth && bottom >= 0)
                        {
                            if (currentHexagon.GetComponent<HexagonPiece>().HexagonColor == _hexagones[right, bottom].GetComponent<HexagonPiece>().HexagonColor &&
                                currentHexagon.GetComponent<HexagonPiece>().HexagonColor == _hexagones[right, row].GetComponent<HexagonPiece>().HexagonColor)
                            {
                                //UpdateBoard(_hexagones[column, row], _hexagones[right, bottom], _hexagones[right, row]);

                                _hexagones[column, row].transform.GetChild(1).gameObject.SetActive(true);
                                _hexagones[right, bottom].transform.GetChild(1).gameObject.SetActive(true);
                                _hexagones[right, row].transform.GetChild(1).gameObject.SetActive(true);

                                explodedHexagones.Add(_hexagones[column, row]);
                                explodedHexagones.Add(_hexagones[right, bottom]);
                                explodedHexagones.Add(_hexagones[right, row]);

                                matchFound = true;

                                break;

                                //UpdateBoard(column, row);
                                //UpdateBoard(right, bottom);
                                //UpdateBoard(right, row);
                                //matchFound = true;
                            }
                        }
                    }
                    else
                    {
                        // right then top left
                        if (right < _gridWidth && top < _gridHeight)
                        {
                            if (currentHexagon.GetComponent<HexagonPiece>().HexagonColor == _hexagones[right, top].GetComponent<HexagonPiece>().HexagonColor &&
                                currentHexagon.GetComponent<HexagonPiece>().HexagonColor == _hexagones[column, top].GetComponent<HexagonPiece>().HexagonColor)
                            {
                                //UpdateBoard(_hexagones[column, row], _hexagones[right, top], _hexagones[column, top]);

                                _hexagones[column, row].transform.GetChild(1).gameObject.SetActive(true);
                                _hexagones[right, top].transform.GetChild(1).gameObject.SetActive(true);
                                _hexagones[column, top].transform.GetChild(1).gameObject.SetActive(true);

                                explodedHexagones.Add(_hexagones[column, row]);
                                explodedHexagones.Add(_hexagones[right, top]);
                                explodedHexagones.Add(_hexagones[column, top]);
                                matchFound = true;

                                break;

                                //UpdateBoard(column, row);
                                //UpdateBoard(right, top);
                                //UpdateBoard(column, top);
                                //matchFound = true;
                            }
                        }

                        // right bottom then top
                        if (right < _gridWidth && top < _gridHeight)
                        {
                            if (currentHexagon.GetComponent<HexagonPiece>().HexagonColor == _hexagones[right, row].GetComponent<HexagonPiece>().HexagonColor &&
                                currentHexagon.GetComponent<HexagonPiece>().HexagonColor == _hexagones[right, top].GetComponent<HexagonPiece>().HexagonColor)
                            {
                                // UpdateBoard(_hexagones[column, row], _hexagones[right, row], _hexagones[right, top]);

                                _hexagones[column, row].transform.GetChild(1).gameObject.SetActive(true);
                                _hexagones[right, row].transform.GetChild(1).gameObject.SetActive(true);
                                _hexagones[right, top].transform.GetChild(1).gameObject.SetActive(true);

                                explodedHexagones.Add(_hexagones[column, row]);
                                explodedHexagones.Add(_hexagones[right, row]);
                                explodedHexagones.Add(_hexagones[right, top]);

                                matchFound = true;

                                break;

                                //UpdateBoard(column, row);
                                //UpdateBoard(right, row);
                                //UpdateBoard(right, top);
                                //matchFound = true;
                            }
                        }
                    }
                }
            }

            if (matchFound)
            {
                StartCoroutine(ExplodeMatches(explodedHexagones));
                //FillEmptyPlaces();
            }

            Debug.Log(explodedHexagones.Count);
            gameState = GameState.Idle;
        }

        private void UpdateBoard(int column, int row)
        {
            gameState = GameState.Updating;
            _hexagones[column, row].GetComponent<SpriteRenderer>().enabled = false;

            missingHexagonCounter[column]++;

            for (int i = row; i < _gridHeight - 1; i++)
            {
                GameObject current = _hexagones[column, i];
                GameObject top = _hexagones[column, i + 1];

                int currentRow = current.GetComponent<HexagonPiece>().Row;
                int topRow = top.GetComponent<HexagonPiece>().Row;

                _hexagones[column, i] = top;
                _hexagones[column, i + 1] = current;

                _hexagones[column, i].GetComponent<HexagonPiece>().Row = currentRow;
                _hexagones[column, i + 1].GetComponent<HexagonPiece>().Row = topRow;

                _hexagones[column, i].GetComponent<HexagonPiece>().TargetPosition = CalculatePosition(i, column);
                _hexagones[column, i + 1].GetComponent<HexagonPiece>().TargetPosition = CalculatePosition(i + 1, column);
            }
            gameState = GameState.Idle;
        }

        private void FillEmptyPlaces()
        {
            gameState = GameState.Filling;

            for (int column = 0; column < _gridWidth; column++)
            {
                for (int row = 0; row < missingHexagonCounter[column]; row++)
                {
                    Vector3 initPos = CalculatePosition(_gridWidth + 4, column);
                    Vector3 targetPos = CalculatePosition(_gridWidth - row, column);

                    int hexNum = Random.Range(0, _hexagonePrefabs.Length);

                    GameObject hexagon = Instantiate(_hexagonePrefabs[hexNum], initPos, Quaternion.identity, _hexagoneParent.transform);

                    _hexagones[column, _gridWidth - row] = hexagon;
                    hexagon.GetComponent<HexagonPiece>().Row = _gridWidth - row;
                    hexagon.GetComponent<HexagonPiece>().Column = column;
                    hexagon.GetComponent<HexagonPiece>().TargetPosition = targetPos;
                }
                missingHexagonCounter[column] = 0;
            }

            gameState = GameState.Idle;
        }

        private IEnumerator ExplodeMatches(List<GameObject> explodedObject)
        {
            for (int i = 0; i < explodedObject.Count; i++)
            {
                StartCoroutine(explodedObject[i].GetComponent<HexagonPiece>().Explode());

                missingHexagonCounter[explodedObject[i].GetComponent<HexagonPiece>().Column]++;
            }

            yield return new WaitForSeconds(0.5f);

            for (int i = 0; i < explodedObject.Count; i++)
            {
                int row = explodedObject[i].GetComponent<HexagonPiece>().Row;
                int column = explodedObject[i].GetComponent<HexagonPiece>().Column;

                Debug.Log("Hexagon row: " + row + " column: " + column + " will be exploded!");

                //_hexagones[column, row] = null;

                for (int j = row; j < _gridHeight - 1; j++)
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

            yield return new WaitForSeconds(0.5f);
            CheckMatches();

            //foreach (GameObject hexagon in explodedObject)
            //{
            //    int row = hexagon.GetComponent<HexagonPiece>().Row;
            //    int column = hexagon.GetComponent<HexagonPiece>().Column;

            //    Debug.Log("Hexagon row: " + row + " column: " + column + " will be exploded!");

            //    //_hexagones[column, row] = null;

            //    StartCoroutine(hexagon.GetComponent<HexagonPiece>().Explode());

            //    for (int i = row; i < _gridHeight - 1; i++)
            //    {
            //        GameObject current = _hexagones[column, i];
            //        GameObject top = _hexagones[column, i + 1];

            //        int currentRow = current.GetComponent<HexagonPiece>().Row;
            //        int topRow = top.GetComponent<HexagonPiece>().Row;

            //        _hexagones[column, i] = top;
            //        _hexagones[column, i + 1] = current;

            //        _hexagones[column, i].GetComponent<HexagonPiece>().Row = currentRow;
            //        _hexagones[column, i + 1].GetComponent<HexagonPiece>().Row = topRow;

            //        _hexagones[column, i].GetComponent<HexagonPiece>().TargetPosition = CalculatePosition(i, column);
            //        _hexagones[column, i + 1].GetComponent<HexagonPiece>().TargetPosition = CalculatePosition(i + 1, column);
            //    }
            //}
        }
    }
}