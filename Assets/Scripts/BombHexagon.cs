using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HexfallClone.Hexagon
{
    public class BombHexagon : HexagonPiece
    {
        private void Awake()
        {
            TargetPosition = transform.position;
        }

        private void Start()
        {
        }

        private void Update()
        {
            if (IsActive)
            {
                if (Vector2.Distance(transform.position, TargetPosition) > 0.01f)
                {
                    transform.position = Vector2.Lerp(transform.position, TargetPosition, timeToMove * Time.deltaTime);
                }
                else
                {
                    transform.position = TargetPosition;
                }
            }
        }
    }
}