using UnityEngine;
using UnityEngine.EventSystems;

public enum MouseInputType
{
    PLAYER_MODE = -1,
    DEBUG_MODE = 1
}

public class InputHandler : MonoBehaviour
{
    [SerializeField] private Camera targetCamera;
    [SerializeField] private CameraController cameraController;
    [SerializeField] private Transform selectionToolPrefab;

    [SerializeField, Range(1f, 20f)] private float selectionSmoothFactor = 10f;

    private Ray TouchRay => targetCamera.ScreenPointToRay(Input.mousePosition);
    private Rect selectionRect;
    private Vector3 startMousePosition;

    private bool isDragSelect, clickedOnUI;
    private MouseInputType mouseInputType = MouseInputType.PLAYER_MODE;
    private Transform selectionTool;

    private ShopManager shopManager;

    public void HandleInput(Player player, GameBoard board, ShopManager shopManager)
    {
        HandlePlayerMovement(player);
        player.FreeFall();
        HandleMouseInput(player, board);
        HandleKeyInput(player, board);

        this.shopManager = shopManager;
    }

    private void HandleMouseInput(Player player, GameBoard board)
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (EventSystem.current.IsPointerOverGameObject())
            {
                clickedOnUI = true;
            }
            else
            {
                clickedOnUI = false;
            }

            startMousePosition = Input.mousePosition;
        }

        if (clickedOnUI == true)
        {
            return;
        }

        if (mouseInputType == MouseInputType.DEBUG_MODE)
        {
            if (Input.GetMouseButton(0))
            {
                if (!isDragSelect && (startMousePosition - Input.mousePosition).magnitude > 40)
                {
                    isDragSelect = true;
                }
            }

            if (Input.GetMouseButtonUp(0))
            {
                if (!Input.GetKey(KeyCode.LeftShift))
                {
                    board.ClearSelection();
                }

                if (!isDragSelect)
                {
                    Tile tile = board.GetTile(TouchRay);
                    if (Input.GetKey(KeyCode.LeftShift) && board.SelectedTiles.Contains(tile))
                    {
                        board.UnselectTile(tile);
                    }
                    else
                    {
                        board.MarkAsSelected(tile, true);
                    }
                }
                else
                {
                    foreach (Tile tile in board.GetTiles(selectionRect))
                    {
                        if (Input.GetKey(KeyCode.LeftShift) && board.SelectedTiles.Contains(tile))
                        {
                            board.UnselectTile(tile);
                        }
                        else
                        {
                            board.MarkAsSelected(tile, true);
                        }
                    }
                }

                startMousePosition = Vector3.zero;
                isDragSelect = false;
            }
        }

        else if (mouseInputType == MouseInputType.PLAYER_MODE)
        {
            cameraController.HandleMovement();
            Tile tile = board.GetTile(TouchRay);
            if (tile != null && tile.Content != null)
            {
                Interactable interactable = tile.Content.GetComponent<Interactable>();
                if (interactable != null && player.EquippedItem != null && interactable.CheckPlayerItem(player.EquippedItem.id) && interactable.CheckPlayerClose())
                {
                    foreach (Tile other in board.SelectedTiles)
                    {
                        other.Content.SetOutlineEnabled(false);
                    }
                    board.ClearSelection();

                    if (selectionTool == null)
                    {
                        selectionTool = Instantiate(selectionToolPrefab, tile.transform.position + 0.01f * Vector3.up, Quaternion.Euler(90f, 0f, 0f));
                    }
                    else if (!selectionTool.gameObject.activeInHierarchy)
                    {
                        selectionTool.transform.position = tile.transform.position + 0.01f * Vector3.up;
                        selectionTool.gameObject.SetActive(true);
                    }
                    else
                    {
                        selectionTool.transform.position = Vector3.Lerp(selectionTool.transform.position, tile.transform.position + 0.01f * Vector3.up, selectionSmoothFactor * Time.deltaTime);
                    }

                    tile.Content.SetOutlineEnabled(true);
                    board.MarkAsSelected(tile, false);

                    if (Input.GetMouseButtonDown(0))
                    {
                        player.InteractWith(interactable);
                    }
                }
                else if (selectionTool != null)
                {
                    selectionTool.gameObject.SetActive(false);
                    foreach (Tile other in board.SelectedTiles)
                    {
                        other.Content.SetOutlineEnabled(false);
                    }

                    board.ClearSelection();
                }
            }
            else if (selectionTool != null)
            {
                selectionTool.gameObject.SetActive(false);
                foreach (Tile other in board.SelectedTiles)
                {
                    other.Content.SetOutlineEnabled(false);
                }

                board.ClearSelection();
            }
        }
    }

    private void HandleKeyInput(Player player, GameBoard board)
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            player.Jump();
        }
        if (Input.GetKeyDown(KeyCode.G))
        {
            shopManager.Toggle();
        }
        if (Input.GetKeyDown(KeyCode.V))
        {
            board.ShowPaths = !board.ShowPaths;
        }
        if (Input.GetKeyDown(KeyCode.F1))
        {
            DebugConsoleManager.Instance.IsActive = !DebugConsoleManager.Instance.IsActive;
        }
        if (Input.GetKeyDown(KeyCode.F2))
        {
            foreach (Tile other in board.SelectedTiles)
            {
                other.Content.SetOutlineEnabled(false);
            }
            board.ClearSelection();
            if (selectionTool != null)
            {
                Destroy(selectionTool.gameObject);
                selectionTool = null;
            }
            mouseInputType = (MouseInputType)(-(int)mouseInputType);
            InGameUI.Instance.ToogleDebugView();
        }

        if (Input.GetKeyDown(KeyCode.Keypad1) || Input.GetKeyDown(KeyCode.Alpha1))
        {
            player.QuickItemBar.SelectItem(0);
        }
        else if (Input.GetKeyDown(KeyCode.Keypad2) || Input.GetKeyDown(KeyCode.Alpha2))
        {
            player.QuickItemBar.SelectItem(1);
        }
        else if (Input.GetKeyDown(KeyCode.Keypad3) || Input.GetKeyDown(KeyCode.Alpha3))
        {
            player.QuickItemBar.SelectItem(2);
        }
        else if (Input.GetKeyDown(KeyCode.Keypad4) || Input.GetKeyDown(KeyCode.Alpha4))
        {
            player.QuickItemBar.SelectItem(3);
        }
        else if (Input.GetKeyDown(KeyCode.Keypad5) || Input.GetKeyDown(KeyCode.Alpha5))
        {
            player.QuickItemBar.SelectItem(4);
        }
        else if (Input.GetKeyDown(KeyCode.Keypad6) || Input.GetKeyDown(KeyCode.Alpha6))
        {
            player.QuickItemBar.SelectItem(5);
        }
        else if (Input.GetKeyDown(KeyCode.Keypad7) || Input.GetKeyDown(KeyCode.Alpha7))
        {
            player.QuickItemBar.SelectItem(6);
        }
        else if (Input.GetKeyDown(KeyCode.Keypad8) || Input.GetKeyDown(KeyCode.Alpha8))
        {
            player.QuickItemBar.SelectItem(7);
        }
    }

    private void HandlePlayerMovement(Player player)
    {
        if (player.canMove)
        {
            Vector3 moveInput = Vector3.ClampMagnitude(new Vector3(Input.GetAxisRaw("Horizontal"), 0f, Input.GetAxisRaw("Vertical")), 1f);
            if (moveInput.sqrMagnitude >= 0.01f)
            {
                player.IsMoving = true;
                player.IsAutoMove = false;
                float turnAngle = Mathf.Atan2(moveInput.x, moveInput.z) * Mathf.Rad2Deg + targetCamera.transform.eulerAngles.y;
                float smoothedAngle = Mathf.SmoothDampAngle(player.transform.eulerAngles.y, turnAngle, ref player.moveTurnSmoothVelocity, player.TurnSmoothTime);
                player.transform.rotation = Quaternion.Euler(0f, smoothedAngle, 0f);
                Vector3 moveDirection = Quaternion.Euler(0f, turnAngle, 0f) * Vector3.forward;
                player.Controller.Move(moveDirection * player.MoveSpeed * Time.deltaTime);
            }
            else if (!player.IsAutoMove)
            {
                player.IsMoving = false;
            }
        }

        if (player.IsMoving || !player.IsGrounded || player.IsAutoMove)
        {
            cameraController.LerpToTarget();
        }
    }

    private void OnGUI()
    {
        if (isDragSelect)
        {
            selectionRect = BoxDraw.GetScreenRect(startMousePosition, Input.mousePosition);
            BoxDraw.DrawScreenRect(selectionRect, new Color(0.8f, 0.8f, 0.95f, 0.25f));
            BoxDraw.DrawScreenRectBorder(selectionRect, 2f, new Color(0.8f, 0.8f, 0.95f));
        }
    }
}
