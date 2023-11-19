using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DemoSceneManager : MonoBehaviour
{
    private EMGRawReader emgRawReader;
    private VTFeedback tactor;
    private Renderer rend;

    public bool _isOnV;
    public bool _isOnVT;
    private bool firstTime;
    // Start is called before the first frame update
    void Start()
    {
        // if (firstTime) {
        //     PlayerPrefs.SetInt("FeedbackType", Random.Range(0,1));
        // } else {
        //     PlayerPrefs.GetInt("FeedbackType");
        //     int ft = -PlayerPrefs.GetInt("FeedbackType") + 1;
        //     PlayerPrefs.SetInt("FeedbackType", ft);
        // }

        // if (PlayerPrefs.GetInt("FeedbackType") == 1) {
        //     transform.Rotate(0,0,0);
        // }
        emgRawReader = FindObjectOfType<EMGRawReader>();
        //tactor = FindObjectOfType<VTFeedback>();
        rend = GetComponent<Renderer>();  
    }

    // Update is called once per frame
    void Update()
    {
        if (emgRawReader.readVal == "2")
        {
            transform.Rotate(0,0,0);
            //tactor.Write(true);
        }
        if (emgRawReader.readVal == "1")
        {
            transform.Rotate(1,0,0);
        }
        if (emgRawReader.readVal == "0")
        {
            transform.Rotate(-1,0,0);
            //tactor.Write(false);
        }
        if (emgRawReader.readVal == "3")
        {
            transform.Rotate(0,0,1);
        }
        if (emgRawReader.readVal == "4")
        {
            transform.Rotate(0,0,-1);
        }
        if (emgRawReader.readVal == "5")
        {
            transform.Rotate(0,1,0);
        }
        if (emgRawReader.readVal == "6")
        {
            transform.Rotate(0,-1,0);
        }
        Color someColor = new Color(1-emgRawReader.velocity, emgRawReader.velocity, emgRawReader.velocity, 1f);
        rend.material.color = someColor;
    }
    
    // private void UpdateFeedbackUI() {
    //     // Vibrotactile Feedback
    //     if (PlayerPrefs.GetInt("FeedbackType") == 0) {
    //         if (_isOnVT) {
    //             tactor.Write(true);
    //         }
    //         else {
    //             tactor.Write(false);
    //         }
    //     }
    //     // Visual Feedback
    //     if (PlayerPrefs.GetInt("FeedbackType") == 1) {
    //         if (_isOnV) {
                
    //         } 
    //         else {
    //             tactor.Write(false);
    //         }
    //     }
          
    // }
}
