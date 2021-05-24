using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Greedy.Architecture;
using System.Drawing;

namespace Greedy
{
    public class DijkstraData
    {
        public Point Previus { get; set; }
        public int Price { get; set; }
    }

    public class DijkstraPathFinder
    {
        public IEnumerable<PathWithCost> GetPathsByDijkstra(State state, Point start,
            IEnumerable<Point> targets)
        {
            var pathList = new Dictionary<Point, List<Tuple<Point, int>>>();
            var queue = new Queue<Point>();
            var visitedPoint = new HashSet<Point>();
            var price = 0;
            //var result = new List<PathWithCost> {new PathWithCost(price, start) };
            var result = new List<Point>() { start };

            visitedPoint.Add(start);
            queue.Enqueue(start);
            pathList[start] = new List<Tuple<Point, int>>();

            while (queue.Count != 0)
            {
                PointGeneration(state, queue, pathList, visitedPoint);
            }

            var point = pathList[start].OrderBy(x => x.Item2).First();
            visitedPoint.Clear();
            visitedPoint.Add(point.Item1);

            while (true)
            {
                price += point.Item2;
                result.Add(point.Item1);
                if (price <= state.InitialEnergy && state.Chests.Any(x => x == point.Item1))
                {
                    yield return new PathWithCost(price, result.ToArray());
                    price = 0;
                    result.RemoveRange(1, result.Count - 1);
                    point = pathList[start].Where(x => !visitedPoint.Contains(x.Item1)).First();
                    visitedPoint.Add(point.Item1);
                }
                if (visitedPoint.Count == state.Chests.Count) break;
                else point = pathList[point.Item1].First();
            }

            //yield return result;
        }

        public static void PointGeneration(
            State state, Queue<Point> queue, Dictionary<Point, List<Tuple<Point, int>>> pathList, HashSet<Point> visitedPoint)
        {
            var point = queue.Dequeue();

            for (var dy = -1; dy <= 1; dy++)
                for (var dx = -1; dx <= 1; dx++)
                    if (dx != 0 && dy != 0) continue;
                    else
                    {
                        var tPoint = new Point { X = point.X + dx, Y = point.Y + dy };
                        if (state.InsideMap(tPoint) && !state.IsWallAt(tPoint)
                            && !visitedPoint.Contains(tPoint) && !queue.Contains(tPoint))
                        {
                            if (pathList.ContainsKey(point))
                            {
                                pathList[point].Add(Tuple.Create(tPoint, state.CellCost[tPoint.X, tPoint.Y]));
                                queue.Enqueue(tPoint);
                                visitedPoint.Add(tPoint);
                            }
                            else
                            {
                                pathList.Add(point, new List<Tuple<Point, int>>
                                { Tuple.Create(tPoint, state.CellCost[tPoint.X, tPoint.Y]) });
                                visitedPoint.Add(tPoint);
                                queue.Enqueue(tPoint);
                            }
                        }
                    }
        }


    }
}