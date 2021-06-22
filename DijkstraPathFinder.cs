using System;
using System.Collections.Generic;
using System.Linq;
using Greedy.Architecture;
using System.Drawing;
using NUnit.Framework;

namespace Greedy
{
    //    public class DijkstraData
    //    {
    //        public DijkstraData Previus { get; set; }
    //        public Point Actual { get; set; }
    //        public int Price { get; set; }
    //    }

    public class DijkstraPathFinder
    {
        public IEnumerable<PathWithCost> GetPathsByDijkstra(State state, Point start,
            IEnumerable<Point> targets)
        {
            var pathList = new Dictionary<Point, List<Tuple<Point, int>>>();
            var queue = new Queue<Point>();
            var visitedPoint = new HashSet<Point>();
            var chestFind = targets.ToList();
            var result = new List<PathWithCost>();
            //var result = new List<List<Tuple<Point, int>>> { new List<Tuple<Point, int>> {Tuple.Create(start, 1) } };

            visitedPoint.Add(start);
            queue.Enqueue(start);
            //pathList[start] = new List<Tuple<Point, int>>();

            while (chestFind.Count > 0 && queue.Count > 0)
            {
                var stepPoint = PointGeneration(state, queue, visitedPoint);

                pathList[stepPoint.Take(1).Single().Item1] = stepPoint.Skip(1).ToList();

                if (pathList.Count == 1 && targets.Contains(stepPoint.First().Item1))
                {
                    yield return new PathWithCost(0, stepPoint.First().Item1);
                    continue;
                }

                if (stepPoint.First().Item1 == new Point(10, 9))
                {

                }

                if (chestFind.Any(p => p == stepPoint.Take(1).Single().Item1))
                {
                    var keySelector = new Queue<Tuple<Point, int>>();
                    keySelector.Enqueue(stepPoint.Take(1).Single());
                    //var tKey = stepPoint.Take(1).Single();
                    //var pathPrice = 0;
                    //var notVisitPoint = new HashSet<Tuple<Point, int>>();
                    //AddToHash(notVisitPoint, pathList[stepPoint.Take(1).Single().Item1]
                    //    .Where(key => pathList.ContainsKey(key.Item1)));
                    //var resultPath = new List<Tuple<Point, int>>() { tKey };
                    var visitPoints = new HashSet<Point>();
                    var resultPath = new Dictionary<Tuple<Point, int>, Tuple<Point, int>>();


                    while (keySelector.Count != 0)
                    {
                        var tKey = keySelector.Dequeue();
                        //AddToHash(notVisitPoint, pathList[tKey.Item1].Where(key => pathList.ContainsKey(key.Item1))
                        //    .Where(x => x.Item1 != stepPoint.First().Item1));
                        //var linkPoints = pathList[tKey.Item1]
                        //    .Where(key => pathList.ContainsKey(key.Item1) && !visitPoints.Contains(key.Item1))
                        //    .OrderBy(price => price.Item2).ToList();

                        var linkPoints = pathList[tKey.Item1]
                            .Where(key => !visitPoints.Contains(key.Item1))
                            .OrderBy(price => price.Item2).ToList();

                        visitPoints.Add(tKey.Item1);

                        if (linkPoints.Count == 0)
                        {
                            continue;
                            //resultPath.RemoveAt(resultPath.Count - 1);
                            //tKey = resultPath.Last();
                        }


                        foreach (var item in linkPoints)
                        {
                            var key = Tuple.Create(item.Item1, item.Item2 + tKey.Item2);
                            if (item.Item1 == start)
                                resultPath[Tuple.Create(item.Item1, tKey.Item2)] = tKey;
                            else resultPath[key] = tKey;

                            keySelector.Enqueue(resultPath.Keys.Last());
                        }

                        //if (tKey.Item1 == start)
                        //{
                        //    var x = resultPath.OrderBy(price => price.Key.Item2).Where(p => p.Key.Item1 == tKey.Item1).First().Key;
                        //    var pathPrice = x.Item2;
                        //    var list = new List<Point>() { x.Item1 };
                        //    while (resultPath.ContainsKey(x))
                        //    {
                        //        x = resultPath[x];
                        //        list.Add(x.Item1);
                        //    }
                        //    chestFind.Remove(list.Last());
                        //    yield return new PathWithCost(pathPrice, list.ToArray());
                        //}
                    }

                    var x = resultPath
                        .OrderBy(price => price.Key.Item2)
                        .Where(p => p.Key.Item1 == start)
                        .First().Key;
                    var pathPrice = x.Item2;
                    var list2 = new List<Point>() { x.Item1 };
                    while (resultPath.ContainsKey(x))
                    {
                        x = resultPath[x];
                        list2.Add(x.Item1);
                    }
                    chestFind.Remove(list2.Last());
                    result.Add(new PathWithCost(pathPrice, list2.ToArray()));
                    //yield return new PathWithCost(pathPrice, list2.ToArray());

                    //yield return result.OrderBy(price => price.Cost).First();
                    //chestFind.RemoveAt(chestFind.IndexOf(resultPath.Last().Item1));
                    //yield return new PathWithCost(resultPath.Skip(1).Select(p => p.Item2).Sum(),
                    //    resultPath.Select(p => p.Item1).ToArray());
                }

            }

            foreach (var item in result.OrderBy(price => price.Cost))
            {
                yield return item;
            }
        }

        public static void AddToHash(HashSet<Tuple<Point, int>> hashset, IEnumerable<Tuple<Point, int>> items)
        {
            foreach (var tuple in items)
            {
                hashset.Add(tuple);
            }
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
    }
}