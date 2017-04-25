using UnityEngine;
using System;
using System.Collections.Generic;

public enum ItemID { NULL, AXE, CLUB, KNIFE, SWORD, SPEAR, APPLE, ELIXIR, HALBERD, VILLAGER, MEATSTEW, MEATCLUB, BREADLOAF, ASGARDMEAL, HIDEARMOUR, STONEARMOUR, LEATHERARMOUR, _LENGTH }

public class Items : MonoBehaviour {

    public GameObject playerGiant;
    public GameObject enemyGiant;

    public Sprite[] weaponIcons;

    public static Dictionary<ItemID, Item> itemList = new Dictionary<ItemID, Item> {
            { ItemID.AXE,           new Weapon      ("Yxa",             "Väger 5 ton.",                             new Resources(3,0,0,0,5),       null, 0.0f, true,  new Stats(30,0,0,0,0),           0, new Stats(30, 0, 0, 0, 0))       },
            { ItemID.CLUB,          new Weapon      ("Klubba",          "Mer likt ett träd, faktiskt.",             new Resources(6,0,0,0,0),       null, 0.0f, true,  new Stats(25,0,0,0,0),           0, new Stats(25, 0, 0, 0, 0))       },
            { ItemID.KNIFE,         new Weapon      ("Kniv",            "Det HÄR är en kniv!",                      new Resources(1,0,0,0,1),       null, 0.0f, true,  new Stats(0,1f,0,0,0),           0, new Stats(0, 0.5f, 0, 0, 0))     },
            { ItemID.SWORD,         new Weapon      ("Svärd",           "Ett stort stensvärd. Snabb att svinga.",   new Resources(5,0,0,0,10),      null, 0.0f, true,  new Stats(50,0.5f,0,0,0),        0, new Stats(20, 0.5f, 0, 0, 0))    },
            { ItemID.SPEAR,         new Weapon      ("Spjut",           "Stock o sten.",                            new Resources(2,0,0,0,1),       null, 0.0f, true,  new Stats(5,0.5f,0,0,0),         0, new Stats(0, 0.2f, 0, 0, 0))     },
            { ItemID.HALBERD,       new Weapon      ("Hillebard",       "Nej, det är inte en bard på en kulle.",    new Resources(10,0,0,0,5),      null, 0.0f, true,  new Stats(150,-0.25f,0,0,0),     0, new Stats(125, -0.25f, 0, 0, 0)) },
            { ItemID.HIDEARMOUR,    new Armour      ("Hudrustning",     "Av den finaste kvalitén.",                 new Resources(0,0,15,0,0),      null, 0.0f, true,  new Stats(0,0,10,500,5),         0, new Stats(0, 0, 0, 500, 0))      },
            { ItemID.STONEARMOUR,   new Armour      ("Stenrustning",    "Den här rustningen är stenhård.",          new Resources(0,0,0,0,8),       null, 0.0f, true,  new Stats(0,0,5,0,0),            0, new Stats(0, 0, 5, 0, 0))        },
            { ItemID.LEATHERARMOUR, new Armour      ("Läderrustning",   "Riktiga kläder.",                          new Resources(0,0,4,0,0),       null, 0.0f, true,  new Stats(0,0.25f,2,0,0),        0, new Stats(0, 0.25f, 2, 0, 0))    },
            { ItemID.APPLE,         new Consumable  ("Äpple",           "Håller doktorn borta.",                    new Resources(1,0,0,0,0),       null, 0.0f, true,  0.0f,  AppleAction(),            AppleReverseAction())               },
            { ItemID.ELIXIR,        new Consumable  ("Elixir",          "En magisk brygd.",                         new Resources(0,4,8,8,0),       null, 0.0f, true,  5.0f,  ElixirAction(),           ElixirReverseAction())              },
            { ItemID.VILLAGER,      new Consumable  ("Bybo",            "En till bybo för att samla resurser.",     new Resources(0,1,2,4,0),       null, 0.0f, false, 0.0f,  VillagerAction(),         VillagerReverseAction())            },
            { ItemID.MEATSTEW,      new Consumable  ("Köttgryta",       "Bubblande med näring.",                    new Resources(0,0,3,2,0),       null, 0.0f, true,  60.0f, MeatstewAction(),         MeatstewReverseAction())            },
            { ItemID.MEATCLUB,      new Consumable  ("Köttklubba",      "Inte för att slåss.",                      new Resources(1,0,8,0,0),       null, 0.0f, true,  10.0f, MeatclubAction(),         MeatclubReverseAction())            },
            { ItemID.BREADLOAF,     new Consumable  ("Limpa",           "Stor som ett hus.",                        new Resources(0,8,0,2,0),       null, 0.0f, true,  10.0f, BreadloafAction(),        BreadloafReverseAction())           },
            { ItemID.ASGARDMEAL,    new Consumable  ("Asgårdmål",       "Tors favorit.",                            new Resources(0,20,20,15,0),    null, 0.0f, true,  0.0f,  AsgardmealAction(),       AsgardmealReverseAction())          }
        };
    
    void Start()
    {
        itemList[ItemID.AXE].icon = weaponIcons[0];
        itemList[ItemID.CLUB].icon = weaponIcons[1];
        itemList[ItemID.KNIFE].icon = weaponIcons[2];
        itemList[ItemID.SWORD].icon = weaponIcons[3];
        itemList[ItemID.SPEAR].icon = weaponIcons[4];
        itemList[ItemID.HALBERD].icon   = weaponIcons[5];
    }

    static Action<Giant> AppleAction()
    {
        return new Action<Giant>((Giant giant) => {
            giant.hp += 25;
        });
    }
    static Action<Giant> AppleReverseAction()
    {
        return new Action<Giant>((Giant giant) => {
            
        });
    }
    static Action<Giant> ElixirAction()
    {
        return new Action<Giant>((Giant giant) => {
            giant.AddMultiplier(ItemID.ELIXIR, new Stats(2, 2, 2, 1, 2));
        });
    }
    static Action<Giant> ElixirReverseAction()
    {
        return new Action<Giant>((Giant giant) => {
            giant.RemoveMultiplier(ItemID.ELIXIR);
        });
    }
    static Action<Giant> VillagerAction()
    {
        return new Action<Giant>((Giant giant) => {

        });
    }
    static Action<Giant> VillagerReverseAction()
    {
        return new Action<Giant>((Giant giant) => {
            Camp camp = giant.camp.GetComponent<Camp>();
            if (camp.villagerCount < Camp.maxVillagers)
            {
                camp.villagerCount++;
            }
            else
            {
                Debug.Log("Cannot assess more villagers.");
            }
        });
    }

    static Action<Giant> MeatstewAction()
    {
        return new Action<Giant>((Giant giant) => {
            giant.hp += 300;
            giant.AddBuff(ItemID.MEATSTEW, new Stats(5, 0, 0, 0, 10));
        });
    }
    static Action<Giant> MeatstewReverseAction()
    {
        return new Action<Giant>((Giant giant) => {
            giant.RemoveBuff(ItemID.MEATSTEW);
        });
    }

    static Action<Giant> MeatclubAction()
    {
        return new Action<Giant>((Giant giant) => {
            giant.AddBuff(ItemID.MEATCLUB, new Stats(0, 0, 0, 0, 100));
        });
    }
    static Action<Giant> MeatclubReverseAction()
    {
        return new Action<Giant>((Giant giant) => {
            giant.RemoveBuff(ItemID.MEATCLUB);
            giant.camp.GetComponent<Camp>().resources.wood += 1;
            giant.hp += 400;
        });
    }

    static Action<Giant> BreadloafAction()
    {
        return new Action<Giant>((Giant giant) => {
            giant.hp += 500;
            giant.AddBuff(ItemID.BREADLOAF, new Stats(0, 1, 0, 0, 0));
        });
    }

    static Action<Giant> BreadloafReverseAction()
    {
        return new Action<Giant>((Giant giant) => {
            giant.RemoveBuff(ItemID.BREADLOAF);
        });
    }

    static Action<Giant> AsgardmealAction()
    {
        return new Action<Giant>((Giant giant) => {
            giant.hp += giant.baseStats.maxHP;
            giant.AddBuff(ItemID.ASGARDMEAL, new Stats(0, 0, 0, giant.baseStats.maxHP, 0));
        });
    }

    static Action<Giant> AsgardmealReverseAction()
    {
        return new Action<Giant>((Giant giant) => {

        });
    }
}
