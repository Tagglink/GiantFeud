using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class Giant : MonoBehaviour {

    public Weapon currentWeapon;
    public Armour currentArmour;
    public Stats baseStats;
    public List<Stats> buffs;
    public List<Stats> multipliers;
    public Stats stats;

    public GameObject healthDisplay;
    public GameObject enemyGiant;

    public GameObject camp;

    private int timer;
    private int atkTime;
    

    void Attack()
    {
        int damageDealt;

        if (currentWeapon != null)
            damageDealt = stats.atk + currentWeapon.stats.atk;
        else
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
            AddBuff(currentWeapon.reinforcementStats);
        }
        else if (item is Armour)
        {
            currentArmour.reinforcementCount += 1;
            AddBuff(currentArmour.reinforcementStats);
        }
    }

    void Eat(Consumable cons)
    {
        cons.action(GetComponent<Giant>());

        if (cons.duration > 0)
        {
            StartCoroutine(DelayTemporaryBuff(cons));
        }
    }

    void Equip(Equipment equipment)
    {
        if (equipment is Armour)
        {
            if (currentArmour != null)
                UnEquip(currentArmour);

            currentArmour = equipment as Armour;
        }
        else if (equipment is Weapon)
        {
            if (currentWeapon != null)
                UnEquip(currentWeapon);

            currentWeapon = equipment as Weapon;
        }

        AddBuff(equipment.stats);
    }

    void UnEquip(Equipment equipment)
    {
        RemoveBuff(equipment.stats + (equipment.reinforcementStats * equipment.reinforcementCount));

        if (equipment is Armour)
            currentArmour = null;
        else if (equipment is Weapon)
            currentWeapon = null;
    }

    public void AddBuff(Stats buff)
    {
        buffs.Add(buff);
    }

    public void RemoveBuff(Stats buff)
    {
        
    }

    public void AddMultiplier(Stats multiplier)
    {
        multipliers.Add(multiplier);
    }

    public void RemoveMultiplier(Stats multiplier)
    {

    }

    IEnumerator DelayTemporaryBuff(Consumable consumable)
    {
        yield return new WaitForSeconds(consumable.duration);
        consumable.reverseAction(GetComponent<Giant>());
    }

    void Start()
    {
        // default Giant stats
        baseStats = new Stats(5, 0.5f, 0, 3000, 3000, 0);
    }

    void Update()
    {
        if (stats.hp > stats.maxHP)
        {
            stats.hp = stats.maxHP;
        }
        if (stats.hp < 0)
        {
            stats.hp = 0;
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
