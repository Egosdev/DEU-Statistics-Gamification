using Pixelplacement;
using System.Collections;
using System.Collections.Generic;
using TMPro;
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

    [SerializeField] TextMeshProUGUI tmpCounter;
    [SerializeField] TextMeshPro statusTextPlaceholder;
    [SerializeField] TextMeshPro statusSubTextPlaceholder;
    [SerializeField] Transform sign;
    [SerializeField] Chart chart;
    [SerializeField] string[] statusTexts;
    [SerializeField] ParticleSystem[] _confettiVFX;
    [SerializeField] RectTransform bottomPanel;
    [SerializeField] RectTransform upperPanel;
    [SerializeField] RectTransform chartPanel;

    [HideInInspector]
    public GameObject selected;

    private GameObject initSelected;
    private string winLoseText;
    private bool isWon = false;
    private bool isSwitch = false;
    private int tryCounter = 0;

    [Header("Temp")]
    int stayWinFreq;
    int switchWinFreq;
    [SerializeField] int repeatCount = 10;


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
            DoorManager.Instance.ResetAllDoorsHoverAnimation();
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
        if (isWon) DummyConfetti();
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

        if (Input.GetKeyDown(KeyCode.K))
        {
            ComputerPlay(true); //random & switch
        }
        if (Input.GetKeyDown(KeyCode.L))
        {
            ComputerPlay(false); //random & stay
        }

        if (Input.GetKeyDown(KeyCode.M))
        {
            switchWinFreq = 0;
            stayWinFreq = 0;
            //chart.ClearChart();

            for (int i = 0; i < repeatCount; i++)
            {
                ComputerPlay(true);
                ComputerPlay(false);
            }

            Debug.Log($"{switchWinFreq / (float) repeatCount * 100}% ({repeatCount} deneme) Switch & Win");
            Debug.Log($"{stayWinFreq / (float) repeatCount * 100}% ({repeatCount} deneme) sStay & Win");
        }
    }

    void DummyConfetti()
    {
        foreach (ParticleSystem psItem in _confettiVFX)
        {
            psItem.Play();
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
            }
            else
            {
                winLoseText = "Eyvah, hiçbir þey kazanamadýn :(";
                isWon = false;
            }
            clickedDoor.GetComponent<Door>().Open(true);

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

    public void BottomPanelAnimation()
    {
        Tween.LocalPosition(bottomPanel, new Vector3(0, -540, 0), 1f, 0.5f, Tween.EaseInOutStrong);
    }
    public void ChartPanelAnimation()
    {
        Tween.LocalPosition(chartPanel, new Vector3(-745, 90, 0), 1f, 1f, Tween.EaseInOutStrong);
    }
    public void UpperPanelAnimation()
    {
        Tween.LocalPosition(upperPanel, new Vector3(0, 505, 0), 1f, 1.5f, Tween.EaseInOutStrong);
    }
    public void ComputerPlay(bool _isSwitch)
    {
        tryCounter++;
        tmpCounter.text = tryCounter.ToString();
        CurrentGameState = GameState.SELECT_DOOR;
        DoorManager.Instance.DoorsSetup();
        statusTextPlaceholder.text = statusTexts[0];
        statusSubTextPlaceholder.gameObject.SetActive(false);
        statusSubTextPlaceholder.text = "Onaylamak için tekrar týkla!";
        int selectRandomDoor = Random.Range(0, DoorManager.Instance.DoorCount);

        if (_isSwitch)
        {
            if (selectRandomDoor != DoorManager.Instance.CorrectDoorIndex)
            {
                chart.AddNewData(DataType.SWITCH_WIN, 1);
                switchWinFreq++;
            }
            else
            {
                chart.AddNewData(DataType.SWITCH_LOSE, 1);
                //switchLoseFreq++;
            }
        }
        else
        {
            if (selectRandomDoor != DoorManager.Instance.CorrectDoorIndex)
            {
                chart.AddNewData(DataType.STAY_LOSE, 1);
                //stayLoseFreq++;
            }
            else
            {
                chart.AddNewData(DataType.STAY_WIN, 1);
                stayWinFreq++;
            }
        }
    }
}
