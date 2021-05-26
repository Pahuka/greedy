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
            //var pathList = new Dictionary<Point, List<Tuple<Point, int>>>();
            var queue = new Queue<Point>();
            var visitedPoint = new HashSet<Point>();
            var chestFind = 0;
            var chestPoint = new HashSet<Point>();
            //var result = new List<List<Tuple<Point, int>>> { new List<Tuple<Point, int>> {Tuple.Create(start, 1) } };
            var result = new List<DijkstraData> { new DijkstraData { Previus = null, Actual = start, Price = 1 } };

            visitedPoint.Add(start);
            queue.Enqueue(start);
            //pathList[start] = new List<Tuple<Point, int>>();

            while (chestFind < state.Chests.Count)
            {
                var stepPoint = PointGeneration(state, queue, visitedPoint);

                //if (state.Chests.Contains(stepPoint.Last().Item1))
                //{
                //    chestFind++;
                //    yield return new PathWithCost(stepPoint.Last().Item2, stepPoint.Select(p => p.Item1).ToArray());
                //}

                foreach (var point in stepPoint.Skip(1).OrderBy(p => p.Item2))
                {

                    if (result.Last().Previus == null)
                    {
                        var tData = new DijkstraData
                        {
                            Previus = result.First(),
                            Actual = point.Item1,
                            Price = point.Item2
                        };
                        result.Add(tData);
                    }
                    else
                    {
                        var tData = new DijkstraData
                        {
                            Previus = result.Where(p => p.Actual == stepPoint.First().Item1).First(),
                            Actual = point.Item1,
                            Price = point.Item2
                        };
                        result.Add(tData);
                    };
                    if (state.Chests.Contains(point.Item1) && !chestPoint.Contains(point.Item1))
                    {
                        chestPoint.Add(point.Item1);
                        chestFind++;
                        var prevData = result.Where(p => p.Actual == point.Item1)
                            .Select(p => p.Previus).Single();
                        var takePath = new List<Point> { point.Item1 };
                        var price = point.Item2;
                        while (prevData.Previus != null)
                        {
                            takePath.Add(prevData.Actual);
                            price += prevData.Price;
                            prevData = prevData.Previus;
                        }
                        takePath.Add(prevData.Actual);
                        //price += prevData.Price;
                        takePath.Reverse();
                        yield return new PathWithCost(price, takePath.ToArray());
                    }
                    //yield return new PathWithCost(point.Item2, result.Where(p => p.Previus).ToArray());
                }
            }

            #region

            //while (queue.Count != 0)
            //{
            //    PointGeneration(state, queue, pathList, visitedPoint);
            //}

            //var point = pathList[start].OrderBy(x => x.Item2).First();
            //visitedPoint.Clear();
            //visitedPoint.Add(point.Item1);

            //while (true)
            //{
            //    price += point.Item2;
            //    result.Add(point.Item1);
            //    if (price <= state.InitialEnergy && state.Chests.Any(x => x == point.Item1))
            //    {
            //        yield return new PathWithCost(price, result.ToArray());
            //        price = 0;
            //        result.RemoveRange(1, result.Count - 1);
            //        point = pathList[start].Where(x => !visitedPoint.Contains(x.Item1)).First();
            //        visitedPoint.Add(point.Item1);
            //    }
            //    if (visitedPoint.Count == state.Chests.Count) break;
            //    else point = pathList[point.Item1].First();
            //}

            #endregion

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
                        if (state.InsideMap(tPoint) && !state.IsWallAt(tPoint)
                            && tPoint != point && !visitedPoint.Contains(tPoint))
                        {
                            //visitedPoint.Add(tPoint);
                            queue.Enqueue(tPoint);
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