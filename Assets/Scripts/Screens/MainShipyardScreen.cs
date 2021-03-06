﻿/*****************************************************************************************************************************************
Author: Michael Shoots
Email: michael.shoots@live.com
Project: Open Space 4x
License: MIT License
Notes:
******************************************************************************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class MainShipyardScreen : BaseShipDesignScreen
{
    #region Variables

    #endregion

    protected override void Start()
    {
        base.Start();

        BuildModuleSetLists();
        BuildShipHullDatas();
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
    }

    protected override void OnGUI()
    {
        base.OnGUI();
    }

    protected override void BuildModuleSetLists()
    {
        List<ModuleSet> WeaponModuleSets = new List<ModuleSet>();
        List<ModuleSet> DefenseModuleSets = new List<ModuleSet>();
        List<ModuleSet> SystemModuleSets = new List<ModuleSet>();

        //Distribute modules into lists
        foreach (KeyValuePair<string, ModuleSet> keyVal in ResourceManager.instance.GetModuleSets())
        {
            if (CheckModuleSetAllowed(keyVal.Value))
            {
                if (keyVal.Value.ModuleCategory == ModuleCategory.Weapons)
                {
                    WeaponModuleSets.Add(keyVal.Value);
                }
                else if (keyVal.Value.ModuleCategory == ModuleCategory.Defences)
                {
                    DefenseModuleSets.Add(keyVal.Value);
                }
                else
                {
                    SystemModuleSets.Add(keyVal.Value);
                }             
            }
        }

        //Apply to module panel
        ModulePanel.SetWeaponModuleSetList(WeaponModuleSets);
        ModulePanel.SetDefenseModuleSetList(DefenseModuleSets);
        ModulePanel.SetSystemModuleSetList(SystemModuleSets);

        ModulePanel.BuildModuleSetButtons(WeaponModuleSets);
    }

    protected override void BuildShipHullDatas()
    {
        shipHullPanel.SetShipHullDatas(ResourceManager.instance.GetShipHulls(), ChangeHull);
    }

    public override void BuildShipDesigns(ShipHullData hullData)
    {
        shipHullPanel.SetShipDesigns(ResourceManager.instance.GetShipDesigns(hullData, false), LoadShipDesign, DeleteShipDesign);
    }

    protected override void ChangeHull(ShipHullData data)
    {
        base.ChangeHull(data);
    }

    protected override void LoadShipDesign(ShipDesign design)
    {

    }
}
