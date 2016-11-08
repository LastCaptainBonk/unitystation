﻿using UnityEngine;
using System.Collections;
using SS.TileTypes;

namespace SS.GameLogic {
	
	public class GameManager : MonoBehaviour {
		
		public GameObject tile;
		public GameObject tileGrid;
		public int gridSizeX = 100;
		public int gridSizeY = 100;

		public GameObject floorTile;
		public GameObject wallTile;
		public GameObject airLockTile;

		public GameObject playerCamera;
		public float panSpeed = 10.0f;

		public enum Direction {Up, Left, Down, Right};
		public enum TileType {Space, Floor, Wall};
		public enum ItemTile {Tile, Item};

		private GameObject[,] grid;
		private TextAsset map;
        private bool mapLoaded = false;

		// Use this for initialization
		void Start () {
			InitTiles();
            map = Resources.Load<TextAsset>("maps/map");
        }
		
		// Update is called once per frame
		void Update () {

			if (!mapLoaded && HasGridLoaded()) {
				 //TODO Get rid of ghetto map and resources.load
				LoadMap(map);
                mapLoaded = true;
			}
            if (Input.GetKeyDown(KeyCode.O))
            {
                //TODO Get rid of ghetto map and resources.load
                LoadMap(map);
                mapLoaded = true;
            }
        }

		private void InitTiles() {
			grid = new GameObject[gridSizeX, gridSizeY];
			TileManager tileManager;

			/*
			TODO find size of the sprite then use it to dynamically build the grid. Set to 32px right now
			int tileWidth = (int)spaceSheet[0].bounds.size.x;
			int tileHeight = (int)spaceSheet[0].bounds.size.y;

			Debug.Log(tileWidth + " " + tileHeight);
			*/
			for (int i = 0; i < gridSizeX; i++) {
				for (int j = 0; j < gridSizeY; j++) {
					grid[i, j] = Instantiate(tile);
					grid[i, j].transform.position = new Vector3(i * 32, j * 32);
					grid[i, j].transform.SetParent(tileGrid.transform);

					tileManager = grid[i, j].GetComponent<TileManager>();
					tileManager.gridX = i;
					tileManager.gridY = j;
                    tileManager.gameManager = gameObject.GetComponent<GameManager>();

                }
			}
		}
		
        private bool HasGridLoaded()
        {
            bool gridLoaded = true;
            int count = 0;
            foreach (GameObject gridObj in grid)
            {
                count++;
                if (!gridObj.activeSelf)
                {
                    gridLoaded = false;
                    break;
                }
            }
            return gridLoaded;
        }	

		private void LoadMap(TextAsset map) {
			string[] lines = map.text.Split('\r');
			for (int i = 0; i < lines.Length - 1; i++) {
				for (int j = 0; j < lines[i].Length - 1; j++) {
					TileManager thisTileManager = grid[i,j].GetComponent<TileManager>();
					switch(lines[i][j]) {
					case 'w':
						GameObject wt = Instantiate (wallTile);
						wt.GetComponent<WallTile> ().SetTile (Standard_Wall.walls_20, grid [i, j].transform.position);
						wt.SetActive (true);
						grid[i, j].GetComponent<TileManager>().passable = new bool[4]{false, false, false, false};
						thisTileManager.addObject(wt, ItemTile.Tile);
						break;
					case 'f':
						GameObject ft = Instantiate(floorTile);
						ft.GetComponent<FloorTile>().SetTile(Construction_Floors.floors_1, grid[i, j].transform.position);
						ft.SetActive(true);
						thisTileManager.addObject(ft, ItemTile.Tile);
						break;
					case 'a':
						//kaffo no idea wtf I am doing but just mocking this up - Doobly
						GameObject al = Instantiate (airLockTile);
						Vector2 newTempPos = grid [i, j].transform.position;
						newTempPos.x += 16f;
						newTempPos.y -= 16f;
						al.transform.position = newTempPos;
						break;
					}
				}
			}
		}

		public bool CheckPassable(int gridX, int gridY, Direction direction) {
			int newGridX = gridX;
			int newGridY = gridY;

			Direction newDirection = Direction.Up;

			switch (direction) {
			case Direction.Up:
				newGridY = newGridY + 1;
				newDirection = Direction.Down;
				break;
			case Direction.Right:
				newGridX = newGridX + 1;
				newDirection = Direction.Left;
				break;
			case Direction.Down:
				newGridY = newGridY - 1;
				newDirection = Direction.Up;
				break;
			case Direction.Left:
				newGridX = newGridX - 1;
				newDirection = Direction.Right;
				break;
			}

			if (newGridX > grid.GetLength(0) - 1 || newGridX < 0 || newGridY > grid.GetLength(1) - 1 || newGridY < 0) {
				Debug.Log("Attempting to move off map");
				return false;
			}

			return (
				grid[gridX, gridY].GetComponent<TileManager>().passable[(int)direction] && 
				grid[newGridX, newGridY].GetComponent<TileManager>().passable[(int)newDirection]
			);
		}

		public Vector3 GetGridCoords(int gridX, int gridY) {
			return grid[gridX, gridY].transform.position;
		}
	}
}