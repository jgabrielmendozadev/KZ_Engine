using System;
using System.Collections.Generic;

namespace KZ.FSM {
    public class State<T> {
        public string name { get; private set; }

        public event Action<T> OnEntr = delegate { };
        public event Action OnUpdt = delegate { };
        public event Action<T> OnExit = delegate { };

        Dictionary<T, Transition<T>> transitions;

        //Constructor
        public State() { }
        public State(string name) { this.name = name; }

        public State<T> Configure(Dictionary<T, Transition<T>> transitions) {
            this.transitions = transitions;
            return this;
        }

        public Transition<T> GetTransition(T input) {
            return transitions.ContainsKey(input) ?
                transitions[input] : null;
        }

        public bool Feed(T input, out State<T> next) {
            if (transitions.ContainsKey(input)) {
                var transition = transitions[input];
                transition.OnTransitionExecute(input);
                next = transition.targetState;
                return true;
            }
            next = this;
            return false;
        }

        public void Enter(T input) { OnEntr(input); }
        public void Update() { OnUpdt(); }
        public void Exit(T input) { OnExit(input); }

    }
}