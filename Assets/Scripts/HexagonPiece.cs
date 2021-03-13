using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HexfallClone.GameController;

namespace HexfallClone.Hexagon
{
    public class HexagonPiece : MonoBehaviour
    {
        [SerializeField] private HexagonColor _hexagonColor;
        [SerializeField] private int _column;
        [SerializeField] private int _row;
        [SerializeField] private float timeToMove = 5f;

        private Vector2 _originalPos;
        private Vector2 _targetPos;
        private bool _isActive = true;

        public int Row { get => _row; set => _row = value; }
        public int Column { get => _column; set => _column = value; }
        public string HexagonColor { get => _hexagonColor.ToString(); }

        public Vector2 TargetPosition { get => _targetPos; set => _targetPos = value; }

        public bool IsActive { get => _isActive; set => _isActive = value; }

        private void Awake()
        {
            _originalPos = transform.position;
            _targetPos = transform.position;
        }

        private void Start()
        {
        }

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

        public IEnumerator Explode()
        {
            IsActive = false;
            yield return new WaitForSeconds(0.5f);
            transform.GetChild(1).gameObject.SetActive(true);
            Destroy(gameObject, 0.5f);
        }
    }
}