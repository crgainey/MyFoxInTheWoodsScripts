using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{
    [Header("UI")]
    public Image healthBar;
    public Image recedingHealthBar;
    public Image hungerBar;


    [Tooltip("Player character. It is auto assigned when game launches")]
    public Player character;
    public bool isPlayer;
    private void Awake()
    {
        character = GetComponent<Player>();
    }
    public void SetUI()
    {
        healthBar.fillAmount = character.health / character.maxHealth;
        recedingHealthBar.fillAmount = healthBar.fillAmount;
        hungerBar.fillAmount = character.hunger / character.maxHunger;

    }

    public void SetStats()
    {
        character.health = character.maxHealth;
        character.hunger = character.maxHunger;

        StartCoroutine(RecedingHunger());
    }

    public void UpdateHPBar()
    {
        healthBar.fillAmount = character.health / character.maxHealth;
        StartCoroutine(RecedingHealthBar());
    }

    public void UpdateHungerBar()
    {
        hungerBar.fillAmount = character.hunger / character.maxHunger;
    }

    public IEnumerator RecedingHealthBar()
    {
        yield return new WaitForSeconds(1f);
        while (recedingHealthBar.fillAmount > healthBar.fillAmount)
        {
            recedingHealthBar.fillAmount -= 0.01f;
            yield return null;
        }

        recedingHealthBar.fillAmount = healthBar.fillAmount;
    }

    public IEnumerator RecedingHunger()
    {
        while (character.hunger > 0)
        {
            yield return new WaitForSeconds(.5f);
            character.hunger -= character.hungrySpeed;
            UpdateHungerBar();
            yield return null;
        }
    }

    public IEnumerator IsVeryHungry()
    {
        while (character.hunger <= 0)
        {
            yield return new WaitForSeconds(5f);
            if (character.isHungry)
            {
                character.TakeDamage(.5f);
                UpdateHPBar();
            }
            yield return null;
        }
    }
}

