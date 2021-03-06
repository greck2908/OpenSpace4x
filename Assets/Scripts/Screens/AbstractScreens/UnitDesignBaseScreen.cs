﻿/*****************************************************************************************************************************************
Author: Michael Shoots
Email: michael.shoots@live.com
Project: Open Space 4x
License: MIT License
Notes:
******************************************************************************************************************************************/

using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public abstract class UnitDesignBaseScreen : ScreenParent
{
    #region Variables
    protected GameObject unitModel;
    protected ModuleListTypes moduleCategory = ModuleListTypes.Weapon;
    protected ModuleSet selectedModuleSet = null;
    protected Module selectedModule = null;
    protected Texture2D selectedModuleTexture;
    //protected Texture2D SlotTexture = ResourceManager.instance.GetUITexture("ShipSlot");
    //protected Texture2D WeaponArcCircle = ResourceManager.instance.GetUITexture("WeaponArcCircle");
    protected Texture2D WeaponArcTex;
    protected Rect SelectedModuleRect;
    protected Rect SlotsAreaRect;

    protected Rect RightSidePanelRect;
    protected Vector2 ModuleListEntrySize;
    protected Vector2 DesignListEntrySize;
    protected float HullListEntrySize;

    //Weapon Arc Rects
    protected Rect WeaponArcRect;

    //Modules
    protected Rect LeftSidePanelRect;
    protected Rect WeaponCategoryButtonRect;
    protected Rect DefenseCategoryButtonRect;
    protected Rect SystemCategoryButtonRect;
    protected Rect ModuleScrollWindowRect;
    protected Rect ModuleScrollViewRect;
    protected Vector2 ModuleScrollPosition;
    protected Vector2 ModuleStatPosition;
    protected float moduleScale = 32f;
    protected float ModuleListY;
    protected float ModuleRotation = 0;
    protected Vector2 ModuleStatSize;
    protected List<ModuleSetEntry> ModuleSetWeaponList = new List<ModuleSetEntry>();
    protected List<ModuleSetEntry> ModuleSetDefenceList = new List<ModuleSetEntry>();
    protected List<ModuleSetEntry> ModuleSetSystemList = new List<ModuleSetEntry>();
    protected List<ModuleListEntry> ModuleList = new List<ModuleListEntry>();
    protected List<IconStatEntry> ModuleStatsList = new List<IconStatEntry>();
    protected List<DesignIconStatEntry> DesignStatList = new List<DesignIconStatEntry>();

    //Module Weapon Graph
    protected Rect ModuleWeaponGraphRect;
    protected Rect ModuleWeaponGraphInnerRect;
    protected Rect ModuleWeaponGraphTitleRect;
    protected Rect ModuleWeaponDamageRect;
    protected Rect ModuleWeaponRangeRect;
    protected float SelectedWeaponMaxDamage = -1;
    protected float SelectedWeaponMaxRange = -1;
    protected List<GUILine> GraphLines = new List<GUILine>();

    //Design Stat Window
    protected Rect DesignStatsWindowRect;
    protected Rect DesignStatsViewRect;
    protected Vector2 DesignStatsPosition;

    //Buttons
    protected Rect CloseButtonRect;
    protected Rect ModuleRotationButtonRect;
    protected Rect ModuleSwapButtonRect;
    protected Rect MirrirModeButtonRect;

    //Naming and saving
    protected string DesignName = "";
    protected Rect DesignNameFieldRect;
    protected Rect DesignSaveButtonRect;

    //Symmetric design/mirror mode
    protected bool useSymmetricCursor = false;
    protected Vector2 SymmetricMouse;

    //Overwrite design window
    protected bool overwriteWarningPopupOpen = false;
    protected Rect overwriteWarningRect;
    protected Rect overwriteWarningMessageRect;
    protected Rect overwriteAcceptButtonRect;
    protected Rect overwriteCancelButtonRect;

    //Hull info Rects
    protected Rect HullNameRect;
    protected string FormattedHullName;

    //Design stats Header Rects
    protected Rect DesignResourceCostHeaderRect;
    protected Rect DesignPersonnelHeaderRect;
    protected Rect DesignPowerHeaderRect;
    protected Rect DesignAmmoHeaderRect;
    protected Rect DesignSensorsHeaderRect;
    protected Rect DesignCombatBonusHeaderRect;
    protected Rect DesignJammingHeaderRect;
    protected Rect DesignHangarHeaderRect;
    protected Rect DesignEngineeringHeaderRect;
    protected Rect DesignMiscHeaderRect;

    //Design stats
    protected float DesignProductionCost;
    protected float DesignAlloyCost;
    protected float DesignAdvancedAlloyCost;
    protected float DesignSuperiorAlloyCost;
    protected float DesignCrystalCost;
    protected float DesignRareCrystalCost;
    protected float DesignExoticCrystalCost;
    protected float DesignExoticParticleCost;
    protected float DesignPower;
    protected float DesignPowerGenerated;
    protected float DesignAmmo;
    protected float DesignPowerStorage;
    protected float DesignCrew;
    protected float DesignRequiredCrew;
    protected float DesignSensor;
    protected float DesignLongRangeSensor;
    protected float DesignAdvancedSensor;
    protected float DesignResearch;
    protected float DesignMining;
    protected float DesignRepair;
    protected float DesignDamageBonus;
    protected float DesignDefenseBonus;
    protected float DesignPowerEfficiency;
    protected float DesignAmmoGenerated;
    protected int DesignTroops;
    protected int DesignFighters;
    protected int DesignHeavyFighters;
    protected int DesignAssaultPods;
    protected float DesignMedical;
    protected float DesignBoardingDefense;
    protected float DesignTransporter;
    protected float DesignFirePowerAmmo;
    protected float DesignFirePowerPower;
    protected float DesignFirePowerCombination;
    protected float DesignFirePowerFighter;
    protected float DesignMaxRange;
    protected float DesignCost;
    protected float DesignExperience;
    protected float DesignDiplomacy;

    //Jamming Stats
    protected float DesignJammingCount;
    protected float DesignJammingRange;
    protected float DesignJammingDelay;

    //Calculated design stats
    protected float DesignPowerSum;
    protected float DesignLongRangeSensorSum;
    protected float DesignWeaponAmmoConsumption;
    protected float DesignWeaponPowerConsumption;
    protected float DesignWeaponAmmoTime;
    protected float DesignWeaponPowerTime;
    protected float DesignFirepower;
    protected float DesignDefenseRating;

    //Hovered Module Data
    protected Module LastHoveredModule;
    protected List<IconStatEntry> LastHoveredModuleStats;

    protected QuitResumeSubScreen quitResumeSubScreen;

    #endregion

    public override void Update()
    {
        SetMousePosition();

        if (Input.GetKeyDown(KeyCode.R))
        {
            RotateModule();
        }

        if (Input.GetKeyDown(KeyCode.M))
        {
            useSymmetricCursor = !useSymmetricCursor;
        }

        //Remove modules with right click
        if (Input.GetMouseButton(1))
        {
            DeselectModule();
            if (SlotsAreaRect.Contains(mousePosition))
            {
                CheckModuleRemoval();
            }
        }
    }

    public override void Draw()
    {
        if (PopupOpen())
        {
            GUI.enabled = false;
        }

        GUI.Box(LeftSidePanelRect, "", GameManager.instance.standardBackGround);
        GUI.Box(RightSidePanelRect, "", GameManager.instance.standardBackGround);

        if (GUI.Button(WeaponCategoryButtonRect, "Weapons", GameManager.instance.standardButtonStyle))
        {
            if (moduleCategory != ModuleListTypes.Weapon)
                ChangeModuleSetList(ModuleListTypes.Weapon);
            PlayMainButtonClick();
        }
        if (GUI.Button(DefenseCategoryButtonRect, "Defences", GameManager.instance.standardButtonStyle))
        {
            if (moduleCategory != ModuleListTypes.Defence)
                ChangeModuleSetList(ModuleListTypes.Defence);
            PlayMainButtonClick();
        }
        if (GUI.Button(SystemCategoryButtonRect, "Systems", GameManager.instance.standardButtonStyle))
        {
            if (moduleCategory != ModuleListTypes.System)
                ChangeModuleSetList(ModuleListTypes.System);
            PlayMainButtonClick();
        }

        //Draw Module set scrolling list
        ModuleScrollPosition = GUI.BeginScrollView(ModuleScrollWindowRect, ModuleScrollPosition, ModuleScrollViewRect);
        List<ModuleSetEntry> ModuleSetList;
        if (moduleCategory == ModuleListTypes.Weapon)
        {
            ModuleSetList = ModuleSetWeaponList;
        }
        else if (moduleCategory == ModuleListTypes.Defence)
        {
            ModuleSetList = ModuleSetDefenceList;
        }
        else
        {
            ModuleSetList = ModuleSetSystemList;
        }
        foreach (ModuleSetEntry entry in ModuleSetList)
        {
            entry.Draw(selectedModuleSet);
        }
        GUI.EndScrollView();

        //Draw modules of selected module set
        foreach (ModuleListEntry entry in ModuleList)
        {
            entry.Draw(selectedModule);
        }

        if (selectedModuleSet != null && ModuleSetCanSwap(selectedModuleSet))
        {
            if (ModuleSwapButtonRect.Contains(mousePosition))
            {
                ToolTip.SetText("armorSwap", "armorSwapDesc");
            }

            //if (GUI.Button(ModuleSwapButtonRect, ResourceManager.instance.GetIconTexture("Icon_Swap")))
            //{
               // ModuleSwap(selectedModuleSet);
               // PlayMainButtonClick();
            //}
        }

        DrawModuleStats();

        DrawWeaponGraph();

        DrawDesignStats();

        DesignName = GUI.TextField(DesignNameFieldRect, DesignName);
    }

    protected void DrawSelectedModuleTexture()
    {
        Matrix4x4 matrixBackup = GUI.matrix;
        GUIUtility.RotateAroundPivot(ModuleRotation, SelectedModuleRect.center);
        if (selectedModuleTexture != null)
            GUI.DrawTexture(SelectedModuleRect, selectedModuleTexture);
        if (useSymmetricCursor)
        {
            GUI.matrix = matrixBackup;
            SymmetricMouse.y = mousePosition.y;
            SymmetricMouse.x = Screen.width - mousePosition.x;
            if (ModuleRotation == 0 || ModuleRotation == 180)
            {
                SymmetricMouse.x -= selectedModule.SizeX * moduleScale - moduleScale;
                SelectedModuleRect.x = SymmetricMouse.x - moduleScale;
            }
            else
            {
                SymmetricMouse.x -= selectedModule.SizeY * moduleScale - moduleScale;
                SelectedModuleRect.x = SymmetricMouse.x;
            }
            GUIUtility.RotateAroundPivot(ModuleRotation, SelectedModuleRect.center);
            if (selectedModuleTexture != null)
                GUI.DrawTexture(SelectedModuleRect, selectedModuleTexture);
        }
        GUI.matrix = matrixBackup;
    }

    protected void DrawWeaponGraph()
    {
        if (selectedModule != null && selectedModule.GetWeaponExists())
        {
            //Check for Weapon Graph hover
            if (ModuleWeaponGraphInnerRect.Contains(mousePosition))
            {
                float range = (mousePosition.x - ModuleWeaponGraphInnerRect.x) / ModuleWeaponGraphInnerRect.width * SelectedWeaponMaxRange;
                float damage = (ModuleWeaponGraphInnerRect.yMax - mousePosition.y) / ModuleWeaponGraphInnerRect.height * SelectedWeaponMaxDamage;
                GUI.Box(new Rect(new Vector2(mousePosition.x, mousePosition.y - GameManager.instance.StandardLabelSize.y), new Vector2(GameManager.instance.StandardLabelSize.x, GameManager.instance.StandardLabelSize.y)), range.ToString("0") + " : " + damage.ToString("0"));
            }

            //GUI.DrawTexture(ModuleWeaponGraphRect, ResourceManager.instance.GetUITexture("WeaponDamageGraph"));
            GUI.Label(ModuleWeaponGraphTitleRect, "Weapon Damage Over Range", GameManager.instance.ModuleTitleStyle);
            GUI.Label(ModuleWeaponDamageRect, "Damage: " + SelectedWeaponMaxDamage.ToString("0"), GameManager.instance.standardLabelStyle);
            GUI.Label(ModuleWeaponRangeRect, "Range: " + SelectedWeaponMaxRange.ToString("0"), GameManager.instance.standardLabelStyle);
            foreach (GUILine line in GraphLines)
            {
                line.Draw();
            }
        }
    }

    protected virtual void Initialize()
    {
        LeftSidePanelRect = new Rect(0, 0, Screen.width * 0.25f, Screen.height);
        RightSidePanelRect = new Rect(Screen.width - Screen.width * 0.25f, 0, Screen.width * 0.25f, Screen.height);

        WeaponCategoryButtonRect = new Rect(LeftSidePanelRect.width * 0.5f - GameManager.instance.StandardButtonSize.x * 1.5f, LeftSidePanelRect.y + GameManager.instance.QuarterButtonSpacing, GameManager.instance.StandardButtonSize.x, GameManager.instance.StandardButtonSize.y);
        DefenseCategoryButtonRect = new Rect(LeftSidePanelRect.width * 0.5f - GameManager.instance.StandardButtonSize.x * 0.5f, WeaponCategoryButtonRect.y, GameManager.instance.StandardButtonSize.x, GameManager.instance.StandardButtonSize.y);
        SystemCategoryButtonRect = new Rect(LeftSidePanelRect.width * 0.5f + GameManager.instance.StandardButtonSize.x * 0.5f, WeaponCategoryButtonRect.y, GameManager.instance.StandardButtonSize.x, GameManager.instance.StandardButtonSize.y);

        ModuleScrollWindowRect = new Rect(LeftSidePanelRect.width * 0.5f - LeftSidePanelRect.width * 0.45f, DefenseCategoryButtonRect.y + GameManager.instance.StandardButtonSize.y + GameManager.instance.QuarterButtonSpacing, LeftSidePanelRect.width * 0.9f, LeftSidePanelRect.height * 0.39f);
        ModuleScrollViewRect = new Rect(0, 0, ModuleScrollWindowRect.width * 0.9f, ModuleScrollWindowRect.height * 5f);
        ModuleScrollPosition = Vector2.zero;

        WeaponArcRect = new Rect(Screen.width / 2 - Screen.width * 0.2f, Screen.height / 2 - Screen.width * 0.2f, Screen.width * 0.4f, Screen.width * 0.4f);    

        CloseButtonRect = new Rect(Screen.width - GameManager.instance.StandardButtonSize.x * 1.15f, Screen.height - GameManager.instance.StandardButtonSize.y * 1.2f, GameManager.instance.StandardButtonSize.x, GameManager.instance.StandardButtonSize.y);
        MirrirModeButtonRect = new Rect(RightSidePanelRect.x + (RightSidePanelRect.width - GameManager.instance.StandardButtonSize.x) / 2f, CloseButtonRect.y, GameManager.instance.StandardButtonSize.x, GameManager.instance.StandardButtonSize.y);

        ModuleListEntrySize = new Vector2(ModuleScrollViewRect.width, ModuleScrollWindowRect.height * 0.2f);
        
        DesignSaveButtonRect = new Rect(RightSidePanelRect.x + RightSidePanelRect.width - GameManager.instance.StandardButtonSize.x, Screen.height * 0.34f + GameManager.instance.QuarterButtonSpacing, GameManager.instance.StandardButtonSize.x, GameManager.instance.StandardButtonSize.y);
        DesignNameFieldRect = new Rect(RightSidePanelRect.x + GameManager.instance.StandardButtonSize.x * 0.025f, DesignSaveButtonRect.y, RightSidePanelRect.width - GameManager.instance.StandardButtonSize.x * 1.05f, GameManager.instance.StandardButtonSize.y);

        DesignStatsWindowRect = new Rect(RightSidePanelRect.x, DesignNameFieldRect.yMax + DesignNameFieldRect.height * 0.08f, RightSidePanelRect.width, Screen.height * .57f);
        DesignStatsViewRect = new Rect(0, 0, DesignStatsWindowRect.width * 0.95f, DesignStatsWindowRect.height * 2f);
        DesignStatsPosition = Vector2.zero;

        ModuleListY = ModuleScrollWindowRect.y + ModuleScrollWindowRect.height + HullListEntrySize * 0.25f;

        ModuleRotationButtonRect = new Rect(LeftSidePanelRect.xMax - GameManager.instance.StandardButtonSize.y * 1.5f, ModuleListY, GameManager.instance.StandardButtonSize.y * 1.2f, GameManager.instance.StandardButtonSize.y);
        ModuleSwapButtonRect = new Rect(ModuleRotationButtonRect.x, ModuleRotationButtonRect.yMax, GameManager.instance.StandardButtonSize.y, GameManager.instance.StandardButtonSize.y);

        ModuleStatPosition = new Vector2(ModuleScrollWindowRect.x, ModuleListY + ModuleListEntrySize.y + GameManager.instance.standardLabelSpacing);

        ModuleStatSize = new Vector2(ModuleScrollViewRect.width * 0.2f, GameManager.instance.StandardLabelSize.y);   

        ModuleWeaponGraphRect = new Rect(LeftSidePanelRect.width * 0.25f, LeftSidePanelRect.height * 0.84f, LeftSidePanelRect.width * 0.5f, LeftSidePanelRect.height * 0.13f);
        ModuleWeaponGraphInnerRect = new Rect(ModuleWeaponGraphRect.x + ModuleWeaponGraphRect.width * 0.05f, ModuleWeaponGraphRect.y + ModuleWeaponGraphRect.height * 0.05f, ModuleWeaponGraphRect.width * 0.9f, ModuleWeaponGraphRect.height * 0.9f);
        ModuleWeaponGraphTitleRect = new Rect(ModuleWeaponGraphRect.x, ModuleWeaponGraphRect.y - GameManager.instance.StandardLabelSize.y * 1.05f, ModuleWeaponGraphRect.width, GameManager.instance.StandardLabelSize.y);
        ModuleWeaponDamageRect = new Rect(ModuleWeaponGraphRect.x - ModuleStatSize.x, ModuleWeaponGraphRect.y, ModuleStatSize.x, ModuleStatSize.y);
        ModuleWeaponRangeRect = new Rect(ModuleWeaponGraphRect.xMax - ModuleStatSize.x, ModuleWeaponGraphRect.yMax, ModuleStatSize.x, ModuleStatSize.y);

        ToolTip = new GUIToolTip(new Vector2(LeftSidePanelRect.xMax, 0), LeftSidePanelRect.width * 0.6f);
    }

    protected bool CheckHullModuleAllow(ModuleLimitType limit, ModuleCategory category)
    {
        if (category == ModuleCategory.Systems)
        {
            return true;
        }
        if (category == ModuleCategory.Engines)
        {
            if (limit == ModuleLimitType.NoEngines || limit == ModuleLimitType.NoWeaponsOrEngines)
            {
                return false;
            }
        }
        if (category == ModuleCategory.Weapons)
        {
            if (limit == ModuleLimitType.NoWeapons || limit == ModuleLimitType.NoWeaponsOrEngines)
            {
                return false;
            }
        }
        return true;
    }

    protected abstract void CheckPlacedModuleHover();

    protected void SetWeaponArcTexture(int angle)
    {/*
        switch (angle)
        {
            case 15:
                {
                    WeaponArcTex = ResourceManager.instance.GetUITexture("WeaponArc15");
                    break;
                }
            case 30:
                {
                    WeaponArcTex = ResourceManager.instance.GetUITexture("WeaponArc30");
                    break;
                }
            case 60:
                {
                    WeaponArcTex = ResourceManager.instance.GetUITexture("WeaponArc60");
                    break;
                }
            case 90:
                {
                    WeaponArcTex = ResourceManager.instance.GetUITexture("WeaponArc90");
                    break;
                }
            case 120:
                {
                    WeaponArcTex = ResourceManager.instance.GetUITexture("WeaponArc120");
                    break;
                }
            case 150:
                {
                    WeaponArcTex = ResourceManager.instance.GetUITexture("WeaponArc150");
                    break;
                }
            case 180:
                {
                    WeaponArcTex = ResourceManager.instance.GetUITexture("WeaponArc180");
                    break;
                }
            case 210:
                {
                    WeaponArcTex = ResourceManager.instance.GetUITexture("WeaponArc210");
                    break;
                }
            case 240:
                {
                    WeaponArcTex = ResourceManager.instance.GetUITexture("WeaponArc240");
                    break;
                }
            case 270:
                {
                    WeaponArcTex = ResourceManager.instance.GetUITexture("WeaponArc270");
                    break;
                }
            case 300:
                {
                    WeaponArcTex = ResourceManager.instance.GetUITexture("WeaponArc300");
                    break;
                }
            case 330:
                {
                    WeaponArcTex = ResourceManager.instance.GetUITexture("WeaponArc330");
                    break;
                }
            case 360:
                {
                    WeaponArcTex = ResourceManager.instance.GetUITexture("WeaponArc360");
                    break;
                }
        }*/
    }

    protected void DrawModuleStats()
    {
        foreach (IconStatEntry entry in ModuleStatsList)
        {
            entry.Draw();
        }

        if(LeftSidePanelRect.Contains(mousePosition))
        {
            foreach (IconStatEntry entry in ModuleStatsList)
            {
                if (entry.CheckForToolTip(mousePosition))
                {
                    break;
                }
            }
        }
    }

    protected abstract void DrawDesignStats();

    protected void ChangeModule(Module module)
    {
        selectedModule = module;
        ModuleStatsList = BuildModuleStats(module, ModuleStatPosition);
        Weapon weapon = module.GetWeapon();
        if(weapon != null)
        {
            BuildWeaponDamageGraph(weapon);
        }
        SelectedModuleRect = new Rect(0, 0, moduleScale * selectedModule.SizeX, moduleScale * selectedModule.SizeY);
        //selectedModuleTexture = selectedModule.GetTexture();
        ModuleRotation = 0;

        if (selectedModuleSet != selectedModule.GetParentSet())
        {
            ChangeModuleSet(selectedModule.GetParentSet());
        }
    }

    protected void RotateModule()
    {
        //Check to make sure there is a module and it should be able to rotate     
        if (selectedModule == null)
        {
            return;
        }
        Weapon weapon = selectedModule.GetWeapon();
        if (weapon != null && !weapon.AlwaysForward)
        {
            return;
        }

        ModuleRotation += 90;
        if (ModuleRotation == 360)
        {
            ModuleRotation = 0;
        }
    }

    protected bool ModuleCanRotate(Module module)
    {
        Weapon weapon = module.GetWeapon();
        if(weapon != null && weapon.AlwaysForward || weapon == null && module.SizeX != module.SizeY)
        {
            return true;
        }
        return false;
    }

    protected List<IconStatEntry> BuildModuleStats(Module module, Vector2 position)
    {
        List<IconStatEntry> statList = new List<IconStatEntry>();
        //ModuleStatsList.Clear();

        float cost = module.GetCost();
        if (cost != 0)
        {
            AddModuleStatEntry(statList, position, "Icon_Money", cost.ToString("0.##"), "moduleCost", "moduleCostDesc");
        }
        if (module.ProductionCost != 0)
        {
            AddModuleStatEntry(statList, position, "Icon_Production", module.ProductionCost.ToString("0.##"), "productionCost", "productionCostDesc");
        }
        if (module.AlloyCost != 0)
        {
            AddModuleStatEntry(statList, position, "Icon_Alloy", module.AlloyCost.ToString("0.##"), "alloyCost", "alloyCostDesc");
        }
        if (module.AdvancedAlloyCost != 0)
        {
            AddModuleStatEntry(statList, position, "Icon_AlloyAdvanced", module.AdvancedAlloyCost.ToString("0.##"), "advancedAlloyCost", "advancedAlloyCostDesc");
        }
        if (module.SuperiorAlloyCost != 0)
        {
            AddModuleStatEntry(statList, position, "Icon_AlloySuperior", module.SuperiorAlloyCost.ToString("0.##"), "superiorAllorCost", "superiorAllorCostDesc");
        }
        if (module.CrystalCost != 0)
        {
            AddModuleStatEntry(statList, position, "Icon_Crystal", module.CrystalCost.ToString("0.##"), "crystalCost", "crystalCostDesc");
        }
        if (module.RareCrystalCost != 0)
        {
            AddModuleStatEntry(statList, position, "Icon_CrystalRare", module.RareCrystalCost.ToString("0.##"), "rareCrystalCost", "rareCrystalCostDesc");
        }
        if (module.ExoticCrystalCost != 0)
        {
            AddModuleStatEntry(statList, position, "Icon_CrystalExotic", module.ExoticCrystalCost.ToString("0.##"), "exoticCrystalCost", "exoticCrystalCostDesc");
        }
        if (module.ExoticParticleCost != 0)
        {
            AddModuleStatEntry(statList, position, "Icon_ParticleExotic", module.ExoticParticleCost.ToString("0.##"), "exoticParticleCost", "exoticParticleCostDesc");
        }
        if (module.Health != 0)
        {
            AddModuleStatEntry(statList, position, "Icon_Health", module.Health.ToString("0.##"), "health", "modHealth");
        }
        if (module.Mass != 0)
        {
            AddModuleStatEntry(statList, position, "Icon_Mass", module.Mass.ToString("0.##"), "mass", "modMass");
        }
        if (module.Power != 0)
        {
            AddModuleStatEntry(statList, position, "Icon_Power", module.Power.ToString("0.##"), "power", "modPower");
        }
        if (module.PowerGenerated != 0)
        {
            AddModuleStatEntry(statList, position, "Icon_PowerGenerated", module.PowerGenerated.ToString("0.##"), "powerGenerated", "powerGeneratedDesc");
        }
        if (module.PowerEfficiency != 0)
        {
            AddModuleStatEntry(statList, position, "Icon_PowerEfficiency", "%" + (module.PowerEfficiency * 100).ToString("0.##"), "powerEfficiency", "powerEfficiencyDesc");
        }
        if (module.Fuel != 0)
        {
            AddModuleStatEntry(statList, position, "Icon_Fuel", module.Fuel.ToString("0.##"), "fuel", "modFuel");
        }
        if (module.ArmorHealth != 0)
        {
            AddModuleStatEntry(statList, position, "Icon_ArmorHealth", module.ArmorHealth.ToString("0.##"), "armorHealth", "modArmorHealth");
        }
        if (module.ArmorRating != 0)
        {
            AddModuleStatEntry(statList, position, "Icon_ArmorRating", module.ArmorRating.ToString("0.##"), "armorRating", "modArmorRating");
        }
        if (module.ShieldHealth != 0)
        {
            AddModuleStatEntry(statList, position, "Icon_ShieldHealth", module.ShieldHealth.ToString("0.##"), "shieldHealth", "modShieldHealth");
        }
        if (module.ShieldRating != 0)
        {
            AddModuleStatEntry(statList, position, "Icon_ShieldRating", module.ShieldRating.ToString("0.##"), "shieldRating", "modShieldRating");
        }
        if (module.ShieldRechargeRate != 0)
        {
            AddModuleStatEntry(statList, position, "Icon_ShieldRecharge", module.ShieldRechargeRate.ToString("0.##"), "shieldRecharge", "modShieldRecharge");
        }
        if (module.ShieldRechargeDelay != 0)
        {
            AddModuleStatEntry(statList, position, "Icon_ShieldDelay", module.ShieldRechargeDelay.ToString("0.##"), "shieldRechargeDelay", "modShieldRechargeDelay");
        }
        if (module.Ammo != 0)
        {
            AddModuleStatEntry(statList, position, "Icon_Ammo", module.Ammo.ToString("0.##"), "ammo", "modAmmo");
        }
        if (module.PowerStorage != 0)
        {
            AddModuleStatEntry(statList, position, "Icon_PowerStorage", module.PowerStorage.ToString("0.##"), "powerStorage", "modPowerStorage");
        }
        if (module.Crew != 0)
        {
            AddModuleStatEntry(statList, position, "Icon_Crew", module.Crew.ToString("0.##"), "crew", "modCrew");
        }
        if (module.RequiredCrew != 0)
        {
            AddModuleStatEntry(statList, position, "Icon_RequiredCrew", module.RequiredCrew.ToString("0.##"), "requiredCrew", "modRequiredCrew");
        }
        if (module.Supplies != 0)
        {
            AddModuleStatEntry(statList, position, "Icon_Supplies", module.Supplies.ToString("0.##"), "supplies", "modSupplies");
        }
        if (module.EngineThrust != 0)
        {
            AddModuleStatEntry(statList, position, "Icon_EngineThrust", module.EngineThrust.ToString("0.##"), "engineTrust", "modEngineTrust");
        }
        if (module.EngineTurn != 0)
        {
            AddModuleStatEntry(statList, position, "Icon_EngineTurn", module.EngineTurn.ToString("0.##"), "engineTurn", "modEngineTurn");
        }
        if (module.EngineFTL != 0)
        {
            AddModuleStatEntry(statList, position, "Icon_FTL", module.EngineFTL.ToString("0.##"), "engineFTL", "modEngineFTL");
        }
        if (module.EngineBonus != 0)
        {
            AddModuleStatEntry(statList, position, "Icon_EngineBonus", (module.EngineBonus * 100).ToString("0.##") + "%", "engineBonus", "modEngineBonus");
        }
        if (module.Sensor != 0)
        {
            AddModuleStatEntry(statList, position, "Icon_Sensor", module.Sensor.ToString("0.##"), "rangeSensor", "modRangeSensor");
        }
        if (module.LongRangeSensor != 0)
        {
            AddModuleStatEntry(statList, position, "Icon_SensorLongRange", module.LongRangeSensor.ToString("0.##"), "rangeSensorLong", "modRangeSensorLong");
        }
        if (module.AdvancedSensor != 0)
        {
            AddModuleStatEntry(statList, position, "Icon_SensorAdvanced", module.AdvancedSensor.ToString("0.##"), "rangeSensorAdvanced", "modRangeSensorAdvanced");
        }
        if (module.DamageBonus != 0)
        {
            AddModuleStatEntry(statList, position, "Icon_Damage", "%" + (module.DamageBonus * 100f).ToString("0.##"), "damageBonus", "modDamageBonus");
        }
        if (module.DamageBonusFleet != 0)
        {
            AddModuleStatEntry(statList, position, "Icon_DamageFleet", "%" + (module.DamageBonusFleet * 100f).ToString("0.##"), "fleetDamageBonus", "fleetDamageBonusDesc");
        }
        if (module.DefenseBonus != 0)
        {
            AddModuleStatEntry(statList, position, "Icon_Defense", "%" + (module.DefenseBonus * 100f).ToString("0.##"), "defenseBonus", "modDefenseBonus");
        }
        if (module.DefenseBonusFleet != 0)
        {
            AddModuleStatEntry(statList, position, "Icon_DefenseFleet", "%" + (module.DefenseBonusFleet * 100).ToString("0.##"), "fleetDefenseBonus", "fleetDefenseBonusDesc");
        }
        if (module.Repair != 0)
        {
            AddModuleStatEntry(statList, position, "Icon_Repair", module.Repair.ToString("0.##"), "repairRate", "modRepairRate");
        }
        if (module.Research != 0)
        {
            AddModuleStatEntry(statList, position, "Icon_Research", module.Research.ToString("0.##"), "research", "modResearch");
        }
        if (module.Mining != 0)
        {
            AddModuleStatEntry(statList, position, "Icon_Mining", module.Mining.ToString("0.##"), "mining", "modMining");
        }
        if (module.Construction != 0)
        {
            AddModuleStatEntry(statList, position, "Icon_Construction", module.Construction.ToString("0.##"), "construction", "modConstruction");
        }
        if (module.AmmoGenerated != 0)
        {
            AddModuleStatEntry(statList, position, "Icon_AmmoGenerated", module.AmmoGenerated.ToString("0.##"), "ammoGenerated", "modAmmoGenerated");
        }
        if (module.Medical != 0)
        {
            AddModuleStatEntry(statList, position, "Icon_Medical", module.Medical.ToString("0.##"), "medical", "modMedical");
        }
        if (module.Transporter != 0)
        {
            AddModuleStatEntry(statList, position, "Icon_Transporter", module.Transporter.ToString("0.##"), "transporter", "modTransporter");
        }
        if (module.Troops != 0)
        {
            AddModuleStatEntry(statList, position, "Icon_Troop", module.Troops.ToString("0.##"), "troops", "modTroops");
        }
        if (module.BoardingDefense != 0)
        {
            AddModuleStatEntry(statList, position, "Icon_BoardingDefense", module.BoardingDefense.ToString("0.##"), "boardingDefense", "modBoardingDefense");
        }
        if (module.Colonies != 0)
        {
            AddModuleStatEntry(statList, position, "Icon_Colony", module.Colonies.ToString("0.##"), "colonySupplies", "modColonySupplies");
        }
        if (module.CloakingPowerPerMass != 0)
        {
            AddModuleStatEntry(statList, position, "Icon_CloakPower", module.CloakingPowerPerMass.ToString("0.##"), "cloakingPowerPerMass", "modCloakingPower");
        }
        if (module.Stealth != 0)
        {
            AddModuleStatEntry(statList, position, "Icon_Stealth", module.Stealth.ToString("0.##"), "stealth", "modStealth");
        }
        if (module.Diplomacy != 0)
        {
            AddModuleStatEntry(statList, position, "Icon_Diplomacy", module.Diplomacy.ToString("0.##"), "diplomacy", "modDiplomacy");
        }
        if (module.CommandPointReduction != 0)
        {
            AddModuleStatEntry(statList, position, "Icon_CommandPoint", module.CommandPointReduction.ToString("0.##"), "commandPointReduction", "modCommandPointRecution");
        }
        if (module.CommandPointBonusFleet != 0)
        {
            AddModuleStatEntry(statList, position, "Icon_CommandPointFleet", module.CommandPointBonusFleet.ToString("0.##"), "fleetCommandPoints", "modFleetCommandPoints");
        }
        if (module.ExperienceBonus != 0)
        {
            AddModuleStatEntry(statList, position, "Icon_LevelBonus", "%" + (module.ExperienceBonus * 100f).ToString("0.##"), "experienceBonus", "experienceBonusDesc");
        }
        if (module.JammingCount > 0)
        {
            AddModuleStatEntry(statList, position, "Icon_Jamming", module.JammingCount.ToString("0.##"), "JammingCount", "modJammingCount");
        }
        if (module.JammingRange > 0)
        {
            AddModuleStatEntry(statList, position, "Icon_JammingRange", module.GetJammingRangeDisplay().ToString("0.##"), "JammingRange", "modJammingRange");
        }
        if (module.JammingDelay > 0)
        {
            AddModuleStatEntry(statList, position, "Icon_JammingDelay", module.JammingDelay.ToString("0.##"), "JammingDelay", "modJammingDelay");
        }
        if (module.BombArmyDamage > 0)
        {
            AddModuleStatEntry(statList, position, "Icon_BombArmy", module.BombArmyDamage.ToString("0.##"), "BombingArmyDamage", "BombingArmyDamageDesc");
        }
        if (module.BombStructureDamage > 0)
        {
            AddModuleStatEntry(statList, position, "Icon_BombStructure", module.BombStructureDamage.ToString("0.##"), "BombingStructureDamage", "BombingStructureDamageDesc");
        }
        if (module.BombPopulationDamage > 0)
        {
            AddModuleStatEntry(statList, position, "Icon_BombPopulation", module.BombPopulationDamage.ToString("0.##"), "BombingPopulationDamage", "BombingPopulationDamageDesc");
        }
        if (module.BombPollution > 0)
        {
            AddModuleStatEntry(statList, position, "Icon_BombPolution", module.BombPollution.ToString("0.##"), "BombingPollution", "BombingPollutionDesc");
        }
        float defenseRating = module.GetDefenseRating();
        if(defenseRating > 0)
        {
            AddModuleStatEntry(statList, position, "Icon_DefenseRating", defenseRating.ToString("0.##"), "defenseRating", "moduleDefenseRating");
        }
        if (module.Fighters.Count > 0)
        {
            FighterDefinition firstFighter = module.GetFirstFighter();
            if (firstFighter != null)
            {
                AddModuleStatEntry(statList, position, "Icon_Fighter", firstFighter.MaxSquadronSize.ToString("0.##"), "figters", "modFighters");
                AddModuleStatEntry(statList, position, "Icon_FighterHealth", firstFighter.Health.ToString("0.##"), "modFighterHealth", "modFighterHealthDesc");
                if (firstFighter.ArmorHealth > 0)
                {
                    AddModuleStatEntry(statList, position, "Icon_FighterArmorHealth", firstFighter.Health.ToString("0.##"), "modFighterArmorHealth", "modFighterArmorHealthDesc");
                }
                if (firstFighter.ArmorRating > 0)
                {
                    AddModuleStatEntry(statList, position, "Icon_FighterArmor", firstFighter.ArmorRating.ToString("0.##"), "modFighterArmor", "modFighterArmorDesc");
                }
                if (firstFighter.ShieldHealth > 0)
                {
                    AddModuleStatEntry(statList, position, "Icon_FighterShieldsHealth", firstFighter.ShieldHealth.ToString("0.##"), "modFighterShieldHealth", "modFighterShieldHealthDesc");
                }
                if (firstFighter.ShieldRating > 0)
                {
                    AddModuleStatEntry(statList, position, "Icon_FighterShields", firstFighter.ShieldRating.ToString("0.##"), "modFighterShield", "modFighterShieldDesc");
                }
                if (firstFighter.ShieldRecharge > 0)
                {
                    AddModuleStatEntry(statList, position, "Icon_FighterShieldsRecharge", firstFighter.ShieldRecharge.ToString("0.##"), "modFighterShieldRecharge", "modFighterShieldRechargeDesc");
                }
                if (firstFighter.ShieldDelay > 0)
                {
                    AddModuleStatEntry(statList, position, "Icon_FighterShieldsDelay", firstFighter.ShieldDelay.ToString("0.##"), "modFighterShieldDelay", "modFighterShieldDelayDesc");
                }
                float firePower = firstFighter.GetFirePower();
                if (firePower > 0)
                {
                    AddModuleStatEntry(statList, position, "Icon_FighterFirePower", firePower.ToString("0.##"), "modFighterFirepower", "modFighterFirepowerDesc");
                }
                if (firstFighter.Crew > 0)
                {
                    AddModuleStatEntry(statList, position, "Icon_FighterCrew", firstFighter.Crew.ToString("0.##"), "fighterCrew", "fighterCrewDesc");
                }
            }
        }
        if (module.HeavyFighters.Count > 0)
        {
            FighterDefinition firstHeavyFighter = module.GetFirstHeavyFighter();
            if (firstHeavyFighter != null)
            {
                AddModuleStatEntry(statList, position, "Icon_HeavyFighter", firstHeavyFighter.MaxSquadronSize.ToString("0.##"), "heavyFighters", "modHeavyFighters");
                AddModuleStatEntry(statList, position, "Icon_HeavyFighterHealth", firstHeavyFighter.Health.ToString("0.##"), "modHeavyFighterHealth", "modHeavyFighterHealthDesc");

                if (firstHeavyFighter.ArmorHealth > 0)
                {
                    AddModuleStatEntry(statList, position, "Icon_HeavyFighterArmorHealth", firstHeavyFighter.ArmorHealth.ToString("0.##"), "modHeavyFighterArmorHealth", "modHeavyFighterArmorHealthDesc");
                }
                if (firstHeavyFighter.ArmorRating > 0)
                {
                    AddModuleStatEntry(statList, position, "Icon_HeavyFighterArmor", firstHeavyFighter.ArmorRating.ToString("0.##"), "modHeavyFighterArmor", "modHeavyFighterArmorDesc");
                }
                if (firstHeavyFighter.ShieldHealth > 0)
                {
                    AddModuleStatEntry(statList, position, "Icon_HeavyFighterShieldsHealth", firstHeavyFighter.ShieldHealth.ToString("0.##"), "modHeavyFighterShieldHealth", "modHeavyFighterShieldHealthDesc");
                }
                if (firstHeavyFighter.ShieldRating > 0)
                {
                    AddModuleStatEntry(statList, position, "Icon_HeavyFighterShields", firstHeavyFighter.ShieldRating.ToString("0.##"), "modHeavyFighterShield", "modHeavyFighterShieldDesc");
                }
                if (firstHeavyFighter.ShieldRecharge > 0)
                {
                    AddModuleStatEntry(statList, position, "Icon_HeavyFighterShieldsRecharge", firstHeavyFighter.ShieldRecharge.ToString("0.##"), "modHeavyFighterShieldRecharge", "modHeavyFighterShieldRechargeDesc");
                }
                if (firstHeavyFighter.ShieldDelay > 0)
                {
                    AddModuleStatEntry(statList, position, "Icon_HeavyFighterShieldsDelay", firstHeavyFighter.ShieldDelay.ToString("0.##"), "modHeavyFighterShieldDelay", "modHeavyFighterShieldDelayDesc");
                }
                float firePower = firstHeavyFighter.GetFirePower();
                if (firePower > 0)
                {
                    AddModuleStatEntry(statList, position, "Icon_HeavyFighterFirePower", firePower.ToString("0.##"), "modHeavyFighterFirepower", "modHeavyFighterFirepowerDesc");
                }
                if(firstHeavyFighter.Crew > 0)
                {
                    AddModuleStatEntry(statList, position, "Icon_HeavyFighterCrew", firstHeavyFighter.Crew.ToString("0.##"), "HeavyFighterCrew", "HeavyFighterCrewDesc");
                }
            }
        }
        if(module.AssaultPods.Count > 0)
        {
            FighterDefinition firstAssaultPod = module.GetFirstAssaultPod();
            if(firstAssaultPod != null)
            {
                AddModuleStatEntry(statList, position, "Icon_AssaultPod", firstAssaultPod.MaxSquadronSize.ToString("0.##"), "assaultPods", "modHeavyFighters");
                AddModuleStatEntry(statList, position, "Icon_AssaultPodHealth", firstAssaultPod.Health.ToString("0.##"), "AssaultPodHealth", "AssaultPodHealthDesc");
                if(firstAssaultPod.Troops > 0)
                {
                    AddModuleStatEntry(statList, position, "Icon_AssaultPodTroops", firstAssaultPod.Troops.ToString("0.##"), "AssaultPodTroops", "AssaultPodTroopsDesc");
                }
                if(firstAssaultPod.Crew > 0)
                {
                    AddModuleStatEntry(statList, position, "Icon_AssaultPodCrew", firstAssaultPod.Crew.ToString("0.##"), "AssaultPodCrew", "AssaultPodCrewDesc");
                }
            }
        }

        //Weapon stats
        Weapon weapon = module.GetWeapon();
        if (weapon != null)
        {
            if (weapon.SalvoSize != 0)
            {
                AddModuleStatEntry(statList, position, "Icon_WeaponVolley", (weapon.SalvoSize * weapon.Projectiles).ToString("0.##"), "salvoSize", "salvoSizeDesc");
            }
            if (weapon.Arc != 0)
            {
                AddModuleStatEntry(statList, position, "Icon_WeaponArc", weapon.Arc.ToString("0.##"), "weaponArc", "weaponArcDesc");
            }
            if (weapon.AmmoCost != 0)
            {
                AddModuleStatEntry(statList, position, "Icon_WeaponAmmo", weapon.AmmoCost.ToString("0.##"), "ammoCost", "ammoCostDesc");
            }
            if (weapon.PowerCost != 0)
            {
                AddModuleStatEntry(statList, position, "Icon_WeaponPower", weapon.PowerCost.ToString("0.##"), "powerCost", "powerCostDesc");
            }
            if (weapon.isBeam && weapon.BeamPowerCost != 0)
            {
                AddModuleStatEntry(statList, position, "Icon_BeamPower", weapon.BeamPowerCost.ToString("0.##"), "beamPower", "beamPowerDesc");
            }
            if (weapon.isBeam && weapon.BeamDuration != 0)
            {
                AddModuleStatEntry(statList, position, "Icon_BeamDuration", weapon.BeamDuration.ToString("0.##"), "beamDuration", "beamDurationDesc");
            }
            if (weapon.Delay != 0)
            {
                AddModuleStatEntry(statList, position, "Icon_WeaponDelay", weapon.Delay.ToString("0.##"), "fireDelay", "fireDelayDesc");
            }
            if (weapon.Spread != 0)
            {
                AddModuleStatEntry(statList, position, "Icon_WeaponSpread", weapon.Spread.ToString("0.##"), "weaponSpread", "weaponSpreadDesc");
            }
            if (weapon.HealthDamageModifier != 1f)
            {
                AddModuleStatEntry(statList, position, "Icon_WeaponDamageHealth", "%" + (weapon.HealthDamageModifier * 100f).ToString("0.##"), "HealthDamageModifier", "HealthDamageModifierDesc");
            }
            if (weapon.ShieldDamageModifier != 1f)
            {
                AddModuleStatEntry(statList, position, "Icon_WeaponDamageShield", "%" + (weapon.ShieldDamageModifier * 100f).ToString("0.##"), "ShieldDamageModifier", "ShieldDamageModifierDesc");
            }
            if (weapon.ArmorDamageModifier != 1f)
            {
                AddModuleStatEntry(statList, position, "Icon_WeaponDamageArmor", "%" + (weapon.ArmorDamageModifier * 100f).ToString("0.##"), "ArmorDamageModifier", "ArmorDamageModifierDesc");
            }
            if (weapon.FighterDamageModifier != 1f)
            {
                AddModuleStatEntry(statList, position, "Icon_WeaponDamageFighter", "%" + (weapon.FighterDamageModifier * 100f).ToString("0.##"), "FighterDamageModifier", "FighterDamageModifierDesc");
            }
            if (weapon.ProjectileDamageModifier != 1f)
            {
                AddModuleStatEntry(statList, position, "Icon_WeaponDamagePointDefense", "%" + (weapon.ProjectileDamageModifier * 100f).ToString("0.##"), "pointDefenseDamageModifier", "pointDefenseDamageModifierDesc");
            }
            if (weapon.DamageAllQuads)
            {
                AddModuleStatEntry(statList, position, "Icon_WeaponDamageAllSides", "True", "DamageAllSides", "DamageAllSidesDesc");
            }
            if (weapon.PowerDamageModifier != 0f)
            {
                AddModuleStatEntry(statList, position, "Icon_WeaponDamagePower", "%" + (weapon.PowerDamageModifier * 100f).ToString("0.##"), "powerDamage", "powerDamageDesc");
            }
            if (weapon.IgnoreShieldChance > 0)
            {
                AddModuleStatEntry(statList, position, "Icon_WeaponShieldIgnore", "%" + (weapon.IgnoreShieldChance * 100f).ToString("0.##"), "ShieldIgnoreChance", "ShieldIgnoreChanceDesc");
            }
            if (weapon.IgnoreArmorChance > 0)
            {
                AddModuleStatEntry(statList, position, "Icon_WeaponArmorIgnore", "%" + (weapon.IgnoreArmorChance * 100f).ToString("0.##"), "ArmorIgnoreChance", "ArmorIgnoreChanceDesc");
            }
            if (weapon.IgnoreArmorRatingChance > 0)
            {
                AddModuleStatEntry(statList, position, "Icon_WeaponArmorRatingIgnore", "%" + (weapon.IgnoreArmorRatingChance * 100f).ToString("0.##"), "ArmorRatingIgnoreChance", "ArmorRatingIgnoreChanceDesc");
            }
            if (weapon.PointDefense)
            {
                if (weapon.PointDefenseOnly)
                {
                    AddModuleStatEntry(statList, position, "Icon_PointDefenseOnly", "True", "PointDefenseOnly", "PointDefenseOnlyDesc");
                }
                else
                {
                    AddModuleStatEntry(statList, position, "Icon_PointDefense", "True", "PointDefense", "PointDefenseDesc");
                }
            }
            if (weapon.ProjectileHealth > 0)
            {
                AddModuleStatEntry(statList, position, "Icon_ProjectileHealth", weapon.ProjectileHealth.ToString("0.##"), "ProjectileHealth", "ProjectileHealthDesc");
            }
            float firePowerRating = weapon.GetAverageDPS() / 10f;
            if(firePowerRating > 0)
            {
                AddModuleStatEntry(statList, position, "Icon_FirePower", firePowerRating.ToString("0.##"), "firepower", "weaponFirepower");
            }
        }
        return statList;
    }

    protected void BuildWeaponDamageGraph(Weapon weapon)
    {
        //Get max damage and range
        float DamageBonus = GetModifiedDamage(weapon);
        SelectedWeaponMaxDamage = weapon.GetMaxDamage() + weapon.GetMaxDamage() * DamageBonus;
        SelectedWeaponMaxRange = weapon.GetMaxRangeDisplay();

        //Prevent possibility of divide by zero
        if (SelectedWeaponMaxDamage == 0) SelectedWeaponMaxDamage = 1;
        if (SelectedWeaponMaxRange == 0) SelectedWeaponMaxRange = 1;

        //Build Damage graph
        GraphLines.Clear();
        List<Vector2> Points = new List<Vector2>();
        foreach (Weapon.DamageNode node in weapon.DamageGraph)
        {
            Points.Add(new Vector2(ModuleWeaponGraphInnerRect.x + ModuleWeaponGraphInnerRect.width * (node.GetDisplayRange() / SelectedWeaponMaxRange), ModuleWeaponGraphInnerRect.yMax - ModuleWeaponGraphInnerRect.height * ((node.Damage + node.Damage * DamageBonus) / SelectedWeaponMaxDamage)));
        }
        for (int i = 0; i < Points.Count - 1; i++)
        {
            GraphLines.Add(new GUILine(Points[i], Points[i + 1], Color.red));
        }
    }

    protected void AddModuleStatEntry(List<IconStatEntry> list, Vector2 position, string iconTexture, string Value, string ToolTipTitle, string ToolTipBody)
    {
        Rect rect = new Rect(position.x + (list.Count % 5) * ModuleStatSize.x, position.y + (list.Count / 5) * ModuleStatSize.y, ModuleStatSize.x, ModuleStatSize.y);
        IconStatEntry iconStatEntry = new IconStatEntry(rect, iconTexture, Value, ToolTipTitle, ToolTipBody, ToolTip);
        list.Add(iconStatEntry);
    }

    protected abstract void ClearFormatedSlots();

    protected abstract void ClearSlottedModules();

    protected abstract void CheckModuleRemoval();

    protected void DeselectModule()
    {
        selectedModule = null;
        ModuleStatsList.Clear();
    }

    protected abstract void CheckPlacedModuleSelected();

    protected abstract void ClearDesignModuleStats();

    protected abstract void RecalculateDesignStats();

    //This currently just sums, but needs more complex calculations to account for enough ammo and energy
    protected void CalculateFirePower()
    {
        DesignFirepower = (DesignFirePowerAmmo + DesignFirePowerPower + DesignFirePowerCombination + DesignFirePowerFighter) / 10f;
    }

    protected abstract bool CheckValidDesign();

    protected abstract void SaveDesign();

    protected abstract void ModuleSwap(ModuleSet newModuleSet);

    protected void SwapModulesInList(ModuleSet newModuleSet, List<SlotedModule> sectionModules)
    {
        foreach (SlotedModule slotedModule in sectionModules)
        {
            if (slotedModule.module.GetParentSet().SwapType == newModuleSet.SwapType)
            {
                foreach (Module newModule in newModuleSet.GetModules())
                {
                    if (slotedModule.module.SizeX == newModule.SizeX && slotedModule.module.SizeY == newModule.SizeY)
                    {
                        slotedModule.SwapModule(newModule);
                    }
                }
            }
        }
    }

    protected bool ModuleSetCanSwap(ModuleSet modSet)
    {
        if (modSet.SwapType != ModuleSetSwapType.None)
        {
            return true;
        }
        return false;
    }

    protected virtual bool PopupOpen()
    {
        return overwriteWarningPopupOpen;
    }

    protected void ChangeModuleSet(ModuleSet modset)
    {
        selectedModuleSet = modset;
        ModuleList.Clear();
        //Determine where the individual modules will be draw and create their entries
        float Indent = LeftSidePanelRect.width / 2 - selectedModuleSet.Modules.Count * ModuleListEntrySize.y / 2;
        foreach(Module module in modset.GetModules())
        {
            Rect rect = new Rect(Indent + ModuleList.Count * ModuleListEntrySize.y, ModuleListY, ModuleListEntrySize.y, ModuleListEntrySize.y);
            //ModuleListEntry MLE = new ModuleListEntry(rect, module, module.GetTexture(), ChangeModule);
            //ModuleList.Add(MLE);
        }

        if (selectedModule == null || selectedModule.GetParentSet() != selectedModuleSet)
        {
            Module firstModule = modset.GetFirstModule();
            if(firstModule != null)
                ChangeModule(firstModule);
        }

        if (selectedModuleSet.ModuleCategory == ModuleCategory.Weapons)
        {
            if (moduleCategory != ModuleListTypes.Weapon)
                ChangeModuleSetList(ModuleListTypes.Weapon);
        }
        else if (selectedModuleSet.ModuleCategory == ModuleCategory.Defences)
        {
            if (moduleCategory != ModuleListTypes.Defence)
                ChangeModuleSetList(ModuleListTypes.Defence);
        }
        else
        {
            if (moduleCategory != ModuleListTypes.System)
                ChangeModuleSetList(ModuleListTypes.System);
        }
    }

    protected virtual void BuildModuleSetLists()
    {
        ModuleSetWeaponList.Clear();
        ModuleSetDefenceList.Clear();
        ModuleSetSystemList.Clear();
        ModuleList.Clear();
        selectedModuleSet = null;
        selectedModule = null;
        foreach (KeyValuePair<string, ModuleSet> keyVal in ResourceManager.instance.GetModuleSets())
        {
            if (CheckModuleSetAllowed(keyVal.Value))
            {
                List<ModuleSetEntry> setList;

                if (keyVal.Value.ModuleCategory == ModuleCategory.Weapons)
                {
                    setList = ModuleSetWeaponList;
                }
                else if (keyVal.Value.ModuleCategory == ModuleCategory.Defences)
                {
                    setList = ModuleSetDefenceList;
                }
                else
                {
                    setList = ModuleSetSystemList;
                }

                Rect rect = new Rect(0, ModuleListEntrySize.y * setList.Count, ModuleListEntrySize.x, ModuleListEntrySize.y);
                //ModuleSetEntry MLE = new ModuleSetEntry(rect, keyVal.Value, keyVal.Value.GetTexture(), ChangeModuleSet);
                //setList.Add(MLE);
            }
        }
    }

    protected void ChangeModuleSetList(ModuleListTypes listCategory)
    {
        moduleCategory = listCategory;
        List<ModuleSetEntry> setList;

        bool resetSelectedModuleSet = false;

        if (listCategory == ModuleListTypes.Weapon)
        {
            setList = ModuleSetWeaponList;
            if (selectedModuleSet != null && selectedModuleSet.ModuleCategory != ModuleCategory.Weapons)
                resetSelectedModuleSet = true;
        }
        else if (listCategory == ModuleListTypes.Defence)
        {
            setList = ModuleSetDefenceList;
            if (selectedModuleSet != null && selectedModuleSet.ModuleCategory != ModuleCategory.Defences)
                resetSelectedModuleSet = true;
        }
        else
        {
            setList = ModuleSetSystemList;
            if (selectedModuleSet != null && selectedModuleSet.ModuleCategory != ModuleCategory.Systems && selectedModuleSet.ModuleCategory != ModuleCategory.Engines)
                resetSelectedModuleSet = true;
        }

        ModuleScrollViewRect.height = Mathf.Max(setList.Count * ModuleListEntrySize.y, ModuleScrollWindowRect.height);

        if (selectedModuleSet == null)
            resetSelectedModuleSet = true;

        if (resetSelectedModuleSet && setList.Count > 0)
        {
            ModuleScrollPosition = Vector2.zero;
            ChangeModuleSet(setList[0].moduleSet);
        }
    }

    protected void ScrollToModuleSet(ModuleSet moduleSet)
    {
        List<ModuleSetEntry> setList;

        if (moduleCategory == ModuleListTypes.Weapon)
        {
            setList = ModuleSetWeaponList;
        }
        else if (moduleCategory == ModuleListTypes.Defence)
        {
            setList = ModuleSetDefenceList;
        }
        else
        {
            setList = ModuleSetSystemList;
        }

        foreach (ModuleSetEntry entry in setList)
        {
            if (entry.GetModuleSet() == moduleSet)
            {
                ModuleScrollPosition = entry.GetPosition();
                return;
            }
        }
    }

    protected float GetTotalValue(float production, float alloy, float advancedAlloy, float superiorAlloy, float crystal, float rareCrystal, float exoticCrystal, float exoticParticle)
    {
        return ResourceManager.instance.GetGameConstants().GetBaseResourceValue(production, alloy, advancedAlloy, superiorAlloy, crystal, rareCrystal, exoticCrystal, exoticParticle);
    }

    protected void DrawHoveredModuleInfo(Vector2 position, Module module)
    {
        if(LastHoveredModule == null || LastHoveredModule != module)
        {
            LastHoveredModule = module;
            LastHoveredModuleStats = BuildModuleStats(module, Vector2.zero);
        }

        float xPos = position.x + Screen.width * 0.01f;
        float width = Screen.width * 0.18f;
        float Indent = xPos + width * 0.05f;
        float InsideWidth = width * 0.9f;
        float height;
        string text;
        Weapon weapon = module.GetWeapon();

        Rect ModuleSetNameRect = new Rect(Indent, position.y, InsideWidth, Screen.height * 0.03f);

        text = ResourceManager.instance.GetLocalization(module.GetParentSet().Description);
        GameManager.instance.UIContent.text = text;
        height = GameManager.instance.ModuleDescStyle.CalcHeight(GameManager.instance.UIContent, width * 0.9f);

        Rect ModuleSetDescRect = new Rect(Indent, ModuleSetNameRect.yMax, InsideWidth, height);

        height = ModuleSetNameRect.height + ModuleSetDescRect.height + ModuleStatSize.y * (LastHoveredModuleStats.Count / 4 + 1);
        if (weapon != null)
        {
            height += ModuleWeaponGraphRect.height + ModuleWeaponRangeRect.height + ModuleWeaponGraphTitleRect.height * 1.25f;
        }

        Rect ModuleInfoRect = new Rect(xPos, position.y, width, height);

        GUI.Box(ModuleInfoRect, "", GameManager.instance.standardBackGround);
        GUI.Label(ModuleSetNameRect, ResourceManager.instance.GetLocalization(module.GetParentSet().Name), GameManager.instance.ModuleTitleStyle);
        GUI.Label(ModuleSetDescRect, ResourceManager.instance.GetLocalization(module.GetParentSet().Description), GameManager.instance.ModuleDescStyle);

        for(int i = 0; i < LastHoveredModuleStats.Count; i++)
        {
            Vector2 statPosition = new Vector2(Indent + ModuleStatSize.x * (i % 4), ModuleSetDescRect.yMax + ModuleStatSize.y * (i / 4));
            LastHoveredModuleStats[i].DrawOffset(statPosition);
        }

        if(weapon != null)
        {
            float DamageBonus = GetModifiedDamage(weapon);
            float maxDamage = Mathf.Max(weapon.GetMaxDamage() + weapon.GetMaxDamage() * DamageBonus, 1f);
            float maxRange = Mathf.Max(weapon.GetMaxRangeDisplay(), 1f);
            float halfIndent = width * 0.025f;

            Vector2 graphPosition = new Vector2(xPos + ModuleWeaponDamageRect.width + halfIndent, ModuleSetDescRect.yMax + ModuleStatSize.y * (LastHoveredModuleStats.Count / 4 + 1));

            GUI.Label(new Rect(graphPosition, ModuleWeaponGraphTitleRect.size), "Weapon Damage Over Range", GameManager.instance.ModuleTitleStyle);

            graphPosition = new Vector2(xPos + halfIndent, graphPosition.y + ModuleWeaponGraphTitleRect.height);

            GUI.Label(new Rect(graphPosition, ModuleWeaponDamageRect.size), "Damage: " + maxDamage.ToString("0"), GameManager.instance.standardLabelStyle);

            graphPosition = new Vector2(xPos + ModuleWeaponDamageRect.width + halfIndent, graphPosition.y);
            Rect DamageGraphRect = new Rect(graphPosition.x + ModuleWeaponGraphRect.width * 0.05f, graphPosition.y + ModuleWeaponGraphRect.height * 0.05f, ModuleWeaponGraphRect.width * 0.9f, ModuleWeaponGraphRect.height * 0.9f);

            //GUI.DrawTexture(new Rect(graphPosition, ModuleWeaponGraphRect.size), ResourceManager.instance.GetUITexture("WeaponDamageGraph"));

            graphPosition = new Vector2(xPos + ModuleWeaponDamageRect.width + ModuleWeaponGraphRect.width - ModuleWeaponRangeRect.width + halfIndent, graphPosition.y + ModuleWeaponGraphRect.height);

            GUI.Label(new Rect(graphPosition, ModuleWeaponRangeRect.size), "Range: " + maxRange.ToString("0"), GameManager.instance.standardLabelStyle);


            List<Vector2> Points = new List<Vector2>();
            foreach (Weapon.DamageNode node in weapon.DamageGraph)
            {
                Points.Add(new Vector2(DamageGraphRect.x + DamageGraphRect.width * (node.GetDisplayRange() / maxRange), DamageGraphRect.yMax - DamageGraphRect.height * ((node.Damage + node.Damage * DamageBonus) / maxDamage)));
            }
            for (int i = 0; i < Points.Count - 1; i++)
            {
                new GUILine(Points[i], Points[i + 1], Color.red).Draw();
            }
        }
    }

    protected virtual bool CheckDesignAllowed(ShipDesign design)
    {
        return true;
    }

    protected virtual bool CheckDesignAllowed(StationDesign design)
    {
        return true;
    }

    protected virtual bool CheckHullAllowed(ShipHullData hull)
    {
        return true;
    }

    protected virtual bool CheckHullAllowed(StationHullData hull)
    {
        return true;
    }

    protected virtual bool CheckModuleSetAllowed(ModuleSet moduleSet)
    {
        return true;
    }

    protected virtual float GetModifiedDamage(Weapon weapon)
    {
        float moduleBonus = Mathf.Max(DesignDamageBonus);

        return moduleBonus;
    }

    protected void CalculateDesignWeaponPowerAmmoTime()
    {
        if (DesignWeaponAmmoConsumption > 0)
        {
            if (DesignAmmoGenerated >= DesignWeaponAmmoConsumption)
            {
                DesignWeaponAmmoTime = 9999;
            }
            else
            {
                DesignWeaponAmmoTime = DesignAmmo / (DesignWeaponAmmoConsumption - DesignAmmoGenerated);
            }
        }
        if (DesignWeaponPowerConsumption > 0)
        {
            if (DesignPowerSum >= DesignWeaponPowerConsumption)
            {
                DesignWeaponPowerTime = 9999;
            }
            else
            {
                DesignWeaponPowerTime = DesignPowerStorage / (DesignWeaponPowerConsumption - DesignPowerSum);
            }
        }
    }

    public string GetFormatedDesignRequiredCrew()
    {
        if (DesignRequiredCrew > DesignCrew)
        {
            return "<color=yellow>" + GetStandardDesignStatFormat(DesignRequiredCrew) + "/" + DesignCrew.ToString() + "</color>";
        }
        else
        {
            return GetStandardDesignStatFormat(DesignRequiredCrew) + "/" + DesignCrew.ToString();
        }
    }

    public string GetFormatedDesignTroops()
    {
        return GetStandardDesignStatFormat(DesignTroops);
    }

    public string GetFormattedDesignCost()
    {
        return GetStandardDesignStatFormat(DesignCost);
    }

    public string GetFormatedDesignProductionCost()
    {
        return GetStandardDesignStatFormat(DesignProductionCost);
    }

    public string GetFormattedDesignAlloyCost()
    {
        return GetStandardDesignStatFormat(DesignAlloyCost);
    }

    public string GetFormattedDesignAdvancedAlloyCost()
    {
        return GetStandardDesignStatFormat(DesignAdvancedAlloyCost);
    }

    public string GetFormattedDesignSuperiorAlloyCost()
    {
        return GetStandardDesignStatFormat(DesignSuperiorAlloyCost);
    }

    public string GetFormattedDesignCrystalCost()
    {
        return GetStandardDesignStatFormat(DesignCrystalCost);
    }

    public string GetFormattedDesignRareCrystalCost()
    {
        return GetStandardDesignStatFormat(DesignRareCrystalCost);
    }

    public string GetFormattedDesignExoticCrystalCost()
    {
        return GetStandardDesignStatFormat(DesignExoticCrystalCost);
    }

    public string GetFormattedDesignExoticParticleCost()
    {
        return GetStandardDesignStatFormat(DesignExoticParticleCost);
    }

    public string GetFormattedDesignPowerSum()
    {
        return GetOnlyPositiveDesignStatFormat(DesignPowerSum);
    }

    public string GetFormattedDesignAmmo()
    {
        return GetStandardDesignStatFormat(DesignAmmo);
    }

    public string GetFormattedDesignPowerStorage()
    {
        return GetStandardDesignStatFormat(DesignPowerStorage);
    }

    public string GetFormattedDesignSensor()
    {
        return GetStandardDesignStatFormat(DesignSensor);
    }

    public string GetFormattedDesignLongRangeSensorSum()
    {
        return GetStandardDesignStatFormat(DesignLongRangeSensorSum);
    }

    public string GetFormattedDesignAdvancedSensor()
    {
        return GetStandardDesignStatFormat(DesignAdvancedSensor);
    }

    public string GetFormattedDesignResearch()
    {
        return GetStandardDesignStatFormat(DesignResearch);
    }

    public string GetFormattedDesignMining()
    {
        return GetStandardDesignStatFormat(DesignMining);
    }

    public string GetFormattedDesignRepair()
    {
        return GetStandardDesignStatFormat(DesignRepair);
    }

    public string GetFormattedDesignAmmoGenerated()
    {
        return GetStandardDesignStatFormat(DesignAmmoGenerated);
    }

    public string GetFormattedDesignFighters()
    {
        return GetStandardDesignStatFormat(DesignFighters);
    }

    public string GetFormattedDesignHeavyFighters()
    {
        return GetStandardDesignStatFormat(DesignHeavyFighters);
    }

    public string GetFormattedDesignAssaultPods()
    {
        return GetStandardDesignStatFormat(DesignAssaultPods);
    }

    public string GetFormattedDesignMedical()
    {
        return GetStandardDesignStatFormat(DesignMedical);
    }

    public string GetFormattedDesignBoardingDefense()
    {
        return GetStandardDesignStatFormat(DesignBoardingDefense);
    }

    public string GetFormattedDesignTransporter()
    {
        return GetStandardDesignStatFormat(DesignTransporter);
    }

    public string GetFormattedDesignFirepower()
    {
        return GetStandardDesignStatFormat(DesignFirepower);
    }

    public string GetFormattedDesignWeaponAmmoConsumption()
    {
        return GetStandardDesignStatFormat(DesignWeaponAmmoConsumption);
    }

    public string GetFormattedDesignWeaponPowerConsumption()
    {
        return GetStandardDesignStatFormat(DesignWeaponPowerConsumption);
    }

    public string GetFormattedDesignDefenseRating()
    {
        return GetStandardDesignStatFormat(DesignDefenseRating);
    }

    public string GetFormattedDesignExperience()
    {
        return GetStandardDesignStatFormat(DesignExperience);
    }

    public string GetFormattedDesignWeaponPowerTime()
    {
        if (DesignWeaponPowerTime > 9998)
        {
            return "Infinite";
        }
        else
        {
            return GetStandardDesignStatFormat(DesignWeaponPowerTime);
        }
    }

    public string GetFormattedDesignWeaponAmmoTime()
    {
        if (DesignWeaponAmmoTime > 9998)
        {
            return "Infinite";
        }
        else
        {
            return GetStandardDesignStatFormat(DesignWeaponAmmoTime);
        }
    }

    public string GetFormattedDesignJammingCount()
    {
        return GetStandardDesignStatFormat(DesignJammingCount);
    }

    public string GetFormattedDesignJammingRange()
    {
        return GetStandardDesignStatFormat(DesignJammingRange);
    }

    public string GetFormattedDesignJammingDelay()
    {
        return GetStandardDesignStatFormat(DesignJammingDelay);
    }

    public string GetFormattedDesignDamageBonus()
    {
        return GetPercentageDesignStatFormat(DesignDamageBonus);
    }

    public string GetFormattedDesignDefenseBonus()
    {
        return GetPercentageDesignStatFormat(DesignDefenseBonus);
    }

    public string GetFormattedDesignPowerEfficiency()
    {
        return GetPercentageDesignStatFormat(DesignPowerEfficiency);
    }

    public string GetFormattedDesignDiplomacy()
    {
        return GetStandardDesignStatFormat(DesignDiplomacy);
    }

    protected string GetStandardDesignStatFormat(float value)
    {
        return value.ToString("0.#");
    }

    protected string GetPercentageDesignStatFormat(float value)
    {
        return "%" + GetStandardDesignStatFormat(value * 100);
    }

    protected string GetOnlyPositiveDesignStatFormat(float value)
    {
        if (value > 0)
        {
            return GetStandardDesignStatFormat(value);
        }
        else
        {
            return "<color=red>" + GetStandardDesignStatFormat(value) + "</color>";
        }
    }

    protected enum ModuleListTypes
    {
        Weapon,
        Defence,
        System
    }
}
