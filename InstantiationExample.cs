using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class InstantiationExample : MonoBehaviour 
{
    // Reference to the Prefab. Drag a Prefab into this field in the Inspector.
    public GameObject Wall_Horizontal;
	public GameObject Wall_Vertical;
	public GameObject obstacle;
	public Rigidbody Player;
	public int rows= 3;
	public int cols= 3;
	public List<Vector2> obstacles = new List<Vector2>();

	
    // This script will simply instantiate the Prefab when the game starts.
    void Start()
    {
		
		// Adjusting position of Main Camera
        if(rows>=cols)
			GameObject.Find("Main Camera").transform.position = new Vector3(cols*3+3,rows*6+25,rows*3+3);
		else{
			GameObject.Find("Main Camera").transform.position = new Vector3(cols*3+3,cols*6+25,rows*3+3);
		}
		
		
		//Creating the maze
		for(int i=0;i<=rows;++i)
		{
			for(int j=0;j<2*cols;++j)
			{
				if(j%2==0)
					Instantiate(Wall_Horizontal, new Vector3(6*(j/2),0, 6*i), Quaternion.identity);
				else
					Instantiate(Wall_Horizontal, new Vector3(6*(j/2)+4,0, 6*i), Quaternion.identity);
			}
        			
		}
		
		for(int i=0;i<=cols;++i)
		{
			for(int j=0;j<rows*2;++j)
			{
				if(j%2==0)
					Instantiate(Wall_Vertical, new Vector3(6*i-1,0, 6*(j/2)+1), Quaternion.identity);
				else
					Instantiate(Wall_Vertical, new Vector3(6*i-1,0, 6*(j/2)+5),Quaternion.identity);
			}
		}
		
		// Adding Obstacles to the maze
		foreach(Vector2 obs in obstacles)
		{
			Instantiate(obstacle, new Vector3(obs.y*6+2,0,obs.x*6+3), Quaternion.identity);
		}
		
		
		
		
    }
}