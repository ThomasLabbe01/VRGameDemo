using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoseChanger : MonoBehaviour
{
    // Start is called before the first frame update
    //public List<GameObject> poseList;
    //public List<bool> poseState;
    //public GameObject allObjects;
    private GameObject allObjects;
    private List<bool> poseState;
    private List<GameObject> poseList;

    private EMGRawReader EMG_classifier;
    private EMGSetUp _emg;

    private float previousVal = 100;

    public void ChangeState(int index)
    {
        bool currentState = poseState[index];
        poseState[index] = !currentState;
        // The index was previously active, so deactivate all other elements
         for (int i = 0; i < poseState.Count; i++)
        {
            if (i != index)
            {
                poseState[i] = false;
                poseList[i].SetActive(false);
            }
        }
    }

    private void DestroyChild(List<GameObject> poseList)
    {
        foreach (GameObject pose in poseList) 
        {
            foreach (Transform child in pose.transform.Find("R_Wrist/R_Palm"))
            {
                if (child.name.Contains("Clone"))
                {
                    Transform firstChild = child.GetChild(0);
                    GameObject childObject = firstChild.gameObject;

                    foreach (Transform ghost in childObject.transform)
                    {
                        // Get the "TrackedTowards" component of the child
                        TrackTowards trackedComponent = ghost.GetComponent<TrackTowards>();

                        // If the component exists, disable it
                        if (trackedComponent != null)
                        {
                            trackedComponent.enabled = false;
                        }
                    }

                    //GameObject copy = Instantiate(childObject, allObjects.transform); // Create a copy of the child game object
                    //.transform.SetPositionAndRotation(childObject.transform.position, childObject.transform.rotation);

                    firstChild.transform.SetParent(allObjects.transform); // replace the gameobject at its right place
                    Destroy(child.gameObject); // Destroy the original child game object

                }
            }
        }
        
    }

    private void Start()
    {
        allObjects = transform.parent.Find("NewObjects v2").gameObject;
        EMG_classifier = FindObjectOfType<EMGRawReader>();
        _emg = FindObjectOfType<EMGSetUp>();
        // Initiate poseList with all child objects of the "Right" transform
        poseList = new List<GameObject>();
        Transform rightTransform = transform.Find("Right");
        foreach (Transform child in rightTransform)
        {
            poseList.Add(child.gameObject);
        }

        // Initiate poseState with the same size as poseList
        poseState = new List<bool>();
        for (int i = 0; i < poseList.Count; i++)
        {
            if (i == 0)
            {
                // Make the first one true
                poseState.Add(true);
            }
            else
            {
                // Make the rest false
                poseState.Add(false);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        float accuracy = EMG_classifier.EMG_accuracy;
        int prediction = EMG_classifier.EMG_prediction;
        PoseChanger poseStateManager = GetComponent<PoseChanger>();
        if (_emg.EMG)
        {
            if (previousVal != prediction)
            {
                previousVal = prediction;
                if (prediction == 2)
                {
                    DestroyChild(poseList);
                    poseList[0].SetActive(!poseState[0]);
                    poseStateManager.ChangeState(0);
                }
                if (prediction == 0)
                {
                    DestroyChild(poseList);
                    poseList[1].SetActive(!poseState[1]);
                    poseStateManager.ChangeState(1);
                }

                if (prediction == 1)
                {
                    DestroyChild(poseList);
                    poseList[2].SetActive(!poseState[2]);
                    poseStateManager.ChangeState(2);
                }

                if (prediction == 3)
                {
                    DestroyChild(poseList);
                    poseList[3].SetActive(!poseState[3]);
                    poseStateManager.ChangeState(3);
                }

                if (prediction == 4)
                {
                    DestroyChild(poseList);
                    poseList[4].SetActive(!poseState[4]);
                    poseStateManager.ChangeState(4);
                }

                if (prediction == 5)
                {
                    DestroyChild(poseList);
                    poseList[5].SetActive(!poseState[5]);
                    poseStateManager.ChangeState(5);
                }

                if (prediction == 6)
                {
                    DestroyChild(poseList);
                    poseList[6].SetActive(!poseState[6]);
                    poseStateManager.ChangeState(6);
                }

                if (prediction == 7)
                {
                    DestroyChild(poseList);
                    poseList[7].SetActive(!poseState[7]);
                    poseStateManager.ChangeState(7);
                }

                if (prediction == 8)
                {
                    DestroyChild(poseList);
                    poseList[8].SetActive(!poseState[8]);
                    poseStateManager.ChangeState(8);
                }
            }
        }
        
        


         if (Input.GetKeyDown(KeyCode.Keypad1))
         {
             DestroyChild(poseList);
             poseList[0].SetActive(!poseState[0]);
             poseStateManager.ChangeState(0);
         }
         if (Input.GetKeyDown(KeyCode.Keypad2))
         {
             DestroyChild(poseList);
             poseList[1].SetActive(!poseState[1]);
             poseStateManager.ChangeState(1);
         }

         if (Input.GetKeyDown(KeyCode.Keypad3))
         {
             DestroyChild(poseList);
             poseList[2].SetActive(!poseState[2]);
             poseStateManager.ChangeState(2);
         }

         if (Input.GetKeyDown(KeyCode.Keypad4))
         {
             DestroyChild(poseList);
             poseList[3].SetActive(!poseState[3]);
             poseStateManager.ChangeState(3);
         }

         if (Input.GetKeyDown(KeyCode.Keypad5))
         {
             DestroyChild(poseList);
             poseList[4].SetActive(!poseState[4]);
             poseStateManager.ChangeState(4);
         }

         if (Input.GetKeyDown(KeyCode.Keypad6))
         {
             DestroyChild(poseList);
             poseList[5].SetActive(!poseState[5]);
             poseStateManager.ChangeState(5);
         }

         if (Input.GetKeyDown(KeyCode.Keypad7))
         {
             DestroyChild(poseList);
             poseList[6].SetActive(!poseState[6]);
             poseStateManager.ChangeState(6);
         }

         if (Input.GetKeyDown(KeyCode.Keypad8))
         {
             DestroyChild(poseList);
             poseList[7].SetActive(!poseState[7]);
             poseStateManager.ChangeState(7);
         }

         if (Input.GetKeyDown(KeyCode.Keypad9))
         {
             DestroyChild(poseList);
             poseList[8].SetActive(!poseState[8]);
             poseStateManager.ChangeState(8);
         }
    }
}
