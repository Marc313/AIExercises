using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Astar
{
    private Material debugMaterial;

    public Astar(Material mat = null)
    {
        debugMaterial = mat;
    }

    /// <summary>
    /// TODO: Implement this function so that it returns a list of Vector2Int positions which describes a path
    /// from the startPos to the endPos
    /// Note that you will probably need to add some helper functions
    /// </summary>
    /// <param name="startPos"></param>
    /// <param name="endPos"></param>
    /// <param name="grid"></param>
    /// <returns></returns>
    public List<Vector2Int> FindPathToTarget(Vector2Int startPos, Vector2Int endPos, Cell[,] grid)
    {
        List<Node> openNodes = new List<Node>();
        List<Vector2Int> path = new List<Vector2Int>();
        HashSet<Cell> visitedCells = new HashSet<Cell>();
        int visitingCount = 0;

        Node startNode = new Node(position: startPos, 
                                    parent: null, 
                                    GScore: 0, 
                                    HScore: CalculateHScore(startPos, endPos));
        openNodes.Add(startNode);
        visitedCells.Add(GetCell(startPos, grid));

        while (openNodes.Count > 0)
        {
            visitingCount++;
            Node currentNode = openNodes.OrderBy(n => n.FScore).FirstOrDefault();   // TODO: slimmer
            openNodes.Remove(currentNode);

            // Return the path if endPosition is found
            if (currentNode.position == endPos)
            {
                path.Add(endPos);

                while (currentNode.parent != null)
                {
                    currentNode = currentNode.parent;
                    path.Add(currentNode.position);
                }

                Debug.Log($"Visited Nodes: {visitingCount}");
                path.Reverse();
                return path;
            }

/*            // Debug  Start //
            GameObject obj = MazeGeneration.GetCellObjectAt(currentNode.position);
            obj.GetComponentInChildren<MeshRenderer>().material = debugMaterial;
            // Debug End //*/

            Cell currentCell = GetCell(currentNode.position, grid);
            List<Cell> neighbours = currentCell.GetAccesibleNeighbours(grid);

            foreach (Cell neighbourCell in neighbours)
            {
                if (visitedCells.Contains(neighbourCell))
                {
                    // Get node, Calculate new score, Update if necassary
                    Node neighbourNode = GetNode(neighbourCell.gridPosition, openNodes);
                    if (!openNodes.Contains(neighbourNode)) continue;   // Make sure the node is still open

                    float newGScore = neighbourNode.parent.GScore + 1;
                    if (neighbourNode.GScore > newGScore)
                    {
                        neighbourNode.GScore = newGScore;
                        neighbourNode.parent = currentNode;
                    }
                }
                else
                {
                    // Create new node, add to openNodes
                    Node neighbourNode = new Node(position: neighbourCell.gridPosition,
                                                    parent: currentNode,
                                                    GScore: (int) currentNode.GScore + 1,
                                                    HScore: CalculateHScore(neighbourCell.gridPosition, endPos));

                    openNodes.Add(neighbourNode);
                    visitedCells.Add(neighbourCell);
                }
            }
        }

        return null;
    }

    public int CalculateHScore(Vector2Int gridPosition, Vector2Int endPos)
    {
        Vector2Int difference = endPos - gridPosition;
        int score = Mathf.Abs(difference.x) + Mathf.Abs(difference.y);
        return score;
    }

    public Node GetNode(Vector2Int position, IEnumerable<Node> nodeList)
    {
        foreach (Node node in nodeList)
        {
            if (node.position == position)
            {
                return node;
            }
        }

        return null;
    }

    public Cell GetCell(Vector2Int position, Cell[,] grid)
    {
        foreach (Cell cell in grid)
        {
            if (cell.gridPosition == position)
            {
                return cell;
            }
        }

        return null;
    }

    /// <summary>
    /// This is the Node class you can use this class to store calculated FScores for the cells of the grid, you can leave this as it is
    /// </summary>
    public class Node
    {
        public Vector2Int position; //Position on the grid
        public Node parent; //Parent Node of this node

        public float FScore { //GScore + HScore
            get { return GScore + HScore; }
        }
        public float GScore; //Current Travelled Distance
        public float HScore; //Distance estimated based on Heuristic

        public Node() { }
        public Node(Vector2Int position, Node parent, int GScore, int HScore)
        {
            this.position = position;
            this.parent = parent;
            this.GScore = GScore;
            this.HScore = HScore;
        }
    }
}
