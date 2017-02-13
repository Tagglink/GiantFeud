using UnityEngine;
using System.Collections.Generic;

public enum ItemID { NULL, AXE, CLUB, KNIFE, SWORD, SPEAR, APPLE, ELIXIR, HALBERD, VILLAGER, MEATSTEW, MEATCLUB, BREADLOAF, ASGARDMEAL, HIDEARMOUR, STONEARMOUR, LEATHERARMOUR, _LENGTH }

public class Items : MonoBehaviour {

    public GameObject playerGiant;
    public GameObject enemyGiant;

    public static Dictionary<ItemID, Item> itemList = new Dictionary<ItemID, Item> {
            { ItemID.AXE,           new Weapon      ("Yxa",             "Väger 5 ton.",                             new Resources(3,0,0,0,5),       null, 0.0f, true,  new Stats(30,0,0,0,0),           0, new Stats(30, 0, 0, 0, 0))                },
            { ItemID.CLUB,          new Weapon      ("Klubba",          "Mer likt ett träd, faktiskt.",             new Resources(6,0,0,0,0),       null, 0.0f, true,  new Stats(25,0,0,0,0),           0, new Stats(25, 0, 0, 0, 0))                },
            { ItemID.KNIFE,         new Weapon      ("Kniv",            "Det HÄR är en kniv!",                      new Resources(1,0,0,0,1),       null, 0.0f, true,  new Stats(0,1f,0,0,0),           0, new Stats(0, 0.5f, 0, 0, 0))              },
            { ItemID.SWORD,         new Weapon      ("Svärd",           "Ett stort stensvärd. Snabb att svinga.",   new Resources(5,0,0,0,10),      null, 0.0f, true,  new Stats(50,0.5f,0,0,0),        0, new Stats(20, 0.5f, 0, 0, 0))             },
            { ItemID.SPEAR,         new Weapon      ("Spjut",           "Stock o sten.",                            new Resources(2,0,0,0,1),       null, 0.0f, true,  new Stats(5,0.5f,0,0,0),         0, new Stats(2, 0.25f, 0, 0, 0))             },
            { ItemID.HALBERD,       new Weapon      ("Hillebard",       "Nej, det är inte en bard på en kulle.",    new Resources(10,0,0,0,5),      null, 0.0f, true,  new Stats(150,0,0,0,0),          0, new Stats(125, -0.25f, 0, 0, 0))          },
            { ItemID.HIDEARMOUR,    new Armour      ("Hudrustning",     "Av den finaste kvalitén.",                 new Resources(0,0,15,0,0),      null, 0.0f, true,  new Stats(0,0,5,250,5),          0, new Stats(0, 0, 0, 250, 0))               },
            { ItemID.STONEARMOUR,   new Armour      ("Stenrustning",    "Den här rustningen är stenhård.",          new Resources(0,0,0,0,8),       null, 0.0f, true,  new Stats(0,0,5,0,0),            0, new Stats(0, 0, 5, 0, 0))                 },
            { ItemID.LEATHERARMOUR, new Armour      ("Läderrustning",   "Riktiga kläder.",                          new Resources(0,0,4,0,0),       null, 0.0f, true,  new Stats(0,0.25f,2,0,0),        0, new Stats(0, 0.25f, 2, 0, 0))             },
            { ItemID.APPLE,         new Consumable  ("Äpple",           "Håller doktorn borta.",                    new Resources(1,0,0,0,0),       null, 0.0f, true,  new Stats(0,0,0,0,0), 0.0f,      (Giant giant) => { giant.hp += 25; }, (Giant giant) => { }) },
            { ItemID.ELIXIR,        new Consumable  ("Elixir",          "En magisk brygd.",                         new Resources(0,4,8,8,0),       null, 0.0f, true,  new Stats(0,0,0,0,0), 20.0f,     (Giant giant) => { giant.AddMultiplier(ItemID.ELIXIR, new Stats(2, 2, 2, 2, 2)); }, (Giant giant) => { giant.RemoveMultiplier(ItemID.ELIXIR); }) },
            { ItemID.VILLAGER,      new Consumable  ("Bybo",            "En till bybo för att samla resurser.",     new Resources(0,1,2,4,0),       null, 0.0f, false, new Stats(0,0,0,0,0), 0.0f,      (Giant giant) => { Camp camp = giant.camp.GetComponent<Camp>(); if (camp.villagerCount < Camp.maxVillagers) { camp.villagerCount++; } else { Debug.Log("Cannot asses more villagers."); } }, (Giant giant) => {  }) },
            { ItemID.MEATSTEW,      new Consumable  ("Köttgryta",       "Bubblande med näring.",                    new Resources(0,0,3,2,0),       null, 0.0f, true,  new Stats(0,0,0,0,0), 60.0f,     (Giant giant) => { giant.hp += 100; giant.AddBuff(ItemID.MEATSTEW, new Stats(2, 0, 0, 0, 2)); }, (Giant giant) => { giant.RemoveBuff(ItemID.MEATSTEW); }) },
            { ItemID.MEATCLUB,      new Consumable  ("Köttklubba",      "Inte för att slåss.",                      new Resources(1,0,8,0,0),       null, 0.0f, true,  new Stats(0,0,0,0,0), 10.0f,     (Giant giant) => { giant.AddBuff(ItemID.MEATCLUB, new Stats(0, 0, 0, 0, 10)); }, (Giant giant) => { giant.RemoveBuff(ItemID.MEATCLUB); giant.camp.GetComponent<Camp>().resources.wood += 1; giant.hp += 400; }) },
            { ItemID.BREADLOAF,     new Consumable  ("Limpa",           "Stor som ett hus.",                        new Resources(0,8,0,2,0),       null, 0.0f, true,  new Stats(0,0,0,0,0), 10.0f,     (Giant giant) => { giant.hp += 200; giant.AddBuff(ItemID.BREADLOAF, new Stats(0, 1, 0, 0, 0)); }, (Giant giant) => { giant.RemoveBuff(ItemID.BREADLOAF); }) },
            { ItemID.ASGARDMEAL,    new Consumable  ("Asgårdmål",       "Tors favorit.",                            new Resources(0,20,20,15,0),    null, 0.0f, true,  new Stats(0,0,0,0,0), 0.0f,      (Giant giant) => { giant.AddBuff(ItemID.ASGARDMEAL, new Stats(0, 0, 0, 1000, 0)); giant.hp += 1000; }, (Giant giant) => { }) }
        };
}
