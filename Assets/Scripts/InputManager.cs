using System.Collections;
using UnityEngine;
using HexfallClone.GameController;
using HexfallClone.Hexagon;

namespace HexfallClone.PlayerInput
{
    #region ENUMS

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

    #endregion ENUMS

    public class InputManager : MonoBehaviour
    {
        #region Private Fields

        private int _hexagonLayerMask;
        private GameManager _gameManager;
        private Sides side;

        private Touch touch;
        private Vector2 startTouchPos;
        private Vector2 endTouchPos;
        private SwipeDirection _swipeDirection;

        private bool isSelected;
        private bool isMatched;

        private GameObject[] _hexagonGroup;

        #endregion Private Fields

        [SerializeField] private GameVariables _gameVariables;

        private void Start()
        {
            isSelected = false;
            isMatched = false;
            _hexagonGroup = new GameObject[3];
            _gameManager = GameManager.Instance;
            _hexagonLayerMask = LayerMask.GetMask("Hexagon");
        }

        private void Update()
        {
            if (_gameManager.GameState != GameState.Idle)
                return;

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
                                if (!isSelected)
                                    return;
                                Debug.Log("Swipe");
                                StartCoroutine(StartRotate());
                            }
                            else
                            {
                                Debug.Log("Touch");
                                SelectNeighbors();
                                isSelected = true;
                                _gameManager.IsGameStarted = true;
                            }
                            break;

                        default:
                            Debug.Log("Default STATE!");
                            break;
                    }
                }
            }
        }

        private void SelectNeighbors()
        {
            RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(startTouchPos), Vector2.zero, 0, _hexagonLayerMask);

            if (hit.collider != null)
            {
                ClearHexagonGroup();    // clear old hexagon group

                Vector3 selectedObj = Camera.main.WorldToScreenPoint(hit.transform.position);
                Vector3 mousePos = startTouchPos;

                int row = hit.transform.GetComponent<HexagonPiece>().Row;
                int column = hit.transform.GetComponent<HexagonPiece>().Column;

                int left = column - 1;
                int right = column + 1;
                int bottom = row - 1;
                int top = row + 1;

                // player touched which part of hexagon
                CheckHexagonSide(mousePos, selectedObj);

                if (column % 2 == 0)
                {
                    Debug.Log("Player touched even column!");

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
                        _hexagonGroup[0] = _gameManager.Hexagones[column, top];
                        _hexagonGroup[1] = _gameManager.Hexagones[right, row];
                        _hexagonGroup[2] = _gameManager.Hexagones[column, row];
                    }
                    else if (side == Sides.RightBot && right < _gameVariables.GridWidth && bottom >= 0)
                    {
                        _hexagonGroup[0] = _gameManager.Hexagones[column, row];
                        _hexagonGroup[1] = _gameManager.Hexagones[right, bottom];
                        _hexagonGroup[2] = _gameManager.Hexagones[column, bottom];
                    }
                    else if (side == Sides.LeftTop && left >= 0 && top < _gameVariables.GridHeight)
                    {
                        _hexagonGroup[0] = _gameManager.Hexagones[column, top];
                        _hexagonGroup[1] = _gameManager.Hexagones[column, row];
                        _hexagonGroup[2] = _gameManager.Hexagones[left, row];
                    }
                    else if (side == Sides.LeftBot && left >= 0 && bottom >= 0)
                    {
                        _hexagonGroup[0] = _gameManager.Hexagones[column, row];
                        _hexagonGroup[1] = _gameManager.Hexagones[column, bottom];
                        _hexagonGroup[2] = _gameManager.Hexagones[left, bottom];
                    }
                    else
                    {
                        Debug.Log(column);
                        Debug.Log(row);
                        Debug.Log(side);
                        Debug.Log("Conditions are not met! - Even Column");
                    }
                }
                else
                {
                    Debug.Log("Player touched odd column!");

                    // check if player input is valid
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
                        else if (row == _gameVariables.GridHeight - 1)
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
                            Debug.Log(side);
                            Debug.Log("Conditions are not met - is player input valid? !");
                        }
                    }

                    if (side == Sides.RightTop && right < _gameVariables.GridWidth && top < _gameVariables.GridHeight)
                    {
                        _hexagonGroup[0] = _gameManager.Hexagones[column, top];
                        _hexagonGroup[1] = _gameManager.Hexagones[right, top];
                        _hexagonGroup[2] = _gameManager.Hexagones[column, row];
                    }
                    else if (side == Sides.RightBot && right < _gameVariables.GridWidth && bottom >= 0)
                    {
                        _hexagonGroup[0] = _gameManager.Hexagones[column, row];
                        _hexagonGroup[1] = _gameManager.Hexagones[right, row];
                        _hexagonGroup[2] = _gameManager.Hexagones[column, bottom];
                    }
                    else if (side == Sides.LeftTop && left >= 0 && top < _gameVariables.GridHeight)
                    {
                        _hexagonGroup[0] = _gameManager.Hexagones[column, top];
                        _hexagonGroup[1] = _gameManager.Hexagones[column, row];
                        _hexagonGroup[2] = _gameManager.Hexagones[left, top];
                    }
                    else if (side == Sides.LeftBot && left >= 0 && bottom >= 0)
                    {
                        _hexagonGroup[0] = _gameManager.Hexagones[column, row];
                        _hexagonGroup[1] = _gameManager.Hexagones[column, bottom];
                        _hexagonGroup[2] = _gameManager.Hexagones[left, row];
                    }
                    else
                    {
                        Debug.Log(column);
                        Debug.Log(row);
                        Debug.Log(side);
                        Debug.Log("Conditions are not met! - Odd Column");
                    }
                }

                //active new hexagon group outline
                for (int i = 0; i < _hexagonGroup.Length; i++)
                {
                    if (_hexagonGroup[i] != null)
                    {
                        _hexagonGroup[i].transform.GetChild(0).gameObject.SetActive(true);
                    }
                }
            }
        }

        // check if it is swipe or touch
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

        /// <summary>
        /// player touched which part of hexagon
        /// </summary>
        /// <param name="mousePos"></param>
        /// <param name="selectedObj"></param>
        private void CheckHexagonSide(Vector3 mousePos, Vector3 selectedObj)
        {
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
            else
            {
                Debug.Log("Conditions are not met. - CheckHexagonSide !");
            }
        }

        /// <summary>
        /// start rotating hexagon group
        /// if it matches then it stops rotating
        /// </summary>
        /// <returns></returns>
        private IEnumerator StartRotate()
        {
            // if there hexagon group is null then do nothing
            for (int i = 0; i < 3; i++)
            {
                if (_hexagonGroup[i] == null)
                {
                    yield break;
                }
            }

            // update game state
            _gameManager.GameState = GameState.Rotating;

            // start rotating
            for (int i = 0; i < 3; i++)
            {
                StartCoroutine(RotateHexagonGroup());

                // if is matched then stop
                if (isMatched)
                {
                    _gameManager.UpdateMoveCounter();

                    yield break;
                }
                else
                {
                    // if it does not match and its end of the for loop then update game state to idle
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

        /// <summary>
        /// this method rotates hexagon group
        /// </summary>
        /// <returns></returns>
        private IEnumerator RotateHexagonGroup()
        {
            _gameManager.GameState = GameState.Rotating;

            GameObject firstHexagon = _hexagonGroup[0];
            int firstRow = firstHexagon.GetComponent<HexagonPiece>().Row;
            int firstColumn = firstHexagon.GetComponent<HexagonPiece>().Column;

            GameObject secondHexagon = _hexagonGroup[1];
            int secondRow = secondHexagon.GetComponent<HexagonPiece>().Row;
            int secondColumn = secondHexagon.GetComponent<HexagonPiece>().Column;

            GameObject thirdHexagon = _hexagonGroup[2];
            int thirdRow = thirdHexagon.GetComponent<HexagonPiece>().Row;
            int thirdColumn = thirdHexagon.GetComponent<HexagonPiece>().Column;

            // clock wise rotate 1 -> 2 -> 3 ->1
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

                _hexagonGroup[0] = thirdHexagon;
                _hexagonGroup[1] = firstHexagon;
                _hexagonGroup[2] = secondHexagon;
            }
            else
            {
                // counterclockwise rotate 1 -> 3 -> 2 -> 1
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

                _hexagonGroup[0] = secondHexagon;
                _hexagonGroup[1] = thirdHexagon;
                _hexagonGroup[2] = firstHexagon;

                //counterclockwise rotate
            }

            isMatched = _gameManager.CheckMatches();

            // checkmatches method may change game state from rotating to idle if there is no match so to prevent this we change it to ratating again
            _gameManager.GameState = GameState.Rotating;

            yield return new WaitForSeconds(0.1f);

            // if is matched deactive hexagon outline of other hexagons that were in the hexagon group
            if (isMatched)
            {
                for (int i = 0; i < 3; i++)
                {
                    if (_hexagonGroup[i] != null)
                    {
                        _hexagonGroup[i].transform.GetChild(0).gameObject.SetActive(false);
                    }
                }
            }

            yield return new WaitForSeconds(0.5f);
        }

        /// <summary>
        /// this method is only for one condition if selected row = 0 and column is width - 1 also it is odd then select neighbors is different
        /// </summary>
        /// <param name="hit"></param>
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
                    _hexagonGroup[0] = _gameManager.Hexagones[column, top];
                    _hexagonGroup[1] = _gameManager.Hexagones[right, row];
                    _hexagonGroup[2] = _gameManager.Hexagones[column, row];
                }
                else if (side == Sides.RightBot && right < _gameVariables.GridWidth && bottom >= 0)
                {
                    _hexagonGroup[0] = _gameManager.Hexagones[column, row];
                    _hexagonGroup[1] = _gameManager.Hexagones[right, bottom];
                    _hexagonGroup[2] = _gameManager.Hexagones[column, bottom];
                }
                else if (side == Sides.LeftTop && left >= 0 && top < _gameVariables.GridHeight)
                {
                    _hexagonGroup[0] = _gameManager.Hexagones[column, top];
                    _hexagonGroup[1] = _gameManager.Hexagones[column, row];
                    _hexagonGroup[2] = _gameManager.Hexagones[left, row];
                }
                else if (side == Sides.LeftBot && left >= 0 && bottom >= 0)
                {
                    _hexagonGroup[0] = _gameManager.Hexagones[column, row];
                    _hexagonGroup[1] = _gameManager.Hexagones[column, bottom];
                    _hexagonGroup[2] = _gameManager.Hexagones[left, bottom];
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
            for (int i = 0; i < _hexagonGroup.Length; i++)
            {
                if (_hexagonGroup[i] != null)
                    _hexagonGroup[i].transform.GetChild(0).gameObject.SetActive(true);
            }
        }

        /// <summary>
        /// clear hexagon group
        /// </summary>
        private void ClearHexagonGroup()
        {
            for (int i = 0; i < _hexagonGroup.Length; i++)
            {
                if (_hexagonGroup[i] != null)
                {
                    _hexagonGroup[i].transform.GetChild(0).gameObject.SetActive(false);
                }

                _hexagonGroup[i] = null;
            }
        }
    }   // input manager
}   // namepsace