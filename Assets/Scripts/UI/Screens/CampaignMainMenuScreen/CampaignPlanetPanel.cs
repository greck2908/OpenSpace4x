﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CampaignPlanetPanel : MonoBehaviour
{
    [SerializeField]
    PlanetController planetController;

    [SerializeField]
    bool playerControl;

    [SerializeField]
    Text planetName;

    [SerializeField]
    PlanetSurfaceGrid planetSurfaceGrid;

    [SerializeField]
    CampaignPlanetPanelExtension panelExtension;

	// Use this for initialization
	void Start ()
    {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
		
	}

    void OnEnable()
    {
        panelExtension.gameObject.SetActive(false);
    }

    public void SetPlanet(PlanetController newPlanet, bool controllable)
    {
        planetController = newPlanet;
        playerControl = controllable;

        planetName.text = planetController.GetDisplayName();

        planetSurfaceGrid.CreatePlanetTiles(planetController.GetPlanetTilesList());
    }













    public void UpdateDay()
    {

    }

    public void ClickOverviewButton()
    {

    }

    public void ClickSurfaceButton()
    {

    }

    public void ClickMilitaryButton()
    {

    }

    public void ClickShipyardButton()
    {

    }

    public void ClickStarbaseButton()
    {

    }
}

