using UnityEngine;
using UnityEngine.UI;
using RPG.Characters;
using RPG.CameraUI;
using RPG.Weapons;
using System;
using System.Linq;
using UMA;
using UMA.CharacterSystem;

public class Character : MonoBehaviour
{
    [SerializeField] DynamicCharacterAvatar _player;
    [SerializeField] AnimatorOverrideController animatorOverrideController;
    Animator animator;
    AnimationClip castAnim;


    public float level = 1f;
    public float experiencePoints;
    public float experienceToNextLevel;

    public CharacterStat Strength;
    public CharacterStat Agility;
    public CharacterStat Intelligence;
    public CharacterStat Vitality;

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
        statPanel.SetStats(Strength, Agility, Intelligence, Vitality);
        statPanel.UpdateStatValues();

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
        animator = GetComponent<Animator>();

    }

    //private void Start()
    //{
    //    // new event created
    //    AnimationEvent instantiateProjectile;
    //    instantiateProjectile = new AnimationEvent();

    //    // put some parameters on the AnimationEvent
    //    //  - call the function called PrintEvent()
    //    //  - the animation on this object lasts 2 seconds
    //    //    and the new animation created here is
    //    //    set up to happen 1.3s into the animation
    //    //instantiateProjectile.intParameter = 12345;
    //    instantiateProjectile.time = 0.08f;
    //    instantiateProjectile.functionName = "InstantiateProjectile";

    //    // get the animation clip and add the AnimationEvent
    //    castAnim = animator.runtimeAnimatorController.animationClips[29];
    //    castAnim.AddEvent(instantiateProjectile);

    //}

    void FixedUpdate()
    {
       nearestTarget = Enemy.FindClosestEnemy(transform.position);
        //do stuff with that enemy
    }

    private void Update()
    { 
        if (currentTarget != null)
        {
            if(Input.GetKeyDown(KeyCode.Alpha1))
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
    }

    //void Attack()
    //{
    //    Transform leftHand = _player.umaData.skeleton.GetBoneGameObject(UMASkeleton.StringToHash("LeftHand")).transform;
    //    Vector3 aimoffset = new Vector3(0, 1f, 0);

    //    float damage = Mathf.Round(UnityEngine.Random.Range(minDamage, maxDamage));
    //    GameObject _castProjectile = Instantiate(castProjectile, leftHand.position, Quaternion.identity);
    //    Projectile projectileComponent = _castProjectile.GetComponent<Projectile>();
    //    projectileComponent.SetDamage(damage);
    //    projectileComponent.SetShooter(gameObject);

    //    Vector3 unitVectorToTarget = (currentTarget.transform.position + aimoffset - leftHand.position).normalized;
    //    float projectileSpeed = projectileComponent.projectileSpeed;
    //    _castProjectile.GetComponent<Rigidbody>().velocity = unitVectorToTarget * projectileSpeed;
    //    Debug.Log("Aaaand Pew!");
    //}

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
    void OnMouseOverEnemy(Enemy enemyToSet)
    {
        if (Input.GetMouseButton(0))
        {
            this.currentTarget = enemyToSet;
        }
    }

    public void SetNearestTarget()
    {
        currentTarget = nearestTarget;
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

    public float GetTargetLevel()
    {
        return currentTarget.GetLevel();
    }
    //public Text GetTargetLevel()
    //{
    //    return currentTarget.level
    //}
}
