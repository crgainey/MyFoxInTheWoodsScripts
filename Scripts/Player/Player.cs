using System.Collections;
using TMPro;
using UnityEngine;

public class Player : HealthSystem
{
    public float hunger;
    public float maxHunger;
    [Tooltip("Amount of hunger the pathPlayerMovement loses per tick")]
    public float hungrySpeed;
    public bool isHungry;

    //public CraftingUI craftingUI;
    public InventoryOBJ inventory;
    public CraftingObj crafting;
    [Tooltip("UI for the pathPlayerMovement. Auto set when game launches")]
    public PlayerUI playerUI;
    private GameManager gameManager;

    //FORADVENTURMODE

    public bool itemOne = false;
    public bool itemTwo = false;
    public bool itemThree = false;
    public bool itemFour = false;

    public GameObject pausePanel;
    public GameObject craftingPanel;
    public GameObject questPanel;

    public Animator effectsAnim;

    public GameObject craftingArrow;

    public AudioSource useItemSound;


    void Start()
    {
        gameManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();
        playerUI = GetComponent<PlayerUI>();
        playerUI.SetStats();
        playerUI.SetUI();

    }

    void Update()
    {
        if (hunger <= 0 && !isHungry)
        {
            isHungry = true;
            StartCoroutine(playerUI.IsVeryHungry());
        }
        else if (hunger > 0)
        {
            if (isHungry == true)
            {

                StartCoroutine(playerUI.RecedingHunger());

                isHungry = false;
            }
        }

        if (defense > 0)
            StartCoroutine(TrackDefense());

        InputHandler();
    }

    public void HungerCheck()
    {
        if (hunger <= 0 && !isHungry)
        {
            isHungry = true;
            StartCoroutine(playerUI.IsVeryHungry());
        }
        else if (hunger > 0)
        {
            isHungry = false;

        }
    }

    public void InputHandler()
    {
        /*
        if (Input.GetKeyDown(KeyCode.Space))
        {
            UseItem(1);
        }
        */
    }

    public void UseItem(int index)
    {
        //Debug.Log("ASDASDASd");
        var hoveredItem = inventory.inventory.inventorySlots[index].item;
        if (hoveredItem == null) return;
        useItemSound.Play();
        if (gameManager.state == 1)
        {
            if (hoveredItem.isUsable)
            {
                if (hoveredItem.ability.Use(GetComponent<Player>(), hoveredItem.ability.power))
                {
                    if (hoveredItem.itemID == 4 || hoveredItem.itemID == 7 || hoveredItem.itemID == 10)
                    {
                        effectsAnim.SetBool("isHealth", true);
                        StartCoroutine(EndAnimation());
                    }

                    else if (hoveredItem.itemID == 3 || hoveredItem.itemID == 6 || hoveredItem.itemID == 9)
                    {
                        effectsAnim.SetBool("isAttack", true);
                        StartCoroutine(EndAnimation());
                    }

                    else if (hoveredItem.itemID == 5 || hoveredItem.itemID == 8 || hoveredItem.itemID == 11)
                    {
                        effectsAnim.SetBool("isSheild", true);
                        StartCoroutine(EndAnimation());
                    }

                    inventory.RemoveItem(hoveredItem, false);

                }

                else return;
            }
        }

        else if (gameManager.state == 2)
        {
            if (hoveredItem.isUsable)
            {
                if (inventory.inventory.inventorySlots[index].isSelected == false)
                {
                    crafting.AddItem(Instantiate(hoveredItem));
                    inventory.RemoveItem(hoveredItem, false);
                    //inventory.inventory.inventorySlots[index].isSelected = true;
                }
                else
                {
                    return;
                }
            }
            else return;

        }

        else if (gameManager.state == 3)
        {
            if (hoveredItem.itemID == 14)
            {
                if (hoveredItem.ability.Use(GetComponent<Player>(), hoveredItem.ability.power))
                {
                    // change season thing here.
                    Debug.Log("QuestItem1");
                    itemOne = true;
                    inventory.RemoveItem(hoveredItem, false);
                }

            }

            else if (hoveredItem.itemID == 15)
            {
                if (hoveredItem.ability.Use(GetComponent<Player>(), hoveredItem.ability.power))
                {
                    // change season thing here.
                    Debug.Log("QuestItem2");
                    itemTwo = true;
                    inventory.RemoveItem(hoveredItem, false);
                }

            }

            else if (hoveredItem.itemID == 12)
            {
                if (hoveredItem.ability.Use(GetComponent<Player>(), hoveredItem.ability.power))
                {
                    // change season thing here.
                    Debug.Log("QuestItem3");
                    itemThree = true;
                    inventory.RemoveItem(hoveredItem, false);
                }

            }

            else if (hoveredItem.itemID == 13)
            {
                if (hoveredItem.ability.Use(GetComponent<Player>(), hoveredItem.ability.power))
                {
                    // change season thing here.
                    Debug.Log("QuestItem4");
                    itemFour = true;
                    inventory.RemoveItem(hoveredItem, false);
                }

            }

            else return;
        }


        /*
        for (int i = 0; i < inventory.inventory.inventorySlots.Length; i++)
        {
            if (inventory.inventory.inventorySlots[i].selectedSlot == true)
            {
                var hoveredItem = inventory.inventory.inventorySlots[i].item;
                if (hoveredItem == null) return;

                if(hoveredItem.isUsable)
                {
                    if (hoveredItem.ability.Use(GetComponent<Player>(), hoveredItem.ability.power))
                    {
                        inventory.RemoveItem(hoveredItem, true);
                    }
                    else return;
                }
            }
        }
        */
    }

    protected override void Die()
    {
        craftingArrow.SetActive(false);
        gameManager.LoseGame();
        Debug.Log("You Died");
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Pickup")|| other.CompareTag("Spring")|| other.CompareTag("Summer")||other.CompareTag("Fall")||other.CompareTag("Winter"))
        {
            inventory.AddItem(Instantiate(other.GetComponent<PickupItem>().item));
            other.gameObject.SetActive(false);
            gameManager.UpdateItemInfo();
        }

        if (other.CompareTag("CraftingTable"))
        {
            if (pausePanel.activeSelf == true)
            {
                pausePanel.SetActive(false);
            }

            if (craftingPanel.activeInHierarchy == false)
            {
                craftingPanel.SetActive(true);
            }

            if (questPanel.activeSelf == true)
            {
                questPanel.SetActive(false);
            }

            gameManager.state = 2;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("CraftingTable"))
        {
            gameManager.state = 1;
            if(crafting.inventory.craftingSlots[0].item != null)
            {
                //Debug.Log("this is active");
                inventory.AddItem(Instantiate(crafting.inventory.craftingSlots[0].item));
                crafting.RemoveItem(crafting.inventory.craftingSlots[0].item, false);
            }
            if (crafting.inventory.craftingSlots[1].item != null)
            {
                inventory.AddItem(Instantiate(crafting.inventory.craftingSlots[1].item));
                crafting.RemoveItem(crafting.inventory.craftingSlots[1].item, false);
            }
            inventory.UnselectItems();
           //------------------------------------
            if (pausePanel.activeSelf == false)
            {
                pausePanel.SetActive(true);
            }

            if (craftingPanel.activeSelf == true)
            {
                craftingPanel.SetActive(false);
            }

            if (questPanel.activeSelf == true)
            {
                questPanel.SetActive(false);
            }
        }
    }
    public override void TakeDamage(float dmgAmount)
    {
        dmgAmount -= defense;
        if (dmgAmount <= 0)
        {
            dmgAmount = 0;
        }
        health -= dmgAmount;
        //Debug.Log("Player Health = " + health);
        playerUI.UpdateHPBar();

        if (health <= 0)
        {
            inventory.inventory.ClearInventory();
            Die();
        }
    }
    IEnumerator EndAnimation()
    {
        yield return new WaitForSeconds(1f);
        effectsAnim.SetBool("isHealth", false);
        effectsAnim.SetBool("isAttack", false);
        effectsAnim.SetBool("isSheild", false);
    }

    IEnumerator TrackDefense()
    {
        yield return new WaitForSeconds(10f);
        defense = 0;
    }
}
