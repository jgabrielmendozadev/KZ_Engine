using KZ.StateMachine;

namespace KZ.Managers {
    public class State_Intro : State<string> {

        public State_Intro(StateMachine<string> sm, string name) : base(sm, name) { }

        public override void Enter(string input) {
            base.Enter(input);
            SceneManager.LoadScene(SceneManager.INTRO_KZ);
            IntroAnim_KZ.OnAnimationEnd += () => _stateMachine.Feed("intro anim ended");
        }

    }
}