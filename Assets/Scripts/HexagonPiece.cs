using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HexfallClone.GameManager;

namespace HexfallClone.Hexagon
{
    public class HexagonPiece : MonoBehaviour
    {
        [SerializeField] private HexagonColor _hexagonColor;
        [SerializeField] private int _column;
        [SerializeField] private int _row;

        public int Row { get => _row; set => _row = value; }
        public int Column { get => _column; set => _column = value; }
        public string HexagonColor { get => _hexagonColor.ToString(); }
    }
}