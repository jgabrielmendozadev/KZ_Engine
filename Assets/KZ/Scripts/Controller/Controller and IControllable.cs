//T_Input is generally a struct or class with data about input, example:
//class OR struct PlayerInput{
//  bool jump;
//  Vector3 dirMove;
//}
public interface IControllable<T_Input> {

    void SetInput(T_Input newInput);

}



public abstract class UserController<T_Controllable, T_Input> where T_Controllable : IControllable<T_Input> {

    public virtual void GetInput(T_Controllable owner) {
        owner.SetInput(GetInputInternal());
    }

    protected abstract T_Input GetInputInternal();

}