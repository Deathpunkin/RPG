using UnityEngine;
using UnityEngine.UI;
using RPG.Core;
using RPG.Characters;
using RPG.CameraUI;
using RPG.Weapons; //Do I need these?
using RPG.Armor;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UMA;
using UMA.CharacterSystem;

public class Character : MonoBehaviour, IDamageable
{
    [SerializeField] DynamicCharacterAvatar _player;
    [SerializeField] AnimatorOverrideController animatorOverrideController;
    Animator animator;
    AnimationClip castAnim;

    [SerializeField] bool updateStats = false;

    AudioSource audioSource;
    [SerializeField] AudioClip[] hurtSounds;
    [SerializeField] AudioClip[] deathSounds;

    [SerializeField] AbilityConfig[] abilities;
    GameObject mainHandWeaponObject;


    [SerializeField] float level;
    [SerializeField] Text levelText;
    [SerializeField] float currentExperiencePoints;
    [SerializeField] float _experienceToNextLevel;
    [SerializeField] float experienceToNextLevel = 150;
    [SerializeField] float maxHealthPoints = 100f;
    [SerializeField] float currentHealthPoints;
    float prevHealth;
    float respawnHealth;
    float regenHealthDelay = 5.5f;
    float baseRegenHealthSpeed = 0.5f;
    [SerializeField] float regenHealthSpeed;
    [SerializeField] bool isDead = false;
    public Button respawnButton;  //TODO get respawn working
    float respawnInvuln = 5f;
    public float respawnInvulnTimer;

    [SerializeField] CharacterStat strength;
    [SerializeField] CharacterStat agility;
    [SerializeField] CharacterStat intelligence;
    [SerializeField] CharacterStat vitality;

    [SerializeField] double defence;

    [SerializeField] Inventory inventory;
    [SerializeField] EquipmentPanel equipmentPanel;
    [SerializeField] StatPanel statPanel;
    [SerializeField] ItemTooltip itemToolTip;
    [SerializeField] Image draggableItem;
    private ItemSlot draggedSlot;

    [SerializeField] Enemy currentTarget;
    [SerializeField] Enemy nearestTarget = null;

    [SerializeField] GameObject castProjectile;
    [SerializeField] float attackDelay;
    [SerializeField] float minDamage;
    [SerializeField] float maxDamage;
    [SerializeField] float damage;
    [SerializeField] float critChance = 10f;
    [SerializeField] float critDamage;
    [SerializeField] float critMultiplyer = 1.5f; // 150% extra dmg

    [SerializeField] float lastAttackTime = 0f;
    public float timeSinceLastDamaged; //TODO remove public after debugging 
    [SerializeField] float highestDamage;
    [SerializeField] float highestCrit;
    Enemy enemy = null;


    CameraRaycaster cameraRaycaster;
    RaycastHit cursorHitInfo;
    [SerializeField] Projector projector;
    [SerializeField] bool groundTargeting;


    private void OnValidate()
    {
        if (itemToolTip == null)
        {
            itemToolTip = FindObjectOfType<ItemTooltip>();
        }
        if (_player == null)
        {
            _player = GetComponent<DynamicCharacterAvatar>();
        }
    }

    private void Awake()
    {
        statPanel.SetStats(strength, agility, intelligence, vitality);
        statPanel.UpdateStatValues();
        statPanel.UpdateLevelNumber(level);

        //Setup Events:
        //RightClick
        inventory.OnRightClickEvent += Equip;
        equipmentPanel.OnRightClickEvent += Unequip;
        //Pointer Enter
        inventory.OnPointerEnterEvent += ShowToolTip;
        equipmentPanel.OnPointerEnterEvent += ShowToolTip;
        //Pointer Exit
        inventory.OnPointerExitEvent += HideToolTip;
        equipmentPanel.OnPointerExitEvent += HideToolTip;
        //Begin Drag
        inventory.OnBeginDragEvent += BeginDrag;
        equipmentPanel.OnBeginDragEvent += BeginDrag;
        //Drag
        inventory.OnDragEvent += Drag;
        equipmentPanel.OnDragEvent += Drag;
        //End Drag
        inventory.OnEndDragEvent += EndDrag;
        equipmentPanel.OnEndDragEvent += EndDrag;
        //Drop
        inventory.OnDropEvent += Drop;
        equipmentPanel.OnDropEvent += Drop;
        cameraRaycaster = FindObjectOfType<CameraRaycaster>();
        cameraRaycaster.onMouseOverEnemy += OnMouseOverEnemy;
        //cameraRaycaster.onMouseOverLootable += OnMouseOverLootable; //TODO Fix when Loot Works.
        currentHealthPoints = maxHealthPoints;
        prevHealth = currentHealthPoints;
        regenHealthSpeed = baseRegenHealthSpeed;
        audioSource = GetComponent<AudioSource>();
        SetupRuntimeAnimator();
        DamageTextController.Initialize();
        AttachInitialAbilities();
        //Button respawn = respawnButton.GetComponent<Button>();
        respawnHealth = Mathf.Round(currentHealthPoints / 3);


        animator = GetComponent<Animator>();
    }

    private void Start()
    {
        _experienceToNextLevel = experienceToNextLevel;
    }

    void FixedUpdate()
    {
       nearestTarget = Enemy.FindClosestEnemy(transform.position);
        //do stuff with that enemy
    }

    public void Update()
    {
        if (currentExperiencePoints >= experienceToNextLevel)
        {
            LevelUp();
        }
        if (currentTarget != null)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                transform.LookAt(currentTarget.transform);
                //Attack();
                animator.SetTrigger("LeftHandCast");
            }
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            groundTargeting = true;
            Vector3 mousePos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 500f);

            Vector3 worldPos;
            Ray ray = Camera.main.ScreenPointToRay(mousePos);
            RaycastHit hit;
            Physics.Raycast(ray, out hit, 1000f);
            worldPos = hit.point;
            Instantiate(projector, worldPos, Quaternion.Euler(90, 0, 0));
        }
        if (groundTargeting)
        {
            //projector.transform.position = Input.mousePosition;
            if(Input.GetKeyDown(KeyCode.Escape))
            {
                groundTargeting = false;
                Destroy(projector);
            }
        }
        if (strength.Value != 0)
        {
            defence = Math.Round(strength.Value / 2);
        }
        //defence +=
        levelText.text = level.ToString();

        if (updateStats)
        {
            statPanel.UpdateStatValues();
        }
        if (GetHealthAsPercentage() > Mathf.Epsilon)
        {
            if ((Time.time - timeSinceLastDamaged) >= regenHealthDelay && currentHealthPoints != maxHealthPoints && !isDead)
            {
                StartCoroutine(regenHealth());
            }
            else if (currentHealthPoints == maxHealthPoints && (Time.time - timeSinceLastDamaged) <= regenHealthDelay)
            {
                //CancelInvoke();
                StopCoroutine(regenHealth());
                regenHealthSpeed = baseRegenHealthSpeed;
            }
            //Damage Math
            //damage = Mathf.Round(UnityEngine.Random.Range(mainHandWeaponConfig.GetMinDamagePerHit(), mainHandWeaponConfig.GetMaxDamagePerHit()));
            critDamage = Mathf.Round(damage * critMultiplyer);
            if (isDead)
            {
                StopCoroutine(regenHealth());
                this.GetComponent<ThirdPersonUserControl>().enabled = false;
            }
            else
            {
                animator.ResetTrigger("Dead");
                this.GetComponent<ThirdPersonUserControl>().enabled = true;
            }
            //TODO get respawn invuln working
            //respawnInvulnTimer = Mathf.Clamp(respawnInvuln - Time.time, 0, respawnInvuln);
            ScanForAbilityKeyDown();
        }
    }
    IEnumerator regenHealth()
    {
        currentHealthPoints = currentHealthPoints + regenHealthSpeed;
        //regenHealthSpeed = regenHealthSpeed + 0.5f;
        yield return new WaitForSeconds(1);
    }

    private void SetupRuntimeAnimator()
    {
        animator = GetComponent<Animator>();
        animator.runtimeAnimatorController = animatorOverrideController;
    }
        private void ScanForAbilityKeyDown()
    {
        for (int keyIndex = 0; keyIndex < abilities.Length; keyIndex++)
        {
            if (Input.GetKeyDown(keyIndex.ToString()))
            {
                UseAbility(keyIndex);
            }
        }
    }
    private void AttachInitialAbilities()
    {
        for (int abilityIndex = 0; abilityIndex < abilities.Length; abilityIndex++)
        {
            abilities[abilityIndex].AttachAbilityTo(gameObject);
        }
    }
    void OnMouseOverEnemy(Enemy enemyToSet)
    {
        this.enemy = enemyToSet;
        if (Input.GetMouseButton(0) /*&& IsTargetInRange(enemy.gameObject)*/)
        {
            currentTarget = enemyToSet;
            transform.LookAt(enemy.transform);
            animator.SetTrigger("LeftHandCast");
            //AttackTarget();
        }
        else
        {
            if (Input.GetMouseButtonDown(0))
            {
                DamageTextController.CreateFloatingOutOfRangeText("Target out of reach.", enemy.transform);
                //StartCoroutine(moveIntoRange());

            }
        }
    }
    IEnumerator moveIntoRange()
    {
        print("Make me Move to Target");
        yield return new WaitForSeconds(0);
    }

    void OnMouseOverLootable(Vector3 lootable)
    {
        if (Input.GetMouseButton(0))
        {
            print("Lootable!");
        }
        return;
    }
    public void UseAbility(int abilityIndex) //TODO change to individual ability casts per PlayerInput assignment
    {
        var energyComponent = GetComponent<Energy>();
        var energyCost = abilities[abilityIndex].GetEnergyCost();
        if (energyComponent.IsEnergyAvailable(energyCost))
        {
            energyComponent.ConsumeEnergy(energyCost);
            if (UnityEngine.Random.Range(1.0f, 100.0f) < critChance && UnityEngine.Random.Range(1, 100) > enemy.dodgechance)
            {
                var abilityParams = new AbilityUseParams(enemy, critDamage);
                abilities[abilityIndex].Use(abilityParams);
            }
            else if (UnityEngine.Random.Range(1, 100) > enemy.dodgechance)
            {
                var abilityParams = new AbilityUseParams(enemy, damage);
                abilities[abilityIndex].Use(abilityParams);
            }
        }
        else
        {
            DamageTextController.CreateFloatingNotEnoughEnergyText("Not Enough Energy.", transform);
        }

    }
    private void AttackTarget()
    {
        if (Time.time - lastAttackTime > 3)
        {
            animator.SetTrigger("Attack");
            //mainHandWeapon.GetWeaponHitSound();
            //mainHandWeapon.GetWeaponAudioSouce().Play
            if (UnityEngine.Random.Range(1.0f, 100.0f) < critChance && UnityEngine.Random.Range(1, 100) > enemy.dodgechance)
            {
                enemy.TakeDamage(critDamage);
                print("CRIT! Dealt " + critDamage);
                if (critDamage >= highestCrit && enemy.GetLevel() >= GetLevel())
                {
                    highestCrit = critDamage;
                    DamageTextController.CreateFloatingHighestCritDamageText(critDamage.ToString(), enemy.transform);

                }
                if (critDamage <= highestCrit && enemy.GetLevel() <= GetLevel())
                {
                    DamageTextController.CreateFloatingCritDamageText(critDamage.ToString(), enemy.transform);
                }

            }
            else if (UnityEngine.Random.Range(1, 100) > enemy.dodgechance)
            {
                if (damage >= highestDamage && enemy.GetLevel() >= GetLevel())
                {
                    highestDamage = damage;
                    Debug.Log(enemy.transform);
                }
                DamageTextController.CreateFloatingDamageText(damage.ToString(), enemy.transform);
                enemy.TakeDamage(damage);
                print("Dealt " + damage);
                print("Target position - " + enemy.transform.position.ToString());
            }
            else
            {
                print("DODGED!");
                DamageTextController.CreateFloatingDodgeText("Dodged!", enemy.transform);
            }
            lastAttackTime = Time.time;
        }
    }
    public void TakeDamage(float damage) //TODO change back to damage and duplicate for healing
    {
        if (respawnInvulnTimer != 0)
        {
            damage = 0;
        }
        ReduceHealth(damage);
        //Trigger Death
        if (currentHealthPoints <= 0)
        {
            StartCoroutine(TriggerDeath());
        }
    }

    public void Heal(float points)
    {
        currentHealthPoints = Mathf.Clamp(currentHealthPoints + points, 0f, maxHealthPoints);
    }
    public IEnumerator TriggerDeath()
    {
        var energyComponent = GetComponent<Energy>();
        //Kill
        animator.SetTrigger("Dead"); //TODO disable movement when dead
        audioSource.clip = deathSounds[UnityEngine.Random.Range(0, deathSounds.Length)];
        audioSource.Play();
        isDead = true;
        //Drain Energy
        respawnButton.onClick.AddListener(Respawn);
        //TODO get button to Respawn, or wait for revive working
        yield return new WaitForSecondsRealtime(audioSource.clip.length);

    }
    public void Respawn()
    {
        isDead = false;
        currentHealthPoints = respawnHealth;
        timeSinceLastDamaged = Time.time;
        respawnInvuln = +5f;
    }
    private void ReduceHealth(float damage)
    {
        currentHealthPoints = Mathf.Clamp(currentHealthPoints - damage, 0f, maxHealthPoints);
        if (currentHealthPoints < prevHealth)
        {
            timeSinceLastDamaged = Time.time;
            audioSource.clip = hurtSounds[UnityEngine.Random.Range(0, hurtSounds.Length)];
            audioSource.Play();
        }
        prevHealth = currentHealthPoints;
    }


    private void Equip(ItemSlot itemSlot)
    {
        EquippableItem equippableItem = itemSlot.Item as EquippableItem;
        if (equippableItem != null)
        {
            Equip(equippableItem);
        }
    }

    private void Unequip(ItemSlot itemSlot)
    {
        EquippableItem equippableItem = itemSlot.Item as EquippableItem;
        if (equippableItem != null)
        {
            Unequip(equippableItem);
        }
    }

    private void ShowToolTip(ItemSlot itemSlot)
    {
        EquippableItem equippableItem = itemSlot.Item as EquippableItem;
        if (equippableItem != null)
        {
            itemToolTip.ShowToolTip(equippableItem);
            Debug.Log("Show Tooltip");
        }

    }

    private void HideToolTip(ItemSlot itemSlot)
    {
        itemToolTip.HideToolTip();
        Debug.Log("Hide Tooltip)");
    }

    private void BeginDrag(ItemSlot itemSlot)
    {
        if (itemSlot.Item != null)
        {
            draggedSlot = itemSlot;
            draggableItem.sprite = itemSlot.Item.Icon;
            draggableItem.transform.position = Input.mousePosition;
            draggableItem.enabled = true;
            draggableItem.transform.SetAsLastSibling();
        }
    }

    private void Drag(ItemSlot itemSlot)
    {
       if(draggableItem.enabled)
        {
            draggableItem.transform.position = Input.mousePosition;
        }
    }

    private void EndDrag(ItemSlot itemSlot)
    {
        draggedSlot = null;
        draggableItem.enabled = false;
    }

    private void Drop(ItemSlot dropItemSlot)
    {
        if (dropItemSlot.CanRecieveItem(draggedSlot.Item) && draggedSlot.CanRecieveItem(dropItemSlot.Item))
        {
            EquippableItem dragItem = draggedSlot.Item as EquippableItem;
            EquippableItem dropItem = dropItemSlot.Item as EquippableItem;
            if (draggedSlot is EquipmentSlot)
            {
                if (dragItem != null)
                {
                    dragItem.Unequip(this);
                }
                if (dropItem != null)
                {
                    dropItem.Equip(this);
                }

            }
            if (dropItemSlot is EquipmentSlot)
            {
                if (dragItem != null)
                {
                    dragItem.Equip(this);
                }
                if (dropItem != null)
                {
                    dropItem.Unequip(this);
                }
            }
            statPanel.UpdateStatValues();

            Item draggedItem = draggedSlot.Item;
            draggedSlot.Item = dropItemSlot.Item;
            dropItemSlot.Item = draggedItem;
        }
    }

    public void GiveExp(float exp)
    {
        currentExperiencePoints += exp;
        Debug.Log("Exp Gained: " + exp);
    }
    public float GetExpAsPercentage()
    {
        float expPercent = (currentExperiencePoints / experienceToNextLevel) * 100;
            return expPercent ;
    }
    public float GetExpAsDecimal()
    {
        float expDecimal = (currentExperiencePoints / experienceToNextLevel);
        return expDecimal;
    }
    private void LevelUp()
    {
        level += 1;
        currentExperiencePoints -= _experienceToNextLevel;
        experienceToNextLevel = (experienceToNextLevel * 2);
        _experienceToNextLevel = experienceToNextLevel;
        statPanel.UpdateLevelNumber(level);
        Debug.Log("Level Up!");
    }
    public void AddToInventory(Item item)
    {
        inventory.AddItem(item);
    }
    public void AddToInventory(Item item, int amount)
    {
        inventory.AddItem(item, amount);
    }

    public void Equip(EquippableItem item)
    {
        if (inventory.RemoveItem(item))
        {
            EquippableItem previousItem;
            if (equipmentPanel.AddItem(item, out previousItem))
            {
                if (previousItem != null)
                {
                    inventory.AddItem(previousItem);
                    item.Unequip(this);
                    statPanel.UpdateStatValues();
                }
                item.Equip(this);
                statPanel.UpdateStatValues();
            }
            else
            {
                inventory.AddItem(item);
            }
        }
    }

    public void Unequip(EquippableItem item)
    {
        if (!inventory.IsFull() && equipmentPanel.RemoveItem(item))
        {
            inventory.AddItem(item);
            item.Unequip(this);
            statPanel.UpdateStatValues();
        }
    }
    public void BringToFront()
    {
        
    }
    //void OnMouseOverEnemy(Enemy enemyToSet)
    //{
    //    if (Input.GetMouseButton(0))
    //    {
    //        this.currentTarget = enemyToSet;
    //    }
    //}

    public void SetNearestTarget()
    {
        currentTarget = nearestTarget;
    }

    public void GetNextTarget()
    {
        int current;
        for (int i = 0; i < Enemy.Pool.Count(); ++i)
        {
            if (Enemy.Pool.Count() == currentTarget.GetHashCode())
            {
                current = currentTarget.GetHashCode();
            }
        }
        Debug.Log("Current Target Hash: " + currentTarget.GetHashCode());
        //current = (current + 1) % Enemy.Pool.Count();
        //currentTarget = Enemy.Pool<current>;
    }

    //Getters
    public float GetExp()
    {
        return currentExperiencePoints;
    }
    public float GetExpToLevel()
    {
        return experienceToNextLevel;
    }
    public float GetLevel()
    {
        return level;
    }
    public float GetTargetLevel()
    {
        return currentTarget.GetLevel();
    }
    public float GetHealthAsPercentage()
    {
        return currentHealthPoints / maxHealthPoints;
    }
    public CharacterStat GetStrength()
    {
        return strength;
    }
    public CharacterStat GetAgility()
    {
        return agility;
    }
    public CharacterStat GetIntelligence()
    {
        return intelligence;
    }
    public CharacterStat GetVitality()
    {
        return vitality;
    }
    public Enemy GetTarget()
    {
        return currentTarget;
    }

    public float GetMinDamage()
    {
        return minDamage;
    }

    public float GetMaxDamage()
    {
        return maxDamage;
    }

    public GameObject GetCastProjectile()
    {
        return castProjectile;
    }

    public float GetCurrentHealth()
    {
        return currentHealthPoints;
    }

    public float GetMaxHealth()
    {
        return maxHealthPoints;
    }

    public float GetRegenHealthDelay()
    {
        return regenHealthDelay;
    }

    public AbilityConfig GetAbilities(int i)
    {
        return abilities[i];
    }

    public Image GetAbilityIcon(int i)
    {
        return abilities[i].GetSkillIcon();
    }

}