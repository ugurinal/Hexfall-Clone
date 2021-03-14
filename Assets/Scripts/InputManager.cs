﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HexfallClone.GameController;
using HexfallClone.Hexagon;

namespace HexfallClone.PlayerInput
{
    // for neighbors
    public enum Sides
    {
        RightTop,
        RightBot,
        LeftTop,
        LeftBot
    }

    // for swipe
    public enum SwipeDirection
    {
        Up,
        Down,
        Left,
        Right
    }

    public class InputManager : MonoBehaviour
    {
        [SerializeField] private GameVariables _gameVariables;
        private int _hexagonLayerMask;

        private GameManager _gameManager;
        private Sides side;

        private GameObject[] _neighbors;

        private Touch touch;
        private Vector2 startTouchPos;
        private Vector2 endTouchPos;
        private SwipeDirection _swipeDirection;

        private bool isSelected;
        private bool isMatched;

        private void Start()
        {
            isSelected = false;
            isMatched = false;
            _neighbors = new GameObject[3];
            _gameManager = GameManager.Instance;
            _hexagonLayerMask = LayerMask.GetMask("Hexagon");
        }

        private void Update()
        {
            if (_gameManager.GameState == GameState.Idle)
            {
                if (Application.platform == RuntimePlatform.WindowsEditor)
                {
                    if (Input.GetMouseButtonDown(0))
                    {
                        startTouchPos = Input.mousePosition;
                    }
                    if (Input.GetMouseButtonUp(0))
                    {
                        endTouchPos = Input.mousePosition;

                        if (CheckSwipe(startTouchPos, endTouchPos))
                        {
                            if (!isSelected)
                                return;
                            //Debug.Log("Starting Rotate");
                            StartCoroutine(StartRotate());
                        }
                        else
                        {
                            SelectNeighbors();
                            //Debug.Log("Touch");
                            isSelected = true;
                        }
                    }
                }
                else if (Application.platform == RuntimePlatform.Android)
                {
                    if (Input.touchCount > 0)
                    {
                        touch = Input.GetTouch(0);

                        switch (touch.phase)
                        {
                            case TouchPhase.Began:
                                startTouchPos = touch.position;
                                break;

                            case TouchPhase.Ended:
                                endTouchPos = touch.position;

                                if (CheckSwipe(startTouchPos, endTouchPos))
                                {
                                    Debug.Log("Swipe");
                                }
                                else
                                {
                                    SelectNeighbors();
                                    isSelected = true;
                                    Debug.Log("Touch");
                                }
                                break;

                            default:
                                Debug.Log("Default STATE!");
                                break;
                        }
                    }
                }
            }
        }

        private void SelectNeighbors()
        {
            RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(startTouchPos), Vector2.zero, 0, _hexagonLayerMask);

            if (hit.collider != null)
            {
                Vector3 selectedObj = Camera.main.WorldToScreenPoint(hit.transform.position);
                Vector3 mousePos = startTouchPos;

                //Debug.Log(selectedObj);
                //Debug.Log(mousePos);

                int row = hit.transform.GetComponent<HexagonPiece>().Row;
                int column = hit.transform.GetComponent<HexagonPiece>().Column;

                // deselect old ones
                for (int i = 0; i < _neighbors.Length; i++)
                {
                    if (_neighbors[i] != null)
                        _neighbors[i].transform.GetChild(0).gameObject.SetActive(false);
                }

                int left = column - 1;
                int right = column + 1;
                int bottom = row - 1;
                int top = row + 1;

                if (mousePos.x >= selectedObj.x && mousePos.y >= selectedObj.y)
                {
                    side = Sides.RightTop;
                    //Debug.Log("RIGHT TOP!");
                    // chose top right
                }
                else if (mousePos.x >= selectedObj.x && mousePos.y < selectedObj.y)
                {
                    side = Sides.RightBot;
                    //Debug.Log("RIGHT BOT!");
                    // chose bot right
                }
                else if (mousePos.x < selectedObj.x && mousePos.y >= selectedObj.y)
                {
                    side = Sides.LeftTop;
                    //Debug.Log("LEFT TOP!");
                    // chose left top
                }
                else if (mousePos.x < selectedObj.x && mousePos.y < selectedObj.y)
                {
                    side = Sides.LeftBot;
                    //Debug.Log("LEFT BOT!");
                    // chose bot left
                }

                if (column % 2 == 0)
                {
                    if (side == Sides.RightTop && right < _gameVariables.GridWidth && top < _gameVariables.GridHeight)
                    {
                        _neighbors[0] = _gameManager.Hexagones[column, row];
                        _neighbors[1] = _gameManager.Hexagones[right, row];
                        _neighbors[2] = _gameManager.Hexagones[column, top];
                    }
                    else if (side == Sides.RightBot && right < _gameVariables.GridWidth && bottom >= 0)
                    {
                        _neighbors[0] = _gameManager.Hexagones[column, row];
                        _neighbors[1] = _gameManager.Hexagones[right, bottom];
                        _neighbors[2] = _gameManager.Hexagones[column, bottom];
                    }
                    else if (side == Sides.LeftTop && left >= 0 && top < _gameVariables.GridHeight)
                    {
                        _neighbors[0] = _gameManager.Hexagones[column, row];
                        _neighbors[1] = _gameManager.Hexagones[left, row];
                        _neighbors[2] = _gameManager.Hexagones[column, top];
                    }
                    else if (side == Sides.LeftBot && left >= 0 && bottom >= 0)
                    {
                        _neighbors[0] = _gameManager.Hexagones[column, row];
                        _neighbors[1] = _gameManager.Hexagones[left, bottom];
                        _neighbors[2] = _gameManager.Hexagones[column, bottom];
                    }
                    else
                    {
                        //Debug.Log("Conditions are not met!");
                    }
                }
                else
                {
                    if (side == Sides.RightTop && right < _gameVariables.GridWidth && top < _gameVariables.GridHeight)
                    {
                        _neighbors[0] = _gameManager.Hexagones[column, row];
                        _neighbors[1] = _gameManager.Hexagones[right, top];
                        _neighbors[2] = _gameManager.Hexagones[column, top];
                    }
                    else if (side == Sides.RightBot && right < _gameVariables.GridWidth && bottom >= 0)
                    {
                        _neighbors[0] = _gameManager.Hexagones[column, row];
                        _neighbors[1] = _gameManager.Hexagones[right, row];
                        _neighbors[2] = _gameManager.Hexagones[column, bottom];
                    }
                    else if (side == Sides.LeftTop && left >= 0 && top < _gameVariables.GridHeight)
                    {
                        _neighbors[0] = _gameManager.Hexagones[column, row];
                        _neighbors[1] = _gameManager.Hexagones[left, top];
                        _neighbors[2] = _gameManager.Hexagones[column, top];
                    }
                    else if (side == Sides.LeftBot && left >= 0 && bottom >= 0)
                    {
                        _neighbors[0] = _gameManager.Hexagones[column, row];
                        _neighbors[1] = _gameManager.Hexagones[left, row];
                        _neighbors[2] = _gameManager.Hexagones[column, bottom];
                    }
                    else
                    {
                        //Debug.Log("Conditions are not met!");
                    }
                }

                //select new ones
                for (int i = 0; i < _neighbors.Length; i++)
                {
                    if (_neighbors[i] != null)
                        _neighbors[i].transform.GetChild(0).gameObject.SetActive(true);
                }
            }
        }

        private bool CheckSwipe(Vector3 startPos, Vector3 endPos)
        {
            Vector3 temp = endPos - startPos;
            temp.Normalize();

            if (temp.x > 0f && temp.y < 0.5f && temp.y > -0.5f)
            {
                _swipeDirection = SwipeDirection.Right;
                //Debug.Log(_swipeDirection);
            }
            else if (temp.x < 0 && temp.y < 0.5f && temp.y > -0.5f)
            {
                _swipeDirection = SwipeDirection.Left;
                //Debug.Log(_swipeDirection);
            }
            else if (temp.y > 0f && temp.x < 0.5f && temp.x > -0.5f)
            {
                _swipeDirection = SwipeDirection.Up;
                //Debug.Log(_swipeDirection);
            }
            else if (temp.y < 0f && temp.x < 0.5f && temp.x > -0.5f)
            {
                _swipeDirection = SwipeDirection.Down;
                //Debug.Log(_swipeDirection);
            }

            //Debug.Log(Mathf.Abs(Vector3.Distance(startPos, endPos)));
            return Mathf.Abs(Vector3.Distance(startPos, endPos)) > _gameVariables.SwipeSensitivity;
        }

        private IEnumerator StartRotate()
        {
            for (int i = 0; i < 3; i++)
            {
                StartCoroutine(RotateHexagonGroup());

                if (isMatched)
                {
                    Debug.Log("IN BREAK !!!!!");
                    yield break;
                }

                //Debug.Log("BREAK NOT WORKS!");
                yield return new WaitForSeconds(0.7f);
            }
        }

        private IEnumerator RotateHexagonGroup()
        {
            for (int i = 0; i < 3; i++)
            {
                if (_neighbors[i] == null)
                {
                    yield break;
                }
            }

            _gameManager.GameState = GameState.Rotating;
            //Debug.Log("In Rotate Hexagon Group!");
            GameObject firstHexagon = _neighbors[0];
            int firstRow = firstHexagon.GetComponent<HexagonPiece>().Row;
            int firstColumn = firstHexagon.GetComponent<HexagonPiece>().Column;

            GameObject secondHexagon = _neighbors[1];
            int secondRow = secondHexagon.GetComponent<HexagonPiece>().Row;
            int secondColumn = secondHexagon.GetComponent<HexagonPiece>().Column;

            GameObject thirdHexagon = _neighbors[2];
            int thirdRow = thirdHexagon.GetComponent<HexagonPiece>().Row;
            int thirdColumn = thirdHexagon.GetComponent<HexagonPiece>().Column;

            if (_swipeDirection == SwipeDirection.Right || _swipeDirection == SwipeDirection.Down)
            {
                //Debug.Log("SWIPING!!!!!!!!");

                _gameManager.Hexagones[firstColumn, firstRow] = thirdHexagon;
                _gameManager.Hexagones[firstColumn, firstRow].GetComponent<HexagonPiece>().Row = firstRow;
                _gameManager.Hexagones[firstColumn, firstRow].GetComponent<HexagonPiece>().Column = firstColumn;
                _gameManager.Hexagones[firstColumn, firstRow].GetComponent<HexagonPiece>().TargetPosition = firstHexagon.transform.position;

                //Debug.Log(_gameManager.Hexagones[firstColumn, firstRow].GetComponent<HexagonPiece>().TargetPosition);
                //Debug.Log(firstHexagon.transform.position);

                _gameManager.Hexagones[secondColumn, secondRow] = firstHexagon;
                _gameManager.Hexagones[secondColumn, secondRow].GetComponent<HexagonPiece>().Row = secondRow;
                _gameManager.Hexagones[secondColumn, secondRow].GetComponent<HexagonPiece>().Column = secondColumn;
                _gameManager.Hexagones[secondColumn, secondRow].GetComponent<HexagonPiece>().TargetPosition = secondHexagon.transform.position;

                _gameManager.Hexagones[thirdColumn, thirdRow] = secondHexagon;
                _gameManager.Hexagones[thirdColumn, thirdRow].GetComponent<HexagonPiece>().Row = thirdRow;
                _gameManager.Hexagones[thirdColumn, thirdRow].GetComponent<HexagonPiece>().Column = thirdColumn;
                _gameManager.Hexagones[thirdColumn, thirdRow].GetComponent<HexagonPiece>().TargetPosition = thirdHexagon.transform.position;

                _neighbors[0] = thirdHexagon;
                _neighbors[1] = firstHexagon;
                _neighbors[2] = secondHexagon;

                //clockwise rotate
            }
            else
            {
                //counterclockwise rotate
            }

            isMatched = _gameManager.CheckMatches();
            if (isMatched)
            {
                for (int i = 0; i < 3; i++)
                {
                    if (_neighbors[i] != null)
                    {
                        _neighbors[i].transform.GetChild(0).gameObject.SetActive(false);
                    }
                }
            }
            yield return new WaitForSeconds(0.7f);

            _gameManager.GameState = GameState.Idle;
        }
    }   // input manager
}   // namepsace