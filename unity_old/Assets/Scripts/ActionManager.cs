using System;
using System.Collections.Generic;
using UnityEngine;

public class ActionManager : MonoBehaviour
{
    private List<Action<string>> _subs;

    public ActionManager()
    {
        this._subs = new List<Action<string>>();
    }

    public void Subscribe(Action<string> method)
    {
        this._subs.Add(method);
    }

    public void Unsubscribe(Action<string> method)
    {
        this._subs.Remove(method);
    }

    public void Notify(string message)
    {
        this._subs.ForEach(sub => sub(message));
    }
}
