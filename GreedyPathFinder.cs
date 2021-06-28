using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Greedy.Architecture;
using Greedy.Architecture.Drawing;

namespace Greedy
{
    public class GreedyPathFinder : IPathFinder
    {
        public List<Point> FindPathToCompleteGoal(State state)
        {
            var chestsCount = state.Chests.ToHashSet();
            var result = new List<Point>();

            if (state.Chests.Count < state.Goal) return new List<Point>();
            if (chestsCount.Contains(state.Position))
            {
                state.Scores++;
                chestsCount.Remove(state.Position);
            }

            SearchProcess(state, result, chestsCount);

            return result;
        }

        public static List<Point> SearchProcess(State state, List<Point> result, HashSet<Point> chestsCount)
        {
            var path = new DijkstraPathFinder();
            var cost = 0;

            while (state.Scores < state.Goal)
            {
                var step = path.GetPathsByDijkstra(state, state.Position, chestsCount);
                if (step.FirstOrDefault() == null) return new List<Point>();
                cost += step.First().Cost;

                if (cost > state.Energy) return new List<Point>();
                var way = step.First().Path.Skip(1);
                foreach (var item in way)
                {
                    result.Add(item);
                }

                state.Scores++;
                chestsCount.Remove(result.Last());
                state.Position = result.Last();
            }

            return result;
        }
    }
}