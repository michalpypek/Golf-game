using System.Collections;
using UnityEngine;
using System.Collections.Generic;

public class BoardCreator : MonoBehaviour
{
    // The type of tile that will be laid in a specific position.
    public enum TileType
    {
        outOfBounds, fairway, rough, sand, water, tree
    }

    public int sandChance;                                  // Chance of getting sand bunkers (in %)
    public int waterChance;                                 // Chance of getting water (in %)
    public int riverChance;
    public int treesChance;
    public int maxSplit = 2;
    public int maxTrees = 1;
    public int maxSand = 1;
    public int maxWater = 1;
    public int holeLength;
    public float scale;
    public float amplitude;
    public float heightScale;
    public float heightAmplitude;

    public int columns = 50;                                 // The number of columns on the board (how wide it will be).
    public int rows = 100;                                    // The number of rows on the board (how tall it will be).
    public IntRange numBlocks = new IntRange(15, 20);         // The range of the number of blocks there can be.
    public IntRange blockWidth = new IntRange(3, 10);         // The range of widths blocks can have.
    public IntRange blockHeight = new IntRange(3, 10);        // The range of heights blocks can have.
    public IntRange sandHeight = new IntRange(0, 4);          // The range of height sand arrays can have;
    public IntRange sandWidth = new IntRange(0, 4);           // The range of width sand arrays can have;
    public IntRange treeHeight = new IntRange(0, 4);          // The range of height sand arrays can have;
    public IntRange treeWidth = new IntRange(0, 4);           // The range of width sand arrays can have;
    public IntRange waterHeight = new IntRange(0, 4);          // The range of height water arrays can have;
    public IntRange waterWidth = new IntRange(0, 4);           // The range of width water arrays can have;

    public GameObject fairwayTile;
    public GameObject roughTile;
    public GameObject sandTile;
    public GameObject waterTile;
    public GameObject treeTile;
    public GameObject ball;
    public GameObject hole;
    public GameObject treeGroup;

    private List<GameObject> inBoundTiles;
    private List<GameObject> roughTiles;
    private List<GameObject> sandTiles;
    private List<GameObject> waterTiles;
    private List<GameObject> treeTiles;

    private TileType[][] tiles;                               // A jagged array of tile types representing the board, like a grid.
    private float[][] heightMap;                                // A jagged array with height values for each tile.
    public Block[] blocks;                                     // All the rooms that are created for this board.
    private Block[] roughBlocks;                                // Copy of blocks array to generate "Rough" grass.
    private GameObject boardHolder;                           // GameObject that acts as a container for all other tiles.


    private void Start()
    {
        // Create the board holder.
        boardHolder = new GameObject("BoardHolder");
        CreateFairwayList();
        CreateRoughList();
        CreateSandList();
        CreateWaterList();
        CreateTreeList();

        Reset();

        //Debug.Log("Pierwszy: " + blocks[0].xPos + " drugi: " + roughBlocks[0].xPos);
    }
    #region ListsFunctions

    void CreateRoughList()
    {
        roughTiles = new List<GameObject>();
        for (int i = 0; i < columns * rows; i++)
        {
            GameObject rough = Instantiate(roughTile, Vector3.zero, Quaternion.identity) as GameObject;
            rough.SetActive(false);
            rough.transform.SetParent(boardHolder.transform);
            roughTiles.Add(rough);
        }
    }

    void CreateSandList()
    {
        sandTiles = new List<GameObject>();
        for (int i = 0; i < numBlocks.m_Max * (sandHeight.m_Max * sandWidth.m_Max); i++)
        {
            GameObject sand = Instantiate(sandTile, Vector3.zero, Quaternion.identity) as GameObject;
            sand.SetActive(false);
            sand.transform.SetParent(boardHolder.transform);
            sandTiles.Add(sand);
        }
    }

    void CreateWaterList()
    {
        waterTiles = new List<GameObject>();
        for (int i = 0; i < columns * rows; i++)
        {
            GameObject water = Instantiate(waterTile, Vector3.zero, Quaternion.identity) as GameObject;
            water.SetActive(false);
            water.transform.SetParent(boardHolder.transform);
            waterTiles.Add(water);
        }
    }

    void CreateTreeList()
    {
        treeTiles = new List<GameObject>();
        for (int i = 0; i < columns * rows; i++)
        {
            GameObject tree = Instantiate(treeTile, Vector3.zero, Quaternion.identity) as GameObject;
            tree.SetActive(false);
            tree.transform.SetParent(boardHolder.transform);
            waterTiles.Add(tree);
        }
    }

    void CreateFairwayList()
    {
        inBoundTiles = new List<GameObject>();
        for (int i = 0; i < columns * rows; i++)
        {
            GameObject tile = Instantiate(fairwayTile, Vector3.zero, Quaternion.identity) as GameObject;
            tile.SetActive(false);
            tile.transform.SetParent(boardHolder.transform);
            inBoundTiles.Add(tile);
        }
    }

    GameObject GetFairway()
    {
        foreach (GameObject grass in inBoundTiles)
        {
            if (!grass.activeSelf)
            {
                return grass;
            }
        }

        return null;
    }

    GameObject GetRough()
    {
        foreach (GameObject rough in roughTiles)
        {
            if (!rough.activeSelf)
            {
                return rough;
            }
        }

        return null;
    }

    GameObject GetSand()
    {
        foreach (GameObject csand in sandTiles)
        {
            if (!csand.activeSelf)
            {
                return csand;
            }
        }

        GameObject sand = Instantiate(sandTile, Vector3.zero, Quaternion.identity) as GameObject;
        sand.SetActive(false);
        sand.transform.SetParent(boardHolder.transform);
        sandTiles.Add(sand);

        return sand;
    }

    GameObject GetWater()
    {
        foreach (GameObject cwater in waterTiles)
        {
            if (!cwater.activeSelf)
            {
                return cwater;
            }
        }

        GameObject water = Instantiate(waterTile, Vector3.zero, Quaternion.identity) as GameObject;
        water.SetActive(false);
        water.transform.SetParent(boardHolder.transform);
        waterTiles.Add(water);

        return water;
    }

    GameObject GetTree()
    {
        foreach (GameObject ctree in treeTiles)
        {
            if (!ctree.activeSelf)
            {
                return ctree;
            }
        }

        GameObject tree = Instantiate(treeTile, Vector3.zero, Quaternion.identity) as GameObject;
        tree.SetActive(false);
        tree.transform.SetParent(boardHolder.transform);
        waterTiles.Add(tree);

        return tree;
    }

    void ResetFairway()
    {
        foreach (GameObject grass in inBoundTiles)
        {
            grass.SetActive(false);
        }
    }

    void ResetRough()
    {
        foreach (GameObject grass in roughTiles)
        {
            grass.SetActive(false);
        }
    }

    void ResetSand()
    {
        foreach (GameObject sand in sandTiles)
        {
            sand.SetActive(false);
        }
    }

    void ResetWater()
    {
        foreach (GameObject water in waterTiles)
        {
            water.SetActive(false);
        }
    }

    void ResetTree()
    {
        foreach (GameObject tree in treeTiles)
        {
            tree.SetActive(false);
        }
    }


    #endregion

    public void Reset()
    {
        //Deactivate all tiles in lists
        ResetFairway();
        ResetRough();
        ResetSand();
        ResetWater();
        ResetTree();
        treeGroup.SetActive(false);

        SetupTilesArray();
        CreateHeightMap();

        // Create Shape of Fairway and Rough

        SplitFairway();
        CreateRoughBlocks();

        //Set Tile values for fairway  
              
        SetTilesValuesForBlocks(blocks, TileType.fairway);

        //Hole shape is done, Spawn hazards

        SpawnSandBunkers();
        SpawnWaterPonds();
        SpawnRiver();
        SpawnTrees();

        //Setup fairway around the hole and the ball, so it's possible to win

        SetupGreen();
        SetupTee();

        //TestPerlin();

        //Activate correct tiles in their positions

        InstantiateTiles();
        CalculateHoleLength();
        Debug.Log(holeLength);
    }


    void SetupTilesArray()
    {
        // Set the tiles jagged array to the correct width.
        tiles = new TileType[columns][];
        heightMap = new float[columns][];

        // Go through all the tile arrays...
        for (int i = 0; i < tiles.Length; i++)
        {
            // ... and set each tile array is the correct height.
            tiles[i] = new TileType[rows];
            heightMap[i] = new float[rows];
        }
    }

    void CreateHeightMap()
    {
        for (int i = 0; i < heightMap.Length; i++)
        {
            for (int j = 0; j < heightMap[i].Length; j++)
            {
                float sample = Mathf.Clamp01(Mathf.PerlinNoise(Time.time + i / heightScale, Time.time + j / heightScale) * heightAmplitude);
                heightMap[i][j] = sample;
            }
        }
    }

    void ArrayGoInCircles()
    {
        int minX = 0;
        int maxX = 5;
        int minY = 0;
        int maxY = 5;

        for (int i = minX; i < maxX; i++)
        {
            for (int j = minY; j < maxY; j++)
            {
                if(i == minX || i==maxX || j==maxY)
                {
                    //Set the height
                }
            }
        }
    }

    void CreateBlocks()
    {
        // Create the blocks array with a random size.
        blocks = new Block[numBlocks.Random];

        // Create the first block
        blocks[0] = new Block();

        // Setup the first block, there is no previous corridor so we do not use one.
        blocks[0].SetupBlock(blockWidth, blockHeight, columns, rows);

        //spawn ball in the first block
        blocks[0].SpawnBall(ball);

        for (int i = 1; i < blocks.Length; i++)
        {
            // Create a room.
            blocks[i] = new Block();

            // Setup the room based on the previous block
            blocks[i].SetupBlock(blockWidth, blockHeight, columns, rows, blocks[i - 1]);

            if (i == blocks.Length - 1)
            {
                blocks[i].SpawnHole(hole);
            }
        }
    }

    /// <summary>
    /// Create the blocks and split the fairway
    /// </summary>
    void SplitFairway()
    {
        int splitCount = 0;

        do
        {
            CreateBlocks();
            splitCount = 0;
            for (int i = 0; i < blocks.Length; i++)
            {
                Block currentBlock = blocks[i];

                if (!blocks[i].IsHeightAndWidthOk())
                {
                    blocks[i - 1].ReduceHeight();
                    splitCount++;
                }
            }
        }
        while (splitCount > maxSplit);
    }

    /// <summary>
    /// Create a copy of fairway blocks with added height and width
    /// </summary>
    void CreateRoughBlocks()
    {
        roughBlocks = new Block[blocks.Length];

        for (int i = 0; i < roughBlocks.Length; i++)
        {
            roughBlocks[i] = blocks[i].Clone();
        }

        foreach (Block currentBlock in roughBlocks)
        {
            int temp = currentBlock.width;
            int temph = currentBlock.height;
            currentBlock.width = (int)(currentBlock.width * 2.5f);
            currentBlock.height = (int)(currentBlock.height * 1.5f);

            currentBlock.xPos -= (currentBlock.width - temp) / 2;
            currentBlock.yPos -= (currentBlock.height - temph) / 2;
        }

        SetTilesValuesForBlocks(roughBlocks, TileType.rough);
        //CutUpWithPerlinNoise(roughBlocks, TileType.rough);

        for (int i = 0; i < roughBlocks.Length; i++)
        {
            //int xPos = roughBlocks[i].xPos;
            //int yPos = roughBlocks[i].yPos;
            float heightMod = 0.0f;

            for (int xPos = roughBlocks[i].xPos; xPos < roughBlocks[i].width + roughBlocks[i].xPos; xPos++)
            {
                for (int yPos = roughBlocks[i].yPos; yPos < roughBlocks[i].height + roughBlocks[i].yPos; yPos++)
                {
                    heightMap[xPos][yPos] = Random.Range(heightMod - 0.1f, heightMod);
                }

                if( xPos < (roughBlocks[i].xPos + roughBlocks[i].width)/2)
                {
                    heightMod += 0.05f;
                    Debug.Log("adda");
                }
                else
                {
                    heightMod -= 0.05f;
                    Debug.Log("subtra");
                }
            }
        }
    }

    #region SettingTileValuesForBlocks
    void SetTilesValuesForBlocks(Block[] blockArray, TileType tileType)
    {
        for (int i = 0; i < blockArray.Length; i++)
        {
            Block currentBlock = blockArray[i];
            //SetTilesForPerlinBlock(currentBlock);

            switch (currentBlock.shape)
            {
                case Block.Shape.Square:
                    SetTilesForSquareBlock(currentBlock, tileType);
                    break;
                case Block.Shape.LeftTurn:
                    SetTilesForLeftTurnBlock(currentBlock, tileType);
                    break;
                case Block.Shape.RightTurn:
                    SetTilesForRightTurnBlock(currentBlock, tileType);
                    break;
                case Block.Shape.NarrowingUp:
                    SetTilesForNarrowingUpBlock(currentBlock, tileType);
                    break;
                case Block.Shape.NarrowingDown:
                    SetTilesForNarrowingDownBlock(currentBlock, tileType);
                    break;
                default:
                    SetTilesForSquareBlock(currentBlock, tileType);
                    break;
            }
        }
    }

    void CutUpWithPerlinNoise(Block[] blockArray, TileType tileType)
    {
        for (int i = 0; i < blockArray.Length; i++)
        {
            Block currentBlock = blockArray[i];
            SetTilesForPerlinBlock(currentBlock, tileType);
        }
    }

    void SetTilesForPerlinBlock(Block currentBlock, TileType tileType)
    {
        for (int i = 0; i < currentBlock.width; i++)
        {
            int xCoord = currentBlock.xPos + i;

            for (int k = 0; k < currentBlock.height; k++)
            {
                int yCoord = currentBlock.yPos + k;

                float sample = Mathf.Clamp01(Mathf.PerlinNoise(Time.time + xCoord / scale, Time.time + yCoord / scale) * amplitude);

                if (sample <= 0.3f)
                {
                    tiles[xCoord][yCoord] = TileType.water;
                }

                else if (tiles[xCoord][yCoord] != TileType.outOfBounds)
                {
                    tiles[xCoord][yCoord] = tileType;
                }

            }
        }

    }

    void SetTilesForSquareBlock(Block currentBlock, TileType tileType)
    {
        for (int i = 0; i < currentBlock.width; i++)
        {
            int xCoord = currentBlock.xPos + i;

            // For each horizontal tile, go up vertically through the room's height.
            for (int k = 0; k < currentBlock.height; k++)
            {
                int yCoord = currentBlock.yPos + k;

                // The coordinates in the jagged array are based on the room's position and it's width and height.
                tiles[xCoord][yCoord] = tileType;
            }
        }

    }

    void SetTilesForLeftTurnBlock(Block currentBlock, TileType tileType)
    {
        int offset = 0;

        for (int k = currentBlock.height; k >= 0; k--)
        {
            int yCoord = currentBlock.yPos + k;

            for (int i = 0 + offset; i < currentBlock.width + offset; i++)
            {
                int xCoord = currentBlock.xPos + i;
                tiles[xCoord][yCoord] = tileType;
            }
            if (k % 2 == 1)
            {
                offset++;
            }
        }
    }

    void SetTilesForRightTurnBlock(Block currentBlock, TileType tileType)
    {
        int offset = 0;

        for (int k = 0; k < currentBlock.height; k++)
        {
            int yCoord = currentBlock.yPos + k;

            for (int i = 0 + offset; i < currentBlock.width + offset; i++)
            {
                int xCoord = currentBlock.xPos + i;
                tiles[xCoord][yCoord] = tileType;
            }

            if (k % 2 == 1)
            {
                offset++;
            }
        }
    }

    void SetTilesForNarrowingUpBlock(Block currentBlock, TileType tileType)
    {
        int offset = 0;

        for (int k = 0; k < currentBlock.height; k++)
        {
            int yCoord = currentBlock.yPos + k;

            for (int i = 0 + offset; i < currentBlock.width - offset; i++)
            {
                int xCoord = currentBlock.xPos + i;
                //Debug.Log("x:" + xCoord + "  y : " + yCoord);

                tiles[xCoord][yCoord] = tileType;
            }
            if (k % 3 == 1)
            {
                offset++;
            }
        }
    }

    void SetTilesForNarrowingDownBlock(Block currentBlock, TileType tileType)
    {
        int offset = 0;

        for (int k = currentBlock.height; k >= 0; k--)
        {
            int yCoord = currentBlock.yPos + k;

            for (int i = 0 + offset; i < currentBlock.width - offset; i++)
            {
                int xCoord = currentBlock.xPos + i;
                //Debug.Log("x:" + xCoord + "  y : " + yCoord);

                tiles[xCoord][yCoord] = tileType;
            }

            if (k % 3 == 1)
            {
                offset++;
            }
        }
    }
    #endregion

    #region SpawningHazards

    void SpawnSandBunkers()
    {
        int sandCounter = 0;
        for (int i=roughBlocks.Length/2; i<roughBlocks.Length && sandCounter < maxSand; i++)
        {
            Block currentBlock = roughBlocks[i];
            if (RandomChance(sandChance))             // TO DO check whether to spawn or not here
            {
                sandCounter++;

                int sandXPos = Random.Range(currentBlock.xPos, currentBlock.xPos + currentBlock.width - sandWidth.m_Max);
                int sandYPos = Random.Range(currentBlock.yPos, currentBlock.yPos + currentBlock.height - sandHeight.m_Max);

                int[] sand = new int[sandHeight.Random];
                for (int j = 0; j < sand.Length; j++)
                {
                    sand[j] = sandWidth.Random;
                }

                //for (int j = 0; j < sand.Length; j++)
                //{
                //    int xCoord = sandXPos + j;

                //    for (int k = 0; k < sand[j]; k++)
                //    {
                //        int yCoord = sandYPos + k;

                //        if (tiles[xCoord][yCoord] != TileType.outOfBounds)
                //        {
                //            tiles[xCoord][yCoord] = TileType.sand;
                //        }
                //    }
                //}

                for (int j = 0; j < sand.Length; j++)
                {
                    int xCoord = sandXPos + j;

                    for (int k = 0; k < sand[j]; k++)
                    {
                        int yCoord = sandYPos + k;

                        float sample = Mathf.Clamp01(Mathf.PerlinNoise(Time.time + xCoord / scale, Time.time + yCoord / scale) * amplitude);

                        if (sample > 0.5f)
                        {
                            if (tiles[xCoord][yCoord] != TileType.outOfBounds)
                            {
                                tiles[xCoord][yCoord] = TileType.sand;
                            }
                        }
                    }
                }

                // go through the whole sand array, changing the height of the tiles. It changes the color of the borders (like a frame/bounding box) and keeps narrowing down untill it hits the middle
                int minX = 0;
                int maxX = sand.Length;
                int minY = 0;
                int maxY = sand.Length;
                float heightMod = 1f;

                while (minX <= maxX)
                {
                    for (int x = minX; x < maxX; x++)
                    {
                        int xCoord = sandXPos + x;

                        for (int y = minY; y < maxY; y++)
                        {
                            int yCoord = sandYPos + y;

                            if (x == minX || x == maxX-1 || y == maxY-1 || y == minY)
                            {
                                //Set the height
                                if (tiles[xCoord][yCoord] == TileType.sand)
                                {
                                    heightMap[xCoord][yCoord] = Random.Range(heightMod - 0.1f, heightMod);
                                }
                            }
                        }
                    }

                    heightMod -= 0.1f;

                    minX++;
                    maxX--;
                    minY++;
                    maxY--;
                }
            }

        }
    }

    void SpawnRiver()
    {
        int block = Random.Range(1, blocks.Length);
        int offset = 0;
        bool add = true;

        if (RandomChance(riverChance))
        {
            //river.transform.position = new Vector3(-18, blocks[block].yPos, 0.25f);
            //river.SetActive(true);

            for (int i = 0; i < columns; i++)
            {
                int xCoord = i;

                if(i % 3 == 0)
                {
                    add = !add;
                }

                for (int k = 0; k < 4; k++)
                {
                    int yCoord = blocks[block].yPos + k + offset;
                    tiles[xCoord][yCoord] = TileType.water;
                }

                if(add)
                {
                    offset++;
                }

                else
                {
                    offset--;
                }
            }
        }

    }

    void SpawnWaterPonds()
    {
        int waterCounter = 0;

        for (int z= 0; z < roughBlocks.Length && waterCounter < maxWater; z++)
        {
            Block currentBlock = roughBlocks[z];

            if (RandomChance(waterChance))
            {
                waterCounter++;

                int waterXPos = Random.Range(currentBlock.xPos, currentBlock.xPos + currentBlock.width - waterWidth.m_Max);
                int waterYPos = Random.Range(currentBlock.yPos, currentBlock.yPos + currentBlock.height - waterHeight.m_Max);

                int[] water = new int[waterHeight.Random];
                for (int i = 0; i < water.Length; i++)
                {
                    water[i] = waterWidth.Random;
                }

                for (int i = 0; i < water.Length; i++)
                {
                    int xCoord = waterXPos + i;

                    for (int k = 0; k < water[i]; k++)
                    {
                        int yCoord = waterYPos + k;

                        float sample = Mathf.Clamp01(Mathf.PerlinNoise(Time.time + xCoord / scale, Time.time + yCoord / scale) * amplitude);

                        if (sample<0.5f)
                        {
                            tiles[xCoord][yCoord] = TileType.water;
                        }
                    }
                }

                int minX = 0;
                int maxX = water.Length;
                int minY = 0;
                int maxY = water.Length;
                float heightMod = 1f;

                while (minX <= maxX)
                {
                    for (int x = minX; x < maxX; x++)
                    {
                        int xCoord = waterXPos + x;

                        for (int y = minY; y < maxY; y++)
                        {
                            int yCoord = waterYPos + y;

                            if (x == minX || x == maxX - 1 || y == maxY - 1 || y == minY)
                            {
                                //Set the height
                                if (tiles[xCoord][yCoord] == TileType.water)
                                {
                                    heightMap[xCoord][yCoord] = Random.Range(heightMod - 0.1f, heightMod);
                                }
                            }
                        }
                    }

                    heightMod -= 0.1f;

                    minX++;
                    maxX--;
                    minY++;
                    maxY--;
                }

            }

        }
    }

    void SpawnTrees()
    {
        int treeCounter = 0;

        for (int j = roughBlocks.Length / 2; j < roughBlocks.Length && treeCounter < maxTrees; j++)
        {
            Block currentBlock = roughBlocks[j];

            if (RandomChance(treesChance))
            {
                treeCounter++;

                int treeXPos = Random.Range(currentBlock.xPos, currentBlock.xPos + currentBlock.width - treeWidth.m_Max);
                int treeYPos = Random.Range(currentBlock.yPos, currentBlock.yPos + currentBlock.height - treeHeight.m_Max);

                int[] tree = new int[treeHeight.Random];
                for (int i = 0; i < tree.Length; i++)
                {
                    tree[i] = waterWidth.Random;
                }

                for (int i = 0; i < tree.Length; i++)
                {
                    int xCoord = treeXPos + i;

                    for (int k = 0; k < tree[i]; k++)
                    {
                        int yCoord = treeYPos + k;

                        float sample = Mathf.Clamp01(Mathf.PerlinNoise(Time.time + xCoord / scale, Time.time + yCoord / scale) * amplitude);

                        if (sample < 0.5f && tiles[xCoord][yCoord]== TileType.rough || tiles[xCoord][yCoord] == TileType.fairway)
                        {
                            tiles[xCoord][yCoord] = TileType.tree;
                        }
                    }
                }

                //treeGroup.transform.position = new Vector3(Random.Range(blocks[block].xPos, blocks[block].xPos + blocks[block].width), Random.Range(blocks[block].yPos, blocks[block].yPos + blocks[block].height), 0);
                //treeGroup.SetActive(true);
            }
        }
    }

    #endregion
    // Set area around the hole to fairway
    void SetupGreen()
    {
        for (int i = (int)hole.transform.position.x - 3; i < hole.transform.position.x + 3; i++)
        {
            int xCoord = i;

            // For each horizontal tile, go up vertically through the room's height.
            for (int k = (int)hole.transform.position.y - 3; k < hole.transform.position.y + 3; k++)
            {
                int yCoord = k;

                // The coordinates in the jagged array are based on the room's position and it's width and height.
                tiles[xCoord][yCoord] = TileType.fairway;
            }
        }
    }

    // Set area around the ball starting position to fairway
    void SetupTee()
    {
        for (int i = (int)ball.transform.position.x - 3; i < ball.transform.position.x + 3; i++)
        {
            int xCoord = i;

            // For each horizontal tile, go up vertically through the room's height.
            for (int k = (int)ball.transform.position.y - 3; k < ball.transform.position.y + 3; k++)
            {
                int yCoord = k;

                // The coordinates in the jagged array are based on the room's position and it's width and height.
                tiles[xCoord][yCoord] = TileType.fairway;
            }
        }
    }

    void InstantiateTiles()
    {
        // Go through all the tiles in the jagged array...
        for (int i = 0; i < tiles.Length; i++)
        {
            for (int j = 0; j < tiles[i].Length; j++)
            {
                // ... and instantiate correct tile for it
                if (tiles[i][j] == TileType.fairway)
                {
                    GameObject grass = GetFairway();
                    grass.transform.position = new Vector3(i, j, 0);
                    grass.SetActive(true);
                    grass.GetComponent<TileScript>().SetColorFromHeight(heightMap[i][j]);
                }

                if (tiles[i][j] == TileType.rough)
                {
                    GameObject rough = GetRough();
                    rough.transform.position = new Vector3(i, j, 0);
                    rough.SetActive(true);
                    rough.GetComponent<TileScript>().SetColorFromHeight(heightMap[i][j]);
                }

                if (tiles[i][j] == TileType.sand)
                {
                    GameObject sand = GetSand();
                    sand.transform.position = new Vector3(i, j, 0);
                    sand.SetActive(true);
                    sand.GetComponent<TileScript>().SetColorFromHeight(heightMap[i][j]);
                    //GameObject sand = (GameObject)Instantiate(sandTile, new Vector3(i, j, 0), Quaternion.identity);
                }

                if (tiles[i][j] == TileType.water)
                {
                    GameObject water = GetWater();
                    water.transform.position = new Vector3(i, j, 0);
                    water.SetActive(true);
                    water.GetComponent<TileScript>().SetColorFromHeight(heightMap[i][j]);
                    //GameObject sand = (GameObject)Instantiate(sandTile, new Vector3(i, j, 0), Quaternion.identity);
                }

                if (tiles[i][j] == TileType.tree)
                {
                    GameObject tree = GetTree();
                    tree.transform.position = new Vector3(i, j, 0);
                    tree.SetActive(true);
                    //GameObject sand = (GameObject)Instantiate(sandTile, new Vector3(i, j, 0), Quaternion.identity);
                }
            }
        }
    }

    bool RandomChance(int percentage)
    {
        return Random.Range(0, 100) < percentage;
    }

    public TileType GetTileTypeFromPosition(Vector2 position)
    {
        return tiles[Mathf.FloorToInt(position.x)][Mathf.FloorToInt(position.y)];
    }

    public bool IsFairway(Vector2 position)
    {
        return (tiles[Mathf.FloorToInt(position.x)][Mathf.FloorToInt(position.y)] == TileType.fairway);
    }

    public bool IsOutOfBounds(Vector2 position)
    {
        return (tiles[Mathf.FloorToInt(position.x)][Mathf.FloorToInt(position.y)] == TileType.outOfBounds);
    }

    public void CalculateHoleLength()
    {
        int length = 0;

        for (int i = 0; i<blocks.Length; i++)
        {
            length += blocks[i].height;
        }

        length /= (int) GameManager.instance.inputManager.GetNormalClubRange();

        holeLength = length;

        GameManager.instance.inputManager.SetPar(blocks.Length);
    }

    void TestPerlin()
    {
        for (int i = 0; i < tiles.Length; i++)
        {
            for (int j = 0; j < tiles[i].Length; j++)
            {
                float sample = Mathf.Clamp01 (Mathf.PerlinNoise(Time.time + i / scale, Time.time + j / scale) * amplitude);

                if (sample <= 0.3f)
                {
                    tiles[i][j] = TileType.outOfBounds;
                }

                else if (sample < 0.6f && sample > 0.3f)
                {
                    tiles[i][j] = TileType.rough;
                }

                else
                {
                    tiles[i][j] = TileType.fairway;
                }

                //if (sample <= 0.5f)
                //{
                //    tiles[i][j] = TileType.outOfBounds;
                //}

                //else
                //{
                //    tiles[i][j] = TileType.rough;
                //}              
            }
        }

    }
}
