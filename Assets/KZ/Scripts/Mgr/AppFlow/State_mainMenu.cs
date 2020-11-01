using KZ.StateMachine;

namespace KZ.Managers {

    public class State_mainMenu : State<string> {

        public State_mainMenu(StateMachine<string> sm, string name) : base(sm, name) { }
        public override void Enter(string input) {
            base.Enter(input);
            SceneManager.LoadScene(SceneManager.MAIN_MENU);
        }
    }

}