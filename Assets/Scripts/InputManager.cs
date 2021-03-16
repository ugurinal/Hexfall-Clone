using System.Collections;
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
                            // if nothing is selected at the beginning of game then do not execute startrotate method
                            if (!isSelected)
                                return;
                            //Debug.Log("Starting Rotate");
                            Debug.Log("Swipe!");
                            StartCoroutine(StartRotate());
                        }
                        else
                        {
                            Debug.Log("Touch!");
                            SelectNeighbors();
                            isSelected = true;
                            _gameManager.IsGameStarted = true;
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
                Debug.Log("HIT HIT HIT");

                //
                ClearNeigbors();
                //// make old ones outline inactive
                //for (int i = 0; i < _neighbors.Length; i++)
                //{
                //    if (_neighbors[i] != null)
                //    {
                //        _neighbors[i].transform.GetChild(0).gameObject.SetActive(false);
                //    }

                //    _neighbors[i] = null;
                //}

                //ClearNeigbors();

                Vector3 selectedObj = Camera.main.WorldToScreenPoint(hit.transform.position);
                Vector3 mousePos = startTouchPos;

                int row = hit.transform.GetComponent<HexagonPiece>().Row;
                int column = hit.transform.GetComponent<HexagonPiece>().Column;

                int left = column - 1;
                int right = column + 1;
                int bottom = row - 1;
                int top = row + 1;

                if (mousePos.x >= selectedObj.x && mousePos.y >= selectedObj.y)
                {
                    side = Sides.RightTop;
                }
                else if (mousePos.x >= selectedObj.x && mousePos.y < selectedObj.y)
                {
                    side = Sides.RightBot;
                }
                else if (mousePos.x < selectedObj.x && mousePos.y >= selectedObj.y)
                {
                    side = Sides.LeftTop;
                }
                else if (mousePos.x < selectedObj.x && mousePos.y < selectedObj.y)
                {
                    side = Sides.LeftBot;
                }

                if (column % 2 == 0)
                {
                    Debug.Log("Even Column");
                    // check if user input is valid
                    if (column == 0)
                    {
                        if (row == 0)
                        {
                            side = Sides.RightTop;
                        }
                        else
                        {
                            if (row == _gameVariables.GridHeight - 1)
                            {
                                side = Sides.RightBot;
                            }
                            else
                            {
                                if (side == Sides.LeftTop)
                                {
                                    side = Sides.RightTop;
                                }
                                if (side == Sides.LeftBot)
                                {
                                    side = Sides.RightBot;
                                }
                            }
                        }
                    }
                    else if (column == _gameVariables.GridWidth - 1)
                    {
                        if (row == 0)
                        {
                            side = Sides.LeftTop;
                        }
                        else if (row == _gameVariables.GridHeight - 1)
                        {
                            side = Sides.LeftBot;
                        }
                        else
                        {
                            if (side == Sides.RightBot)
                            {
                                side = Sides.LeftBot;
                            }
                            else if (side == Sides.RightTop)
                            {
                                side = Sides.LeftTop;
                            }
                        }
                    }
                    else if (row == _gameVariables.GridHeight - 1)
                    {
                        if (column == _gameVariables.GridHeight - 1)
                        {
                            side = Sides.LeftBot;
                        }
                        else
                        {
                            if (side == Sides.RightTop)
                            {
                                side = Sides.RightBot;
                            }
                            else if (side == Sides.LeftTop)
                            {
                                side = Sides.LeftBot;
                            }
                        }
                    }
                    else if (row == 0)
                    {
                        if (side == Sides.RightBot)
                        {
                            side = Sides.RightTop;
                        }
                        else if (side == Sides.LeftBot)
                        {
                            side = Sides.LeftTop;
                        }
                    }

                    if (side == Sides.RightTop && right < _gameVariables.GridWidth && top < _gameVariables.GridHeight)
                    {
                        _neighbors[0] = _gameManager.Hexagones[column, top];
                        _neighbors[1] = _gameManager.Hexagones[right, row];
                        _neighbors[2] = _gameManager.Hexagones[column, row];
                    }
                    else if (side == Sides.RightBot && right < _gameVariables.GridWidth && bottom >= 0)
                    {
                        _neighbors[0] = _gameManager.Hexagones[column, row];
                        _neighbors[1] = _gameManager.Hexagones[right, bottom];
                        _neighbors[2] = _gameManager.Hexagones[column, bottom];
                    }
                    else if (side == Sides.LeftTop && left >= 0 && top < _gameVariables.GridHeight)
                    {
                        _neighbors[0] = _gameManager.Hexagones[column, top];
                        _neighbors[1] = _gameManager.Hexagones[column, row];
                        _neighbors[2] = _gameManager.Hexagones[left, row];
                    }
                    else if (side == Sides.LeftBot && left >= 0 && bottom >= 0)
                    {
                        _neighbors[0] = _gameManager.Hexagones[column, row];
                        _neighbors[1] = _gameManager.Hexagones[column, bottom];
                        _neighbors[2] = _gameManager.Hexagones[left, bottom];
                    }
                    else
                    {
                        Debug.Log(column);
                        Debug.Log(row);
                        Debug.Log(side);
                        Debug.Log("Conditions are not met!");
                    }
                }
                else
                {
                    Debug.Log("ODD COLUMN!");
                    if (column == _gameVariables.GridWidth - 1)
                    {
                        if (row == 0)
                        {
                            if (side == Sides.LeftBot || side == Sides.RightBot)
                            {
                                SelectNeighborsTest(hit);
                                return;
                            }
                            else if (side == Sides.RightTop)
                            {
                                side = Sides.LeftTop;
                            }
                        }
                        else if (row == _gameVariables.GridHeight - 1)
                        {
                            side = Sides.LeftBot;
                        }
                        else
                        {
                            if (side == Sides.RightTop)
                            {
                                side = Sides.LeftTop;
                            }
                            else if (side == Sides.RightBot)
                            {
                                side = Sides.LeftBot;
                            }
                        }
                    }
                    else
                    {
                        if (row == 0)
                        {
                            if (side == Sides.LeftBot)
                            {
                                side = Sides.LeftTop;
                            }
                            else if (side == Sides.RightBot)
                            {
                                side = Sides.RightTop;
                            }
                        }
                        else if (row == _gameVariables.GridWidth - 1)
                        {
                            if (side == Sides.RightTop)
                            {
                                side = Sides.RightBot;
                            }
                            else if (side == Sides.LeftTop)
                            {
                                side = Sides.LeftBot;
                            }
                        }
                        else
                        {
                            Debug.Log("TEST 2");
                            Debug.Log(side);
                        }
                    }

                    if (side == Sides.RightTop && right < _gameVariables.GridWidth && top < _gameVariables.GridHeight)
                    {
                        _neighbors[0] = _gameManager.Hexagones[column, top];
                        _neighbors[1] = _gameManager.Hexagones[right, top];
                        _neighbors[2] = _gameManager.Hexagones[column, row];
                    }
                    else if (side == Sides.RightBot && right < _gameVariables.GridWidth && bottom >= 0)
                    {
                        _neighbors[0] = _gameManager.Hexagones[column, row];
                        _neighbors[1] = _gameManager.Hexagones[right, row];
                        _neighbors[2] = _gameManager.Hexagones[column, bottom];
                    }
                    else if (side == Sides.LeftTop && left >= 0 && top < _gameVariables.GridHeight)
                    {
                        _neighbors[0] = _gameManager.Hexagones[column, top];
                        _neighbors[1] = _gameManager.Hexagones[column, row];
                        _neighbors[2] = _gameManager.Hexagones[left, top];
                    }
                    else if (side == Sides.LeftBot && left >= 0 && bottom >= 0)
                    {
                        _neighbors[0] = _gameManager.Hexagones[column, row];
                        _neighbors[1] = _gameManager.Hexagones[column, bottom];
                        _neighbors[2] = _gameManager.Hexagones[left, row];
                    }
                    else
                    {
                        Debug.Log(side);
                        Debug.Log("Conditions are not met!");
                    }
                }

                Debug.Log("Before activate new outlines");
                //active new ones outline
                for (int i = 0; i < _neighbors.Length; i++)
                {
                    Debug.Log("AAAAAAAAAAAAAA");
                    if (_neighbors[i] != null)
                    {
                        _neighbors[i].transform.GetChild(0).gameObject.SetActive(true);
                    }
                    else
                    {
                        Debug.Log("Neighbors are null!");
                    }
                }
            }
            else
            {
                Debug.Log("NO HIT NO HIT!");
            }
        }

        private bool CheckSwipe(Vector3 startPos, Vector3 endPos)
        {
            Vector3 temp = endPos - startPos;
            temp.Normalize();

            if (temp.x > 0f && temp.y < 0.5f && temp.y > -0.5f)
            {
                _swipeDirection = SwipeDirection.Right;
            }
            else if (temp.x < 0 && temp.y < 0.5f && temp.y > -0.5f)
            {
                _swipeDirection = SwipeDirection.Left;
            }
            else if (temp.y > 0f && temp.x < 0.5f && temp.x > -0.5f)
            {
                _swipeDirection = SwipeDirection.Up;
            }
            else if (temp.y < 0f && temp.x < 0.5f && temp.x > -0.5f)
            {
                _swipeDirection = SwipeDirection.Down;
            }

            return Mathf.Abs(Vector3.Distance(startPos, endPos)) > _gameVariables.SwipeSensitivity;
        }

        private IEnumerator StartRotate()
        {
            for (int i = 0; i < 3; i++)
            {
                if (_neighbors[i] == null)
                {
                    yield break;
                }
            }

            _gameManager.GameState = GameState.Rotating;

            for (int i = 0; i < 3; i++)
            {
                StartCoroutine(RotateHexagonGroup());

                if (isMatched)
                {
                    _gameManager.UpdateMoveCounter();

                    yield break;
                }
                else
                {
                    if (i == 2)
                    {
                        // wait for hexagon to rotate then change game state to idle
                        yield return new WaitForSeconds(0.3f);
                        _gameManager.GameState = GameState.Idle;
                    }
                }

                yield return new WaitForSeconds(0.3f);
            }
        }

        private IEnumerator RotateHexagonGroup()
        {
            _gameManager.GameState = GameState.Rotating;

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
                _gameManager.Hexagones[firstColumn, firstRow] = thirdHexagon;
                _gameManager.Hexagones[firstColumn, firstRow].GetComponent<HexagonPiece>().Row = firstRow;
                _gameManager.Hexagones[firstColumn, firstRow].GetComponent<HexagonPiece>().Column = firstColumn;
                _gameManager.Hexagones[firstColumn, firstRow].GetComponent<HexagonPiece>().TargetPosition = firstHexagon.transform.position;

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
                _gameManager.Hexagones[firstColumn, firstRow] = secondHexagon;
                _gameManager.Hexagones[firstColumn, firstRow].GetComponent<HexagonPiece>().Row = firstRow;
                _gameManager.Hexagones[firstColumn, firstRow].GetComponent<HexagonPiece>().Column = firstColumn;
                _gameManager.Hexagones[firstColumn, firstRow].GetComponent<HexagonPiece>().TargetPosition = firstHexagon.transform.position;

                _gameManager.Hexagones[secondColumn, secondRow] = thirdHexagon;
                _gameManager.Hexagones[secondColumn, secondRow].GetComponent<HexagonPiece>().Row = secondRow;
                _gameManager.Hexagones[secondColumn, secondRow].GetComponent<HexagonPiece>().Column = secondColumn;
                _gameManager.Hexagones[secondColumn, secondRow].GetComponent<HexagonPiece>().TargetPosition = secondHexagon.transform.position;

                _gameManager.Hexagones[thirdColumn, thirdRow] = firstHexagon;
                _gameManager.Hexagones[thirdColumn, thirdRow].GetComponent<HexagonPiece>().Row = thirdRow;
                _gameManager.Hexagones[thirdColumn, thirdRow].GetComponent<HexagonPiece>().Column = thirdColumn;
                _gameManager.Hexagones[thirdColumn, thirdRow].GetComponent<HexagonPiece>().TargetPosition = thirdHexagon.transform.position;

                _neighbors[0] = secondHexagon;
                _neighbors[1] = thirdHexagon;
                _neighbors[2] = firstHexagon;

                //counterclockwise rotate
            }

            isMatched = _gameManager.CheckMatches();

            // checkmatches may change game state from rotating to idle if there is no match so to prevent this we change it to ratating again
            _gameManager.GameState = GameState.Rotating;

            yield return new WaitForSeconds(0.1f);

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

            yield return new WaitForSeconds(0.5f);
        }

        private void SelectNeighborsTest(RaycastHit2D hit)
        {
            Debug.Log("New Selection Func");
            Vector3 selectedObj = Camera.main.WorldToScreenPoint(hit.transform.position);
            Vector3 mousePos = startTouchPos;

            int row = hit.transform.GetComponent<HexagonPiece>().Row;
            int column = hit.transform.GetComponent<HexagonPiece>().Column;

            column--;

            int left = column - 1;
            int right = column + 1;
            int bottom = row - 1;
            int top = row + 1;

            if (mousePos.x >= selectedObj.x && mousePos.y >= selectedObj.y)
            {
                side = Sides.RightTop;
            }
            else if (mousePos.x >= selectedObj.x && mousePos.y < selectedObj.y)
            {
                side = Sides.RightBot;
            }
            else if (mousePos.x < selectedObj.x && mousePos.y >= selectedObj.y)
            {
                side = Sides.LeftTop;
            }
            else if (mousePos.x < selectedObj.x && mousePos.y < selectedObj.y)
            {
                side = Sides.LeftBot;
            }

            if (column % 2 == 0)
            {
                // check if user input is valid
                if (column == _gameVariables.GridWidth - 2)
                {
                    if (row == 0)
                    {
                        side = Sides.RightTop;
                    }
                    else if (row == _gameVariables.GridHeight - 1)
                    {
                        side = Sides.LeftBot;
                    }
                    else
                    {
                        if (side == Sides.RightTop)
                        {
                            side = Sides.LeftTop;
                        }
                        else if (side == Sides.RightBot)
                        {
                            side = Sides.LeftBot;
                        }
                    }
                }
                else
                {
                    if (row == 0)
                    {
                        if (side == Sides.LeftBot)
                        {
                            side = Sides.LeftTop;
                        }
                        else if (side == Sides.RightBot)
                        {
                            side = Sides.RightTop;
                        }
                    }
                    else if (row == _gameVariables.GridWidth - 1)
                    {
                        if (side == Sides.RightTop)
                        {
                            side = Sides.RightBot;
                        }
                        else if (side == Sides.LeftTop)
                        {
                            side = Sides.LeftBot;
                        }
                    }
                }

                if (side == Sides.RightTop && right < _gameVariables.GridWidth && top < _gameVariables.GridHeight)
                {
                    _neighbors[0] = _gameManager.Hexagones[column, top];
                    _neighbors[1] = _gameManager.Hexagones[right, row];
                    _neighbors[2] = _gameManager.Hexagones[column, row];
                }
                else if (side == Sides.RightBot && right < _gameVariables.GridWidth && bottom >= 0)
                {
                    _neighbors[0] = _gameManager.Hexagones[column, row];
                    _neighbors[1] = _gameManager.Hexagones[right, bottom];
                    _neighbors[2] = _gameManager.Hexagones[column, bottom];
                }
                else if (side == Sides.LeftTop && left >= 0 && top < _gameVariables.GridHeight)
                {
                    _neighbors[0] = _gameManager.Hexagones[column, top];
                    _neighbors[1] = _gameManager.Hexagones[column, row];
                    _neighbors[2] = _gameManager.Hexagones[left, row];
                }
                else if (side == Sides.LeftBot && left >= 0 && bottom >= 0)
                {
                    _neighbors[0] = _gameManager.Hexagones[column, row];
                    _neighbors[1] = _gameManager.Hexagones[column, bottom];
                    _neighbors[2] = _gameManager.Hexagones[left, bottom];
                }
                else
                {
                    Debug.Log(column);
                    Debug.Log(row);
                    Debug.Log(side);
                    Debug.Log("Conditions are not met!");
                }
            }

            //active new ones outline
            for (int i = 0; i < _neighbors.Length; i++)
            {
                if (_neighbors[i] != null)
                    _neighbors[i].transform.GetChild(0).gameObject.SetActive(true);
            }
        }

        private void ClearNeigbors()
        {
            Debug.Log("In clear!");
            Debug.Log(_neighbors.Length);
            for (int i = 0; i < _neighbors.Length; i++)
            {
                if (_neighbors[i] != null)
                {
                    _neighbors[i].transform.GetChild(0).gameObject.SetActive(false);
                    _neighbors[i] = null;
                }

                _neighbors[i] = null;
            }
        }
    }   // input manager
}   // namepsace