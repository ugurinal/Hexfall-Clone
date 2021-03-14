using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HexfallClone.GameController
{
    [CreateAssetMenu(menuName = "HexfallClone/Game/GameVariables")]
    public class GameVariables : ScriptableObject
    {
        [Header("Grid")]
        [Range(1, 15)] public int GridWidth = 8;
        [Range(1, 15)] public int GridHeight = 9;
        public float GapBetweenHexagones = 0.1f;

        [Header("Hexagon")]
        [Space(7.5f)]
        public GameObject[] HexagonPrefabs;
        public float HexagonWidth = 0.7f;
        public float HexagonHeight = 0.7f;

        [Header("Game")]
        [Space(7.5f)]
        [Range(0.1f, 5f)] public float GameSpeed = 0.25f;
        public float SwipeSensitivity = 20f;
        public int ScorePerHexagon = 5;
    }
}