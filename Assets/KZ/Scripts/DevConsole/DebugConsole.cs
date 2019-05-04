using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using KZ;

public class DebugConsole : MonoBehaviour {

    #region SINGLETON
    public static DebugConsole instance;
    void AwakeSingleton() {
        if (instance != null) Destroy(instance.gameObject);
        instance = this;
    }
    #endregion

    private void Awake() {
        AwakeSingleton();
        _isOpen = false;
        anim.SetBool("isOpen", false);
        AssignCommand("HELP", Help, "Shows all commands");
        AssignCommand("CLEAR", Clear, "Clears this text");
        OnOpenConsole += () => inpCommand.text = "";
        OnOpenConsole += () => inpCommand.Select();
        OnCloseConsole += () => inpCommand.text = "";
        Application.logMessageReceived += PrintLog;
        InputManager.AssignKey("ToggleConsole", KeyCode.F1);
        InputManager.AssignKey("ToggleConsole", KeyCode.Escape);
        InputManager.AssignKey("Execute", KeyCode.Return);
        InputManager.AssignKey("Execute", KeyCode.KeypadEnter);
        Utility.ExecuteInFrames(1, () => Write("LOADED COMMANDS: " + _allCommands.Keys.Count));
    }

    private void OnDestroy() {
        Application.logMessageReceived -= PrintLog;
    }

    public Text txtOut;
    public InputField inpCommand;
    public Animator anim;
    public int maxLines = 100;
    bool _isOpen;

    //Dictionary<string, Action<string>> _commands = new Dictionary<string, Action<string>>();
    //Dictionary<string, string> _help = new Dictionary<string, string>();
    Dictionary<string, ConsoleCommand> _allCommands = new Dictionary<string, ConsoleCommand>();

    public struct ConsoleCommand {
        public Action<string> command;
        public string help;
        public ConsoleCommand(Action<string> action, string helpText = "") {
            command = action;
            help = helpText;
        }
    }

    public void Write(string s) {
        txtOut.text += "\n·" + s ;
        LimitateLines();
    }

    void LimitateLines() {
        Canvas.ForceUpdateCanvases();
        var l = txtOut.cachedTextGenerator.lines;
        var c = l.Count;
        if (c > maxLines)
            txtOut.text = txtOut.text.Remove(0, l[c - maxLines].startCharIdx);
    }

    #region COMMAND HANDLING
    public void RemoveCommand(string commandName) {
        string cmd = commandName.ToUpper();
        _allCommands.Remove(cmd);
    }
    //no params
    public void AssignCommand(string commandName, Action command, string helpText = "") {
        string cn = commandName.ToUpper();
        if (_allCommands.ContainsKey(cn))
            Debug.Log("command already contained: " + cn);
        else {
            _allCommands[cn] = new ConsoleCommand(param => command(), helpText);
        }
    }
    //int
    public void AssignCommand(string commandName, Action<int> command, string helpText = "") {
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
    public void AssignCommand(string commandName, Action<string> command, string helpText = "") {
        string cn = commandName.ToUpper();
        if (_allCommands.ContainsKey(cn))
            Debug.Log("command already contained: " + cn);
        else {
            _allCommands[cn] = new ConsoleCommand(command, helpText);
        }
    }
    #endregion

    #region BASIC COMMANDS
    void Help() {
        string s = "HELP:\nPossible Commands:" +
            _allCommands.Aggregate("", (r, kv) => r + "\n" + kv.Key + " -> " + kv.Value.help);
        Write(s);
    }
    void Clear() {
        txtOut.text = "";
    }
    #endregion


    public void Execute() {
        //receive text
        string input = inpCommand.text;

        //process
        if (input == "")
            return;
        List<string> inputs = input.Split(' ').Where(x => x != "").ToList();
        if (inputs.Count == 0)
            return;
        string command = inputs[0].ToUpper();
        string param = inputs.Count > 1 ?
            inputs.Skip(1).Aggregate("",(r,x)=>r+" "+x).Remove(0,1) :
            "";
        if (!_allCommands.ContainsKey(command))
            Write("Unknown command: " + command);
        else
            _allCommands[command].command(param);

        //reset input
        inpCommand.text = "";
        inpCommand.Select();
    }

    public void ToggleConsole() {
        if (_isOpen = !_isOpen)
            OnOpenConsole();
        else
            OnCloseConsole();
        anim.SetBool("isOpen", _isOpen);
    }


    public event Action OnOpenConsole = delegate { };
    public event Action OnCloseConsole = delegate { };



    private void Update() {
        if (InputManager.GetInput("ToggleConsole")) ToggleConsole();
        if (InputManager.GetInput("Execute")) Execute();
    }


    readonly Dictionary<LogType, string> _logColors = new Dictionary<LogType, string>() {
        {LogType.Exception,"red" },
        {LogType.Error,    "red" },
        {LogType.Warning,  "yellow" },
        {LogType.Assert,   "yellow" },
        {LogType.Log,      "silver" }
    };

    void PrintLog(string condition, string stackTrace, LogType type) {
        //add new line
        string s = "<b>" + DateTime.Now.ToKzFormat2() + "</b>" +
            "<color=" + _logColors[type] + ">" + type + "</color>: "
            + condition.Replace("\n", "\n  ");
        Write(s);
    }
}