using UnityEngine;
using System.Collections.Generic;

public enum ItemID { NULL, AXE, CLUB, SWORD, SPEAR, ELIXIR, HALBERD, MEATSTEW, BREADLOAF, STONEARMOUR }

public static class Items {

    public static Dictionary <ItemID, Item> itemList = new Dictionary<ItemID, Item> {
            { ItemID.AXE, new Weapon("Yxa", "Väger 5 ton.", new Resources(3,0,0,0,5), null, new Stats(30, 0, 0, 0, 0, 0, -1), 0, new Stats(30, 0, 0, 0, 0, 0, -1)) },
            { ItemID.CLUB, new Weapon("Klubba", "Mer likt ett träd, faktiskt.", new Resources(6,0,0,0,0),null, new Stats(25,0,0,0,0,0,-1),0, new Stats(25,0,0,0,0,0,-1)) },
            { ItemID.SWORD, new Weapon("Svärd", "Ett stort, sten svärd. Snabb att svinga.", new Resources(5,0,0,0,10), null, new Stats(50,0.5f,0,0,0,0,-1), 0, new Stats(20,0.5f,0,0,0,0,-1)) },
            { ItemID.SPEAR, new Weapon("Spjut", "Stock o sten.", new Resources(2, 0, 0, 0, 1), null, new Stats(5, 0.5f, 0, 0, 0, 0, -1), 0, new Stats(2, 0.25f, 0, 0, 0, 0, -1)) },
            { ItemID.ELIXIR, new Consumable("Elixir", "En magisk brygd.", new Resources(0,4,8,8,0), null, new Stats(0,0,0,0,0,0,20), (Giant giant) => { giant.stats += giant.stats; }, (Giant giant) => { giant.stats = giant.stats / 2; }) },
            { ItemID.HALBERD, new Weapon("Hillebard", "Nej, det är inte en bard på en kulle.", new Resources(10,0,0,0,5), null, new Stats(150,0,0,0,0,0,-1), 0, new Stats(125,-0.25f, 0, 0, 0, 0, -1)) },
            { ItemID.MEATSTEW, new Consumable("Kött Gryta", "Bubblande med näring.", new Resources(0, 0, 3, 2, 0), null, new Stats(2, 0, 0, 0, 200, 2, 60), (Giant giant) => {  }, (Giant giant) => { giant.stats.hp += 200; }) },
            { ItemID.BREADLOAF, new Consumable("Limpa", "Stor som ett hus.", new Resources(0,8,0,2,0), null, new Stats(0,1,0,0,200,0,10), (Giant giant) => { }, (Giant giant) => { giant.stats.hp += 200; }) },
            { ItemID.STONEARMOUR, new Armour("Sten Rustning", "Den här rustningen är stenhård.", new Resources(0,0,0,0,8), null, new Stats(0,0,5,0,0,0,-1), 0, new Stats(0,0,5,0,0,0,-1)) }
        };
}
