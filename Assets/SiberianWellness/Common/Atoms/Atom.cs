using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace SiberianWellness.Common.Atoms
{
    public class Atom<T>
    {
        class AtomWaiter<T2>:CustomYieldInstruction
        {
            Atom<T2> atom;
            T2              value;
            UnityAction<T2> callBack;
            bool isWaiting = true;

            public AtomWaiter(Atom<T2> atom, T2 value)
            {
                this.atom = atom;
                this.value    = value; 
            }
            
            public AtomWaiter(Atom<T2> atom, T2 value, UnityAction<T2> callBack)
            {
                this.atom     = atom;
                this.value    = value;
                this.callBack = callBack; 
            } 

            public void OnChanged(T2 newValue)
            {
                if (newValue.Equals(value))
                {
                    isWaiting = false;
                    atom.changed -= OnChanged;
                     
                    callBack?.Invoke(newValue);
                }
            }

            public override bool keepWaiting => isWaiting;
        }

        [NonSerialized]
        T value;

        UnityAction<T> changed;

        public T Value
        {
            get => value;
            set
            {
                if (!value.Equals(this.value))
                {
                    this.value = value;

                    changed?.Invoke(this.value);
                }
            }
        }

        public IEnumerator WaitFor(T value)
        {
            if (this.value.Equals(value))
            {
                yield break;
            }
            else
            { 
                AtomWaiter<T> waiter = new AtomWaiter<T>(this, value);
                changed += waiter.OnChanged;
                yield return waiter;
            }
        }

        public void WaitFor(T waitFor, UnityAction<T> callBack)
        {
            if (waitFor.Equals(value))
            {
                callBack?.Invoke(value);
            }
            else
            {
                AtomWaiter<T> waiter = new AtomWaiter<T>(this, waitFor, callBack);
                changed += waiter.OnChanged;
            }
        }

        public static implicit operator T(Atom<T> atom) => atom.value;
    }

  
}
