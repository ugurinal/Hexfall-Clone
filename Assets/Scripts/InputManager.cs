using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HexfallClone.GameController;
using HexfallClone.Hexagon;

namespace HexfallClone.PlayerInput
{
    public enum Sides
    {
        RightTop,
        RightBot,
        LeftTop,
        LeftBot
    }

    public class InputManager : MonoBehaviour
    {
        [SerializeField] private GameVariables _gameVariables;
        private int _hexagonLayerMask;

        private GameManager _gameManager;
        private Sides side;

        private GameObject[] _neighbors;

        private void Start()
        {
            _neighbors = new GameObject[3];
            _gameManager = GameManager.Instance;
            _hexagonLayerMask = LayerMask.GetMask("Hexagon");
        }

        private void Update()
        {
            if (_gameManager.GameState == GameState.Idle)
            {
                if (Input.GetMouseButtonUp(0))
                {
                    Vector3 mousePosition = Input.mousePosition;
                    RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(mousePosition), Vector2.zero, 0, _hexagonLayerMask);

                    if (hit.collider != null)
                    {
                        int row = hit.transform.GetComponent<HexagonPiece>().Row;
                        int column = hit.transform.GetComponent<HexagonPiece>().Column;

                        SelectNeighbors(Camera.main.WorldToScreenPoint(hit.transform.position), column, row, mousePosition);
                    }
                }
            }
        }

        private void SelectNeighbors(Vector3 selectedObj, int column, int row, Vector3 mousePos)
        {
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
                Debug.Log("RIGHT TOP!");
                // chose top right
            }
            else if (mousePos.x >= selectedObj.x && mousePos.y < selectedObj.y)
            {
                side = Sides.RightBot;
                Debug.Log("RIGHT BOT!");
                // chose bot right
            }
            else if (mousePos.x < selectedObj.x && mousePos.y >= selectedObj.y)
            {
                side = Sides.LeftTop;
                Debug.Log("LEFT TOP!");
                // chose left top
            }
            else if (mousePos.x < selectedObj.x && mousePos.y < selectedObj.y)
            {
                side = Sides.LeftBot;
                Debug.Log("LEFT BOT!");
                // chose bot left
            }

            if (column % 2 == 0)
            {
                if (side == Sides.RightTop && right < _gameVariables.GridWidth && top < _gameVariables.GridHeight)
                {
                    _neighbors[0] = _gameManager.Hexagones[column, row];
                    _neighbors[1] = _gameManager.Hexagones[right, row];
                    _neighbors[2] = _gameManager.Hexagones[column, top];

                    //_gameManager.Hexagones[column, row].transform.GetChild(0).gameObject.SetActive(true);
                    //_gameManager.Hexagones[right, row].transform.GetChild(0).gameObject.SetActive(true);
                    //_gameManager.Hexagones[column, top].transform.GetChild(0).gameObject.SetActive(true);
                }
                else if (side == Sides.RightBot && right < _gameVariables.GridWidth && bottom >= 0)
                {
                    _neighbors[0] = _gameManager.Hexagones[column, row];
                    _neighbors[1] = _gameManager.Hexagones[right, bottom];
                    _neighbors[2] = _gameManager.Hexagones[column, bottom];

                    //_gameManager.Hexagones[column, row].transform.GetChild(0).gameObject.SetActive(true);
                    //_gameManager.Hexagones[right, bottom].transform.GetChild(0).gameObject.SetActive(true);
                    //_gameManager.Hexagones[column, bottom].transform.GetChild(0).gameObject.SetActive(true);
                }
                else if (side == Sides.LeftTop && left >= 0 && top < _gameVariables.GridHeight)
                {
                    _neighbors[0] = _gameManager.Hexagones[column, row];
                    _neighbors[1] = _gameManager.Hexagones[left, row];
                    _neighbors[2] = _gameManager.Hexagones[column, top];
                    //_gameManager.Hexagones[column, row].transform.GetChild(0).gameObject.SetActive(true);
                    //_gameManager.Hexagones[left, row].transform.GetChild(0).gameObject.SetActive(true);
                    //_gameManager.Hexagones[column, top].transform.GetChild(0).gameObject.SetActive(true);
                }
                else if (side == Sides.LeftBot && left >= 0 && bottom >= 0)
                {
                    _neighbors[0] = _gameManager.Hexagones[column, row];
                    _neighbors[1] = _gameManager.Hexagones[left, bottom];
                    _neighbors[2] = _gameManager.Hexagones[column, bottom];
                    //_gameManager.Hexagones[column, row].transform.GetChild(0).gameObject.SetActive(true);
                    //_gameManager.Hexagones[left, bottom].transform.GetChild(0).gameObject.SetActive(true);
                    //_gameManager.Hexagones[column, bottom].transform.GetChild(0).gameObject.SetActive(true);
                }
                else
                {
                    Debug.Log("Conditions are not met!");
                }
            }
            else
            {
                if (side == Sides.RightTop && right < _gameVariables.GridWidth && top < _gameVariables.GridHeight)
                {
                    _neighbors[0] = _gameManager.Hexagones[column, row];
                    _neighbors[1] = _gameManager.Hexagones[right, top];
                    _neighbors[2] = _gameManager.Hexagones[column, top];

                    //_gameManager.Hexagones[column, row].transform.GetChild(0).gameObject.SetActive(true);
                    //_gameManager.Hexagones[right, top].transform.GetChild(0).gameObject.SetActive(true);
                    //_gameManager.Hexagones[column, top].transform.GetChild(0).gameObject.SetActive(true);
                }
                else if (side == Sides.RightBot && right < _gameVariables.GridWidth && bottom >= 0)
                {
                    _neighbors[0] = _gameManager.Hexagones[column, row];
                    _neighbors[1] = _gameManager.Hexagones[right, row];
                    _neighbors[2] = _gameManager.Hexagones[column, bottom];

                    //_gameManager.Hexagones[column, row].transform.GetChild(0).gameObject.SetActive(true);
                    //_gameManager.Hexagones[right, row].transform.GetChild(0).gameObject.SetActive(true);
                    //_gameManager.Hexagones[column, bottom].transform.GetChild(0).gameObject.SetActive(true);
                }
                else if (side == Sides.LeftTop && left >= 0 && top < _gameVariables.GridHeight)
                {
                    _neighbors[0] = _gameManager.Hexagones[column, row];
                    _neighbors[1] = _gameManager.Hexagones[left, top];
                    _neighbors[2] = _gameManager.Hexagones[column, top];

                    //_gameManager.Hexagones[column, row].transform.GetChild(0).gameObject.SetActive(true);
                    //_gameManager.Hexagones[left, top].transform.GetChild(0).gameObject.SetActive(true);
                    //_gameManager.Hexagones[column, top].transform.GetChild(0).gameObject.SetActive(true);
                }
                else if (side == Sides.LeftBot && left >= 0 && bottom >= 0)
                {
                    _neighbors[0] = _gameManager.Hexagones[column, row];
                    _neighbors[1] = _gameManager.Hexagones[left, row];
                    _neighbors[2] = _gameManager.Hexagones[column, bottom];

                    //_gameManager.Hexagones[column, row].transform.GetChild(0).gameObject.SetActive(true);
                    //_gameManager.Hexagones[left, row].transform.GetChild(0).gameObject.SetActive(true);
                    //_gameManager.Hexagones[column, bottom].transform.GetChild(0).gameObject.SetActive(true);
                }
                else
                {
                    Debug.Log("Conditions are not met!");
                }
            }

            //select new ones
            for (int i = 0; i < _neighbors.Length; i++)
            {
                if (_neighbors[i] != null)
                    _neighbors[i].transform.GetChild(0).gameObject.SetActive(true);
            }
        }
    }   // input manager
}   // namepsace