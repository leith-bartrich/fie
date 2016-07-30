using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

namespace fie.gui.net {

	/// <summary>
	/// An object that allows authority to be requested from client or server side, and allows that authority to be verified on the server side.
	/// </summary>
	public interface IRequestAuthority {
		
		/// <summary>
		/// Called by the AuthorityRequestor on the server side to verify
		/// if authority should be granted.  Implement security and permissions
		/// here.
		/// 
		/// Be aware, this code exists on the client. So don't expect the
		/// methodology of what you do here to be secret. Rather, make sure
		/// what you do here, is secure by its nature.
		/// </summary>
		/// <returns><c>true</c>, if authority should be granted, <c>false</c> otherwise.</returns>
		/// <param name="clientIdentity">The target object.</param>
		bool RequestAuthority (NetworkIdentity clientIdentity);

		/// <summary>
		/// Gets the NetworkIdentity of this obejct.  Usually can be implemented using GetComponent<NetworkIdentity>()
		/// </summary>
		/// <returns>The NetworkIdentity.</returns>
		NetworkIdentity GetNetworkIdentity ();

	}

}