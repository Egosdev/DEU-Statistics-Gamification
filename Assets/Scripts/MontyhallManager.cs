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
    [SerializeField] Chart chart;
    int tryCounter = 0;

    [SerializeField] string[] statusTexts;
    string winLoseText;
    bool isWon = false;
    bool isSwitch = false;
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
            statusSubTextPlaceholder.gameObject.SetActive(false);
            StartCoroutine(OpenSomeDoors());
        }
        else if (state == GameState.KEEP_OR_CHANGE)
        {
            statusTextPlaceholder.text = statusTexts[2];
        }
        else if (state == GameState.DONE)
        {
            StartCoroutine(TrialDone());
        }
    }

    IEnumerator TrialDone()
    {
        yield return new WaitForSeconds(1f);
        tryCounter++;
        tmpCounter.text = tryCounter.ToString();
        statusTextPlaceholder.text = winLoseText;
        yield return new WaitForSeconds(1.75f);
        CurrentGameState = GameState.SELECT_DOOR;
        DoorManager.Instance.DoorsSetup();
        statusTextPlaceholder.text = statusTexts[0];
        statusSubTextPlaceholder.gameObject.SetActive(false);
        statusSubTextPlaceholder.text = "Onaylamak için tekrar týkla!";
        if (isWon)
        {
            if (isSwitch) chart.AddNewData(DataType.SWITCH_WIN, 1);
            else chart.AddNewData(DataType.STAY_WIN, 1);
        }
        else
        {
            if (isSwitch) chart.AddNewData(DataType.SWITCH_LOSE, 1);
            else chart.AddNewData(DataType.STAY_LOSE, 1);
        }
    }
    IEnumerator OpenSomeDoors()
    {
        yield return new WaitForSeconds(1f);
        DoorManager.Instance.OpenNonPrizeDoors();
        yield return new WaitForSeconds(2f);
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
        if (!(CurrentGameState == GameState.SELECT_DOOR || CurrentGameState == GameState.KEEP_OR_CHANGE)) return;
        if (clickedDoor.GetComponent<Door>().IsOpen()) return;
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
            if (DoorManager.Instance.CheckForPrizeDoor(clickedDoor.GetComponent<Door>()))
            {
                winLoseText = "Tebrikler, büyük ödülü kazandýn!";
                isWon = true;
                clickedDoor.GetComponent<Door>().OpenPrizeDoor(true);
            }
            else
            {
                winLoseText = "Eyvah, hiçbir þey kazanamadýn :(";
                isWon = false;
                clickedDoor.GetComponent<Door>().OpenDoor(true);
            }

            if (initSelected == clickedDoor)
            {
                statusSubTextPlaceholder.text = "Kapýyý sabit tuttun.";
                isSwitch = false;
            }
            else
            {
                statusSubTextPlaceholder.text = "Kapýyý deðiþtirdin.";
                isSwitch = true;
            }

            sign.gameObject.SetActive(false);
            statusTextPlaceholder.text = "...";
            CurrentGameState = GameState.DONE;
        }
    }
}
