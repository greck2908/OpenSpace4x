﻿/*****************************************************************************************************************************************
Author: Michael Shoots
Email: michael.shoots@live.com
Project: Open Space 4x
License: MIT License
Notes:
******************************************************************************************************************************************/

using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class GalaxyManager : MonoBehaviour
{
    public static GalaxyManager instance;

    [Header("Galaxy Generation Settings")]

    [SerializeField]
    int galaxySize;

    [SerializeField]
    int systemCount;

    [SerializeField]
    float minSystemDistance;

    [SerializeField]
    int minPlanetsPerSystem;

    [SerializeField]
    int maxPlanetsPerSystem;

    Dictionary<Guid, PlanetController> planets = new Dictionary<Guid, PlanetController>();

    void Awake()
    {
        //Make a Singleton
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(this);
        }

        //This stays in every scene
        DontDestroyOnLoad(gameObject);
    }

    // Use this for initialization
    void Start ()
    {
        BuildNewGalaxy(GetRandomSystemPoints());

    }
	
	// Update is called once per frame
	void Update ()
    {
	
	}



    void BuildNewGalaxy(Vector3[] solarSystemPositions)
    {
        List<PlanetTypeDefinition> planetDefinitions = ResourceManager.instance.GetPlanetTypesList();

        float[] planetDefinitionWeights = new float[planetDefinitions.Count];

        for(int i = 0; i < planetDefinitions.Count; i++)
        {
            planetDefinitionWeights[i] = planetDefinitions[i].randomGalaxyWeight;
        }

        foreach (Vector3 systemPosition in solarSystemPositions)
        {
            GameObject starObject = ResourceManager.instance.CreateStar("Star_Red");
            starObject.transform.position = systemPosition;
            starObject.transform.rotation = UnityEngine.Random.rotation;

            Vector3[] planetPositions = GetRandomPlanetPositions(UnityEngine.Random.Range(minPlanetsPerSystem, maxPlanetsPerSystem + 1));
            foreach (Vector3 planetPosition in planetPositions)
            {
                PlanetTypeDefinition definition = planetDefinitions[StaticHelpers.GetRandomIndexByWeight(planetDefinitionWeights)];
                PlanetController newPlanet = definition.CreatePlanetInstance();
                newPlanet.SetSize(definition.GetRandomSize());
                newPlanet.SetLightSourcePosition(systemPosition);
                newPlanet.transform.position = planetPosition + systemPosition;
                //planetObject.transform.rotation = Quaternion.Euler(0, UnityEngine.Random.Range(0f,360f), 0);
                newPlanet.transform.localScale = new Vector3(0.4f,0.4f,0.4f);
            }
        }
    }


    public void AddPlanet(Guid guid, PlanetController planet)
    {
        if (planets.ContainsKey(guid))
        {
            planets[guid] = planet;
        }
        else
        {
            planets.Add(guid, planet);
        }
    }

    public PlanetController GetPlanetData(Guid guid)
    {
        PlanetController planet;

        if (planets.TryGetValue(guid, out planet))
        {
            return planet;
        }

        return null;
    }

    Vector3[] GetRandomSystemPoints()
    {
        Vector3[] systemPoints = new Vector3[systemCount];
        Vector3 systemPoint;
        float minDistance;
        float minDistanceSqrd = minSystemDistance * minSystemDistance;

        for(int i = 0; i < systemCount; i++)
        {
            do
            {
                minDistance = Mathf.Infinity;
                systemPoint = new Vector3(UnityEngine.Random.Range(-galaxySize, galaxySize), -1f, UnityEngine.Random.Range(-galaxySize, galaxySize));

                for (int j = i - 1; j >= 0; j--)
                {
                    float distance = (systemPoint - systemPoints[j]).sqrMagnitude;

                    if (distance < minDistance)
                    {
                        minDistance = distance;
                    }
                }

            } while (minDistance < minDistanceSqrd);

            systemPoints[i] = systemPoint;
        }

        return systemPoints;
    }

    Vector3[] GetRandomPlanetPositions(int count)
    {
        Vector3[] planetPositions = new Vector3[count];

        float radius = 1.25f;

        for(int i = 0; i < count; i++)
        {
            Vector2 direction = StaticHelpers.GetRandomUnitVector2() * radius;
            planetPositions[i] = new Vector3(direction.x, 0f, direction.y);
            radius += 0.5f;
        }

        return planetPositions;
    }
}
