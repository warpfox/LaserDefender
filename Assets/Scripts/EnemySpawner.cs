﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour {
	public GameObject enemyPrefab;
	public float width = 10f;
	public float height = 5;
	public float speed = 5f;
	public float spawnDelay = 0.5f;

	private bool movingRight = true;
	private float xmax;
	private float xmin;

	public void OnDrawGizmos(){
		Gizmos.DrawWireCube(transform.position, new Vector3(width,height));
	}

	// Use this for initialization
	void Start () {
		float distanceToCamera = transform.position.z - Camera.main.transform.position.z;
		Vector3 leftBoundary = Camera.main.ViewportToWorldPoint(new Vector3(0,0, distanceToCamera));
		Vector3 rightBoundary = Camera.main.ViewportToWorldPoint(new Vector3(1,0, distanceToCamera));
		xmax = rightBoundary.x;
		xmin = leftBoundary.x;
		SpawnUntilFull();
	}

	// Update is called once per frame
	void Update () {
		if (movingRight){
			transform.position += Vector3.right*speed*Time.deltaTime;
		} else {
			transform.position += Vector3.left*speed*Time.deltaTime;
		}
		float rightEdgeOfFormation = transform.position.x + (0.5f*width);
		float leftEdgeOfFormation = transform.position.x - (0.5f*width);
		if (leftEdgeOfFormation < xmin){
			movingRight = true;
		} else if (rightEdgeOfFormation > xmax){
			movingRight = false;
		}
		if (AllMembersDead()){
			SpawnUntilFull();
		}
	}

	Transform NextFreePosition(){
		foreach(Transform childPositionGameObject in transform){
			if (childPositionGameObject.childCount == 0){
				return childPositionGameObject;
			}
		}
		return null;
	}

	bool AllMembersDead(){
		foreach(Transform childPositionGameObject in transform){
			if (childPositionGameObject.childCount > 0){
				return false;
			}
		}
		return true;
	}

	void SpawnEnemies(){
		foreach (Transform child in transform){
			GameObject enemy = Instantiate<GameObject>(enemyPrefab, child.transform.position, Quaternion.identity);
			enemy.transform.parent = child;
		}
	}

	void SpawnUntilFull(){
		Transform freePosition = NextFreePosition();
		if (freePosition != null){
			GameObject enemy = Instantiate<GameObject>(enemyPrefab, freePosition.position, Quaternion.identity);
			enemy.transform.parent = freePosition;
			if (NextFreePosition() != null){
				Invoke ("SpawnUntilFull", spawnDelay);
			}
		}
	}
}