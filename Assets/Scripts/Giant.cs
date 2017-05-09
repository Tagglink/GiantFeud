using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class Giant : MonoBehaviour {

    /// <summary>
    /// The Giant's equipped Weapon.
    /// </summary>
    /// <remarks>
    /// 
    /// </remarks>
    public Weapon currentWeapon;

    /// <summary>
    /// The Giant's equipped Armour.
    /// </summary>
    public Armour currentArmour;

    /// <summary>
    /// List of the Giant's temporary additional stats.
    /// </summary>
    public List<KeyValuePair<ItemID, Stats>> buffs;

    /// <summary>
    /// List of what to multiply the Giant's stats with.
    /// Default is 1.
    /// </summary>
    public List<KeyValuePair<ItemID, Stats>> multipliers;

    /// <summary>
    /// Giant's default stats.
    /// </summary>
    public Stats baseStats;

    /// <summary>
    /// Giant's total stats.
    /// </summary>
    public Stats stats;
    
    /// <summary>
    /// The GameObject for the enemy Giant.
    /// </summary>
    /// <remarks>
    /// inspector set
    /// Game Object
    /// </remarks>
    public GameObject enemyGiant;

    /// <summary>
    /// The GameObject for this Giant's base.
    /// </summary>
    /// <remarks>
    /// inspector set
    /// </remarks>
    public GameObject camp;

    /// <summary>
    /// GameObject for the GameManager.
    /// To access its script.
    /// </summary>
    /// <remarks>
    /// inspector set
    /// </remarks>
    public GameObject gameManager;

    /// <summary>
    /// HUD Object to display resources and stats.
    /// </summary>
    /// <remarks>
    /// inspector set
    /// HUD object
    /// </remarks>
    public HUDStatusBox hudStatusBox;

    /// <summary>
    /// Animator component attached to the Giant.
    /// </summary>
    /// <remarks>
    /// component
    /// </remarks>
    public Animator animator;

    /// <summary>
    /// Used to display numbers of damage and healing.
    /// </summary>
    /// <remarks>
    /// Inspector set
    /// </remarks>
    public GameObject numberSpawner;

    /// <summary>
    /// The Giant's health stat.
    /// </summary>
    public int hp;

    /// <summary>
    /// Used to detect if stats have changed.
    /// </summary>
    public bool statsChanged;

    /// <summary>
    /// Integer used to time when the Giant should attack.
    /// </summary>
    private int timer;

    /// <summary>
    /// Integer used to calculate when the Giant should attack.
    /// </summary>
    private int atkTime;

    void Start()
    {
        // Make empty lists for buffs and multipliers
        buffs = new List<KeyValuePair<ItemID, Stats>>();
        multipliers = new List<KeyValuePair<ItemID, Stats>>();

        // Default statsChanged bool to true
        statsChanged = true;

        // Default timer to 0
        timer = 0;

        // Default atkTime to 0
        atkTime = 0;

        // Default hp to 10000
        hp = 10000;

        // Set the animator variable to the Animator component
        animator = GetComponent<Animator>();

        // default Giant stats
        baseStats = new Stats(5, 0.5f, 0, 10000, 0);

        // default Weapon and Armour Equipment
        currentWeapon = new Weapon("Fists", "Unequipped", new Resources(0, 0, 0, 0, 0), null, 0.0f, false, new Stats(0, 0, 0, 0, 0), 0, new Stats(0, 0, 0, 0, 0));
        currentArmour = new Armour("Garments", "Unequipped", new Resources(0, 0, 0, 0, 0), null, 0.0f, false, new Stats(0, 0, 0, 0, 0), 0, new Stats(0, 0, 0, 0, 0));
        
        // Update Stats so they are correct
        UpdateStats();
    }

    void Update()
    {
        // If stats have changed
        // Update them so they are correct
        // Set statsChanged variable to false so it is not called until another occurance of stats changing
        if (statsChanged)
        {
            UpdateStats();
            statsChanged = false;
        }

        // Prevent health from overflowing the set max value
        if (hp > stats.maxHP)
        {
            hp = stats.maxHP;
        }
        
        // If the giant reaches 0 health or below,
        // We end the game and tell GameManager which Giant died,
        // So it can display the correct end screen
        if (hp <= 0)
        {
            gameManager.GetComponent<GameManager>().EndGame(this);
        }
    }

    /// <summary>
    /// Makes the enemy Giant lose health according to this Giants attack stat.
    /// </summary>
    void Attack()
    {
        int damageDealt;
        
        damageDealt = stats.atk;

        enemyGiant.GetComponent<Giant>().TakeDamage(damageDealt);
    }


    /// <summary>
    /// Removes health from this Giant.
    /// Used from the enemy Giants Attack function.
    /// </summary>
    /// <param name="damageTaken">
    /// Damage amount.
    /// </param>
    public void TakeDamage(int damageTaken)
    {
        // Reduce HP with damage amount subtracted by this Giant's DEF stat
        hp -= damageTaken - stats.def;

    }

    /// <summary>
    /// Equips or Consumes an Item.
    /// </summary>
    /// <param name="item">
    /// Item to use.
    /// </param>
    public void UseItem(Item item)
    {
        // Equip the item if it is a weapon or armour
        if (item is Equipment)
        {
            Equipment equipment = item as Equipment;
            Equip(equipment);
        }
        // Consume the item if it is a consumable
        else if (item is Consumable)
        {
            Consumable consumable = item as Consumable;
            Eat(consumable);
        }
    }

    /// <summary>
    /// Uses the effect of a Consumable.
    /// </summary>
    /// <param name="cons">
    /// The Consumable to use.
    /// </param>
    void Eat(Consumable cons)
    {
        // Use the effect of the Consumable on this Giant
        cons.action(GetComponent<Giant>());

        // If the Consumable has a duration,
        // start a coroutine with the Consumable's duration as a delay,
        // which uses the Consumables end effect after the delay
        if (cons.duration > 0)
            StartCoroutine(DelayTemporaryBuff(cons));

        // Update statsChanged bool to true to update the stats
        statsChanged = true;
    }

    /// <summary>
    /// Equips the Equipment to the Giant.
    /// </summary>
    /// <param name="equipment">
    /// The Equipment to be equipped.
    /// </param>
    void Equip(Equipment equipment)
    {
        Armour armor;
        Weapon weapon;

        if (equipment is Armour)
        {
            // If you already have the same Armour equipped,
            // Add to its reinforcementCount to reinforce it
            if (currentArmour.name == equipment.name)
            {
                currentArmour.reinforcementCount++;
            }
            // Otherwise equip a new one
            else
            {
                armor = new Armour(equipment as Armour);
                currentArmour = armor;
            }
        }
        else if (equipment is Weapon)
        {
            // If you already have the same Weapon equipped,
            // Add to its reinforcementCount to reinforce it
            if (currentWeapon.name == equipment.name)
            {
                currentWeapon.reinforcementCount++;
            }
            // Otherwise equip a new one
            else
            {
                weapon = new Weapon(equipment as Weapon);
                currentWeapon = weapon;
            }
        }

        // Update statsChanged bool to true to update the stats
        statsChanged = true;
    }

    /// <summary>
    /// Add a stat addition to the Giant.
    /// </summary>
    /// <param name="item">
    /// ID of the item.
    /// </param>
    /// <param name="buff">
    /// Stats to add to the Giant.
    /// </param>
    public void AddBuff(ItemID item, Stats buff)
    {
        // Add the buff to the buff list
        buffs.Add(new KeyValuePair<ItemID, Stats>(item, buff));

        // Update statsChanged bool to true to update the stats
        statsChanged = true;
    }

    /// <summary>
    /// Remove a stat addition to the Giant.
    /// </summary>
    /// <param name="item">
    /// ID of the item.
    /// </param>
    public void RemoveBuff(ItemID item)
    {
        /// <see cref="RemoveFromKeyValueList(List{KeyValuePair{ItemID, Stats}}, ItemID)"/>
        buffs = RemoveFromKeyValueList(buffs, item);

        // Update statsChanged bool to true to update the stats
        statsChanged = true;
    }

    /// <summary>
    /// Add a Multiplier to the Giant.
    /// </summary>
    /// <param name="item">
    /// ID of the item.
    /// </param>
    /// <param name="buff">
    /// Multiplication values.
    /// </param>
    public void AddMultiplier(ItemID item, Stats buff)
    {
        // Add the multiplier to the multiplier list
        multipliers.Add(new KeyValuePair<ItemID, Stats>(item, buff));

        // Update statsChanged bool to true to update the stats
        statsChanged = true;
    }

    /// <summary>
    /// Remove a Multiplier to the Giant.
    /// </summary>
    /// <param name="item">
    /// ID of the item.
    /// </param>
    public void RemoveMultiplier(ItemID item)
    {
        /// <see cref="RemoveFromKeyValueList(List{KeyValuePair{ItemID, Stats}}, ItemID)"/>
        multipliers = RemoveFromKeyValueList(multipliers, item);

        // Update statsChanged bool to true to update the stats
        statsChanged = true;
    }

    /// <summary>
    /// Finds an ID, removes it from a List, returns an updated List.
    /// </summary>
    /// <param name="list">
    /// List which contains the ID.
    /// </param>
    /// <param name="id">
    /// ID to remove.
    /// </param>
    /// <returns>
    /// Returns the updated list.
    /// </returns>
    List<KeyValuePair<ItemID, Stats>> RemoveFromKeyValueList(List<KeyValuePair<ItemID, Stats>> list, ItemID id)
    {
        for (int i = 0; i < list.Count; i++)
        {
            KeyValuePair<ItemID, Stats> pair = list[i];
            if (id == pair.Key)
                list.RemoveAt(i);
        }

        return list;
    }

    /// <summary>
    /// Calls a Consumable's end action after its duration.
    /// </summary>
    IEnumerator DelayTemporaryBuff(Consumable consumable)
    {
        yield return new WaitForSeconds(consumable.duration);
        consumable.reverseAction(GetComponent<Giant>());
    }

    /// <summary>
    /// Updates the stats with Armour, Weapon, Buffs, and Multiplier taken into account.
    /// As well as updating HUD and Animations.
    /// </summary>
    void UpdateStats()
    {
        Stats multiplier = new Stats(0, 0, 0, 0, 0);

        // Resets stats
        stats = baseStats;

        // Adds the Weapon and Armour stats to the stats
        stats += currentArmour.stats + (currentArmour.reinforcementStats * currentArmour.reinforcementCount);
        stats += currentWeapon.stats + (currentWeapon.reinforcementStats * currentWeapon.reinforcementCount);

        // Adds all buffs to the stats
        foreach (KeyValuePair<ItemID, Stats> buff in buffs)
            stats += buff.Value;

        // If multiplier exists, multiply stats with it
        if (multipliers.Count > 0)
        {
            foreach (KeyValuePair<ItemID, Stats> mult in multipliers)
                multiplier += mult.Value;

            stats *= multiplier;
        }

        // Update Gauges on HUD to fit the updated stats
        if (hudStatusBox)
            hudStatusBox.UpdateStatGauges();

        // Update the attack animation to accomodate to the updated stats
        animator.SetFloat("swing_speed", stats.atkspd);

        // Update the Weapon Sprite the Giant is holding, to match the Weapon currently equipped
        transform.GetChild(0).GetChild(0).GetComponent<SpriteRenderer>().sprite = currentWeapon.weaponSprite;
    }

    void FixedUpdate()
    {
        timer++;

        // Calculate how many attacks per second
        // 50 FixedUpdates is one second (0.02 per update)
        atkTime = Mathf.RoundToInt(50 / stats.atkspd);

        // Once per (1 / Attacks per second) your Giant attacks
        if (timer % atkTime == 0)
        {
            Attack();
        }

        // Health Regeneration is added to Health once per second
        if (timer % 50 == 0)
        {
            hp += stats.hpPerSec;
        }
    }
}