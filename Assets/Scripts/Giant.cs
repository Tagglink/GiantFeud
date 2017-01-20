using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Giant : MonoBehaviour {

    public Weapon currentWeapon;
    public Armour currentArmour;
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
            if (equipment is Armour)
            {
                Armour armour = equipment as Armour;
                if (currentArmour != null)
                    stats -= currentArmour.stats + currentArmour.reinforcementStats * currentArmour.reinforcementCount;
                currentArmour = armour;
                stats += currentArmour.stats;
            }
            if (equipment is Weapon)
            {
                Weapon weapon = equipment as Weapon;
                if (currentWeapon != null)
                    stats -= currentWeapon.stats + currentWeapon.reinforcementStats * currentWeapon.reinforcementCount;
                currentWeapon = weapon;
                stats += currentWeapon.stats;
            }
        }
        else if (item is Consumable)
        {
            Consumable consumable = item as Consumable;
            stats += consumable.stats;
            consumable.action(gameObject.GetComponent<Giant>());
            if (consumable.stats.duration > -1)
                StartCoroutine(DelayTemporaryBuff(consumable));
        }
    }

    public void Reinforce(Item item)
    {
        if (item is Weapon)
        {
            currentWeapon.reinforcementCount += 1;
            stats += currentWeapon.reinforcementStats;
        }
        else if (item is Armour)
        {
            currentArmour.reinforcementCount += 1;
            stats += currentArmour.reinforcementStats;
        }
    }

    IEnumerator DelayTemporaryBuff(Consumable consumable)
    {
        yield return new WaitForSeconds(consumable.stats.duration);
        stats -= consumable.stats;
        consumable.reverseAction(gameObject.GetComponent<Giant>());
    }

    void Start()
    {
        stats = new Stats();
        stats.atk = 5;
        stats.atkspd = 0.5f;
        stats.maxHP = 3000;
        stats.hp = 3000;
        stats.def = 0;
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
