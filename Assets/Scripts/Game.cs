using UnityEngine;
using System.Collections;

public class Game : MonoBehaviour {

    public Texture2D[] pipeTextures;
    public Texture2D[] waterTextures;

    Level lvl;
    float startX;
    float startY;
    bool WaterRunning = false;

	void Start () {
        GameObject.Find("background").AddComponent<StartWater>();

        if (pipeTextures.Length > 3)
        {
            PipeSection[] pipeTypes = new PipeSection[] {
                new PipeSection(pipeTextures[0],waterTextures[0],new int[4] {1,0,1,0}), 
                new PipeSection(pipeTextures[1],waterTextures[1],new int[4] {0,0,1,1}),  
                new PipeSection(pipeTextures[3],waterTextures[3],new int[4] {1,0,1,1}),  
                new PipeSection(pipeTextures[2],waterTextures[2],new int[4] {1,1,1,1})
            };

            lvl = new Level(pipeTypes);

            startX = 0 - ((lvl.grid.GetLength(0) * pipeTextures[0].width) / 2);
            startY = 0 - ((lvl.grid.GetLength(1) * pipeTextures[0].height) / 2);

            for (int x = 0; x < lvl.grid.GetLength(0); x++)
            {
                for (int y = 0; y < lvl.grid.GetLength(1); y++)
                {
                    GameObject g = new GameObject(x + "," + y);
                    g.AddComponent<SpriteRenderer>();
                    g.AddComponent<BoxCollider2D>();
                    g.AddComponent<PipeClick>();
                    g.GetComponent<SpriteRenderer>().sprite = lvl.grid[x, y].pipeSprite;
                    g.transform.position = new Vector3((startX + (x * pipeTextures[0].width)) / 100, (startY + (y * pipeTextures[0].height)) / 100);
                    g.transform.rotation = Quaternion.Euler(new Vector3(0, 0, lvl.grid[x, y].rotation));
                }
            }
        }
	}

    public void onPipeClick(int x, int y)
    {
        if(!WaterRunning)
            lvl.grid[x, y].rotate();
    }

    public void StartWaterTest() {
        WaterRunning = true;
        clearWater();
        if (lvl.grid[0, 4].getExits()[1] == 1)
        {
            waterFlowDown.Add(lvl.grid[0, 4]);
        }
        else
        {
            WaterRunning = false;
        }
    }


    int waterFlowTick = 0;
    ArrayList waterFlowDown = new ArrayList();
    ArrayList waterFlowSide = new ArrayList();
    ArrayList waterFlowUp = new ArrayList();
    
	void Update () {
        if (WaterRunning && System.Environment.TickCount - waterFlowTick > 500)
        {
            ArrayList waterFlow = (ArrayList) waterFlowDown.Clone();
            waterFlowTick = System.Environment.TickCount;
            waterFlowDown.Clear();
            for (int i = 0; i < waterFlow.Count; i++)
            {
                Pipe p = (Pipe)waterFlow[i];
                addWaterTitle((int)p.position.x, (int)p.position.y, (int)p.rotation, p.type.waterTex);

                int[] exits = p.getExits();

                if (exits[0] == 1)
                {
                    Pipe pipe = tryGetPipe((int)p.position.x - 1, (int)p.position.y);
                    if (pipe != null && pipe.getExits()[2] == 1)
                        waterFlowSide.Add(pipe);
                }
                if (exits[1] == 1)
                {
                    Pipe pipe = tryGetPipe((int)p.position.x, (int)p.position.y + 1);
                    if (pipe != null && pipe.getExits()[3] == 1)
                        waterFlowUp.Add(pipe);
                }
                if (exits[2] == 1)
                {
                    Pipe pipe = tryGetPipe((int)p.position.x + 1, (int)p.position.y);
                    if (pipe != null && pipe.getExits()[0] == 1)
                        waterFlowSide.Add(pipe);
                }
                if (exits[3] == 1)
                {
                    Pipe pipe = tryGetPipe((int)p.position.x, (int)p.position.y - 1);
                    if (pipe != null && pipe.getExits()[1] == 1)
                        waterFlowDown.Add(pipe);
                }
            }

            if (waterFlowDown.Count == 0)
            {
                if (waterFlowSide.Count != 0)
                {
                    waterFlowDown = (ArrayList)waterFlowSide.Clone();
                    waterFlowSide.Clear();
                }
                else if (waterFlowUp.Count != 0)
                {
                    waterFlowDown = (ArrayList)waterFlowUp.Clone();
                    waterFlowUp.Clear();
                }
                else
                {
                    WaterRunning = false;
                }
            }
        }
	}

    public Pipe tryGetPipe(int x,int y) {
        if (GameObject.Find(x + "," + y) != null && GameObject.Find("w" + x + "," + y) == null) 
            return lvl.grid[x,y];
        return null;
    }

    public void clearWater()
    {
        SpriteRenderer[] l = GameObject.FindObjectsOfType<SpriteRenderer>();
        for (int i = 0; i < l.Length; i++)
            if (l[i].gameObject.name.StartsWith("w"))
                GameObject.DestroyImmediate(l[i].gameObject);
    }

    public void addWaterTitle(int x, int y,int rotation,Texture2D tex)
    {                                                                 
        GameObject g = new GameObject("w" + x + "," + y);
        g.AddComponent<SpriteRenderer>();
        g.GetComponent<SpriteRenderer>().sprite = Sprite.Create(tex,new Rect(0,0,tex.width,tex.height),new Vector2(0.5f,0.5f));
        g.transform.position = new Vector3((startX + (x * tex.width)) / 100, (startY + (y * tex.height)) / 100,2);
        Debug.LogError(("w" + x + "," + y + " - rot: " + rotation));
        g.transform.rotation = Quaternion.Euler(new Vector3(0,0,rotation));
    }
}

public class StartWater : MonoBehaviour
{
    bool mouseDown = false;
    public void OnMouseUp()
    {
        if(mouseDown)
            GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Game>().StartWaterTest();
    }

    public void OnMouseDown()
    {
        mouseDown = true;
    }
}
