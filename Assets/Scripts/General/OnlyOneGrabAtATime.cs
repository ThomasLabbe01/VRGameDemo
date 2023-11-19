using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnlyOneGrabAtATime : MonoBehaviour
{
    public List<GameObject> AllPoses;
    public GameObject GrabStatusObject;
    private VTFeedback _tactor;
    private EMGSetUp _emg;
    public Color targetColor = Color.green; // Change this to the desired color

    public float trialtime = 0f;


    private Renderer objectRenderer;
    private Color originalColor = Color.red;
    public bool grab = false;

    private void CheckGrab()
    {
        bool shouldEnableOnlyOneGrabAtATime = true; // New variable to track enabling OnlyOneGrabAtATime

        grab = false; // Reset grab to false at the start of each check

        foreach (GameObject obj in AllPoses)
        {
            TrackTowards component = obj.GetComponent<TrackTowards>();

            if (component != null && component.enabled)
            {
                grab = true;
                shouldEnableOnlyOneGrabAtATime = false; // At least one component has grab = true
                SetOnlyOneGrabAtATimeForAllObjects(obj);
                break;
            }
        }

        if (shouldEnableOnlyOneGrabAtATime)
        {
            EnableOnlyOneGrabAtATimeForAllObjects();
        }
    }

    private void SetOnlyOneGrabAtATimeForAllObjects(GameObject currentObject)
    {
        foreach (GameObject obj in AllPoses)
        {
            if (obj != currentObject)
            {
                TrackTowards component = obj.GetComponent<TrackTowards>();
                if (component != null)
                {
                    component.onlyOneGrabAtATime = false;
                }
            }
        }
    }

    private void EnableOnlyOneGrabAtATimeForAllObjects()
    {
        foreach (GameObject obj in AllPoses)
        {
            TrackTowards component = obj.GetComponent<TrackTowards>();
            if (component != null)
            {
                component.onlyOneGrabAtATime = true;
            }
        }
    }

    private void Start()
    {
        objectRenderer = GrabStatusObject.GetComponent<Renderer>();
        originalColor = objectRenderer.material.color;
        _emg = FindObjectOfType<EMGSetUp>();
        if (_emg.VT)
        {
            _tactor = FindObjectOfType<VTFeedback>();
            _tactor.Write(false);
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        trialtime += Time.deltaTime;
        CheckGrab();
        int FeedbackMode = PlayerPrefs.GetInt("FeedbackMode");
        if (FeedbackMode == 0)
        {
            GrabStatusObject.SetActive(false);
        }
        if (FeedbackMode == 1)//Visual
        {
            GrabStatusObject.SetActive(true);
            if (grab) {
                objectRenderer.material.color = targetColor;
            } 
            else {
                objectRenderer.material.color = originalColor;
            }
        }
        if (FeedbackMode == 2 && _emg.VT)//Vibro
        {
            GrabStatusObject.SetActive(false);
            if (grab) {
                _tactor.Write(true);
            } 
            else {
                _tactor.Write(false);
            }
        }
    }
}
