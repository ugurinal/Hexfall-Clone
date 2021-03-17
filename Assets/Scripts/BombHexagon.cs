using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using HexfallClone.UISystem;

namespace HexfallClone.Hexagon
{
    public class BombHexagon : HexagonPiece
    {
        #region Private Fields

        private int _bombCounter;

        private MainUIManager _UIManager;

        #endregion Private Fields

        #region Public Fields

        public int BombCounter { get => _bombCounter; set => _bombCounter = value; }

        #endregion Public Fields

        [SerializeField] private TextMeshPro _bombText;

        private void Awake()
        {
            TargetPosition = transform.position;
        }

        private void Start()
        {
            _UIManager = MainUIManager.Instance;

            _bombText.text = "" + _bombCounter;
        }

        private void Update()
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

        public void DecreaseCounter()
        {
            _bombCounter--;
            _bombText.text = "" + _bombCounter;
            if (_bombCounter <= 0)
            {
                _UIManager.LoadGameOverScreen("BOMB EXPLOADED!");
                Debug.Log("BOMB EXPLOADED!");
                Debug.Log("GAME OVER!");
            }
        }
    }
}