using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;
using PrefabEvolution;

public class PrefabTools : EditorWindow {

	[MenuItem("Prefabs/Instantiate From Project Heirachy")]
	public static void InstantiateFromProjectHeirarchy(){
		var sel = Selection.objects;
		string dir = null;
		GameObject parent = null;
		foreach (var s in Selection.objects) {
			var path = AssetDatabase.GetAssetPath(s);
			if (System.IO.Directory.Exists (path)) {
				dir = path;
			}
		}
		foreach (var s in Selection.objects) {
			var go = s as GameObject;
			if (go != null) {
				if (go.scene != null) {
					parent = go;
				}
			}
		}
		if (Selection.objects.Length != 2 || dir == null || parent == null) {
			return;
		}
		InstantiatePrefabsDirRecursive (dir, parent.transform);
	}

	[MenuItem("Prefabs/Save Prefabs")]
	public static void MakeSelectedPrefabs(){
		
		var folder = EditorUtility.OpenFolderPanel ("Save Prefabs", "", "");
		folder = FileUtil.GetProjectRelativePath (folder);

		var selection = Selection.gameObjects;

		foreach (var s in selection) {
			
			if (s.GetComponent<PrefabEvolution.EvolvePrefab> () != null) {
				var prefab = s.GetComponent<PrefabEvolution.EvolvePrefab> ();
				prefab.ApplyChanges ();
			} else {
				var dir = CheckCreateSubDirs (folder, s);
				var path = System.IO.Path.Combine (dir, s.name + ".prefab");
				path = path.Replace ('\\', '/');
				if (!System.IO.File.Exists (path)) {
					PrefabUtility.CreatePrefab (path, s, ReplacePrefabOptions.ConnectToPrefab);
					PEUtils.MakeNested (s);
				}
			}

		}
		
	}

	private static string CheckCreateSubDirs(string basepath, GameObject go){
		var parent = go.transform.parent;
		List<string> subdirs = new List<string> ();
		while (parent != null) {
			if (parent.GetComponent<fie.SubDir> () != null) {
				subdirs.Add(parent.GetComponent<fie.SubDir>().dirName);
			}
			parent = parent.parent;
		}
		subdirs.Reverse ();
		string path = basepath;
		foreach (var s in subdirs) {
			path = System.IO.Path.Combine (path, s);
			path = path.Replace ('\\', '/');
		}
		var dirinfo = new System.IO.DirectoryInfo (path);
		if (!dirinfo.Exists) {
			dirinfo.Create ();
		}
		return path;
	}

	private static GameObject[] InstantiatePrefabsDirRecursive(string path, Transform parent){
		List<GameObject> ret = new List<GameObject> ();
		var prefabs = PrefabsInDir (path);
		foreach (var prefab in prefabs) {
			var instance = PrefabUtility.InstantiatePrefab (prefab) as GameObject;
			instance.transform.parent = parent;
			ret.Add (instance);
		}
		var subdirs = AssetDatabase.GetSubFolders (path);
		foreach (var subdir in subdirs) {
			var subdirName = System.IO.Path.GetFileName (subdir);
			var s = new GameObject (subdirName);
			var sd = s.AddComponent<fie.SubDir> ();
			sd.dirName = subdirName;
			s.transform.parent = parent;
			ret.AddRange(InstantiatePrefabsDirRecursive (subdir, s.transform));
		}
		return ret.ToArray ();
	}

	private static string[] SubdirsRecursive(string dir){
		List<string> ret = new List<string> ();
		var subdirs = AssetDatabase.GetSubFolders (dir);
		foreach (var s in subdirs) {
			ret.Add (s);
			ret.AddRange(SubdirsRecursive(s));
		}
		return ret.ToArray ();
	}

	private static GameObject[] PrefabsInDir(string path){
		var dir = new System.IO.DirectoryInfo (path);
		List<GameObject> ret = new List<GameObject> ();
		var prefabs = AssetDatabase.FindAssets ("t:GameObject", new string[]{ path });
		foreach (var g in prefabs){
			var p = AssetDatabase.GUIDToAssetPath (g);
			var file = new System.IO.FileInfo (p);
			if (file.Directory.FullName == dir.FullName) {
				ret.Add (AssetDatabase.LoadAssetAtPath<GameObject> (p));
			}
		}
		return ret.ToArray ();
	}

	private static PrefabEvolution.EvolvePrefab[] GetEvolvedPrefabs(){
		var prefabs = Selection.GetFiltered(typeof(PrefabEvolution.EvolvePrefab),SelectionMode.TopLevel).Cast<PrefabEvolution.EvolvePrefab>();
		return prefabs.ToArray();
	}

}
