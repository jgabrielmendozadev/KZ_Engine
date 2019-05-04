using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;
using KZ;

public class DevConsole : MonoBehaviour {

    //INSTANCE
    [SerializeField] GameObject _goConsole = null;
    [SerializeField] RectTransform _rectContent = null;
    [SerializeField] Text _txtOutput = null;
    [SerializeField] InputField _inpCommand = null;
    [SerializeField] int _maxLines = 100;
    private void Awake() {
        Awake_Singleton();

        //BASIC CONSOLE
        _isOpen = false;
        AssignCommand("HELP", Help, "Shows all commands");
        AssignCommand("CLEAR", Clear, "Clears this text");
        AssignCommand("KZ", KZ, "KZ");

        //INPUTS
        InputManager.AssignKey("ToggleConsole", KeyCode.F1);
        OnOpenConsole += () => {
            InputManager.AssignKey("Execute", KeyCode.Return);
            InputManager.AssignKey("Execute", KeyCode.KeypadEnter);
        };
        OnCloseConsole += () => {
            InputManager.RemoveKey("Execute", KeyCode.Return);
            InputManager.RemoveKey("Execute", KeyCode.KeypadEnter);
        };

        //UNITY DEBUG LOG
        Application.logMessageReceived += PrintLog;

        CloseConsole();
    }
    private void Update() {
        if (InputManager.GetInput("ToggleConsole")) ToggleConsole();
        if (InputManager.GetInput("Execute")) Execute();
    }

    static void KZ() {
        for (int i = 0; i < 99; i++) {
            Write(i + ":sdfjkl\n·pepe argento");
        }
    }

    //STATIC

    public struct ConsoleCommand {
        public Action<string> command;
        public string help;
        public ConsoleCommand(Action<string> action, string helpText = "") {
            command = action;
            help = helpText;
        }
    }
    static bool _isOpen;
    public static event Action OnOpenConsole = delegate { };
    public static event Action OnCloseConsole = delegate { };
    static Dictionary<string, ConsoleCommand> _allCommands = new Dictionary<string, ConsoleCommand>();

    

    

    [RuntimeInitializeOnLoadMethod]
    static void SpawnOnGameInit() {
        var dc = Instantiate(Resources.Load<DevConsole>("UI/DevConsole"));
        DontDestroyOnLoad(dc.gameObject);
    }


    #region SINGLETON
    static DevConsole instance;
    void Awake_Singleton() {
        if (instance != null)
            Destroy(gameObject); //Destroy new, keep old
        instance = this;
    }
    #endregion

    #region LOGGER
    public static void Write(string s) {
        instance._txtOutput.text += "\n·" + s;
        LimitLines();
        FixLogHeight();
    }

    static void LimitLines() {
        Canvas.ForceUpdateCanvases();
        var lines = instance._txtOutput.cachedTextGenerator.lines;
        var numerOfLines = lines.Count;
        if (numerOfLines > instance._maxLines)
            instance._txtOutput.text = instance._txtOutput.text.Remove(0, lines[numerOfLines - instance._maxLines].startCharIdx);
    }
    static void FixLogHeight() {
        var height = instance._txtOutput.preferredHeight;
        instance._rectContent.sizeDelta = instance._rectContent.sizeDelta.SetY(height);
    }
    #endregion

    #region COMMAND HANDLING
    public static void RemoveCommand(string commandName) {
        string cmd = commandName.ToUpper();
        _allCommands.Remove(cmd);
    }
    //no params
    public static void AssignCommand(string commandName, Action command, string helpText = "") {
        string cn = commandName.ToUpper();
        if (_allCommands.ContainsKey(cn))
            Debug.Log("command already contained: " + cn);
        else {
            _allCommands[cn] = new ConsoleCommand(param => command(), helpText);
        }
    }
    //int
    public static void AssignCommand(string commandName, Action<int> command, string helpText = "") {
        string cn = commandName.ToUpper();
        if (_allCommands.ContainsKey(cn))
            Debug.Log("command already contained: " + cn);
        else {
            _allCommands[cn] = new ConsoleCommand(
                param => {
                    int value;
                    if (int.TryParse(param, out value))
                        command(value);
                    else
                        Write("Wrong parameter, type expected: INT\n" + cn + ": " + _allCommands[cn].help);
                },
                helpText);
        }
    }
    //string
    public static void AssignCommand(string commandName, Action<string> command, string helpText = "") {
        string cn = commandName.ToUpper();
        if (_allCommands.ContainsKey(cn))
            Debug.Log("command already contained: " + cn);
        else {
            _allCommands[cn] = new ConsoleCommand(command, helpText);
        }
    }
    #endregion

    #region BASIC COMMANDS
    static void Help() {
        string s = "HELP:\nPossible Commands:" +
            _allCommands.Aggregate("", (r, kv) => r + "\n" + kv.Key + " -> " + kv.Value.help);
        Write(s);
    }
    static void Clear() {
        instance._txtOutput.text = "";
        FixLogHeight();
    }
    #endregion

    #region DEVELOPER CONSOLE HANDLING
    public static void Execute() {
        //receive text
        string input = instance._inpCommand.text.Replace("\n", "");

        //process
        if (input == "")
            return;
        List<string> inputs = input.Split(' ').Where(x => x != "").ToList();
        if (inputs.Count == 0)
            return;
        string command = inputs[0];
        string param = inputs.Count > 1 ?
            inputs.Skip(1).MakeString(" ") :
            "";
        if (!_allCommands.ContainsKey(command.ToUpper()))
            Write("Unknown command: " + command);
        else
            _allCommands[command.ToUpper()].command(param);

        //reset input
        instance._inpCommand.text = "";
        Canvas.ForceUpdateCanvases();
        instance._inpCommand.Select();
    }

    public static void ToggleConsole() {
        if (_isOpen = !_isOpen)
            OpenConsole();
        else
            CloseConsole();
    }

    public static void OpenConsole() {
        instance._goConsole.SetActive(true);
        instance._inpCommand.text = "";
        Canvas.ForceUpdateCanvases();
        instance._inpCommand.Select();
        OnOpenConsole();
    }
    public static void CloseConsole() {
        instance._goConsole.SetActive(false);
        instance._inpCommand.text = "";
        Canvas.ForceUpdateCanvases();
        OnCloseConsole();
    }
    #endregion

    #region UNITY CONSOLE LOG
    static void PrintLog(string condition, string stackTrace, LogType type) {
        //add new line
        string s = "<b>" + DateTime.Now.ToKzFormat2() + "</b>" +
            " <color=" + _logColors[type] + ">" + type + "</color>: "
            + condition.Replace("\n", "\n  ");
        Write(s);
    }

    static readonly Dictionary<LogType, string> _logColors = new Dictionary<LogType, string>() {
        {LogType.Exception,"red" },
        {LogType.Error,    "red" },
        {LogType.Warning,  "yellow" },
        {LogType.Assert,   "yellow" },
        {LogType.Log,      "silver" }
    };
    #endregion
}