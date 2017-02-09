using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class Giant : MonoBehaviour {

    public Weapon currentWeapon;
    public Armour currentArmour;
    public List<KeyValuePair<ItemID, Stats>> buffs;
    public List<KeyValuePair<ItemID, Stats>> multipliers;
    public Stats baseStats;
    public Stats stats;
    
    public GameObject enemyGiant; // inspector set
    public GameObject camp; // inspector set
    public GameObject gameManager; // inspector set

    private int timer;
    private int atkTime;
    private bool statsChanged;

    void Start()
    {
        buffs = new List<KeyValuePair<ItemID, Stats>>();
        multipliers = new List<KeyValuePair<ItemID, Stats>>();
        statsChanged = true;
        timer = 0;
        atkTime = 0;

        // default Giant stats
        baseStats = new Stats(5, 0.5f, 0, 3000, 3000, 0);
        stats = baseStats;

        currentWeapon = new Weapon("Fists", "Unequipped", new Resources(0, 0, 0, 0, 0), null, 0.0f, false, new Stats(0, 0, 0, 0, 0, 0), 0, new Stats(0, 0, 0, 0, 0, 0));
        currentArmour = new Armour("Garments", "Unequipped", new Resources(0, 0, 0, 0, 0), null, 0.0f, false, new Stats(0, 0, 0, 0, 0, 0), 0, new Stats(0, 0, 0, 0, 0, 0));
    }

    void Update()
    {
        if (statsChanged)
        {
            UpdateStats();
            statsChanged = false;
        }

        if (stats.hp > stats.maxHP)
        {
            stats.hp = stats.maxHP;
        }

        if (stats.hp <= 0)
        {
            stats.hp = 0;
            gameManager.GetComponent<GameManager>().EndGame(this);
        }
    }


    void Attack()
    {
        int damageDealt;
        
        damageDealt = stats.atk;

        enemyGiant.GetComponent<Giant>().TakeDamage(damageDealt);
    }

    public void TakeDamage(int damageTaken)
    {
        stats.hp -= damageTaken - stats.def;
    }

    public void UseItem(Item item)
    {
        if (item is Equipment)
        {
            Equipment equipment = item as Equipment;
            Equip(equipment);
        }
        else if (item is Consumable)
        {
            Consumable consumable = item as Consumable;
            Eat(consumable);
        }
    }

    public void Reinforce(Equipment item)
    {
        if (item is Weapon)
        {
            currentWeapon.reinforcementCount += 1;
        }
        else if (item is Armour)
        {
            currentArmour.reinforcementCount += 1;
        }

        statsChanged = true;
    }

    void Eat(Consumable cons)
    {
        cons.action(GetComponent<Giant>());

        if (cons.duration > 0)
            StartCoroutine(DelayTemporaryBuff(cons));

        statsChanged = true;
    }

    void Equip(Equipment equipment)
    {
        if (equipment is Armour)
        {
            currentArmour = equipment as Armour;
        }
        else if (equipment is Weapon)
        {
            currentWeapon = equipment as Weapon;
        }

        statsChanged = true;
    }

    public void AddBuff(ItemID item, Stats buff)
    {
        buffs.Add(new KeyValuePair<ItemID, Stats>(item, buff));
        statsChanged = true;
    }

    public void RemoveBuff(ItemID item)
    {
        buffs = RemoveFromKeyValueList(buffs, item);
        statsChanged = true;
    }

    public void AddMultiplier(ItemID item, Stats buff)
    {
        multipliers.Add(new KeyValuePair<ItemID, Stats>(item, buff));
        statsChanged = true;
    }

    public void RemoveMultiplier(ItemID item)
    {
        multipliers = RemoveFromKeyValueList(multipliers, item);
        statsChanged = true;
    }

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

    IEnumerator DelayTemporaryBuff(Consumable consumable)
    {
        yield return new WaitForSeconds(consumable.duration);
        consumable.reverseAction(GetComponent<Giant>());
    }

    void UpdateStats()
    {
        Stats multiplier = new Stats(0, 0, 0, 0, 0, 0);
        Stats baseStatsWithoutHp = baseStats;
        baseStatsWithoutHp.hp = stats.hp;

        stats = baseStatsWithoutHp;

        stats += currentArmour.stats + (currentArmour.reinforcementStats * currentArmour.reinforcementCount);
        stats += currentWeapon.stats + (currentWeapon.reinforcementStats * currentWeapon.reinforcementCount);

        foreach (KeyValuePair<ItemID, Stats> buff in buffs)
            stats += buff.Value;

        if (multipliers.Count > 0)
        {
            foreach (KeyValuePair<ItemID, Stats> mult in multipliers)
                multiplier += mult.Value;

            stats *= multiplier;
        }
    }

    void FixedUpdate()
    {
        timer++;
        atkTime = Mathf.RoundToInt(50 / stats.atkspd);
        if (timer % atkTime == 0)
        {
            Attack();
        }
        if (timer % 50 == 0)
        {
            stats.hp += stats.hpPerSec;
        }
    }
}
