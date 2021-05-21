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
            var result = new List<PathWithCost> {new PathWithCost(price, start) };

            visitedPoint.Add(start);
            queue.Enqueue(start);
            pathList[start] = new List<Tuple<Point, int>>();
            //pointList.Add(start, new List<Point>());

            //while (true)
            //{
            //    BestPrice(state, pathList, openPoint);
            //    if (openPoint == new Point(-1, -1)) yield return null;
            //}

            while (visitedPoint.Count < state.CellCost.Length)
            {
                PointGeneration(state, queue, pathList, visitedPoint);
            }

            var point = pathList[start].OrderBy(x => x.Item2).First();
            visitedPoint.Clear();
            visitedPoint.Add(start);

            while (true)
            {
                if (!visitedPoint.Contains(point.Item1))
                {
                    price += point.Item2;
                    result.Add(new PathWithCost(price, point.Item1));
                    visitedPoint.Add(point.Item1);
                }
                if (state.Chests.Any(x => x == point.Item1)) yield break;
                point = pathList[point.Item1].OrderBy(x => x.Item2).First();
            }

            foreach (var item in result)
            {
                yield return item;
            };
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
                        if (state.InsideMap(tPoint) && !state.IsWallAt(tPoint) && point != tPoint)
                        {
                            if (visitedPoint.Contains(point) && !pathList[point].Any(x => x.Item1 == tPoint))
                            {
                                pathList[point].Add(Tuple.Create(tPoint, state.CellCost[tPoint.X, tPoint.Y]));
                                queue.Enqueue(tPoint);
                            }
                            else if (!pathList.ContainsKey(point))
                            {
                                pathList.Add(point, new List<Tuple<Point, int>>
                                { Tuple.Create(tPoint, state.CellCost[tPoint.X, tPoint.Y]) });
                                visitedPoint.Add(point);
                                queue.Enqueue(tPoint);
                            }
                        }
                    }
        }

        
    }
}