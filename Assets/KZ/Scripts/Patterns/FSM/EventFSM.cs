public class EventFSM<T> {

    public State<T> currentState { get; private set; }

    public EventFSM(State<T> initial) {
        currentState = initial;
        currentState.Enter(default(T));
    }

    public void Feed(T input) {
        State<T> newState;

        if (currentState.Feed(input, out newState)) {
            currentState.Exit(input);
            currentState = newState;
            currentState.Enter(input);
        }
    }

    public void Update() { currentState.Update(); }
}