using UnityEngine;
using System.Collections;

public class Block
{
    public enum Shape
    {
        Square, LeftTurn, RightTurn, NarrowingUp, NarrowingDown,
    }

    public int xPos;
    public int yPos;
    public int width;
    public int height;
    public Shape shape;
    public Block previousBlock;


    // This is used for the first block.
    public void SetupBlock(IntRange widthRange, IntRange heightRange, int columns, int rows)
    {
        // Set a random width and height.
        width = widthRange.Random;
        height = heightRange.Random;

        // Set the x and y coordinates so the block is in the middle and the bottom of the board.
        xPos = Mathf.RoundToInt( (columns - width) / 2f );
        yPos = Mathf.RoundToInt(5);
        shape = (Shape)(int)Random.Range(0, 4);
    }

    public void SetupBlock(IntRange widthRange, IntRange heightRange, int columns, int rows, Block block)
    {
        previousBlock = block;

        // Set random values for width and height.
        width = widthRange.Random;
        height = heightRange.Random;

        // Position of the block should be dependant on the shape of previous block.
        shape = (Shape)(int)Random.Range(0, 5);

        while (shape == previousBlock.shape)
        {
            shape = (Shape)(int)Random.Range(0, 5);
        }

        xPos = previousBlock.xPos;
        yPos = previousBlock.yPos + previousBlock.height;
        
        // fixing positioning issues if previous shape was right turn
        if (previousBlock.shape == Shape.RightTurn)
        {
            if (shape != Shape.LeftTurn)
            {
                xPos = previousBlock.xPos + previousBlock.width/2;
            }
        }

        //fixing positioning issues if this shape is left turn
        if (shape == Shape.LeftTurn)
        {
            if(previousBlock.shape != Shape.RightTurn)
            {
                xPos = previousBlock.xPos - previousBlock.width / 2;
            }
        }
    }

    public void SpawnBall (GameObject ball)
    {
        ball.transform.position =  new Vector3(xPos + (width / 2), yPos + (height / 2), 0);
    }

    public void SpawnHole (GameObject hole)
    {
        hole.transform.position = new Vector3(xPos + width / 2, yPos + height / 2, 0);
    }

    public bool IsHeightAndWidthOk ()
    {
        if(previousBlock != null)
        {

            if( shape == Shape.Square || shape == Shape.LeftTurn || shape == Shape.RightTurn)
            {
                if(previousBlock.shape == Shape.NarrowingUp)
                {
                    return false;
                }

                else
                {
                    return true;
                }
            }

            else if ( shape == Shape.NarrowingDown)
            {
                if( previousBlock.shape == Shape.NarrowingUp)
                {
                    return true;
                }

                else
                {
                    return false;
                }
            }            
        }
        Debug.Log("null");
        return true;
    }

    public void ReduceHeight()
    {
        Debug.Log("height : " + height);
        height -= 3;
        Debug.Log("reduced height : " + height);
    }

    public Block Clone()
    {
        Block toReturn = new Block();
        toReturn.xPos = xPos;
        toReturn.yPos = yPos;
        toReturn.shape = shape;
        toReturn.width = width;
        toReturn.height = height;

        return toReturn;
    }
}
