using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HexfallClone.GameController
{
    [CreateAssetMenu(menuName = "HexfallClone/Game/GameVariables")]
    public class GameVariables : ScriptableObject
    {
        [Range(1, 15)] public int GridWidth;
        [Range(1, 15)] public int GridHeight;
        public GameObject[] HexagonPrefabs;
        public float HexagonWidth;
        public float HexagonHeight;
        public float GapBetweenHexagones;
    }
}