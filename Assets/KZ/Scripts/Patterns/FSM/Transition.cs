namespace KZ.FSM {
    public class Transition<T> {

        public event System.Action<T> OnTransition = delegate { };
        public T input { get; private set; }
        public State<T> targetState { get; private set; }

        public void OnTransitionExecute(T input) {
            OnTransition(input);
        }

        public Transition(T input, State<T> targetState) {
            this.input = input;
            this.targetState = targetState;
        }
    }
}