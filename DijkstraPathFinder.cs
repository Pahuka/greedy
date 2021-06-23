using System;
using System.Collections.Generic;
using System.Linq;
using Greedy.Architecture;
using System.Drawing;
using NUnit.Framework;

namespace Greedy
{
    public class DijkstraPathFinder
    {
        public IEnumerable<PathWithCost> GetPathsByDijkstra(State state, Point start,
            IEnumerable<Point> targets)
        {
            var pathList = new Dictionary<Point, Tuple<Point, int>>();
            var queue = new Queue<Point>();
            var visitedPoint = new HashSet<Point>();
            var chestFind = targets.ToList();

            visitedPoint.Add(start);
            queue.Enqueue(start);
            pathList[start] = Tuple.Create(start, 0);

            while (chestFind.Count > 0 && queue.Count != 0)
            {
                var stepPoint = PointGeneration(state, queue.Dequeue(), visitedPoint);
                var centrePoint = stepPoint.Take(1).Single();

                foreach (var item in stepPoint.Skip(1))
                {
                    if (!pathList.ContainsKey(item.Item1))
                        pathList[item.Item1] = Tuple.Create(
                            centrePoint.Item1, item.Item2 + pathList[centrePoint.Item1].Item2);
                    else
                    {
                        pathList[item.Item1] = pathList[item.Item1].Item2 > item.Item2 + pathList[centrePoint.Item1].Item2 ?
                            Tuple.Create(centrePoint.Item1, item.Item2 + pathList[centrePoint.Item1].Item2)
                            : pathList[item.Item1];
                    }
                }

                foreach (var item in pathList
                        .OrderBy(x => x.Value.Item2)
                        .Where(x => !visitedPoint.Contains(x.Key))
                        .Take(1))
                {
                    visitedPoint.Add(item.Key);
                    queue.Enqueue(item.Key);
                }

                if (chestFind.Any(x => visitedPoint.Contains(x)))
                {
                    var tkey = chestFind.Where(x => visitedPoint.Contains(x)).Single();
                    chestFind.Remove(tkey);
                    var tempRoute = new List<Point>() { tkey };

                    while (tkey != start)
                    {
                        tempRoute.Add(pathList[tkey].Item1);
                        tkey = pathList[tkey].Item1;
                    }

                    tempRoute.Reverse();
                    yield return new PathWithCost(pathList[tempRoute.Last()].Item2, tempRoute.ToArray());
                }
            }
        }

        public static List<Tuple<Point, int>> PointGeneration(State state, Point point, HashSet<Point> visitedPoint)
        {
            var result = new List<Tuple<Point, int>> { Tuple.Create(point, state.CellCost[point.X, point.Y]) };

            for (var dy = -1; dy <= 1; dy++)
                for (var dx = -1; dx <= 1; dx++)
                    if (dx != 0 && dy != 0) continue;
                    else
                    {
                        var tPoint = new Point { X = point.X + dx, Y = point.Y + dy };
                        if (state.InsideMap(tPoint) && tPoint != point)
                        {
                            if (state.IsWallAt(tPoint)) visitedPoint.Add(tPoint);
                            if (!visitedPoint.Contains(tPoint))
                            {
                                result.Add(Tuple.Create(tPoint, state.CellCost[tPoint.X, tPoint.Y]));
                            }
                        }
                    }
            return result;
        }
    }
}