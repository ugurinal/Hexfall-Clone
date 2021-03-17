using System.Collections;
using UnityEngine;
using HexfallClone.GameController;

namespace HexfallClone.Hexagon
{
    public class HexagonPiece : MonoBehaviour
    {
        #region Private Fields

        private int _column;
        private int _row;

        private Vector2 _targetPos;
        private float _movementSpeed;

        #endregion Private Fields

        #region Public Fields

        public int Row { get => _row; set => _row = value; }
        public int Column { get => _column; set => _column = value; }
        public string HexagonColor { get => _hexagonColor.ToString(); }

        public float MovementSpeed { get => _movementSpeed; set => _movementSpeed = value; }

        public Vector2 TargetPosition { get => _targetPos; set => _targetPos = value; }

        #endregion Public Fields

        [SerializeField] private HexagonColor _hexagonColor;    // to select hexagon color from inspector

        private void Awake()
        {
            _targetPos = transform.position;
        }

        private void Update()
        {
            if (Vector2.Distance(transform.position, _targetPos) > 0.01f)
            {
                transform.position = Vector2.Lerp(transform.position, _targetPos, _movementSpeed * Time.deltaTime);
            }
            else
            {
                transform.position = _targetPos;
            }
        }

        public IEnumerator Explode(float timeToExplode)
        {
            transform.GetChild(1).gameObject.SetActive(true);   // star
            yield return new WaitForSeconds(timeToExplode);
            Destroy(gameObject);
        }
    }   // hexagonpiece
}   // namespace