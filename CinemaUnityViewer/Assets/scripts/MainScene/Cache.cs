using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.IO;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Security.Cryptography;

/**
 * Manages a collection of Sprites for a database. Also handles processing of textures into sprites
 */
public class Cache {

	//Maximum number of images that can be stored in the cache
	public static int maxItems;

	//A List of CacheItems representing the stored Sprites (and their names)
	private List<CacheItem> cache;

	//Constructor
	public Cache() {
		cache = new List<CacheItem>();
	}

	//Returns the CacheItem with the given key (or null if it doesn't exist)
	public CacheItem fetch(string key) {
		foreach (CacheItem item in cache) {
			if (item.key.Equals(key)) {
				return item;
			}
		}
		return null;
	}

	//Returns the Sprite of the CacheItem with the given key (or null if it doesn't exist)
	public Sprite fetchSprite(string key) {
		foreach (CacheItem item in cache) {
			if (item.key.Equals(key)) {
				return item.sprite;
			}
		}
		return null;
	}

	//Add a new CacheItem to the cache from the given sprite and key
	public void addItem(Sprite sprite, string key, int bytes) {
		CacheItem newItem = new CacheItem(sprite, key, bytes);
		addItem(newItem);
	}

	//Add a new a CacheItem to the cache (or overwrite, if one with its key already exists)
	public void addItem(CacheItem newItem) {
		CacheItem item = fetch(newItem.key);
		if(item == null) {
			cache.Add(newItem);
			if(cache.Count > maxItems) {
				cache.RemoveAt(0);
			}
		}
		else {
			item.sprite = newItem.sprite;
			item.bytes = newItem.bytes;
		}
	}

	//Returns whether or not a CacheItem with the given key exists in the cache
	public bool contains(string key) {
		foreach (CacheItem item in cache) {
			if (item.key.Equals(key)) {
				return true;
			}
		}
		return false;
	}

	//The sum of the bytes value of all stored CacheItems
	public int getMemorySize() {
		int bytes = 0;
		foreach (CacheItem item in cache) {
			bytes += item.bytes;
		}
		return bytes;
	}

	//The number of CacheItems stored
	public int getSize() {
		return cache.Count;
	}

}


