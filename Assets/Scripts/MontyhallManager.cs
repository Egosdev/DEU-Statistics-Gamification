using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

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

    public GameObject selected;
    [SerializeField] Transform sign;

    public enum GameState
    {
        SELECT_DOOR,
        BUSY,
        KEEP_OR_CHANGE,
        DONE
    }

    private GameState currentGameState;
    public GameState CurrentGameState
    {
        get { return currentGameState; }
        set
        {
            currentGameState = value;
            OnStateChanged(value);
        }
    }

    void OnStateChanged(GameState state)
    {
        Debug.Log(state);
        if (state == GameState.BUSY)
        {
            
        }
        
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 ray = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(ray, Vector2.zero);

            if (hit)
            {
                if (hit.transform.CompareTag("Door")) SelectDoor(hit.transform.gameObject);

            }
        }
    }

    void SelectDoor(GameObject clickedDoor)
    {
        if (CurrentGameState == GameState.SELECT_DOOR)
        {
            if (selected == clickedDoor)
            {
                //selected.GetComponent<Door>().OpenAnimation();
                CurrentGameState = GameState.BUSY;
            }

            selected = clickedDoor;
            sign.gameObject.SetActive(true);
            sign.position = clickedDoor.transform.position + new Vector3(0, 2.5f);
            Debug.Log(clickedDoor.GetComponent<Door>().isCorrect);
        }
    }
}
