using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityContainer
{
    public Ability ability;
    public float currentCooldown;
    public float currentActiveTime;
    public bool isOnCooldown;
    public bool isActive;
    public bool isReady = true;

    public AbilityContainer(Ability ability)
    {
        this.ability = ability;
        currentCooldown = 0f;
        currentActiveTime = 0f;
        isOnCooldown = false;
        isActive = false;
    }

    internal void Cooldown()
    {
        currentCooldown = ability.cooldownTime;
        isOnCooldown = true;
        isReady = false;
    }

    public void ReduceCooldown(float amount)
    {
        if (currentCooldown > 0f)
        {
            currentCooldown -= amount;
        } else if (currentCooldown <= 0f)
        {
            currentCooldown = 0f;
            isOnCooldown = false;
            isReady = true;
        }
    }
}

public class CharacterAbilityHandler : MonoBehaviour
{
    [SerializeField] Ability startingAbility;
    [SerializeField] GameObject[] abilityPrefabs;

    private List<AbilityContainer> abilities;

    private void Start()
    {
        AddAbility(startingAbility);
    }

    private void AddAbility(Ability abilityToAdd)
    {
        if (abilities == null)
        {
            abilities = new List<AbilityContainer>();
        }

        AbilityContainer abilityContainer = new AbilityContainer(abilityToAdd);
        abilities.Add(abilityContainer);
    }

    private void Update()
    {
        ProcessCooldowns();

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            ActivateAbility(0);
        }
    }

    private void ProcessCooldowns()
    {
        for (int i = 0; i < abilities.Count; i++)
        {
            abilities[i].ReduceCooldown(Time.deltaTime);
        }
    }

    public void ActivateAbility(AbilityContainer ability)
    {
        if (ability.currentCooldown > 0f)
        {
            Debug.Log("Ability is on cooldown");
            return;
        }
        Debug.Log("Ability activated");
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0f;
        ability.Cooldown();
        switch (ability.ability.abilityClass)
        {
            case Ability.AbilityClass.Projectile:
                GameObject projectile = Instantiate(ability.ability.prefab, transform.position, Quaternion.identity);
                projectile.GetComponent<ProjectileSpell>().damage = ability.ability.damage;
                projectile.GetComponent<ProjectileSpell>().speed = ability.ability.speed;
                projectile.GetComponent<ProjectileSpell>().range = ability.ability.range;
                projectile.GetComponent<ProjectileSpell>().radius = ability.ability.radius;
                projectile.GetComponent<ProjectileSpell>().SetTarget(mousePos);
                break;
            case Ability.AbilityClass.Oncursor:
                GameObject oncursor = Instantiate(ability.ability.prefab, mousePos, Quaternion.identity);
                break;
            case Ability.AbilityClass.Selfcast:
                GameObject selfcast = Instantiate(ability.ability.prefab, transform.position, Quaternion.identity);
                break;
            default:
                break;
        }
    }

    public void ActivateAbility(int abilityIndex)
    {
        AbilityContainer ability = abilities[abilityIndex];
        ActivateAbility(ability);
    }
}