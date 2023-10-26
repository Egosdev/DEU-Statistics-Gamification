using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameState { SELECT_DOOR, BUSY, KEEP_OR_CHANGE, DONE }
public delegate void OnStateChangeHandler();

public class MontyhallManager : MonoBehaviour
{
    #region Singleton
    public static MontyhallManager Instance;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }
    #endregion

    public event OnStateChangeHandler OnStateChange;
    public GameState gameState { get; private set; }

    public void SetGameState(GameState state)
    {
        gameState = state;
        OnStateChange();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 ray = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(ray, Vector2.zero);

            if (hit)
            {
                //Debug.Log("hit " + hit.collider.name + " " + doorPrizeArray[Convert.ToInt32(hit.collider.name)]);
                //selected = hit.collider.gameObject;
            }
        }
    }
}
