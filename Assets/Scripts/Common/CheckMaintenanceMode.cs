using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.Network;
using UnityEngine.SceneManagement;

public class CheckMaintenanceMode : MonoBehaviour {

    private bool isMaintenanceLoaded = false;
    
    void Update () {

        if (ApiHelper.Instance.isLogin)
        {
            if (ApiHelper.Instance.Maintenance && !isMaintenanceLoaded && SceneManager.GetActiveScene().name != "WebView")
            {
                if (ApiHelper.Instance.isMachineOs)
                {
                    if (DateTime.Now > DateTime.Parse(ApiHelper.Instance.from) && DateTime.Now < DateTime.Parse(ApiHelper.Instance.to))
                    {
                        Debug.Log("CheckMaintenanceMode");
                        SceneManager.LoadScene("WebView");
                        isMaintenanceLoaded = true;
                    }
                }
                
            }

            if (!ApiHelper.Instance.Maintenance)
            {
                isMaintenanceLoaded = false;
            }
        }
	}
}
