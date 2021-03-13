using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HexfallClone.GameController;

namespace HexfallClone.PlayerInput
{
    public class InputManager : MonoBehaviour
    {
        private int _hexagonLayerMask;

        private GameManager _gameManager;

        private void Start()
        {
            _gameManager = GameManager.Instance;
            _hexagonLayerMask = LayerMask.GetMask("Hexagon");
        }

        // Update is called once per frame
        private void Update()
        {
            if (_gameManager.GameState == GameState.Idle)
            {
                if (Input.GetMouseButtonUp(0))
                {
                    RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero, 0, _hexagonLayerMask);

                    if (hit.collider != null)
                    {
                        hit.transform.GetChild(0).gameObject.SetActive(true);
                        Debug.Log(hit.transform.name);
                    }
                }
            }
        }
    }
}