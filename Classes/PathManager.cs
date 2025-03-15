namespace LudoGame.Classes;
using LudoGame.Enums;

public class PathManager
    {
        private Board board;
        private Path fullPath;
        private Dictionary<PieceColor, Path> mainPaths;
        private Dictionary<PieceColor, Path> goalPaths;

        public PathManager(Board board)
        {
            this.board = board;
            fullPath = new Path();
            mainPaths = new Dictionary<PieceColor, Path>();
            goalPaths = new Dictionary<PieceColor, Path>();

            InitializeFullPath();
            InitializeMainPaths();
            InitializeGoalPaths();
        }

        private void InitializeFullPath()
        {
            List<(int, int)> coordinates = new List<(int, int)>
            {
                (6,1),(6,0),(7,0),(8,0),(8,1),(8,2),(8,3),(8,4),(8,5),
                (9,6),(10,6),(11,6),(12,6),(13,6),(14,6),(14,7),(14,8),(13,8),(12,8),(11,8),
                (10,8),(9,8),(8,9),(8,10),(8,11),(8,12),(8,13),(8,14),(7,14),(6,14),(6,13),
                (6,12),(6,11),(6,10),(6,9),(5,8),(4,8),(3,8),(2,8),(1,8),(0,8),(0,7),(0,6),
                (1,6),(2,6),(3,6),(4,6),(5,6),(6,5),(6,4),(6,3),(6,2)
            };

            foreach (var (r, c) in coordinates)
            {
                Square sq = board.GetSquare(r, c);
                fullPath.AddSquare(sq);
            }
        }

        private void InitializeMainPaths()
        {
            mainPaths[PieceColor.RED] = GetWrappedPath(0);
            mainPaths[PieceColor.GREEN] = GetWrappedPath(13);
            mainPaths[PieceColor.YELLOW] = GetWrappedPath(26);
            mainPaths[PieceColor.BLUE] = GetWrappedPath(39);
        }
        private Path GetWrappedPath(int startIndex)
        {
            Path path = new Path();
            int total = fullPath.Count;
            for (int i = 0; i < total; i++)
            {
                int idx = (startIndex + i) % total;
                path.AddSquare(fullPath.GetSquare(idx));
            }
            return path;
        }

        private void InitializeGoalPaths()
        {
            // Red goal path
            Path redGoal = new Path();
            List<(int, int)> redGoalCoords = new List<(int, int)>
            {
                (6,1),(6,0),(7,0),(7,1),(7,2),(7,3),(7,4),(7,5),(7,6)
            };
            string redDot = "\u001b[31m.\u001b[0m";
            foreach (var (r, c) in redGoalCoords)
            {
                Square sq = board.GetSquare(r, c);
                redGoal.AddSquare(sq);
                sq.Occupant = redDot;
                sq.BaseMarker = redDot;
            }
            goalPaths[PieceColor.RED] = redGoal;

            // Green goal path
            Path greenGoal = new Path();
            List<(int, int)> greenGoalCoords = new List<(int, int)>
            {
                (13,6),(14,6),(14,7),(13,7),(12,7),(11,7),(10,7),(9,7),(8,7)
            };
            string greenDot = "\u001b[32m.\u001b[0m";
            foreach (var (r, c) in greenGoalCoords)
            {
                Square sq = board.GetSquare(r, c);
                greenGoal.AddSquare(sq);
                sq.Occupant = greenDot;
                sq.BaseMarker = greenDot;
            }
            goalPaths[PieceColor.GREEN] = greenGoal;

            // Yellow goal path
            Path yellowGoal = new Path();
            List<(int, int)> yellowGoalCoords = new List<(int, int)>
            {
                (8,13),(8,14),(7,14),(7,13),(7,12),(7,11),(7,10),(7,9),(7,8)
            };
            string yellowDot = "\u001b[33m.\u001b[0m";
            foreach (var (r, c) in yellowGoalCoords)
            {
                Square sq = board.GetSquare(r, c);
                yellowGoal.AddSquare(sq);
                sq.Occupant = yellowDot;
                sq.BaseMarker = yellowDot;
            }
            goalPaths[PieceColor.YELLOW] = yellowGoal;

            // Blue goal path
            Path blueGoal = new Path();
            List<(int, int)> blueGoalCoords = new List<(int, int)>
            {
                (1,8),(0,8),(0,7),(1,7),(2,7),(3,7),(4,7),(5,7),(6,7)
            };
            string blueDot = "\u001b[34m.\u001b[0m";
            foreach (var (r, c) in blueGoalCoords)
            {
                Square sq = board.GetSquare(r, c);
                blueGoal.AddSquare(sq);
                sq.Occupant = blueDot;
                sq.BaseMarker = blueDot;
            }
            goalPaths[PieceColor.BLUE] = blueGoal;
        }

        public Path GetFullPath() => fullPath;
        public Path GetMainPath(PieceColor color) => mainPaths[color];
        public Path GetGoalPath(PieceColor color) => goalPaths[color];
    }