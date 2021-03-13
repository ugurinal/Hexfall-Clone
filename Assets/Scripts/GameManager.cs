using System.Collections;
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

        private Vector3 startPos;

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

            _hexagones = new GameObject[_gridWidth, _gridHeight];

            AddGap();
            CalculateStartPosition();
            InitGame();
            CheckMatches();
        }

        private void AddGap()
        {
            _hexagoneHeight += _gridGap;
            _hexagoneWidth += _gridGap;
        }

        private void InitGame()
        {
            CalculateStartPosition();

            for (int column = 0; column < _gridWidth; column++)
            {
                for (int row = 0; row < _gridHeight; row++)
                {
                    Vector3 hexPos = CalculatePosition(row, column);

                    int hexNum = Random.Range(0, _hexagonePrefabs.Length);

                    GameObject hexagon = Instantiate(_hexagonePrefabs[hexNum], hexPos, Quaternion.identity, _hexagoneParent.transform);
                    _hexagones[column, row] = hexagon;
                    hexagon.GetComponent<HexagonPiece>().Row = row;
                    hexagon.GetComponent<HexagonPiece>().Column = column;
                }
            }
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
            for (int column = 0; column < _gridWidth; column++)
            {
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
                                UpdateBoardTest(column, row);
                                UpdateBoardTest(right, row);
                                UpdateBoardTest(column, top);
                            }
                        }

                        // right bottom then top
                        if (right < _gridWidth && bottom >= 0)
                        {
                            if (currentHexagon.GetComponent<HexagonPiece>().HexagonColor == _hexagones[right, bottom].GetComponent<HexagonPiece>().HexagonColor &&
                                currentHexagon.GetComponent<HexagonPiece>().HexagonColor == _hexagones[right, row].GetComponent<HexagonPiece>().HexagonColor)
                            {
                                //UpdateBoard(_hexagones[column, row], _hexagones[right, bottom], _hexagones[right, row]);

                                UpdateBoardTest(column, row);
                                UpdateBoardTest(right, bottom);
                                UpdateBoardTest(right, row);
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

                                UpdateBoardTest(column, row);
                                UpdateBoardTest(right, top);
                                UpdateBoardTest(column, top);
                            }
                        }

                        // right bottom then top
                        if (right < _gridWidth && top < _gridHeight)
                        {
                            if (currentHexagon.GetComponent<HexagonPiece>().HexagonColor == _hexagones[right, row].GetComponent<HexagonPiece>().HexagonColor &&
                                currentHexagon.GetComponent<HexagonPiece>().HexagonColor == _hexagones[right, top].GetComponent<HexagonPiece>().HexagonColor)
                            {
                                // UpdateBoard(_hexagones[column, row], _hexagones[right, row], _hexagones[right, top]);

                                UpdateBoardTest(column, row);
                                UpdateBoardTest(right, row);
                                UpdateBoardTest(right, top);
                            }
                        }
                    }
                }
            }
        }

        private void UpdateBoard(GameObject first, GameObject second, GameObject third)
        {
            int firstColumn = first.GetComponent<HexagonPiece>().Column;
            int firstRow = first.GetComponent<HexagonPiece>().Row;

            int secondColumn = second.GetComponent<HexagonPiece>().Column;
            int secondRow = second.GetComponent<HexagonPiece>().Row;

            int thirdColumn = third.GetComponent<HexagonPiece>().Column;
            int thirdRow = third.GetComponent<HexagonPiece>().Row;

            _hexagones[firstColumn, firstRow].SetActive(false);

            // check first column
            for (int row = firstRow; row < _gridHeight - 1; row++)
            {
                Debug.Log("FIRST FOR!");

                int tempRow;

                GameObject temp = _hexagones[firstColumn, row];
                tempRow = temp.GetComponent<HexagonPiece>().Row;

                _hexagones[firstColumn, row] = _hexagones[firstColumn, row + 1];
                _hexagones[firstColumn, row].GetComponent<HexagonPiece>().Row = _hexagones[firstColumn, row + 1].GetComponent<HexagonPiece>().Row;
                _hexagones[firstColumn, row + 1] = temp;
                _hexagones[firstColumn, row + 1].GetComponent<HexagonPiece>().Row = tempRow;

                _hexagones[firstColumn, row].GetComponent<HexagonPiece>().TargetPosition = CalculatePosition(row + 1, firstColumn);

                //_hexagones[firstColumn, row].GetComponent<HexagonPiece>().TargetPosition = _hexagones[firstColumn, row + 1].transform.position;
            }

            _hexagones[secondColumn, secondRow].SetActive(false);

            // check second column
            for (int row = secondRow; row < _gridHeight - 1; row++)
            {
                Debug.Log("SECOND FOR!");
                int tempRow;

                GameObject temp = _hexagones[secondColumn, row];
                tempRow = temp.GetComponent<HexagonPiece>().Row;

                _hexagones[secondColumn, row] = _hexagones[secondColumn, row + 1];
                _hexagones[secondColumn, row].GetComponent<HexagonPiece>().Row = _hexagones[secondColumn, row + 1].GetComponent<HexagonPiece>().Row;
                _hexagones[secondColumn, row + 1] = temp;
                _hexagones[secondColumn, row + 1].GetComponent<HexagonPiece>().Row = tempRow;

                _hexagones[secondColumn, row].GetComponent<HexagonPiece>().TargetPosition = CalculatePosition(row + 1, secondColumn);
            }

            _hexagones[thirdColumn, thirdRow].SetActive(false);

            // check third column
            for (int row = thirdRow; row < _gridHeight - 1; row++)
            {
                Debug.Log("THIRD FOR!");
                int tempRow;

                GameObject temp = _hexagones[thirdColumn, row];
                tempRow = temp.GetComponent<HexagonPiece>().Row;

                _hexagones[thirdColumn, row] = _hexagones[thirdColumn, row + 1];
                _hexagones[thirdColumn, row].GetComponent<HexagonPiece>().Row = _hexagones[thirdColumn, row + 1].GetComponent<HexagonPiece>().Row;
                _hexagones[thirdColumn, row + 1] = temp;
                _hexagones[thirdColumn, row + 1].GetComponent<HexagonPiece>().Row = tempRow;

                _hexagones[thirdColumn, row].GetComponent<HexagonPiece>().TargetPosition = CalculatePosition(row + 1, thirdColumn);
            }
        }

        private void UpdateBoardTest(int column, int row)
        {
            _hexagones[column, row].SetActive(false);

            for (int i = row; i < _gridHeight - 1; i++)
            {
                Debug.Log("IN FOR!");
                GameObject current = _hexagones[column, i];
                GameObject top = _hexagones[column, i + 1];

                _hexagones[column, i] = top;
                _hexagones[column, i + 1] = current;

                _hexagones[column, i].GetComponent<HexagonPiece>().Row = current.GetComponent<HexagonPiece>().Row;
                _hexagones[column, i + 1].GetComponent<HexagonPiece>().Row = top.GetComponent<HexagonPiece>().Row;

                _hexagones[column, i].GetComponent<HexagonPiece>().TargetPosition = CalculatePosition(i, column);

                /*

                _hexagones[column, i] = top;

                top.GetComponent<HexagonPiece>().Row = current.GetComponent<HexagonPiece>().Row;
                top.GetComponent<HexagonPiece>().TargetPosition = CalculatePosition(i, column);*/
            }
        }
    }
}