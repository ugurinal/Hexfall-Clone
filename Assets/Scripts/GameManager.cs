using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HexfallClone.Hexagon;

namespace HexfallClone.GameManager
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
        [SerializeField] private GameObject _hexagonParent;
        [SerializeField] private GameObject[] _hexagonPrefabs;

        [SerializeField] private int _gridWidth;
        [SerializeField] private int _gridHeight;
        [SerializeField] private float _gridGap;

        [SerializeField] private float _hexagonWidth;
        [SerializeField] private float _hexagonHeight;

        private GameObject[,] _hexagons;
        private Vector3 startPos;

        private void Start()
        {
            // Row = Height
            // Column = Width

            _hexagons = new GameObject[_gridWidth, _gridHeight];

            AddGap();
            CalculateStartPosition();
            InitGame();
            CheckMatches();
        }

        private void AddGap()
        {
            _hexagonHeight += _gridGap;
            _hexagonWidth += _gridGap;
        }

        private void InitGame()
        {
            CalculateStartPosition();

            for (int column = 0; column < _gridWidth; column++)
            {
                for (int row = 0; row < _gridHeight; row++)
                {
                    Vector3 hexPos = CalculatePosition(row, column);

                    int hexNum = Random.Range(0, _hexagonPrefabs.Length);

                    GameObject hexagon = Instantiate(_hexagonPrefabs[hexNum], hexPos, Quaternion.identity, _hexagonParent.transform);
                    _hexagons[column, row] = hexagon;
                    hexagon.GetComponent<HexagonPiece>().Row = row;
                    hexagon.GetComponent<HexagonPiece>().Column = column;
                }
            }
        }

        private void CalculateStartPosition()
        {
            float x = -_hexagonWidth * (_gridWidth / 2.0f) + _hexagonWidth;
            float y = -_hexagonHeight * (_gridHeight / 2.0f);
            startPos = new Vector3(x, y, 0);
        }

        private Vector3 CalculatePosition(int column, int row)
        {
            float offset = 0;

            if (row % 2 != 0)
            {
                offset = _hexagonHeight / 2.0f;
            }

            float x = startPos.x + (row * _hexagonWidth) * 0.9f;
            float y = startPos.y + (column * _hexagonHeight) + offset;

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

                    GameObject currentHexagon = _hexagons[column, row];

                    if (column % 2 == 0)
                    {
                        // right then top left
                        if (right < _gridWidth && top < _gridHeight)
                        {
                            Debug.Log(right + " " + row);
                            Debug.Log(column + " " + top);

                            if (currentHexagon.GetComponent<HexagonPiece>().HexagonColor == _hexagons[right, row].GetComponent<HexagonPiece>().HexagonColor &&
                                currentHexagon.GetComponent<HexagonPiece>().HexagonColor == _hexagons[column, top].GetComponent<HexagonPiece>().HexagonColor)
                            {
                                Debug.Log("Current Hexagon column : " + column + " Row: " + row);

                                currentHexagon.SetActive(false);
                                _hexagons[right, row].SetActive(false);
                                _hexagons[column, top].SetActive(false);
                            }
                        }

                        // right bottom then top
                        if (right < _gridWidth && bottom >= 0)
                        {
                            Debug.Log(right + " " + bottom);
                            Debug.Log(right + " " + row);

                            if (currentHexagon.GetComponent<HexagonPiece>().HexagonColor == _hexagons[right, bottom].GetComponent<HexagonPiece>().HexagonColor &&
                                currentHexagon.GetComponent<HexagonPiece>().HexagonColor == _hexagons[right, row].GetComponent<HexagonPiece>().HexagonColor)
                            {
                                Debug.Log("Current Hexagon column : " + column + " Row: " + row);

                                currentHexagon.SetActive(false);
                                _hexagons[right, bottom].SetActive(false);
                                _hexagons[right, row].SetActive(false);
                            }
                        }
                    }
                    else
                    {
                        // right then top left
                        if (right < _gridWidth && top < _gridHeight)
                        {
                            Debug.Log(right + " " + top);
                            Debug.Log(column + " " + top);

                            if (currentHexagon.GetComponent<HexagonPiece>().HexagonColor == _hexagons[right, top].GetComponent<HexagonPiece>().HexagonColor &&
                                currentHexagon.GetComponent<HexagonPiece>().HexagonColor == _hexagons[column, top].GetComponent<HexagonPiece>().HexagonColor)
                            {
                                Debug.Log("Current Hexagon column : " + column + " Row: " + row);

                                currentHexagon.SetActive(false);
                                _hexagons[right, top].SetActive(false);
                                _hexagons[column, top].SetActive(false);
                            }
                        }

                        // right bottom then top
                        if (right < _gridWidth && top < _gridHeight)
                        {
                            Debug.Log(right + " " + row);
                            Debug.Log(right + " " + top);

                            if (currentHexagon.GetComponent<HexagonPiece>().HexagonColor == _hexagons[right, row].GetComponent<HexagonPiece>().HexagonColor &&
                                currentHexagon.GetComponent<HexagonPiece>().HexagonColor == _hexagons[right, top].GetComponent<HexagonPiece>().HexagonColor)
                            {
                                Debug.Log("Current Hexagon column : " + column + " Row: " + row);

                                currentHexagon.SetActive(false);
                                _hexagons[right, row].SetActive(false);
                                _hexagons[right, top].SetActive(false);
                            }
                        }
                    }
                }
            }
        }
    }
}