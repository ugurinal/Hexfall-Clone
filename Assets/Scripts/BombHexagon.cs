using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HexfallClone.Hexagon
{
    public class BombHexagon : HexagonPiece
    {
        // Start is called before the first frame update
        private void Start()
        {
            _hexagonColor = GameController.HexagonColor.Blue | GameController.HexagonColor.Red
                | GameController.HexagonColor.Green | GameController.HexagonColor.White | GameController.HexagonColor.Yellow;

            if (_hexagonColor == GameController.HexagonColor.Blue)
            {
                Debug.Log("BLUE");
            }
            if (_hexagonColor == GameController.HexagonColor.Red)
            {
                Debug.Log("RED");
            }
            if (_hexagonColor == GameController.HexagonColor.Green)
            {
                Debug.Log("GREEN");
            }
            if (_hexagonColor == GameController.HexagonColor.White)
            {
                Debug.Log("WHITE");
            }
            if (_hexagonColor == GameController.HexagonColor.Yellow)
            {
                Debug.Log("YELLOW");
            }
        }

        // Update is called once per frame
        private void Update()
        {
            if (IsActive)
            {
                if (Vector2.Distance(transform.position, _targetPos) > 0.01f)
                {
                    transform.position = Vector2.Lerp(transform.position, _targetPos, timeToMove * Time.deltaTime);
                }
                else
                {
                    transform.position = _targetPos;
                }
            }
        }
    }
}