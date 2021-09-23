// Project:         Usable Starting Equipment mod for Daggerfall Unity (http://www.dfworkshop.net)
// Copyright:       Copyright (C) 2021 Ralzar
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Author:          Ralzar

using DaggerfallWorkshop;
using DaggerfallWorkshop.Game;
using DaggerfallWorkshop.Game.Utility.ModSupport;
using UnityEngine;
using DaggerfallWorkshop.Game.Entity;
using DaggerfallWorkshop.Game.Utility;
using System;
using DaggerfallWorkshop.Game.Items;
using DaggerfallConnect;
using Wenzil.Console;
using System.Collections.Generic;
using DaggerfallConnect;
using DaggerfallWorkshop.Utility;

public class UsableStartingEquipment : MonoBehaviour
{
    static Mod mod;
    public static UsableStartingEquipment Instance { get; private set; }

    static bool gameStart = false;
    static bool forbiddenIron = false;
    static bool forbiddenSteel = false;
    static bool forbiddenSilver = false;
    static bool forbiddenElven = false;
    static bool forbiddenDwarven = false;
    static bool forbiddenMithril = false;
    static bool forbiddenAdam = false;
    static bool forbiddenEbony = false;
    static bool forbiddenOrcish = false;
    static bool forbiddenDaedric = false;
    static bool forbiddenPlate = false;
    static bool forbiddenChain = false;
    static bool forbiddenLeather = false;
    static ulong time = 0;

    static PlayerEntity playerEntity = GameManager.Instance.PlayerEntity;

    [Invoke(StateManager.StateTypes.Start, 0)]
    public static void Init(InitParams initParams)
    {
        mod = initParams.Mod;
        var go = new GameObject(mod.Title);
        Instance = go.AddComponent<UsableStartingEquipment>();

        StartGameBehaviour.OnStartGame += StartDetection_OnStartGame;
        Debug.Log("[Usable Starting Equipment] Mod Init");
    }

    private void Awake()
    {
        mod.IsReady = true;
        Debug.Log("[Usable Starting Equipment] Mod isReady");
    }

    private void Start()
    {
        RegisterUSECommands();
    }

    private void Update()
    {
        if (gameStart)
        {

            if (!GameManager.IsGamePaused && time == 0)
            {
                time = DaggerfallUnity.Instance.WorldTime.DaggerfallDateTime.ToSeconds();
            }
            if (!GameManager.IsGamePaused && DaggerfallUnity.Instance.WorldTime.DaggerfallDateTime.ToSeconds() > (time + 10))
            {
                time = 0;
                gameStart = false;
                convertEquipment();
            }
        }
    }

    static void StartDetection_OnStartGame(object sender, EventArgs e)
    {
        gameStart = true;
    }

    static void convertEquipment()
    {
        Debug.Log("[Usable Starting Equipment] convertEquipment()");

        SetBools();

        List<DaggerfallUnityItem> wpnArmList = new List<DaggerfallUnityItem>();

        for (int i = 0; i < playerEntity.Items.Count; i++)
        {
            DaggerfallUnityItem item = playerEntity.Items.GetItem(i);

            if (item.ItemGroup == ItemGroups.Armor || item.ItemGroup == ItemGroups.Weapons)
            {
                wpnArmList.Add(item);
            }
        }

        foreach (DaggerfallUnityItem item in wpnArmList)
        {
            Debug.Log("[Usable Starting Equipment] Trying " + item.shortName);
            if (item.ItemGroup == ItemGroups.Armor)
            {
                if (item.IsShield)
                    convertShield(item);
                else
                    ConvertArmor(item);
            }
            if (item.ItemGroup == ItemGroups.Weapons)
            {
                ConvertWeapon(item);
            }
        }
    }

    static void SetBools()
    {
        forbiddenIron = playerEntity.Career.IsMaterialForbidden(DFCareer.MaterialFlags.Iron);
        forbiddenSteel = playerEntity.Career.IsMaterialForbidden(DFCareer.MaterialFlags.Steel);
        forbiddenSilver = playerEntity.Career.IsMaterialForbidden(DFCareer.MaterialFlags.Silver);
        forbiddenElven = playerEntity.Career.IsMaterialForbidden(DFCareer.MaterialFlags.Elven);
        forbiddenDwarven = playerEntity.Career.IsMaterialForbidden(DFCareer.MaterialFlags.Dwarven);
        forbiddenMithril = playerEntity.Career.IsMaterialForbidden(DFCareer.MaterialFlags.Mithril);
        forbiddenAdam = playerEntity.Career.IsMaterialForbidden(DFCareer.MaterialFlags.Adamantium);
        forbiddenEbony = playerEntity.Career.IsMaterialForbidden(DFCareer.MaterialFlags.Ebony);
        forbiddenOrcish = playerEntity.Career.IsMaterialForbidden(DFCareer.MaterialFlags.Orcish);
        forbiddenDaedric = playerEntity.Career.IsMaterialForbidden(DFCareer.MaterialFlags.Daedric);
        forbiddenPlate = playerEntity.Career.IsArmorForbidden(DFCareer.ArmorFlags.Plate);
        forbiddenChain = playerEntity.Career.IsArmorForbidden(DFCareer.ArmorFlags.Chain);
        forbiddenLeather = playerEntity.Career.IsArmorForbidden(DFCareer.ArmorFlags.Leather);
    }

    static void ConvertArmor(DaggerfallUnityItem item)
    {
        ArmorMaterialTypes armorMat = (ArmorMaterialTypes)item.nativeMaterialValue;
        bool forbiddenMat = (playerEntity.Career.IsMaterialForbidden(DFCareer.MaterialFlags.Steel) && item.nativeMaterialValue == (int)ArmorMaterialTypes.Steel) || (playerEntity.Career.IsMaterialForbidden(DFCareer.MaterialFlags.Iron) && item.nativeMaterialValue == (int)ArmorMaterialTypes.Iron);
        bool forbiddenPlate = playerEntity.Career.IsArmorForbidden(DFCareer.ArmorFlags.Plate) && item.nativeMaterialValue > (int)ArmorMaterialTypes.Chain2;
        bool forbiddenChain = playerEntity.Career.IsArmorForbidden(DFCareer.ArmorFlags.Chain) && item.nativeMaterialValue > (int)ArmorMaterialTypes.Leather && !forbiddenPlate;
        bool forbiddenLeather = playerEntity.Career.IsArmorForbidden(DFCareer.ArmorFlags.Leather) && item.nativeMaterialValue == (int)ArmorMaterialTypes.Leather && !forbiddenPlate && !forbiddenChain;

        DFCareer.MaterialFlags matFlag = DFCareer.MaterialFlags.Iron;
        DFCareer.ArmorFlags armFlag = DFCareer.ArmorFlags.Leather;

        switch (item.nativeMaterialValue)
        {
            case (int)ArmorMaterialTypes.Leather:
                armFlag = DFCareer.ArmorFlags.Leather;
                break;
            case (int)ArmorMaterialTypes.Chain:
            case (int)ArmorMaterialTypes.Chain2:
                armFlag = DFCareer.ArmorFlags.Chain;
                break;
            default:
                armFlag = DFCareer.ArmorFlags.Plate;
                break;
        }

        switch (armorMat)
        {
            case ArmorMaterialTypes.Leather:
            case ArmorMaterialTypes.Chain:
            case ArmorMaterialTypes.Chain2:
            case ArmorMaterialTypes.Iron:
                matFlag = DFCareer.MaterialFlags.Iron;
                break;
            case ArmorMaterialTypes.Steel:
                matFlag = DFCareer.MaterialFlags.Steel;
                break;
            case ArmorMaterialTypes.Silver:
                matFlag = DFCareer.MaterialFlags.Silver;
                break;
            case ArmorMaterialTypes.Elven:
                matFlag = DFCareer.MaterialFlags.Elven;
                break;
            case ArmorMaterialTypes.Dwarven:
                matFlag = DFCareer.MaterialFlags.Dwarven;
                break;
            case ArmorMaterialTypes.Mithril:
                matFlag = DFCareer.MaterialFlags.Mithril;
                break;
            case ArmorMaterialTypes.Adamantium:
                matFlag = DFCareer.MaterialFlags.Adamantium;
                break;
            case ArmorMaterialTypes.Ebony:
                matFlag = DFCareer.MaterialFlags.Ebony;
                break;
            case ArmorMaterialTypes.Orcish:
                matFlag = DFCareer.MaterialFlags.Orcish;
                break;
            case ArmorMaterialTypes.Daedric:
                matFlag = DFCareer.MaterialFlags.Daedric;
                break;
        }


        if (playerEntity.Career.IsArmorForbidden(armFlag))
        {
            switch (armFlag)
            {
                case DFCareer.ArmorFlags.Plate:
                    if (!playerEntity.Career.IsArmorForbidden(DFCareer.ArmorFlags.Chain))
                    {
                        armorMat = ArmorMaterialTypes.Chain;
                        matFlag = DFCareer.MaterialFlags.Iron;
                    }
                    else if (!playerEntity.Career.IsArmorForbidden(DFCareer.ArmorFlags.Leather))
                    {
                        armorMat = ArmorMaterialTypes.Leather;
                        matFlag = DFCareer.MaterialFlags.Iron;
                    }
                    break;
                case DFCareer.ArmorFlags.Chain:
                    if (!playerEntity.Career.IsArmorForbidden(DFCareer.ArmorFlags.Leather))
                    {
                        armorMat = ArmorMaterialTypes.Leather;
                        matFlag = DFCareer.MaterialFlags.Iron;
                    }
                    break;
            }
        }
        else if (playerEntity.Career.IsMaterialForbidden(matFlag))
        {
            switch (matFlag)
            {
                case DFCareer.MaterialFlags.Daedric:
                    if (!forbiddenOrcish)
                        armorMat = ArmorMaterialTypes.Orcish;
                    else if (!forbiddenEbony)
                        armorMat = ArmorMaterialTypes.Ebony;
                    else if (!forbiddenAdam)
                        armorMat = ArmorMaterialTypes.Adamantium;
                    else if (!forbiddenMithril)
                        armorMat = ArmorMaterialTypes.Mithril;
                    else if (!forbiddenDwarven)
                        armorMat = ArmorMaterialTypes.Dwarven;
                    else if (!forbiddenElven)
                        armorMat = ArmorMaterialTypes.Elven;
                    else if (!forbiddenSilver)
                        armorMat = ArmorMaterialTypes.Silver;
                    else if (!forbiddenSteel)
                        armorMat = ArmorMaterialTypes.Steel;
                    else if (!forbiddenIron)
                        armorMat = ArmorMaterialTypes.Iron;
                    else if (!forbiddenChain)
                        armorMat = ArmorMaterialTypes.Chain;
                    else if (!forbiddenLeather)
                        armorMat = ArmorMaterialTypes.Leather;
                    break;
                case DFCareer.MaterialFlags.Orcish:
                    if (!forbiddenEbony)
                        armorMat = ArmorMaterialTypes.Ebony;
                    else if (!forbiddenAdam)
                        armorMat = ArmorMaterialTypes.Adamantium;
                    else if (!forbiddenMithril)
                        armorMat = ArmorMaterialTypes.Mithril;
                    else if (!forbiddenDwarven)
                        armorMat = ArmorMaterialTypes.Dwarven;
                    else if (!forbiddenElven)
                        armorMat = ArmorMaterialTypes.Elven;
                    else if (!forbiddenSilver)
                        armorMat = ArmorMaterialTypes.Silver;
                    else if (!forbiddenSteel)
                        armorMat = ArmorMaterialTypes.Steel;
                    else if (!forbiddenIron)
                        armorMat = ArmorMaterialTypes.Iron;
                    else if (!forbiddenChain)
                        armorMat = ArmorMaterialTypes.Chain;
                    else if (!forbiddenLeather)
                        armorMat = ArmorMaterialTypes.Leather;
                    break;
                case DFCareer.MaterialFlags.Ebony:
                    if (!forbiddenAdam)
                        armorMat = ArmorMaterialTypes.Adamantium;
                    else if (!forbiddenMithril)
                        armorMat = ArmorMaterialTypes.Mithril;
                    else if (!forbiddenDwarven)
                        armorMat = ArmorMaterialTypes.Dwarven;
                    else if (!forbiddenElven)
                        armorMat = ArmorMaterialTypes.Elven;
                    else if (!forbiddenSilver)
                        armorMat = ArmorMaterialTypes.Silver;
                    else if (!forbiddenSteel)
                        armorMat = ArmorMaterialTypes.Steel;
                    else if (!forbiddenIron)
                        armorMat = ArmorMaterialTypes.Iron;
                    else if (!forbiddenChain)
                        armorMat = ArmorMaterialTypes.Chain;
                    else if (!forbiddenLeather)
                        armorMat = ArmorMaterialTypes.Leather;
                    break;
                case DFCareer.MaterialFlags.Adamantium:
                    if (!forbiddenMithril)
                        armorMat = ArmorMaterialTypes.Mithril;
                    else if (!forbiddenDwarven)
                        armorMat = ArmorMaterialTypes.Dwarven;
                    else if (!forbiddenElven)
                        armorMat = ArmorMaterialTypes.Elven;
                    else if (!forbiddenSilver)
                        armorMat = ArmorMaterialTypes.Silver;
                    else if (!forbiddenSteel)
                        armorMat = ArmorMaterialTypes.Steel;
                    else if (!forbiddenIron)
                        armorMat = ArmorMaterialTypes.Iron;
                    else if (!forbiddenChain)
                        armorMat = ArmorMaterialTypes.Chain;
                    else if (!forbiddenLeather)
                        armorMat = ArmorMaterialTypes.Leather;
                    break;
                case DFCareer.MaterialFlags.Mithril:
                    if (!forbiddenDwarven)
                        armorMat = ArmorMaterialTypes.Dwarven;
                    else if (!forbiddenElven)
                        armorMat = ArmorMaterialTypes.Elven;
                    else if (!forbiddenSilver)
                        armorMat = ArmorMaterialTypes.Silver;
                    else if (!forbiddenSteel)
                        armorMat = ArmorMaterialTypes.Steel;
                    else if (!forbiddenIron)
                        armorMat = ArmorMaterialTypes.Iron;
                    else if (!forbiddenChain)
                        armorMat = ArmorMaterialTypes.Chain;
                    else if (!forbiddenLeather)
                        armorMat = ArmorMaterialTypes.Leather;
                    break;
                case DFCareer.MaterialFlags.Dwarven:
                    if (!forbiddenElven)
                        armorMat = ArmorMaterialTypes.Elven;
                    else if (!forbiddenSilver)
                        armorMat = ArmorMaterialTypes.Silver;
                    else if (!forbiddenSteel)
                        armorMat = ArmorMaterialTypes.Steel;
                    else if (!forbiddenIron)
                        armorMat = ArmorMaterialTypes.Iron;
                    else if (!forbiddenChain)
                        armorMat = ArmorMaterialTypes.Chain;
                    else if (!forbiddenLeather)
                        armorMat = ArmorMaterialTypes.Leather;
                    break;
                case DFCareer.MaterialFlags.Elven:
                    if (!forbiddenSilver)
                        armorMat = ArmorMaterialTypes.Silver;
                    else if (!forbiddenSteel)
                        armorMat = ArmorMaterialTypes.Steel;
                    else if (!forbiddenIron)
                        armorMat = ArmorMaterialTypes.Iron;
                    else if (!forbiddenChain)
                        armorMat = ArmorMaterialTypes.Chain;
                    else if (!forbiddenLeather)
                        armorMat = ArmorMaterialTypes.Leather;
                    break;
                case DFCareer.MaterialFlags.Silver:
                    if (!forbiddenSteel)
                        armorMat = ArmorMaterialTypes.Steel;
                    else if (!forbiddenIron)
                        armorMat = ArmorMaterialTypes.Iron;
                    else if (!forbiddenChain)
                        armorMat = ArmorMaterialTypes.Chain;
                    else if (!forbiddenLeather)
                        armorMat = ArmorMaterialTypes.Leather;
                    break;
                case DFCareer.MaterialFlags.Steel:
                    if (!forbiddenIron)
                        armorMat = ArmorMaterialTypes.Iron;
                    else if (!forbiddenChain)
                        armorMat = ArmorMaterialTypes.Chain;
                    else if (!forbiddenLeather)
                        armorMat = ArmorMaterialTypes.Leather;
                    break;
                case DFCareer.MaterialFlags.Iron:
                    if (!forbiddenChain)
                        armorMat = ArmorMaterialTypes.Chain;
                    else if (!forbiddenLeather)
                        armorMat = ArmorMaterialTypes.Leather;
                    break;
            }
        }

        if (forbiddenChain && (armorMat == ArmorMaterialTypes.Chain || armorMat == ArmorMaterialTypes.Chain) )
        {
                armorMat = ArmorMaterialTypes.Leather;
        }

        if (forbiddenLeather && armorMat == ArmorMaterialTypes.Leather)
        {
            ArmorNotPossible(item);
        }
        else if (armorMat != (ArmorMaterialTypes)item.nativeMaterialValue)
        {
            DaggerfallUnityItem newItem = ItemBuilder.CreateArmor(playerEntity.Gender, playerEntity.Race, (Armor)item.TemplateIndex, armorMat);
            playerEntity.Items.RemoveItem(item);
            playerEntity.Items.AddItem(newItem);
        }
    }


    static void convertShield(DaggerfallUnityItem item)
    {
        ArmorMaterialTypes armorMat = (ArmorMaterialTypes)item.nativeMaterialValue;
        int templateIndex = item.TemplateIndex;
        DFCareer.ShieldFlags shieldFlag = DFCareer.ShieldFlags.TowerShield;
        DFCareer.MaterialFlags matFlag = DFCareer.MaterialFlags.Iron;

        switch (templateIndex)
        {
            case 109:
                shieldFlag = DFCareer.ShieldFlags.Buckler;
                break;
            case 110:
                shieldFlag = DFCareer.ShieldFlags.RoundShield;
                break;
            case 111:
                shieldFlag = DFCareer.ShieldFlags.KiteShield;
                break;
        }

        switch (armorMat)
        {
            case ArmorMaterialTypes.Leather:
            case ArmorMaterialTypes.Chain:
            case ArmorMaterialTypes.Chain2:
            case ArmorMaterialTypes.Iron:
                matFlag = DFCareer.MaterialFlags.Iron;
                break;
            case ArmorMaterialTypes.Steel:
                matFlag = DFCareer.MaterialFlags.Steel;
                break;
            case ArmorMaterialTypes.Silver:
                matFlag = DFCareer.MaterialFlags.Silver;
                break;
            case ArmorMaterialTypes.Elven:
                matFlag = DFCareer.MaterialFlags.Elven;
                break;
            case ArmorMaterialTypes.Dwarven:
                matFlag = DFCareer.MaterialFlags.Dwarven;
                break;
            case ArmorMaterialTypes.Mithril:
                matFlag = DFCareer.MaterialFlags.Mithril;
                break;
            case ArmorMaterialTypes.Adamantium:
                matFlag = DFCareer.MaterialFlags.Adamantium;
                break;
            case ArmorMaterialTypes.Ebony:
                matFlag = DFCareer.MaterialFlags.Ebony;
                break;
            case ArmorMaterialTypes.Orcish:
                matFlag = DFCareer.MaterialFlags.Orcish;
                break;
            case ArmorMaterialTypes.Daedric:
                matFlag = DFCareer.MaterialFlags.Daedric;
                break;
        }

        if (playerEntity.Career.IsShieldForbidden(shieldFlag))
        {
            switch (shieldFlag)
            {
                case DFCareer.ShieldFlags.TowerShield:
                    if (!playerEntity.Career.IsShieldForbidden(DFCareer.ShieldFlags.KiteShield))
                        templateIndex = 111;
                    else if (!playerEntity.Career.IsShieldForbidden(DFCareer.ShieldFlags.RoundShield))
                        templateIndex = 1110;
                    else if (!playerEntity.Career.IsShieldForbidden(DFCareer.ShieldFlags.Buckler))
                        templateIndex = 109;
                    break;
                case DFCareer.ShieldFlags.KiteShield:
                    if (!playerEntity.Career.IsShieldForbidden(DFCareer.ShieldFlags.RoundShield))
                        templateIndex = 110;
                    else if (!playerEntity.Career.IsShieldForbidden(DFCareer.ShieldFlags.Buckler))
                        templateIndex = 109;
                    break;
                case DFCareer.ShieldFlags.RoundShield:
                    if (!playerEntity.Career.IsShieldForbidden(DFCareer.ShieldFlags.Buckler))
                        templateIndex = 109;
                    break;
            }
        }

        if (playerEntity.Career.IsMaterialForbidden(matFlag))
        {
            switch (matFlag)
            {
                case DFCareer.MaterialFlags.Daedric:
                    if (!forbiddenOrcish)
                        armorMat = ArmorMaterialTypes.Orcish;
                    else if (!forbiddenEbony)
                        armorMat = ArmorMaterialTypes.Ebony;
                    else if (!forbiddenAdam)
                        armorMat = ArmorMaterialTypes.Adamantium;
                    else if (!forbiddenMithril)
                        armorMat = ArmorMaterialTypes.Mithril;
                    else if (!forbiddenDwarven)
                        armorMat = ArmorMaterialTypes.Dwarven;
                    else if (!forbiddenElven)
                        armorMat = ArmorMaterialTypes.Elven;
                    else if (!forbiddenSilver)
                        armorMat = ArmorMaterialTypes.Silver;
                    else if (!forbiddenSteel)
                        armorMat = ArmorMaterialTypes.Steel;
                    else if (!forbiddenIron)
                        armorMat = ArmorMaterialTypes.Iron;
                    else if (!forbiddenChain)
                        armorMat = ArmorMaterialTypes.Chain;
                    else if (!forbiddenLeather)
                        armorMat = ArmorMaterialTypes.Leather;
                    break;
                case DFCareer.MaterialFlags.Orcish:
                    if (!forbiddenEbony)
                        armorMat = ArmorMaterialTypes.Ebony;
                    else if (!forbiddenAdam)
                        armorMat = ArmorMaterialTypes.Adamantium;
                    else if (!forbiddenMithril)
                        armorMat = ArmorMaterialTypes.Mithril;
                    else if (!forbiddenDwarven)
                        armorMat = ArmorMaterialTypes.Dwarven;
                    else if (!forbiddenElven)
                        armorMat = ArmorMaterialTypes.Elven;
                    else if (!forbiddenSilver)
                        armorMat = ArmorMaterialTypes.Silver;
                    else if (!forbiddenSteel)
                        armorMat = ArmorMaterialTypes.Steel;
                    else if (!forbiddenIron)
                        armorMat = ArmorMaterialTypes.Iron;
                    else if (!forbiddenChain)
                        armorMat = ArmorMaterialTypes.Chain;
                    else if (!forbiddenLeather)
                        armorMat = ArmorMaterialTypes.Leather;
                    break;
                case DFCareer.MaterialFlags.Ebony:
                    if (!forbiddenAdam)
                        armorMat = ArmorMaterialTypes.Adamantium;
                    else if (!forbiddenMithril)
                        armorMat = ArmorMaterialTypes.Mithril;
                    else if (!forbiddenDwarven)
                        armorMat = ArmorMaterialTypes.Dwarven;
                    else if (!forbiddenElven)
                        armorMat = ArmorMaterialTypes.Elven;
                    else if (!forbiddenSilver)
                        armorMat = ArmorMaterialTypes.Silver;
                    else if (!forbiddenSteel)
                        armorMat = ArmorMaterialTypes.Steel;
                    else if (!forbiddenIron)
                        armorMat = ArmorMaterialTypes.Iron;
                    else if (!forbiddenChain)
                        armorMat = ArmorMaterialTypes.Chain;
                    else if (!forbiddenLeather)
                        armorMat = ArmorMaterialTypes.Leather;
                    break;
                case DFCareer.MaterialFlags.Adamantium:
                    if (!forbiddenMithril)
                        armorMat = ArmorMaterialTypes.Mithril;
                    else if (!forbiddenDwarven)
                        armorMat = ArmorMaterialTypes.Dwarven;
                    else if (!forbiddenElven)
                        armorMat = ArmorMaterialTypes.Elven;
                    else if (!forbiddenSilver)
                        armorMat = ArmorMaterialTypes.Silver;
                    else if (!forbiddenSteel)
                        armorMat = ArmorMaterialTypes.Steel;
                    else if (!forbiddenIron)
                        armorMat = ArmorMaterialTypes.Iron;
                    else if (!forbiddenChain)
                        armorMat = ArmorMaterialTypes.Chain;
                    else if (!forbiddenLeather)
                        armorMat = ArmorMaterialTypes.Leather;
                    break;
                case DFCareer.MaterialFlags.Mithril:
                    if (!forbiddenDwarven)
                        armorMat = ArmorMaterialTypes.Dwarven;
                    else if (!forbiddenElven)
                        armorMat = ArmorMaterialTypes.Elven;
                    else if (!forbiddenSilver)
                        armorMat = ArmorMaterialTypes.Silver;
                    else if (!forbiddenSteel)
                        armorMat = ArmorMaterialTypes.Steel;
                    else if (!forbiddenIron)
                        armorMat = ArmorMaterialTypes.Iron;
                    else if (!forbiddenChain)
                        armorMat = ArmorMaterialTypes.Chain;
                    else if (!forbiddenLeather)
                        armorMat = ArmorMaterialTypes.Leather;
                    break;
                case DFCareer.MaterialFlags.Dwarven:
                    if (!forbiddenElven)
                        armorMat = ArmorMaterialTypes.Elven;
                    else if (!forbiddenSilver)
                        armorMat = ArmorMaterialTypes.Silver;
                    else if (!forbiddenSteel)
                        armorMat = ArmorMaterialTypes.Steel;
                    else if (!forbiddenIron)
                        armorMat = ArmorMaterialTypes.Iron;
                    else if (!forbiddenChain)
                        armorMat = ArmorMaterialTypes.Chain;
                    else if (!forbiddenLeather)
                        armorMat = ArmorMaterialTypes.Leather;
                    break;
                case DFCareer.MaterialFlags.Elven:
                    if (!forbiddenSilver)
                        armorMat = ArmorMaterialTypes.Silver;
                    else if (!forbiddenSteel)
                        armorMat = ArmorMaterialTypes.Steel;
                    else if (!forbiddenIron)
                        armorMat = ArmorMaterialTypes.Iron;
                    else if (!forbiddenChain)
                        armorMat = ArmorMaterialTypes.Chain;
                    else if (!forbiddenLeather)
                        armorMat = ArmorMaterialTypes.Leather;
                    break;
                case DFCareer.MaterialFlags.Silver:
                    if (!forbiddenSteel)
                        armorMat = ArmorMaterialTypes.Steel;
                    else if (!forbiddenIron)
                        armorMat = ArmorMaterialTypes.Iron;
                    else if (!forbiddenChain)
                        armorMat = ArmorMaterialTypes.Chain;
                    else if (!forbiddenLeather)
                        armorMat = ArmorMaterialTypes.Leather;
                    break;
                case DFCareer.MaterialFlags.Steel:
                    if (!forbiddenIron)
                        armorMat = ArmorMaterialTypes.Iron;
                    else if (!forbiddenChain)
                        armorMat = ArmorMaterialTypes.Chain;
                    else if (!forbiddenLeather)
                        armorMat = ArmorMaterialTypes.Leather;
                    break;
                case DFCareer.MaterialFlags.Iron:
                    if (!forbiddenChain)
                        armorMat = ArmorMaterialTypes.Chain;
                    else if (!forbiddenLeather)
                        armorMat = ArmorMaterialTypes.Leather;
                    break;
            }
        }

        if(armorMat != (ArmorMaterialTypes)item.nativeMaterialValue || templateIndex != item.TemplateIndex)
        {
            DaggerfallUnityItem newItem = ItemBuilder.CreateArmor(playerEntity.Gender, playerEntity.Race, (Armor)templateIndex, ArmorMaterialTypes.Leather);
            playerEntity.Items.RemoveItem(item);
            playerEntity.Items.AddItem(newItem);
        }
        else if (playerEntity.Career.IsShieldForbidden(shieldFlag) || playerEntity.Career.IsMaterialForbidden(matFlag))
        {
            ShieldNotPossible(item);
        }
    }


    static void ConvertWeapon(DaggerfallUnityItem item)
    {
        Debug.Log("[Usable Starting Equipment] ConvertWeapon");
        WeaponMaterialTypes wpnMat = (WeaponMaterialTypes)item.nativeMaterialValue;
        DFCareer.MaterialFlags matFlag = DFCareer.MaterialFlags.Iron;
        bool forbiddenWpn = ((item.GetWeaponSkillUsed() & (int)playerEntity.Career.ForbiddenProficiencies) != 0);

        switch (wpnMat)
        {
            case WeaponMaterialTypes.Iron:
                matFlag = DFCareer.MaterialFlags.Iron;
                break;
            case WeaponMaterialTypes.Steel:
                matFlag = DFCareer.MaterialFlags.Steel;
                break;
            case WeaponMaterialTypes.Silver:
                matFlag = DFCareer.MaterialFlags.Silver;
                break;
            case WeaponMaterialTypes.Elven:
                matFlag = DFCareer.MaterialFlags.Elven;
                break;
            case WeaponMaterialTypes.Dwarven:
                matFlag = DFCareer.MaterialFlags.Dwarven;
                break;
            case WeaponMaterialTypes.Mithril:
                matFlag = DFCareer.MaterialFlags.Mithril;
                break;
            case WeaponMaterialTypes.Adamantium:
                matFlag = DFCareer.MaterialFlags.Adamantium;
                break;
            case WeaponMaterialTypes.Ebony:
                matFlag = DFCareer.MaterialFlags.Ebony;
                break;
            case WeaponMaterialTypes.Orcish:
                matFlag = DFCareer.MaterialFlags.Orcish;
                break;
            case WeaponMaterialTypes.Daedric:
                matFlag = DFCareer.MaterialFlags.Daedric;
                break;
        }

        if (playerEntity.Career.IsMaterialForbidden(matFlag))
        {
            switch (matFlag)
            {
                case DFCareer.MaterialFlags.Daedric:
                    if (!forbiddenOrcish)
                        wpnMat = WeaponMaterialTypes.Orcish;
                    else if (!forbiddenEbony)
                        wpnMat = WeaponMaterialTypes.Ebony;
                    else if (!forbiddenAdam)
                        wpnMat = WeaponMaterialTypes.Adamantium;
                    else if (!forbiddenMithril)
                        wpnMat = WeaponMaterialTypes.Mithril;
                    else if (!forbiddenDwarven)
                        wpnMat = WeaponMaterialTypes.Dwarven;
                    else if (!forbiddenElven)
                        wpnMat = WeaponMaterialTypes.Elven;
                    else if (!forbiddenSilver)
                        wpnMat = WeaponMaterialTypes.Silver;
                    else if (!forbiddenSteel)
                        wpnMat = WeaponMaterialTypes.Steel;
                    else if (!forbiddenIron)
                        wpnMat = WeaponMaterialTypes.Iron;
                    break;
                case DFCareer.MaterialFlags.Orcish:
                    if (!forbiddenEbony)
                        wpnMat = WeaponMaterialTypes.Ebony;
                    else if (!forbiddenAdam)
                        wpnMat = WeaponMaterialTypes.Adamantium;
                    else if (!forbiddenMithril)
                        wpnMat = WeaponMaterialTypes.Mithril;
                    else if (!forbiddenDwarven)
                        wpnMat = WeaponMaterialTypes.Dwarven;
                    else if (!forbiddenElven)
                        wpnMat = WeaponMaterialTypes.Elven;
                    else if (!forbiddenSilver)
                        wpnMat = WeaponMaterialTypes.Silver;
                    else if (!forbiddenSteel)
                        wpnMat = WeaponMaterialTypes.Steel;
                    else if (!forbiddenIron)
                        wpnMat = WeaponMaterialTypes.Iron;
                    break;
                case DFCareer.MaterialFlags.Ebony:
                    if (!forbiddenAdam)
                        wpnMat = WeaponMaterialTypes.Adamantium;
                    else if (!forbiddenMithril)
                        wpnMat = WeaponMaterialTypes.Mithril;
                    else if (!forbiddenDwarven)
                        wpnMat = WeaponMaterialTypes.Dwarven;
                    else if (!forbiddenElven)
                        wpnMat = WeaponMaterialTypes.Elven;
                    else if (!forbiddenSilver)
                        wpnMat = WeaponMaterialTypes.Silver;
                    else if (!forbiddenSteel)
                        wpnMat = WeaponMaterialTypes.Steel;
                    else if (!forbiddenIron)
                        wpnMat = WeaponMaterialTypes.Iron;
                    break;
                case DFCareer.MaterialFlags.Adamantium:
                    if (!forbiddenMithril)
                        wpnMat = WeaponMaterialTypes.Mithril;
                    else if (!forbiddenDwarven)
                        wpnMat = WeaponMaterialTypes.Dwarven;
                    else if (!forbiddenElven)
                        wpnMat = WeaponMaterialTypes.Elven;
                    else if (!forbiddenSilver)
                        wpnMat = WeaponMaterialTypes.Silver;
                    else if (!forbiddenSteel)
                        wpnMat = WeaponMaterialTypes.Steel;
                    else if (!forbiddenIron)
                        wpnMat = WeaponMaterialTypes.Iron;
                    break;
                case DFCareer.MaterialFlags.Mithril:
                    if (!forbiddenDwarven)
                        wpnMat = WeaponMaterialTypes.Dwarven;
                    else if (!forbiddenElven)
                        wpnMat = WeaponMaterialTypes.Elven;
                    else if (!forbiddenSilver)
                        wpnMat = WeaponMaterialTypes.Silver;
                    else if (!forbiddenSteel)
                        wpnMat = WeaponMaterialTypes.Steel;
                    else if (!forbiddenIron)
                        wpnMat = WeaponMaterialTypes.Iron;
                    break;
                case DFCareer.MaterialFlags.Dwarven:
                    if (!forbiddenElven)
                        wpnMat = WeaponMaterialTypes.Elven;
                    else if (!forbiddenSilver)
                        wpnMat = WeaponMaterialTypes.Silver;
                    else if (!forbiddenSteel)
                        wpnMat = WeaponMaterialTypes.Steel;
                    else if (!forbiddenIron)
                        wpnMat = WeaponMaterialTypes.Iron;
                    break;
                case DFCareer.MaterialFlags.Elven:
                    if (!forbiddenSilver)
                        wpnMat = WeaponMaterialTypes.Silver;
                    else if (!forbiddenSteel)
                        wpnMat = WeaponMaterialTypes.Steel;
                    else if (!forbiddenIron)
                        wpnMat = WeaponMaterialTypes.Iron;
                    break;
                case DFCareer.MaterialFlags.Silver:
                    if (!forbiddenSteel)
                        wpnMat = WeaponMaterialTypes.Steel;
                    else if (!forbiddenIron)
                        wpnMat = WeaponMaterialTypes.Iron;
                    break;
                case DFCareer.MaterialFlags.Steel:
                    if (!forbiddenIron)
                        wpnMat = WeaponMaterialTypes.Iron;
                    break;
            }
        }



        if (forbiddenWpn)
        {
            WpnNotPossible(item);
        }
        else if (playerEntity.Career.IsMaterialForbidden(matFlag))
        {
            if (item.nativeMaterialValue == (int)WeaponMaterialTypes.Iron)
            {
                WpnNotPossible(item);
            }
            else
            {
                DaggerfallUnityItem newItem = ItemBuilder.CreateWeapon((Weapons)item.TemplateIndex, wpnMat);
                playerEntity.Items.RemoveItem(item);
                playerEntity.Items.AddItem(newItem);
            }
        }
        else
        {
            WrongWpnDyeCheck(item);
        }
    }


    static void ArmorNotPossible(DaggerfallUnityItem item)
    {
        playerEntity.Items.RemoveItem(item);
        int index;


        if (playerEntity.Gender == Genders.Male)
        {
            index = UnityEngine.Random.Range(151, 182);
            switch ((Armor)item.TemplateIndex)
            {
                case Armor.Boots:
                    index = UnityEngine.Random.Range(147, 151);
                    break;
                case Armor.Cuirass:
                    index = UnityEngine.Random.Range(163, 174);
                    break;
                case Armor.Greaves:
                    index = UnityEngine.Random.Range(151, 153);
                    break;
            }

            DaggerfallUnityItem newItem = ItemBuilder.CreateMensClothing((MensClothing)index, playerEntity.Race);
            playerEntity.Items.AddItem(newItem);
        }
        else
        {
            index = UnityEngine.Random.Range(182, 217);
            switch ((Armor)item.TemplateIndex)
            {
                case Armor.Boots:
                    index = UnityEngine.Random.Range(186, 190);
                    break;
                case Armor.Cuirass:
                    index = UnityEngine.Random.Range(200, 210);
                    break;
                case Armor.Greaves:
                    index = UnityEngine.Random.Range(190, 192);
                    break;
            }

            DaggerfallUnityItem newItem = ItemBuilder.CreateWomensClothing((WomensClothing)index, playerEntity.Race);
            playerEntity.Items.AddItem(newItem);
        }
    }

    static void ShieldNotPossible(DaggerfallUnityItem item)
    {
        playerEntity.Items.RemoveItem(item);
        playerEntity.GoldPieces += UnityEngine.Random.Range(5, 30);
    }

    static void WpnNotPossible(DaggerfallUnityItem item)
    {
        playerEntity.Items.RemoveItem(item);
        playerEntity.GoldPieces += UnityEngine.Random.Range(5, 30);
    }


    static void WrongWpnDyeCheck(DaggerfallUnityItem item)
    {
        Debug.Log("[Usable Starting Equipment] WrongWpnDyeCheck");
        PlayerEntity playerEntity = GameManager.Instance.PlayerEntity;
        WeaponMaterialTypes mat = (WeaponMaterialTypes)item.nativeMaterialValue;
        DFCareer.MaterialFlags matFlag = DFCareer.MaterialFlags.Iron;
        switch (item.dyeColor)
        {
            case DyeColors.Iron:
                mat = WeaponMaterialTypes.Iron;
                break;
            case DyeColors.Steel:
                mat = WeaponMaterialTypes.Steel;
                matFlag = DFCareer.MaterialFlags.Steel;
                break;
            case DyeColors.SilverOrElven:
                mat = WeaponMaterialTypes.Silver;
                matFlag = DFCareer.MaterialFlags.Silver;
                break;
            case DyeColors.Elven:
                mat = WeaponMaterialTypes.Elven;
                matFlag = DFCareer.MaterialFlags.Elven;
                break;
            case DyeColors.Dwarven:
                mat = WeaponMaterialTypes.Dwarven;
                matFlag = DFCareer.MaterialFlags.Dwarven;
                break;
            case DyeColors.Mithril:
                mat = WeaponMaterialTypes.Mithril;
                matFlag = DFCareer.MaterialFlags.Mithril;
                break;
            case DyeColors.Adamantium:
                mat = WeaponMaterialTypes.Adamantium;
                matFlag = DFCareer.MaterialFlags.Adamantium;
                break;
            case DyeColors.Ebony:
                mat = WeaponMaterialTypes.Ebony;
                matFlag = DFCareer.MaterialFlags.Ebony;
                break;
            case DyeColors.Orcish:
                mat = WeaponMaterialTypes.Orcish;
                matFlag = DFCareer.MaterialFlags.Orcish;
                break;
            case DyeColors.Daedric:
                mat = WeaponMaterialTypes.Daedric;
                matFlag = DFCareer.MaterialFlags.Daedric;
                break;
        }
        
        Debug.Log("[Usable Starting Equipment] mat = " + mat.ToString());
        Debug.Log("[Usable Starting Equipment] native = " + ((WeaponMaterialTypes)item.nativeMaterialValue).ToString());
        if (playerEntity.Career.IsMaterialForbidden(matFlag))
        {
            DaggerfallUnityItem newItem = ItemBuilder.CreateWeapon((Weapons)item.TemplateIndex, (WeaponMaterialTypes)item.nativeMaterialValue);
            playerEntity.Items.RemoveItem(item);
            playerEntity.Items.AddItem(newItem);
        }
        else
        {
            DaggerfallUnityItem newItem = ItemBuilder.CreateWeapon((Weapons)item.TemplateIndex, mat);
            playerEntity.Items.RemoveItem(item);
            playerEntity.Items.AddItem(newItem);
        }
    }


    public static void RegisterUSECommands()
    {
        Debug.Log("[Usable Starting Equipment] Trying to register console commands.");
        try
        {
            ConsoleCommandsDatabase.RegisterCommand(ConvertItems.name, ConvertItems.description, ConvertItems.usage, ConvertItems.Execute);
        }
        catch (Exception e)
        {
            Debug.LogError(string.Format("Error Registering UsableStartingEquipment Console commands: {0}", e.Message));
        }
    }

    private static class ConvertItems
    {
        public static readonly string name = "convert_items";
        public static readonly string description = "convert unusable items to usable items";
        public static readonly string usage = "convert_items";

        public static string Execute(params string[] args)
        {
            convertEquipment();



            return "Attempting to Convert Items";
        }
    }
}
