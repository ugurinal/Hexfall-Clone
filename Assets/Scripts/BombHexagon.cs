using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace HexfallClone.Hexagon
{
    public class BombHexagon : HexagonPiece
    {
        private int _bombCounter;

        public int BombCounter { get => _bombCounter; set => _bombCounter = value; }

        private void Awake()
        {
            TargetPosition = transform.position;
        }

        private void Start()
        {
            _bombCounter = 6;
            transform.GetChild(2).GetComponent<TextMeshPro>().text = "" + _bombCounter;
        }

        private void Update()
        {
            if (IsActive)
            {
                if (Vector2.Distance(transform.position, TargetPosition) > 0.01f)
                {
                    transform.position = Vector2.Lerp(transform.position, TargetPosition, MovementSpeed * Time.deltaTime);
                }
                else
                {
                    transform.position = TargetPosition;
                }
            }
        }

        public void DecreaseCounter()
        {
            _bombCounter--;
            transform.GetChild(2).GetComponent<TextMeshPro>().text = "" + _bombCounter;
            if (_bombCounter <= 0)
            {
                Debug.Log("GAME OVER!");
            }
        }
    }
}