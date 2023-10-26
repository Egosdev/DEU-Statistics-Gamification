using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
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
    private GameObject initSelected;
    [SerializeField] Transform sign;
    [SerializeField] TextMeshPro statusTextPlaceholder;
    [SerializeField] TextMeshPro statusSubTextPlaceholder;
    [SerializeField] TextMeshProUGUI tmpCounter;
    int tryCounter = 0;

    [SerializeField] string[] statusTexts;

    private void Start() => statusTextPlaceholder.text = statusTexts[0];

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
            statusTextPlaceholder.text = statusTexts[1];
            statusSubTextPlaceholder.enabled = false;
            StartCoroutine(OpenSomeDoors());
        }
        else if (state == GameState.KEEP_OR_CHANGE)
        {
            statusTextPlaceholder.text = statusTexts[2];
        }
        else if (state == GameState.DONE)
        {
            tryCounter++;
            tmpCounter.text = tryCounter.ToString();
        }
    }

    IEnumerator OpenSomeDoors()
    {
        yield return new WaitForSeconds(1f);
        DoorManager.Instance.OpenDoors();
        yield return new WaitForSeconds(3f);
        CurrentGameState = GameState.KEEP_OR_CHANGE;
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
        if (!statusSubTextPlaceholder.gameObject.activeSelf) statusSubTextPlaceholder.gameObject.SetActive(true);

        if (CurrentGameState == GameState.SELECT_DOOR)
        {
            if (selected == clickedDoor)
            {
                CurrentGameState = GameState.BUSY;
                initSelected = clickedDoor;
            }

            sign.gameObject.SetActive(true);
            sign.position = clickedDoor.transform.position + new Vector3(0, 2.5f);
            Debug.Log("prize: " + clickedDoor.GetComponent<Door>().isCorrect);
            selected = clickedDoor;
        }
        else if (CurrentGameState == GameState.KEEP_OR_CHANGE)
        {
            if (DoorManager.Instance.TestCorrectness(clickedDoor.GetComponent<Door>()))
            {
                statusTextPlaceholder.text = "Tebrikler, büyük ödülü kazandýn!";
            }
            else
            {
                statusTextPlaceholder.text = "Eyvah, hiçbir þey kazanamadýn :(";
            }

            if (initSelected == clickedDoor) statusSubTextPlaceholder.text = "Seçtiðin kapýyý sabit tuttun.";
            else statusSubTextPlaceholder.text = "Seçtiðin kapýyý deðiþtirdin.";

            sign.gameObject.SetActive(false);
            CurrentGameState = GameState.DONE;
        }
    }
}
