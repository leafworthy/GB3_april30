using System.Collections.Generic;


public static class TopologicalSortSimple
{
    private static Dictionary<IsoSpriteSorting, bool> visited = new Dictionary<IsoSpriteSorting, bool>(64);
    private static List<IsoSpriteSorting> allSprites = new List<IsoSpriteSorting>(64);
    public static List<IsoSpriteSorting> Sort(List<IsoSpriteSorting> movableSprites, List<IsoSpriteSorting> sorted)
    {
        allSprites.Clear();
        allSprites.AddRange(movableSprites);
        visited.Clear();
        for (int i = 0; i < allSprites.Count; i++)
        {
            Visit(allSprites[i], sorted, visited);
        }

        return sorted;
    }

    private static void Visit(IsoSpriteSorting item, List<IsoSpriteSorting> sorted, Dictionary<IsoSpriteSorting, bool> visited)
    {

            visited[item] = true;

            // List<IsoSpriteSorting> dependencies = item.ActiveDependencies;
            // for (int i = 0; i < dependencies.Count; i++)
            // {
            //     Visit(dependencies[i], sorted, visited);
            // }

            visited[item] = false;
            sorted.Add(item);

    }
}
