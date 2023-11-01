using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    #region Singleton
    public static CameraManager Instance;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }
    #endregion

    [SerializeField] CinemachineVirtualCamera[] cameras;

    public void ChangeCamera(string camName)
    {
        foreach (CinemachineVirtualCamera vcam in cameras)
        {
            vcam.gameObject.SetActive(false);
        }

        if (camName == "Zoom")
            cameras[0].gameObject.SetActive(true);
        else if (camName == "OuterCam")
            cameras[1].gameObject.SetActive(true);
    }

    private void Start()
    {
        ChangeCamera("Zoom");
        MontyhallManager.Instance.BottomPanelAnimation();
        MontyhallManager.Instance.ChartPanelAnimation();
        MontyhallManager.Instance.UpperPanelAnimation();
    }
}
