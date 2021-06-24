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
            var path = new DijkstraPathFinder();
            var chestsFind = new List<Point>();
            var result = new List<Point>();
            var cost = 0;

            if (state.Chests.Count < state.Goal) return new List<Point>();

            while (cost < state.Energy && chestsFind.Count < state.Chests.Count)
            {
                var step = path.GetPathsByDijkstra(state, state.Position, state.Chests);
                cost += step.Where(x => !chestsFind.Contains(x.Path.Last())).First().Cost;

                if (cost <= state.Energy)
                {
                    var way = step.Select(x => x.Path)
                        .Where(x => !chestsFind.Contains(x.Last()))
                        .First();
                    way.RemoveAt(0);

                    result.AddRange(way);
                    
                }
                else break;

                chestsFind.Add(result.Last());
                state.Position = result.Last();

            }

			return result;
		}
	}
}