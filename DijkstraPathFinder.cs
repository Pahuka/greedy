using System;
using System.Collections.Generic;
using System.Linq;
using Greedy.Architecture;
using System.Drawing;

namespace Greedy
{
    public class DijkstraData
    {
        public DijkstraData Previus { get; set; }
        public Point Actual { get; set; }
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
            var chestFind = targets.ToList();

            //var result = new List<List<Tuple<Point, int>>> { new List<Tuple<Point, int>> {Tuple.Create(start, 1) } };
            var result = new List<DijkstraData> { new DijkstraData { Previus = null, Actual = start, Price = 1 } };

            visitedPoint.Add(start);
            queue.Enqueue(start);
            //pathList[start] = new List<Tuple<Point, int>>();

            while (chestFind.Count > 0 && queue.Count > 0)
            {
                var stepPoint = PointGeneration(state, queue, visitedPoint);

                pathList[stepPoint.Take(1).Single().Item1] = stepPoint.Skip(1).ToList();

                if (chestFind.Any(p => p == stepPoint.Take(1).Single().Item1))
                {
                    var tKey = stepPoint.Take(1).Single();
                    var pathPrice = tKey.Item2;
                    var visitPoint = new List<Tuple<Point, int>>();
                    var resultPath = new List<Tuple<Point, int>>() { tKey };
                    while (tKey.Item1 != start)
                    {
                        var t = pathList[tKey.Item1]
                            .Where(key => pathList.ContainsKey(key.Item1) && !visitPoint.Contains(key))
                            .OrderBy(price => price.Item2).ToList();
                        if (t.Count == 0)
                        {
                            resultPath.RemoveAt(resultPath.Count - 1);
                            tKey = resultPath.Last();
                        }
                        else if (t.Select(p => p.Item1).Contains(start))
                        {
                            resultPath.Add(t.Where(p => p.Item1 == start).Single());
                            break;
                        }
                        else
                        {
                            resultPath.Add(t.First());
                            visitPoint.AddRange(t.Where(p => p.Item1 == resultPath.Last().Item1));
                            pathPrice += resultPath.Last().Item2;
                            tKey = resultPath.Last();
                        }
                    }
                    resultPath.Reverse();
                    chestFind.RemoveAt(chestFind.IndexOf(resultPath.Last().Item1));
                    yield return new PathWithCost(resultPath.Skip(1).Select(p => p.Item2).Sum(),
                        resultPath.Select(p => p.Item1).ToArray());
                }
            }

            //yield return null;
        }

        public static List<Tuple<Point, int>> PointGeneration(State state, Queue<Point> queue, HashSet<Point> visitedPoint)
        {
            var point = queue.Dequeue();
            visitedPoint.Add(point);
            var result = new List<Tuple<Point, int>> { Tuple.Create(point, state.CellCost[point.X, point.Y]) };

            for (var dy = -1; dy <= 1; dy++)
                for (var dx = -1; dx <= 1; dx++)
                    if (dx != 0 && dy != 0) continue;
                    else
                    {
                        var tPoint = new Point { X = point.X + dx, Y = point.Y + dy };
                        if (state.InsideMap(tPoint) && !state.IsWallAt(tPoint) && tPoint != point)
                        {
                            //visitedPoint.Add(tPoint);
                            if (!visitedPoint.Contains(tPoint)) queue.Enqueue(tPoint);
                            result.Add(Tuple.Create(tPoint, state.CellCost[tPoint.X, tPoint.Y]));
                        }
                    }
            return result;
        }

        #region

        //public static void PointGeneration(
        //    State state, Queue<Point> queue, Dictionary<Point, List<Tuple<Point, int>>> pathList, HashSet<Point> visitedPoint)
        //{
        //    var point = queue.Dequeue();

        //    for (var dy = -1; dy <= 1; dy++)
        //        for (var dx = -1; dx <= 1; dx++)
        //            if (dx != 0 && dy != 0) continue;
        //            else
        //            {
        //                var tPoint = new Point { X = point.X + dx, Y = point.Y + dy };
        //                if (state.InsideMap(tPoint) && !state.IsWallAt(tPoint)
        //                    && !visitedPoint.Contains(tPoint) && !queue.Contains(tPoint))
        //                {
        //                    if (pathList.ContainsKey(point))
        //                    {
        //                        pathList[point].Add(Tuple.Create(tPoint, state.CellCost[tPoint.X, tPoint.Y]));
        //                        queue.Enqueue(tPoint);
        //                        visitedPoint.Add(tPoint);
        //                    }
        //                    else
        //                    {
        //                        pathList.Add(point, new List<Tuple<Point, int>>
        //                        { Tuple.Create(tPoint, state.CellCost[tPoint.X, tPoint.Y]) });
        //                        visitedPoint.Add(tPoint);
        //                        queue.Enqueue(tPoint);
        //                    }
        //                }
        //            }
        //}

        #endregion
    }
}