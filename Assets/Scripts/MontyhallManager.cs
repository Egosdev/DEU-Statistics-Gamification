using Pixelplacement;
using System.Collections;
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
        statusSubTextPlaceholder.text = "Onaylamak i�in tekrar t�kla!";
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
                winLoseText = "Tebrikler, b�y�k �d�l� kazand�n!";
                isWon = true;
            }
            else
            {
                winLoseText = "Eyvah, hi�bir �ey kazanamad�n :(";
                isWon = false;
            }
            clickedDoor.GetComponent<Door>().Open(true);

            if (initSelected == clickedDoor)
            {
                statusSubTextPlaceholder.text = "Kap�y� sabit tuttun.";
                isSwitch = false;
            }
            else
            {
                statusSubTextPlaceholder.text = "Kap�y� de�i�tirdin.";
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
}
