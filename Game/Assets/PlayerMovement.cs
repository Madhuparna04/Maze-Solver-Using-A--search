using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;


 
 public class V2Comparer : IComparer<Vector2>
 {
     public int Compare(Vector2 left, Vector2 right)
     {
         var dif = left - right;
         if (dif.x == 0 && dif.y == 0)
             return 0;
         else if (dif.x == 0)
             return (int)Mathf.Sign(dif.y);
         return (int)Mathf.Sign(dif.x);
     }
 }
public class PlayerMovement : MonoBehaviour {


	// Use this for initialization
	public Rigidbody rb;
	int row=4;
	int col=4;
	public Vector2 start_cell;
	public Vector2 goal_cell;
	List<Vector2> obstacles = new List<Vector2>();
	List<string> moves = new List<string>();
	float fraction_of_way_there;
	float timeToGo;
	bool flag=true;
	Vector3 initial_pos;
	Vector3 start_pos;
	Vector3 end_pos;
	
	//Finds the cost to move to a cell b
	int move_cost(Vector2 b){
		
		foreach(Vector2 barrier in obstacles)
		{
			if(barrier==b)
			{	
				return 1000;
			}
		}
		return 1;
	}
	
	//Get the valid neighbouring cells of a cell in the maze.
	List<Vector2> get_neighbours(Vector2 node)
	{
		Vector2 up = new Vector2((int)node.x,(int)node.y+1);
		Vector2 down = new Vector2((int)node.x,(int)node.y-1);
		Vector2 left = new Vector2((int)node.x-1,(int)node.y);
		Vector2 right = new Vector2((int)node.x+1,(int)node.y);
		
		List<Vector2> list = new List<Vector2>();
		list.Add(up);
		list.Add(down);
		list.Add(left);
		list.Add(right);
		
		foreach(Vector2 n in list.ToList())
		{
			if(n.x<0 || n.x>=col || n.y<0 || n.y>=row)
				list.Remove(n);
			
		}
		return list;
		
	}
	//Heuristic function = Manhattan Distance
	int heuristic(Vector2 a, Vector2 b)
	{
		int dis = (int)((a.x - b.x)+(a.y-b.y));
	
		return Mathf.Abs(dis);
	}
	//Astar search algorithms
	List<Vector2> Astar(Vector2 start, Vector2 goal)
	{

		// Code Adapted from https://rosettacode.org/wiki/A*_search_algorithm#Python
		
	
		Dictionary<Vector2, int> G = new Dictionary<Vector2, int>(); 
		Dictionary<Vector2, int> F = new Dictionary<Vector2, int>();
		
		G[start]=0;
		F[start]=heuristic(start,goal);
		
		
		SortedSet<Vector2> closed = new SortedSet<Vector2>(new V2Comparer());
		SortedSet<Vector2> open = new SortedSet<Vector2>(new V2Comparer());

		open.Add(start);
		
		Dictionary<Vector2?, Vector2> came_from = new Dictionary<Vector2?, Vector2>();
		
		List<Vector2> path = new List<Vector2>();
		
		
		while( open.Count > 0)
		{
			
			Vector2? current = null;
			int current_fscore = 0;
			foreach (Vector2 pos in open)
			{
				
				if(current==null || F[pos]<current_fscore)
				{
					current_fscore = F[pos];
					current = pos;
				}
			}
			if(current == goal)
			{
				path.Add((Vector2)current);
				while(came_from.ContainsKey(current))
				{
					current = came_from[current];
					path.Add((Vector2)current);
				}
				path.Reverse();
				return path;
			}

			
			closed.Add((Vector2)current);
			open.Remove((Vector2)current);
	
			foreach(Vector2 neighbour in get_neighbours((Vector2)current))
			{
				if(closed.Contains(neighbour))
				{
					continue;
				}
				int candidateG = G[(Vector2)current]+move_cost(neighbour);
				
				if(!open.Contains(neighbour))
				{
					open.Add(neighbour);
				}
				else if(candidateG > G[neighbour])
					continue;
				
				came_from[neighbour] = (Vector2)current;
				G[neighbour] = candidateG;
				int H = heuristic(neighbour,goal);
				F[neighbour] = G[neighbour] + H;
			}
			
			
		}	
		return path;		
	}
	void CreateMoves(List<Vector2> path)
	{
		Vector2 prev = path[0];
		
		for (var i = 1; i < path.Count; i++)
		{
			
			if(path[i].x==prev.x)
			{
				if(path[i].y>prev.y)
				{
					moves.Add("right");
				}
				else{
					moves.Add("left");
				}
			}
			else{
				if(path[i].x>prev.x)
				{
					moves.Add("up");
				}
				else{
					moves.Add("down");
				}
			}
			prev = path[i];
		}
	}

	
	void Start () {
		
		//Get the details of Maze from Maze Creator Gameobject
		GameObject maze = GameObject.Find("Maze_Creator");
        InstantiationExample obj = maze.GetComponent<InstantiationExample>();
		obstacles = obj.obstacles;
		row = obj.rows;
		col = obj.cols;
		
		//Get the path using Astar
		List<Vector2> path = Astar(start_cell,goal_cell);
		
		//Get the required movements
		CreateMoves(path);
		

		//Setting the initial position to start cell
		transform.position = new Vector3(start_cell.y*6+2,0,start_cell.x*6+3);
		timeToGo = Time.fixedTime + 1.5f;
		
	}
	
	// Update is called once per frame
	void Update () {
		
		if (Time.fixedTime >= timeToGo)
			{

		if(flag){

			if (moves.Count==0)
			{
				flag=false;
			}
			foreach(var name in moves.ToList())
			{	start_pos = transform.position;

				if(name=="up")
				{
					fraction_of_way_there+=1f;
					transform.position = Vector3.Lerp(start_pos,start_pos+new Vector3(0,0,6),fraction_of_way_there);
					moves.RemoveAt(0);
					break;
					
					
				}
				else if(name=="down")
				{
					fraction_of_way_there+=1f;
					transform.position = Vector3.Lerp(start_pos,start_pos+new Vector3(0,0,-6),fraction_of_way_there);
					moves.RemoveAt(0);
					break;
				}
				else if(name=="right")
				{
					fraction_of_way_there+=1f;
					transform.position = Vector3.Lerp(start_pos,start_pos+new Vector3(6,0,0),fraction_of_way_there);
					moves.RemoveAt(0);
					break;
				}
				else if(name=="left")
				{
					fraction_of_way_there+=1f;
					transform.position = Vector3.Lerp(start_pos,start_pos+new Vector3(-6,0,0),fraction_of_way_there);
					moves.RemoveAt(0);
					break;
				}
			}
		}
		         timeToGo = Time.fixedTime + 1.0f;
     }

	}
}
