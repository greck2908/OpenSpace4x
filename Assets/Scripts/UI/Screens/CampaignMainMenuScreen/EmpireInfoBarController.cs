﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EmpireInfoBarController : MonoBehaviour
{
    [SerializeField]
    CampaignMainScreen mainScreen;

    [SerializeField]
    Text moneyText;

    [SerializeField]
    Text mineralsText;

    [SerializeField]
    Text influenceText;

    [SerializeField]
    Text unityText;

    [SerializeField]
    Text physicsText;

    [SerializeField]
    Text societyText;

    [SerializeField]
    Text engineeringText;

    [SerializeField]
    Text strategicResourcesText;

    [SerializeField]
    Text navalCommandText;

	// Use this for initialization
	void Start ()
    {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
		
	}






    public void ClickGovernmentButton()
    {

    }

    public void ClickContactsButton()
    {

    }

    public void ClickSituationLogButton()
    {

    }

    public void CliclResearchButton()
    {
        mainScreen.ToggleResearchWindow(true);
    }

    public void ClickMoreDropDownButton()
    {

    }
}
