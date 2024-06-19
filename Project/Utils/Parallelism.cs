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
        
        for(int i = 0; i < chunks.Length; i++)
        {
            T[] chunk = new T[chunkSize * chunkSize];
            
            foreach(var item in VectorHelper.IterateRectangle(chunkSize, chunkSize))
            {
                int row = (i / chunksOnRow) * chunkSize + item.XInt();
                int col = (i % chunksOnRow) * chunkSize + item.YInt();
                
                if(row >= rowWidth || col >= colHeight) continue;
                
                chunk[item.XInt() * chunkSize + item.YInt()] = collection2D[row, col];
            }
            
            chunks[i] = chunk;
        }

        return chunks;
    }
}