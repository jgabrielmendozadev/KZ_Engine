using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using KZ.Managers;

namespace KZ {
    public class DevConsole : MonoBehaviour {

        //INSTANCE
        [SerializeField] GameObject _goConsole = null;
        [SerializeField] RectTransform _rectContent = null;
        [SerializeField] Text _txtOutput = null;
        [SerializeField] InputField _inpCommand = null;
        [SerializeField] int _maxLines = 100;
        [SerializeField] Color _colorInputCommand = Color.green;
        [SerializeField] Color _colorInputNotFound = Color.yellow;
        [SerializeField] RectTransform _containerDevButtons = null;
        [SerializeField] GridLayoutGroup _grid = null;

#if !UNITY_EDITOR && UNITY_ANDROID
        const KeyCode KeyOpenConsole = KeyCode.Escape;
#else
        const KeyCode KeyOpenConsole = KeyCode.F1;
#endif

        public static DevConsole Create() {
            if (!Locator.settings.GetValue("useDevConsole", DefaultKZValues.settings.allowDevConsole)) {
                Debug.Log("not using devConsole");
                return null;
            }

            var dc = Instantiate(Resources.Load<DevConsole>("UI/DevConsole"));
            DontDestroyOnLoad(dc.gameObject);
            return dc;
        }

        public void Initialize() {
            LoadScene.OnResetApp += CloseConsole; //TODO(check this)
            InitializeButtons(); //TODO(check this)

            //BASIC CONSOLE
            _isOpen = false;
            AssignCommand("help", Help, "Shows all commands");
            AssignCommand("clear", Clear, "Clears this text");
            AssignCommand("exit", Exit, "Quits the game");

            //INPUTS
            InputManager.AssignKey("ToggleConsole", KeyOpenConsole);

            OnOpenConsole += () => {
                InputManager.AssignKey("Execute", KeyCode.Return);
                InputManager.AssignKey("Execute", KeyCode.KeypadEnter);
            };
            OnCloseConsole += () => {
                InputManager.RemoveKey("Execute", KeyCode.Return);
                InputManager.RemoveKey("Execute", KeyCode.KeypadEnter);
            };

            Application.logMessageReceived += PrintLog; //UNITY DEBUG LOG
            CloseConsole();
        }
        public void Dispose() {
            Application.logMessageReceived -= PrintLog;
            LoadScene.OnResetApp -= CloseConsole;
            InputManager.RemoveKey("ToggleConsole", KeyCode.F1);
            InputManager.RemoveKey("Execute", KeyCode.Return);
            InputManager.RemoveKey("Execute", KeyCode.KeypadEnter);
            Destroy(gameObject);
        }
        private void Update() {
            if (InputManager.GetInput("ToggleConsole")) ToggleConsole();
            if (InputManager.GetInput("Execute")) Execute();
        }

        public void ExecuteCommand() { Execute(); }





        public struct ConsoleCommand {
            public Action<string> command;
            public string help;
            public ConsoleCommand(Action<string> action, string helpText = "") {
                command = action;
                help = helpText;
            }
        }
        bool _isOpen;
        Dictionary<string, ConsoleCommand> _allCommands = new Dictionary<string, ConsoleCommand>();
        public event Action OnOpenConsole = delegate { };
        public event Action OnCloseConsole = delegate { };





        #region LOGGER
        void Write(string text) {
            _txtOutput.text += "\n" + text;
            LimitLines();
            FixLogHeight();
        }
        public void Log(string text) {
            Write("·" + text);
        }
        public void Log(string text, Color color) {
            Write("·" + text.Colorize(color));
        }
        void LimitLines() {
            Canvas.ForceUpdateCanvases();
            var lines = _txtOutput.cachedTextGenerator.lines;
            var numerOfLines = lines.Count;
            if (numerOfLines > _maxLines)
                _txtOutput.text = _txtOutput.text.Remove(0, lines[numerOfLines - _maxLines].startCharIdx);
        }
        void FixLogHeight() {
            var height = _txtOutput.preferredHeight;
            _rectContent.sizeDelta = _rectContent.sizeDelta.SetY(height);
        }
        #endregion


        //CONSOLE
        #region COMMAND HANDLING
        public void RemoveCommand(string commandName) {
            string cmd = commandName;
            _allCommands.Remove(cmd);
        }
        //no params
        public void AssignCommand(string commandName, Action command, string helpText = "") {
            string cmd = commandName;
            if (_allCommands.ContainsKey(cmd))
                Debug.Log("command already contained: " + cmd);
            else {
                _allCommands[cmd] = new ConsoleCommand(param => command(), helpText);
            }
        }
        //int
        public void AssignCommand(string commandName, Action<int> command, string helpText = "") {
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
        //float
        public void AssignCommand(string commandName, Action<float> command, string helpText = "") {
            string cmd = commandName;
            if (_allCommands.ContainsKey(cmd))
                Debug.Log("command already contained: " + cmd);
            else {
                _allCommands[cmd] = new ConsoleCommand(
                    param => {
                        if (float.TryParse(param, out float value))
                            command(value);
                        else
                            Log("Wrong parameter, type expected: FLOAT\n" + cmd + ": " + _allCommands[cmd].help);
                    },
                    helpText);
            }
        }
        //string
        public void AssignCommand(string commandName, Action<string> command, string helpText = "") {
            string cmd = commandName;
            if (_allCommands.ContainsKey(cmd))
                Debug.Log("command already contained: " + cmd);
            else
                _allCommands[cmd] = new ConsoleCommand(command, helpText);
        }
        #endregion

        #region DEVELOPER CONSOLE HANDLING
        public void Execute() {
            if (!_isOpen) return;

            //read input
            string input = _inpCommand.text.Replace("\n", "");

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
            Color color = cmdFound ? _colorInputCommand : _colorInputNotFound;

            //Write that input
            Write((">" + command).Colorize(color) + " " + param);

            if (cmdFound)
                _allCommands[key].command(param);
            else
                Log("Unknown command: " + command);

            //reset input
            _inpCommand.text = "";
            SelectInputField();
        }

        public void ToggleConsole() {
            if (_isOpen)
                CloseConsole();
            else
                OpenConsole();
        }

        public void OpenConsole() {
            _isOpen = true;
            _goConsole.SetActive(true);
            _inpCommand.text = "";
            SelectInputField();
            OnOpenConsole();
        }
        public void CloseConsole() {
            _isOpen = false;
            _goConsole.SetActive(false);
            _inpCommand.text = "";
            OnCloseConsole();
        }

        void SelectInputField() {
            UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(null);
            UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(_inpCommand.gameObject);
        }
        #endregion

        #region BASIC COMMANDS
        void Help() {
            var max = _allCommands.Keys.Max(k => k.Length) + 1;
            string s = "Commands are not case sensitive. Possible Commands:\n"
                + _allCommands.MakeString(kv => " -" + kv.Key.Fill(max, " ") + "-> " + kv.Value.help, '\n');
            Log(s);
        }
        void Clear() {
            _txtOutput.text = "";
            FixLogHeight();
        }
        void Exit() {
            Log("CLOSING GAME");
#if UNITY_EDITOR
            Debug.Break();
#endif
            Application.Quit();
        }
        #endregion

        #region UNITY CONSOLE LOG
        public void PrintLog(string condition, string stackTrace, LogType type) {
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


        //ACTIONS
        #region BUTTON HANDLING
        List<DevButton> _devButtons = new List<DevButton>();

        public DevButton AddButton(Action<DevButton> onClick, string defaultTitle = "button") {
            var parent = _containerDevButtons;
            var btn = Instantiate(Resources.Load<DevButton>("UI/DevButton"));
            btn.Initialize();
            btn.SetAction(onClick);
            btn.title = defaultTitle;
            btn.transform.SetParent(parent);
            btn.transform.localScale = Vector3.one;
            Canvas.ForceUpdateCanvases();
            parent.sizeDelta = parent.sizeDelta.SetY(_grid.preferredHeight);
            _devButtons.Add(btn);
            return btn;
        }

        void InitializeButtons() {
            foreach (var btn in _devButtons) Destroy(btn.gameObject);
            _devButtons.Clear();
            AddButton(x => Clear(), "Clear log");
        }
        #endregion

        //Button commands
        public void BtnClear() { Clear(); }
    }
}