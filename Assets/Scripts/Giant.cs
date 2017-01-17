using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Giant : MonoBehaviour {

    public Weapon currentWeapon;
    public Stats stats;
    public Stats[] buffs;

    public GameObject healthDisplay;
    public GameObject enemyGiant;

    private int timer;
    private int atkTime;

    void Attack()
    {
        int damageDealt = stats.atk + currentWeapon.stats.atk;
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
            stats += equipment.stats;
            if (equipment is Weapon)
            {
                Weapon weapon = equipment as Weapon;
                currentWeapon = weapon;
            }
        }
        else if (item is Consumable)
        {
            Consumable consumable = item as Consumable;
            stats += consumable.stats;
            StartCoroutine(DelayTemporaryBuff(consumable.stats));
        }
    }

    IEnumerator DelayTemporaryBuff(Stats _stats)
    {
        yield return new WaitForSeconds(_stats.duration);
        stats -= _stats;
    }

    void Start()
    {
        stats = new Stats();
        stats.atk = 5;
        stats.atkspd = 0.5f;
        stats.maxHP = 3000;
        stats.hp = 3000;
        stats.def = 0;
        UseItem(Items.itemList[ItemID.SPEAR]);
    }

    void Update()
    {
        if (stats.hp > stats.maxHP)
        {
            stats.hp = stats.maxHP;
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
