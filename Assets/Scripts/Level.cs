using UnityEngine;
using System.Collections;

public class Level {


    public Pipe[,] grid;

    public Level(PipeSection[] types)
    {
        this.grid = new Pipe[6, 5];

        for (int x = 0; x < grid.GetLength(0); x++)
        {
            for (int y = 0; y < grid.GetLength(1); y++)
            {
                if (x == 0 || x == grid.GetLength(0) - 1)
                    grid[x, y] = new Pipe(new Vector2(x, y), types[Random.Range(0, types.Length - 1)]);
                else
                    grid[x, y] = new Pipe(new Vector2(x, y), types[Random.Range(0, types.Length)]);
            }
        }

    }

    public bool isLevelComplete()
    {

        return false;
    }
}

public class Pipe
{
    public PipeSection type;
    public int rotation = 0;
    public Vector2 position;
    public Sprite pipeSprite;
    public bool rotatable;

    public Pipe(Vector2 position, PipeSection type) : this(position, type, true)
    {
    }

    public Pipe(Vector2 position, PipeSection type, bool rotatable)
    {
        this.rotatable = rotatable;
        this.position = position;
        this.type = type;
        pipeSprite = Sprite.Create(type.image, new Rect(0, 0, type.image.width, type.image.height), new Vector2(0.5f, 0.5f));
    }

    public void rotate()
    {
        rotation += 90;
        if (rotation > 270)
            rotation = 0;

        GameObject g = GameObject.Find(position.x + "," + position.y);
        Debug.LogError(rotation);
        g.transform.rotation = Quaternion.Euler(new Vector3(0,0,rotation));
        rotation = (int)g.transform.rotation.eulerAngles.z;
    }

    public int[] getExits()
    {
        return type.getPipeExits((int)rotation);
    }
}

public class PipeClick : MonoBehaviour
{
    bool mouseDown = false;

    public void OnMouseUp()
    {
        if (mouseDown)
        {
            string[] splitA = gameObject.name.Split(',');
            int x = int.Parse(splitA[0]);
            int y = int.Parse(splitA[1]);
            GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Game>().onPipeClick(x, y);
        }
    }

    public void OnMouseDown()
    {
        mouseDown = true;
    }
}

public class PipeSection
{
    public Texture2D image;
    public Texture2D waterTex;
    int[] exits;

    public PipeSection(Texture2D image,Texture2D waterTex,int [] exits)
    {
        this.waterTex = waterTex;
        this.image = image;
        this.exits = exits;
    }

    public int[] getPipeExits(int rotation)
    {
        switch (rotation)
        {
            case 0:
                return exits;
            case 90:
                return new int[4] { exits[1], exits[2], exits[3], exits[0] };
            case 180:
                return new int[4] { exits[2], exits[3], exits[0], exits[1] };
            case 270:
                return new int[4] { exits[3], exits[0], exits[1], exits[2] };
        }
        return exits;
    }
}
