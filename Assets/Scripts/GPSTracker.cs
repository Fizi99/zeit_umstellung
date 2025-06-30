using System;
using System.Collections;
using UnityEngine;

public class GPSTracker : MonoBehaviour
{
    // TODO: probably needs different architecture to only init location service once and update every some seconds
    private UIManager uiManager;
    private GameManager gameManager;
    [SerializeField] private double distanceThreshhold = 20;

    void Start()
    {
        this.uiManager = GameObject.Find("UIManager").GetComponent<UIManager>();
        this.gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        InvokeRepeating("UpdateGPSPosition", 0f, 10f);
    }

    public void UpdateGPSPosition()
    {
        StartCoroutine(UpdatePosition());
    }

    IEnumerator UpdatePosition()
    {
        //if (!Input.location.isEnabledByUser)
            //Debug.Log("Location not enabled on device or app does not have permission to access location");

        // Starts the location service.

        float desiredAccuracyInMeters = 10f;
        float updateDistanceInMeters = 10f;

        Input.location.Start(desiredAccuracyInMeters, updateDistanceInMeters);

        // Waits until the location service initializes
        int maxWait =9;
        while (Input.location.status == LocationServiceStatus.Initializing && maxWait > 0)
        {
            yield return new WaitForSeconds(1);
            maxWait--;
        }

        // If the service didn't initialize in 9 seconds this cancels location service use.
        if (maxWait < 1)
        {
            Debug.Log("Timed out");
            yield break;
        }

        // If the connection failed this cancels location service use.
        if (Input.location.status == LocationServiceStatus.Failed)
        {
            //Debug.LogError("Unable to determine device location");
            yield break;
        }
        else
        {
            // If the connection succeeded, this retrieves the device's current location and displays it in the Console window.
            //this.uiManager.ShowDebug("Location: " + Input.location.lastData.latitude + " " + Input.location.lastData.longitude + " " + Input.location.lastData.altitude + " " + Input.location.lastData.horizontalAccuracy + " " + Input.location.lastData.timestamp);
            //Debug.Log("Location: " + Input.location.lastData.latitude + " " + Input.location.lastData.longitude + " " + Input.location.lastData.altitude + " " + Input.location.lastData.horizontalAccuracy + " " + Input.location.lastData.timestamp);
            this.gameManager.SetPlayerCoords(Input.location.lastData.latitude, Input.location.lastData.longitude);
        }

        // Stops the location service if there is no need to query location updates continuously.
        Input.location.Stop();
    }

    public double CalcDistanceBetweenCordsInM(double lat1, double lon1, double lat2, double lon2)
    {

            var R = 6378000; // Radius of the earth in m
            var dLat = deg2rad(lat2 - lat1);  // deg2rad below
            var dLon = deg2rad(lon2 - lon1);
            var a =
              Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
              Math.Cos(deg2rad(lat1)) * Math.Cos(deg2rad(lat2)) *
              Math.Sin(dLon / 2) * Math.Sin(dLon / 2)
              ;
            var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            var d = R * c; // Distance in m
            return Math.Round(d);
        
    }

    private double deg2rad(double deg)
    {
        return deg * (Math.PI / 180);
    }

    public double GetDistanceThreshhold()
    {
        return this.distanceThreshhold;
    }

}
