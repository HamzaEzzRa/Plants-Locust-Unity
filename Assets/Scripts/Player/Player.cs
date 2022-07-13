using UnityEngine;

[RequireComponent(typeof(CharacterController), typeof(AnimationManager))]
public class Player : MonoBehaviour
{
    public int HAND_LAYER { get; private set; }

    public readonly int
        IDLE_ANIMATION = Animator.StringToHash("Player_Idle"),
        RUN_ANIMATION = Animator.StringToHash("Player_Run"),
        AXE_HOLD_ANIMATION = Animator.StringToHash("Player_Axe_Hold"),
        AXE_SLASH_ANIMATION = Animator.StringToHash("Player_Axe_Slash"),
        HOE_HOLD_ANIMATION = Animator.StringToHash("Player_Hoe_Hold"),
        HOE_DIG_ANIMATION = Animator.StringToHash("Player_Hoe_Dig"),
        SICKLE_HOLD_ANIMATION = Animator.StringToHash("Player_Sickle_Hold"),
        SHOTGUN_HOLD_ANIMATION = Animator.StringToHash("Player_Shotgun_Hold"),
        BAG_HOLD_ANIMATION = Animator.StringToHash("Player_Bag_Hold"),
        SEED_PLANT_ANIMATION = Animator.StringToHash("Player_Seed_Plant");

    public readonly PlayerIdleState IDLE_STATE = new PlayerIdleState();
    public readonly PlayerRunState RUN_STATE = new PlayerRunState();
    public readonly PlayerAxeSlashState AXE_SLASH_STATE = new PlayerAxeSlashState();
    public readonly PlayerHoeDigState HOE_DIG_STATE = new PlayerHoeDigState();
    public readonly PlayerSeedPlantState SEED_PLANT_STATE = new PlayerSeedPlantState();

    [HideInInspector] public float moveTurnSmoothVelocity;

    public float MoveSpeed => moveSpeed;

    public float TurnSmoothTime => turnSmoothTime;

    public CharacterController Controller => controller;

    public AnimationManager AnimationManager => animationManager;

    public PlayerBaseState CurrentState => currentState;

    public bool IsGrounded => isGrounded;

    public bool IsMoving { get => isMoving; set => isMoving = value; }

    public bool IsAutoMove { get => isAutoMove; set => isAutoMove = value; }

    public bool HasTarget => targetInteractable != null;

    public Item EquippedItem => equippedItem;

    public Item LastEquippedItem => lastEquippedItem;

    public QuickItemBar QuickItemBar => quickItemBar;

    [SerializeField, Range(1f, 10f)] private float moveSpeed = 5f;
    [SerializeField, Range(0.1f, 5f)] private float turnSmoothTime = 0.5f;

    [SerializeField, Range(1f, 50f)] private float gravityPull = 9.81f;
    [SerializeField, Range(0.5f, 5f)] private float jumpHeight = 2f;
    [SerializeField, Range(0.1f, 10f)] private float distanceFromGround = 0.15f;

    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundMask;

    [SerializeField] private QuickItemBar quickItemBar;
    [SerializeField] private Transform equipmentHolder;

    [SerializeField, Range(1f, 100f)] private float baseAxeDPH = 30f;

    public bool canMove = true;
    public bool canSwitchEquipped = true;

    private bool isMoving, isGrounded, isAutoMove;
    private float ySpeed;

    private CharacterController controller;
    private AnimationManager animationManager;
    private PlayerBaseState currentState;

    private Item[] playerEquipments;
    private Item equippedItem;
    private Item lastEquippedItem;

    private Interactable targetInteractable;

    private LTDescr autoMoveDescr;

    private int WOOD_AXE_ID = -1,
                HOE_ID = -1;

    public float CurrentHealth => currentHealth;
    public int CurrentMoney => currentMoney;

    private float currentHealth;
    private int currentMoney;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        animationManager = GetComponent<AnimationManager>();
    }

    private void Start()
    {
        HAND_LAYER = animationManager.Animator.GetLayerIndex("Hand Layer");
        ChangeState(IDLE_STATE);

        playerEquipments = equipmentHolder.GetComponentsInChildren<Item>(true);
    }

    private void Update()
    {
        currentState.UpdateState(this);
    }

    public void ChangeState(PlayerBaseState newState)
    {
        currentState = newState;
        currentState.EnterState(this);
    }

    public void FreeFall()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, distanceFromGround, groundMask);
        if (!isGrounded)
        {
            ySpeed -= gravityPull * Time.deltaTime;
            ySpeed = Mathf.Clamp(ySpeed, -40f, 40f);
        }
        else if (ySpeed < 0f)
        {
            ySpeed = -controller.stepOffset;
        }
        controller.Move(new Vector3(0f, 0.5f * ySpeed * Time.deltaTime, 0f));
    }

    public bool Jump()
    {
        if (isGrounded && canMove)
        {
            ySpeed = Mathf.Sqrt(2 * jumpHeight * gravityPull);
            return true;
        }
        return false;
    }

    public void InteractWith(Interactable interactable)
    {   
        canMove = false;
        Vector3 targetPosition = interactable.transform.position;
        targetPosition.y = transform.position.y;
        float distance = Vector3.Distance(targetPosition, transform.position);
        //float inverseDistance = 1 / distance;

        transform.LookAt(targetPosition);
        
        if (distance > 0.55f)
        {
            isAutoMove = true;
            isMoving = true;

            Vector3 correctedTarget = targetPosition + (transform.position - targetPosition).normalized * 0.5f;
            correctedTarget.y = transform.position.y;

            if (autoMoveDescr != null)
            {
                LeanTween.cancel(autoMoveDescr.uniqueId);
            }
            Vector3 startPosition = transform.position;
            autoMoveDescr = LeanTween.value(0f, 1f, distance * moveSpeed * 0.07f)
                .setOnUpdate((float value) => {
                    transform.rotation = Quaternion.Euler(Vector3.up * transform.rotation.eulerAngles.y);
                    transform.position = startPosition + (correctedTarget - startPosition) * value;
                })
                .setOnComplete(() => {
                    isMoving = isAutoMove = false;
                    canMove = true;
                    autoMoveDescr = null;

                    ManageAction(interactable);
                });
        }
        else
        {
            if (autoMoveDescr != null)
            {
                LeanTween.cancel(autoMoveDescr.uniqueId);
            }
            canMove = true;

            ManageAction(interactable);
        }
    }

    private void ManageAction(Interactable interactable)
    {
        canSwitchEquipped = false;
        
        if (WOOD_AXE_ID <= 0)
        {
            WOOD_AXE_ID = InventoryManager.Instance.GetItemData("Wood Axe").ID;
        }
        if (equippedItem.id == WOOD_AXE_ID)
        {
            //GameTree tree = (GameTree)interactable;
            ChangeState(AXE_SLASH_STATE);
            targetInteractable = interactable;
            return;
        }

        if (HOE_ID <= 0)
        {
            HOE_ID = InventoryManager.Instance.GetItemData("Hoe").ID;
        }
        if (equippedItem.id == HOE_ID)
        {
            //Diggable diggable = (Diggable)interactable;
            ChangeState(HOE_DIG_STATE);
            targetInteractable = interactable;
            return;
        }

        if (equippedItem.ItemData.Type == ItemType.SEED)
        {
            ChangeState(SEED_PLANT_STATE);
            targetInteractable = interactable;
            return;
        }
    }

    private void OnEquippedItemUpdate(int equippedId)
    {
        if (equippedId == -1)
        {
            if (canSwitchEquipped && equippedItem != null)
            {
                equippedItem.gameObject.SetActive(false);
            }
            else
            {
                lastEquippedItem = equippedItem;
            }

            equippedItem = null;
            return;
        }

        if (equippedItem != null)
        {
            if (canSwitchEquipped)
            {
                equippedItem.gameObject.SetActive(false);
                equippedItem = null;
            }
            else
            {
                lastEquippedItem = equippedItem;
            }
        }

        foreach (Item equipment in playerEquipments)
        {
            if (equipment.id == equippedId)
            {
                equippedItem = equipment;

                if (canSwitchEquipped)
                {
                    equippedItem.gameObject.SetActive(true);
                }
                break;
            }
        }
    }

    private void OnMoneyUpdateEvent(int amount)
    {
        currentMoney += amount;
    }

    private void OnEnable()
    {
        EventHandler.EquippedItemUpdateEvent += OnEquippedItemUpdate;
        EventHandler.MoneyUpdateEvent += OnMoneyUpdateEvent;
    }

    private void OnDisable()
    {
        EventHandler.EquippedItemUpdateEvent -= OnEquippedItemUpdate;
        EventHandler.MoneyUpdateEvent -= OnMoneyUpdateEvent;
    }

    public void TreeChop()
    {
        if (targetInteractable != null)
        {
            if (targetInteractable.transform.TryGetComponent(out GameTree tree))
            {
                tree.ApplyDamage((int)((1 + Random.Range(-0.2f, 0.2f)) * baseAxeDPH));
            }
        }
    }

    public void HoeDig(int count)
    {
        if (targetInteractable != null)
        {
            if (targetInteractable.transform.TryGetComponent(out Diggable diggable))
            {
                diggable.Interact(count);
            }
        }
    }

    public void SeedPlant(int count)
    {
        if (targetInteractable != null)
        {
            if (targetInteractable.transform.TryGetComponent(out GameSoil soil))
            {
                soil.Interact(count, equippedItem.ItemData.TileType);
            }
        }
    }
}
