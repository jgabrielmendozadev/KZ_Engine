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
    [SerializeField] Color _colorInputCommand = Color.green;
    [SerializeField] Color _colorInputNotFound = Color.yellow;

    private void Awake() {
        Awake_Singleton();

        //BASIC CONSOLE
        _isOpen = false;
        AssignCommand("help", Help, "Shows all commands");
        AssignCommand("clear", Clear, "Clears this text");
        AssignCommand("exit", Exit, "Quits the game");
        
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
    static Dictionary<string, ConsoleCommand> _allCommands = new Dictionary<string, ConsoleCommand>();
    public static event Action OnOpenConsole = delegate { };
    public static event Action OnCloseConsole = delegate { };

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
    static void Write(string text) {
        instance._txtOutput.text += "\n" + text;
        LimitLines();
        FixLogHeight();
    }
    public static void Log(string text) {
        Write("·" + text);
    }
    public static void Log(string text, Color color) {
        //instance._txtOutput.text += "\n·" + text.Colorize(color);
        //LimitLines();
        //FixLogHeight();
        Write("·" + text.Colorize(color));
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
        //string cmd = commandName.ToUpper();
        string cmd = commandName;
        _allCommands.Remove(cmd);
    }
    //no params
    public static void AssignCommand(string commandName, Action command, string helpText = "") {
        //string cmd = commandName.ToUpper();
        string cmd = commandName;
        if (_allCommands.ContainsKey(cmd))
            Debug.Log("command already contained: " + cmd);
        else {
            _allCommands[cmd] = new ConsoleCommand(param => command(), helpText);
        }
    }
    //int
    public static void AssignCommand(string commandName, Action<int> command, string helpText = "") {
        //string cmd = commandName.ToUpper();
        string cmd = commandName;
        if (_allCommands.ContainsKey(cmd))
            Debug.Log("command already contained: " + cmd);
        else {
            _allCommands[cmd] = new ConsoleCommand(
                param => {
                    int value;
                    if (int.TryParse(param, out value))
                        command(value);
                    else
                        Log("Wrong parameter, type expected: INT\n" + cmd + ": " + _allCommands[cmd].help);
                },
                helpText);
        }
    }
    //string
    public static void AssignCommand(string commandName, Action<string> command, string helpText = "") {
        //string cmd = commandName.ToUpper();
        string cmd = commandName;
        if (_allCommands.ContainsKey(cmd))
            Debug.Log("command already contained: " + cmd);
        else {
            _allCommands[cmd] = new ConsoleCommand(command, helpText);
        }
    }
    #endregion

    #region DEVELOPER CONSOLE HANDLING
    public static void Execute() {
        //read input
        string input = instance._inpCommand.text.Replace("\n", "");

        //if input is empty
        if (input == "") return; 

        List<string> inputs = input.Split(' ').Where(x => x != "").ToList();

        //input has no words, only empty spaces
        if (inputs.Count == 0) return;

        string
            command = inputs[0],
            param = inputs.Skip(1).MakeString(" ");

        string key = "";
        bool cmdFound = _allCommands.Keys.Find(k => k.ToUpper() == command.ToUpper(), ref key);
        Color color = cmdFound ? instance._colorInputCommand : instance._colorInputNotFound;

        //Write that input
        Write((">" + command).Colorize(color) + " " + param);

        if (cmdFound)
            _allCommands[key].command(param);
        else
            Log("Unknown command: " + command);
        
        //reset input
        instance._inpCommand.text = "";
        SelectInputField();
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
        SelectInputField();
        OnOpenConsole();
    }
    public static void CloseConsole() {
        instance._goConsole.SetActive(false);
        instance._inpCommand.text = "";
        OnCloseConsole();
    }

    static void SelectInputField() {
        UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(null);
        UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(instance._inpCommand.gameObject);
    }
    #endregion

    #region BASIC COMMANDS
    static void Help() {
        var max = _allCommands.Keys.Max(k => k.Length) + 1;
        string s = "Commands are not case sensitive. Possible Commands:\n"
            + _allCommands.MakeString(kv => " -" + kv.Key.Fill(max, " ") + "-> " + kv.Value.help, '\n');
        Log(s);
    }
    static void Clear() {
        instance._txtOutput.text = "";
        FixLogHeight();
    }
    static void Exit() {
        Log("CLOSING GAME");
#if UNITY_EDITOR
        Debug.Break();
#endif
        Application.Quit();
    }
    #endregion

    #region UNITY CONSOLE LOG
    static void PrintLog(string condition, string stackTrace, LogType type) {
        //add new line
        var c = _logColors[type];
        string s = "█".Colorize(c)
            + "<b>" + DateTime.Now.ToKzFormat2() + "</b> "
            + ("-" + type).Colorize(c) + ": "
            + condition.Replace("\n", "\n  ");
        Write(s);
    }

    static readonly Dictionary<LogType, Color> _logColors = new Dictionary<LogType, Color>() {
        {LogType.Exception, Color.red},
        {LogType.Error,     Color.red},
        {LogType.Warning,   Color.yellow},
        {LogType.Assert,    Color.yellow},
        {LogType.Log,       new Color(192f/255f, 192f/255f, 192f/255f, 1)}
    };
    #endregion
}