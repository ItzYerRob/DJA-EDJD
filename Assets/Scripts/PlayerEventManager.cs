using System;

public static class PlayerEventManager
{
    //Defining events using Action.
    public static event Action OnAttack, OnParry, OnDash, OnDodge;

    public static void TriggerAttack()
    {
        if (OnAttack != null)
        {
            OnAttack.Invoke();
        }
    }

    public static void TriggerParry()
    {
        if (OnParry != null)
        {
            OnParry.Invoke();
        }
    }
    public static void TriggerDash()
    {
        if (OnDash != null)
        {
            OnDash.Invoke();
        }
    }

    public static void TriggerDodge()
    {
        if (OnDodge != null)
        {
            OnDodge.Invoke();
        }
    }
}
