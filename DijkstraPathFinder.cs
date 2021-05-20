using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Greedy.Architecture;
using System.Drawing;

namespace Greedy
{
    public class DijkstraPathFinder
    {
        public IEnumerable<PathWithCost> GetPathsByDijkstra(State state, Point start,
            IEnumerable<Point> targets)
        {
            var pointList = new LinkedList<Point>();
            var queue = new Queue<Point>();
            var visitPoint = new HashSet<Point>();

            queue.Enqueue(start);
            pointList.AddFirst(start);
            visitPoint.Add(start);

            PointGeneration(state, pointList);

            yield break;
        }

        public static void PointGeneration(
            State state, LinkedList<Point> pointList)
        {
            for (var dy = 0; dy < state.MapHeight; dy++)
                for (var dx = 0; dx < state.MapWidth; dx++)
                {
                    var tPoint = new Point(dx, dy);
                    if (state.InsideMap(tPoint) && !state.IsWallAt(tPoint))
                    {
                        pointList.AddAfter(pointList.Last, tPoint);
                    }
                }
        }
    }
}