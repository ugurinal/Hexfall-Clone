using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    private int _hexagonLayerMask;

    private void Start()
    {
        _hexagonLayerMask = LayerMask.GetMask("Hexagon");
    }

    // Update is called once per frame
    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
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