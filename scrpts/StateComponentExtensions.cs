using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using System;

namespace fie
{

    public static class StateComponentExtensions
    {
		/// <summary>
		/// Used to treat behaviors as states.
		/// It is assumed an abstract behavior of
		/// type U has multiple subtypes, of which T is one.
		/// Checks if the given state T is applied.
		/// If not, it applies it.
		/// It will wipe out any states of type U currently applied.
		/// T must be a U.
		/// </summary>
		/// <returns>The set state.  If it already existed, it's the existing one.</returns>
		/// <param name="self">Self.</param>
		/// <typeparam name="T">The state type to create.</typeparam>
		/// <typeparam name="U">The parent state type.  All substates of this type are removed unless they're a T.</typeparam>
		public static T CheckSetState<T, U>(this GameObject self) where U : MonoBehaviour where T : U
        {
            T ret = self.GetComponent<T>();
            if (ret != null)
            {
                return ret;
            }
            foreach (U s in self.GetComponents<U>())
            {
                GameObject.Destroy(s);
            }
            ret = self.AddComponent<T>();
            return ret;
        }
    }


}