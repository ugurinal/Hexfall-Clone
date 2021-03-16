using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HexfallClone.GameController
{
    [CreateAssetMenu(menuName = "HexfallClone/Game/GameVariables")]
    public class GameVariables : ScriptableObject
    {
        [Header("Grid")]
        [Range(4f, 12f)] public int GridWidth = 8;
        [Range(4f, 12f)] public int GridHeight = 9;
        public float GapBetweenHexagones = 0.1f;

        [Header("Default Hexagons")]
        [Space(10f)]
        public GameObject[] HexagonPrefabs;
        public float HexagonWidth = 0.7f;
        public float HexagonHeight = 0.7f;
        [Range(20f, 50f)] public float HexagonMovementSpeed = 20f;

        [Header("Bomb Hexagons")]
        [Space(10f)]
        public GameObject[] BombPrefabs;
        public int BombLife = 5;

        [Header("Game")]
        [Space(10f)]
        [Range(0.1f, 5f)] public float ExplosionTime = 0.25f;
        public float SwipeSensitivity = 20f;
        public int ScorePerHexagon = 5;
        public int ScoreForBomb = 200;

        [Header("Possible Matches Ray Directions")]
        [Space(10f)]
        public List<Vector2> RaycastDirections;
    }
}