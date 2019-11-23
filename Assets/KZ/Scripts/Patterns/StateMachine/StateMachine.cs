using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace KZ.StateMachine {

    public class StateMachine<T> {

        public State<T> currentState { get; private set; }
        protected Dictionary<State<T>, Dictionary<T, State<T>>> transitions;

        public StateMachine() {
            currentState = null;
            transitions = new Dictionary<State<T>, Dictionary<T, State<T>>>();
        }

        public StateMachine<T> AddTransition(State<T> from, T input, State<T> to) {
            
            if (!transitions.ContainsKey(from))
                transitions[from] = new Dictionary<T, State<T>>();

            if (transitions[from].ContainsKey(input))
                Debug.LogWarning("Overriding transition: "
                    + from.GetType() + "_" + input + "_" + transitions[from][input].GetType()
                    + " -> "
                    + from.GetType() + "_" + input + "_" + to.GetType());

            transitions[from][input] = to;

            return this;
        }


        public StateMachine<T> Begin(State<T> initial, bool callInitialEnter = true) {
            currentState = initial;
            if (callInitialEnter) currentState.Enter(default);
            return this;
        }


        public bool TryFeed(T input) {
            return transitions.TryGetValue(currentState, out var possibleTrans) && possibleTrans.ContainsKey(input);
        }
        public void Feed(T input) {
            Dictionary<T, State<T>> possibleTrans;
            if (transitions.TryGetValue(currentState, out possibleTrans)) {
                if (possibleTrans.TryGetValue(input, out State<T> newState)) {
                    currentState.Exit(input);
                    currentState = newState;
                    currentState.Enter(input);
                }
            }
        }
    }


    public abstract class State<T> {

        public abstract void Enter(T input);
        public abstract void Update(T input);
        public abstract void Exit(T input);

    }


}