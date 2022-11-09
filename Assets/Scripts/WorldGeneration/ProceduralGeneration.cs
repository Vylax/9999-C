using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using static CustomClasses;

public class ProceduralGeneration : MonoBehaviour
{
    float variationSpeed1 = 13.684329f;
    float variationSpeed2 = 6.6235236f;

    public int seedX;
    public int seedY;
    public bool setSeed;

    private List<Chunk> world = new List<Chunk>();

    private GameObject environmentObj;

    private void Start()
    {
        environmentObj = GameObject.FindGameObjectWithTag("Environment");
    }

    public void Init(bool setSeed = false, int seedX = 0, int seedY = 0)
    {
        this.setSeed = setSeed;
        if (setSeed)
        {
            GenerateWorld(seedX, seedY, 10);
        }
        else
        {
            GenerateWorld(10);
        }
    }

    public void GenerateWorld(int chunksCount = 1, bool load = true)
    {
        GenerateWorld(Random.Range(-10000, 10000), Random.Range(-10000, 10000), chunksCount, load);
    }

    public void GenerateWorld(int seedX, int seedY, int chunksCount = 1, bool load = true)
    {
        this.seedX = seedX;
        this.seedY = seedY;

        float temp = seedX + variationSpeed1 * world.Count + variationSpeed2 * GetCurrChunkType();
        //Debug.Log($"temp={temp}, pn={Mathf.PerlinNoise(temp, seedY)}, map={MapNoise(Mathf.PerlinNoise(temp, seedY))}");
        world.Add(new Chunk(world.Count == 0 ? 0 : MapNoise(Mathf.PerlinNoise(temp, seedY)), load));
        world[world.Count - 1].instance.transform.parent = environmentObj.transform;
        //set chunk instance gameobject to disabled here
        if (chunksCount > 1)
        {
            GenerateWorld(seedX, seedY, chunksCount-1, load);
        }
    }

    public void ResetWorld(bool sameSeed = false)
    {
        for (int i = 0; i < world.Count; i++)
        {
            Destroy(world[i].instance);
            world[i] = null;
        }
        world = new List<Chunk>();
        System.GC.Collect();
        currChunkId = -1;

        Init(sameSeed, this.seedX, this.seedY);
    }

    private int MapNoise(float value)
    {
        for (int i = 0; i < chunks.Length; i++)
        {
            if (value < (i + 1) * 1f / (chunks.Length))
            {
                return i;
            }
        }
        return chunks.Length - 1;
    }

    private int GetCurrChunkType()
    {
        return currChunkId > 0 ? world[currChunkId].type : 0;
    }

    public void UnloadChunkFromId(int id)
    {
        //Debug.Log($"Unload id:{id}");
        if (id < 0 || id >= world.Count)
            return;

        world[id].Unload();
    }

    public void LoadChunkFromId(int id)
    {
        //Debug.Log($"Load id:{id}");
        if (id < 0 || id >= world.Count)
            return;

        world[id].Load();
    }
}