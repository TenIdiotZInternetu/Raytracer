namespace rt004.Utils;

public static class Parallelism
{
    public static T[][] Chunkify2D<T>(T[,] collection2D, int chunkSize)
    {
        int rowWidth = collection2D.GetLength(0);
        int colHeight = collection2D.GetLength(1);
        
        int chunksOnRow = (rowWidth / chunkSize) + 1;
        int chunksOnCol = (colHeight / chunkSize) + 1;
        
        T[][] chunks = new T[chunksOnCol * chunksOnRow][];
        Array.Fill(chunks, new T[chunkSize * chunkSize]);

        foreach (var itemPos in VectorHelper.IterateRectangle(rowWidth, colHeight))
        {
            int chunkX = itemPos.XInt() / chunkSize;
            int chunkY = itemPos.YInt() / chunkSize;
            int chunkIndex = chunkY * chunksOnRow + chunkX;
            
            int chunkOffsetX = itemPos.XInt() % chunkSize;
            int chunkOffsetY = itemPos.YInt() % chunkSize;
            int chunkOffset = chunkOffsetY * chunkSize + chunkOffsetX;
            
             T item = collection2D[itemPos.XInt(), itemPos.YInt()];
             chunks[chunkIndex][chunkOffset] = item;
        }

        return chunks;
    }
}