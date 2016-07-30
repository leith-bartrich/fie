using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;
using System.Linq;
using fie.gui.net;

namespace fie {



	public partial class Avatar : NetworkBehaviour {

		void Awake(){
			DontDestroyOnLoad (this.gameObject);
			OnPreSpawn ();
		}

		partial void OnPreSpawn () ;

		#region Prefs

		public string SaveName;

		public void SavePrefs(){
			PlayerPrefs.SetString (SaveName + "_avatar_prefs", JsonUtility.ToJson (this));
			this.BroadcastMessage ("OnSavePrefs", SaveName, SendMessageOptions.DontRequireReceiver);
		}


		public void LoadPrefs() {
			var prefName = SaveName + "_avatar_prefs";
			if (PlayerPrefs.HasKey (prefName)) {
				JsonUtility.FromJsonOverwrite (PlayerPrefs.GetString (prefName), this);
			}
			this.BroadcastMessage ("OnLoadPrefs", SaveName, SendMessageOptions.DontRequireReceiver);

		}
		#endregion


		#region AuthoritySystem

		/// <summary>
		/// Request that the server move authority of the requested network object to the local player.
		/// This is safe to call from Client or Server code.
		/// </summary>
		/// <param name="target">The target network object, which must implement IRequestAuthority interface.</param>
		public void RequestAuthory(IRequestAuthority target){
			CmdRequestAuthority (target.GetNetworkIdentity());
		}

		[Command]
		private void CmdRequestAuthority(NetworkIdentity targetNetIden){
			//todo: security issue.  check all potential IRequestAuthority components and deny if any have a deny response.  Not just the first we find.
			var target = targetNetIden.GetComponent<IRequestAuthority> ();
			if ((target != null) && isServer) {
				var clientConnection = this.connectionToClient;
				if (target.RequestAuthority(this.GetComponent<NetworkIdentity>())){
					SetAuthority (targetNetIden,clientConnection);
				}
			}
		}

		[Server]
		private void SetAuthority(NetworkIdentity ni, NetworkConnection nc){
			//if we have problems here, we might want to revoke authority from others first?  Not sure if that's done automatically.
			ni.AssignClientAuthority (nc);
		}


		#endregion

		#region Profile
		//this may need to be changed dramaatically.
		//right now, we need it to be an enum so it can be known as a SyncVar at both ends.  But really, this might need a whole subclass or network behavior system instead.

		public enum EnumProfileType
		{
			LOCAL,
			//The local user.  The owner of the current user account.
			ANON,
			//An anonymous guest granted access by the local user
			FOREIGN,
			//Someone else who has a profile, who is a guest granted access by the local user.
		}

		/// <summary>
		/// The type of user profile for this avatar.
		/// </summary>
		[SyncVar]
		public EnumProfileType ProfileType;

		#endregion


		public static Avatar[] Avatars(){
			return (from a in GameObject.FindObjectsOfType<Avatar> ()
			        select a).ToArray ();
		}

		public static Avatar[] RemoteAvatars(){
			return (from a in GameObject.FindObjectsOfType<Avatar> ()
				where !a.hasAuthority && a.isLocalPlayer
				select a).ToArray ();
		}

		public static Avatar[] LocalAvatars(){
			return (from a in GameObject.FindObjectsOfType<Avatar> ()
			 where a.hasAuthority && a.isLocalPlayer
			 select a).ToArray ();
		}

		public static Avatar LocalAvatar(int index = 0){
			var avatars = LocalAvatars ();
			if (avatars.Length < index + 1){
				return null;
			}
			return LocalAvatars () [index];
		}

		public static bool FindLocalAvatar(int index, out Avatar found){
			var avatars = LocalAvatars ();
			if (avatars.Count() >= index + 1) {
				found = avatars [index];
				return true;
			} else {
				found = null;
				return false;
			}
		}

		#region Teleporting
		//All teleporting is called from the sever but executed on the client, because the clients are authoritative for "Players."
		//A client that gets this call that is not the local player, will still do it based on latest information.
		//But will be updated by the true authority later.  So it's okay.

		/// <summary>
		/// Instantly teleport to the given world position and rotation.
		/// The object's parent is not changed.
		/// </summary>
		/// <param name="position">Position.</param>
		/// <param name="rotation">Rotation.</param>
		[ClientRpc]
		public void RpcTeleport (Vector3 position, Quaternion rotation)
		{
			this.transform.position = position;
			this.transform.rotation = rotation;
		}

		/// <summary>
		/// Instantly teleport to the given local position and rotation, in the given local space.
		/// The object's parent is not changed.
		/// </summary>
		/// <param name="position">Position.</param>
		/// <param name="rotation">Rotation.</param>
		/// <param name="parent">Local object or space for the coordinates.</param>
		[ClientRpc]
		public void RpcTeleportLocal (Vector3 position, Quaternion rotation, GameObject space)
		{
			var origParent = this.transform.parent;
			this.transform.parent = space.transform;
			this.transform.localPosition = position;
			this.transform.localRotation = rotation;
			this.transform.parent = origParent;
		}

		/// <summary>
		/// Instantly teleport from the given object to the given object, preserving the local offset.
		/// hint: If they're from the same prefab, the user might not even notice they've been moved.
		/// The object's parent is not changed.
		/// </summary>
		/// <param name="relFrom">Offset calculated from this object.</param>
		/// <param name="relTo">Player is teleported to this object, maintaining their offset from the 'from' object.</param>
		[ClientRpc]
		public void RpcTeleportFromTo (GameObject relFrom, GameObject relTo)
		{
			var origParent = this.transform.parent;
			this.transform.parent = relFrom.transform;
			var pos = this.transform.localPosition;
			var rot = this.transform.localRotation;
			this.transform.parent = relTo.transform;
			this.transform.localPosition = pos;
			this.transform.localRotation = rot;
			this.transform.parent = origParent;
		}

		#endregion

	}

}