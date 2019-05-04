using System.Collections.Generic;


public class StateConfigurer<T> {
    State<T> state;
    Dictionary<T, Transition<T>> transitions = new Dictionary<T, Transition<T>>();

    public StateConfigurer(State<T> state) {
        this.state = state;
    }

    public StateConfigurer<T> AddTransition(T input, State<T> target) {
        transitions.Add(input, new Transition<T>(input, target));
        return this;
    }

    public void Done() { state.Configure(transitions); }
}

public static class StateConfigurer {
    public static StateConfigurer<T> Create<T>(State<T> state) {
        return new StateConfigurer<T>(state);
    }
}