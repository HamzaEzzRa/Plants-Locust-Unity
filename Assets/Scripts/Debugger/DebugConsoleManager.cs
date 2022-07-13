using UnityEngine;

public class DebugConsoleManager : SingletonMonoBehavior<DebugConsoleManager>
{
    [SerializeField] private Color textColor = Color.white;

    private static readonly DebugCommand DEBUG_HELP = new DebugCommand(
        "help",
        "List the available debugging commands and their effects.",
        "help",
        () => {
            foreach (DebugCommandBase commandBase in commandList)
            {
                outputHistory.Enqueue(commandBase.CommandFormat + " : " + commandBase.CommandDesc);
            }
        }
    );
    private static readonly DebugCommand DEBUG_CLEAR = new DebugCommand(
        "clear",
        "Clear the command console history.",
        "clear",
        () => {
            outputHistory.Clear();
        }
    );
    private static readonly DebugCommand<int> PLAYER_ADD_MONEY = new DebugCommand<int>(
        "player.addMoney",
        "Add specified amount of money to the player.",
        "player.addMoney <amount>",
        (int amount) => {
            EventHandler.CallMoneyUpdateEvent(amount);
        }
    );
    private static readonly DebugCommand<int, int> PLAYER_ADD_ITEM = new DebugCommand<int, int>(
        "player.addItem",
        "Add specified quantity of item to the player inventory.",
        "player.addItem <id> <quantity>",
        (int id, int quantity) => {
            InventoryManager.Instance.AddItem(InventoryLocation.PLAYER, id, quantity);
        }
    );

    private static DebugCommandBase[] commandList = {
        DEBUG_CLEAR,
        DEBUG_HELP,
        PLAYER_ADD_MONEY,
        PLAYER_ADD_ITEM,
    };

    private static int historySize = 10;
    private static HistoryQueue<string> outputHistory = new HistoryQueue<string>(historySize);
    private static HistoryQueue<string> commandHistory = new HistoryQueue<string>(historySize);
    private int historyIndex = -1;

    public bool IsActive { get; set; }

    private string input;
    private Vector2 scrollView;

    private void OnGUI()
    {
        if (!IsActive)
        {
            ResetInput();
            GUI.FocusControl(null);
            
            return;
        }

        float y = 0f;
        float visibleViewportHeight = Screen.height * 0.3f;
        GUI.Box(new Rect(0f, y, Screen.width, visibleViewportHeight), "");

        GUI.contentColor = textColor;
        float labelHeight = Screen.height * 0.03f;
        int fontSize = (int)(labelHeight * 0.75f);
        GUIStyle guiStyle = new GUIStyle()
        {
            fontSize = fontSize,
        };
        guiStyle.normal.textColor = textColor;
        GUI.skin.textField.fontSize = fontSize;

        float totalViewportHeight = labelHeight * (historySize + 1);
        Rect viewport = new Rect(0f, y, Screen.width, totalViewportHeight);
        scrollView = GUI.BeginScrollView(new Rect(0f, y + 5f, viewport.width, visibleViewportHeight * 0.95f), scrollView, viewport);
        for (int i = 0; i < outputHistory.Count; i++)
        {
            Rect rect = new Rect(5f, labelHeight * i, viewport.width, labelHeight);
            GUI.Label(rect, outputHistory.At(i), guiStyle);
        }
        GUI.EndScrollView();

        y += visibleViewportHeight;
        GUI.backgroundColor = new Color(0f, 0f, 0f);
        GUI.SetNextControlName("CommandInput");
        input = GUI.TextField(new Rect(0f, y + 1f, Screen.width, Screen.height * 0.035f), input);

        if (GUI.GetNameOfFocusedControl() == "CommandInput" && Event.current.type == EventType.KeyUp)
        {
            if (Event.current.keyCode == KeyCode.Return)
            {
                if (input == "")
                {
                    return;
                }

                outputHistory.Enqueue(input);
                commandHistory.Enqueue(input);

                ParseInput();
                ResetInput();
            }

            if (Event.current.keyCode == KeyCode.F1)
            {
                IsActive = !IsActive;
            }
            else if (Event.current.keyCode == KeyCode.UpArrow)
            {
                Event.current.Use();

                if (historyIndex < 0)
                {
                    historyIndex = commandHistory.Count;
                }

                if (historyIndex <= 0)
                {
                    return;
                }

                historyIndex -= 1;
                input = commandHistory.At(historyIndex);
            }
            else if (Event.current.keyCode == KeyCode.DownArrow)
            {
                Event.current.Use();

                if (historyIndex < 0)
                {
                    return;
                }

                if (historyIndex >= commandHistory.Count - 1)
                {
                    ResetInput();
                    return;
                }

                historyIndex += 1;
                input = commandHistory.At(historyIndex);
            }
        }
    }

    private void ParseInput()
    {
        string[] args = input.Split(' ');

        for (int i = 0; i < commandList.Length; i++)
        {
            DebugCommandBase commandBase = commandList[i];
            if (args[0].Equals(commandBase.CommandId))
            {
                if (commandBase as DebugCommand != null)
                {
                    (commandBase as DebugCommand).Invoke();
                }
                else if (args.Length == 2 && commandBase as DebugCommand<int> != null)
                {
                    (commandBase as DebugCommand<int>).Invoke(int.Parse(args[1]));
                }
                else if (args.Length == 3 && commandBase as DebugCommand<int, int> != null)
                {
                    (commandBase as DebugCommand<int, int>).Invoke(int.Parse(args[1]), int.Parse(args[2]));
                }

                return;
            }
        }

        outputHistory.Enqueue("Invalid command \"" + outputHistory.At(outputHistory.Count - 1) + "\" - Type \"help\" to list available commands.");
    }

    private void ResetInput()
    {
        input = "";
        historyIndex = -1;
    }
}
