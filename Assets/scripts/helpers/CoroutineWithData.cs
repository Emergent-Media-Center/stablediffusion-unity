using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoroutineWithData<T>
{
    private IEnumerator _target;
    private object result;
    public Coroutine Coroutine { get; private set; }
 
    public CoroutineWithData(MonoBehaviour owner_, IEnumerator<T> target_)
    {
        _target = target_;
        Coroutine = owner_.StartCoroutine(Run());
    }
 
    private IEnumerator Run()
    {
        while(_target.MoveNext())
        {
            result = _target.Current;
            yield return result;
        }
    }
    
    public T Result => (T) result;
}
