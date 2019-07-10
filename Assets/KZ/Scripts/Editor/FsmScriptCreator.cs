using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;
using KZ;

public class FsmScriptCreator : MonoBehaviour {


    [SerializeField] List<string> stateNames;
    [TextArea(10, 100)]
    public string OUTPUT;

    private void OnValidate() {
        //!null || !empty || !empty values || !repeated values
        if (stateNames == null 
            || stateNames.Count <= 0
            || stateNames.Any(x => x == "") 
            || stateNames.Aggregate(new HashSet<string>(), (r, x) => r.AddReturn(x)).Count != stateNames.Count) {
            OUTPUT = "ADD NAMES TO STATES LIST";
            return;
        }
        OUTPUT = stateNames
            .Select(x => "public static event Action OnEnter" + F(x) + " = delegate { };")
            .MakeString("\n");
        OUTPUT += "\n\n";
        OUTPUT += "[SerializeField] GameScreens Game_Screens = new GameScreens();\n";
        OUTPUT += "[Serializable] class GameScreens {\n";
        OUTPUT += "    public List<GameObject> \n" + stateNames.MakeString(x => "        SCR_" + F(x) + " = null", ",\n") + ";\n";
        OUTPUT += "    public List<GameObject> GetAll() {\n";
        OUTPUT += "        List<GameObject> r = new List<GameObject>();\n";
        OUTPUT += "        foreach (var f in this.GetType().GetFields()) {\n";
        OUTPUT += "            if (f.FieldType == typeof(List<GameObject>))\n";
        OUTPUT += "                r.AddRange((List<GameObject>)f.GetValue(this));\n";
        OUTPUT += "        }\n";
        OUTPUT += "        return r;\n";
        OUTPUT += "    }\n";
        OUTPUT += "}\n";
        OUTPUT += "\n";
        OUTPUT += "void TurnOnObjects(List<GameObject> l) {\n";
        OUTPUT += "    foreach (var x in Game_Screens.GetAll().Where(x => !l.Contains(x))) x.SetActive(false);\n";
        OUTPUT += "    l.ForEach(x => x.SetActive(true));\n";
        OUTPUT += "}\n";
        OUTPUT += "\n";
        OUTPUT += "const string\n";
        OUTPUT += "    FSM_NEXT = \"NEXT\",\n";
        OUTPUT += "    FSM_BACK = \"BACK\";\n";
        OUTPUT += "EventFSM<string> fsm;\n";
        OUTPUT += "void Start_FSM() {\n";
        OUTPUT += "    State<string>\n";
        OUTPUT += stateNames.Select(x => "        " + x + " = new State<string>(\"" + x + "\")").MakeString(",\n") + ";\n";
        OUTPUT += "\n";
        OUTPUT += stateNames.Select(x => "    " + x + ".OnEntr += x => { OnEnter" + F(x) + "(); TurnOnObjects(Game_Screens.SCR_" + F(x) + "); };").MakeString("\n") + "\n";
        OUTPUT += "\n";
        for (int i = 0; i < stateNames.Count; i++) {
            OUTPUT += "    StateConfigurer.Create(" + stateNames[i] + ")\n";
            OUTPUT += "        .AddTransition(FSM_NEXT, " + stateNames[(i + 1).Cycle(0, stateNames.Count - 1)] + ")\n";
            OUTPUT += "        .AddTransition(FSM_BACK, " + stateNames.First() + ")\n";
            OUTPUT += "        .Done();\n";
        }
        OUTPUT += "\n    fsm = new EventFSM<string>(" + stateNames.First() + ");\n";
        OUTPUT += "}\n";
    }

    string F(string x) {
        return x.First().ToString().ToUpper() +
            x.Skip(1).MakeString(c => c.ToString(), "");
    }
}