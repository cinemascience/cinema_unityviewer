using UnityEngine;
using UnityEngine.UI;
using System;

/**
 * Represents a single item in a database's sprite cache. Contains a sprite and a key (usually the filename or path) to refer to it by
 * Also keeps track of the size (in bytes) of the item
 */
public class CacheItem {

	public Sprite sprite;
	public string key;
	public int bytes;

	public CacheItem(Sprite s, string k, int b) {
		sprite = s;
		key = k;
		bytes = b;
	}
}