// namespace Echiquier
// {
//     // L'origine du plateau est en haut à gauche
//     public class Plateau
//     {
//         private const int DIMENSION = 10;
//         private readonly CaseEtat[,] _cases;

//         Plateau(CaseEtat[] cases)
//         {
//             for (int x = 0; x < DIMENSION; x++)
//             {
//                 for (int y = 0; y < DIMENSION; y++)
//                 {
//                     this._cases[x][y] = cases[Plateau.CoordinatesToIndex(x, y)];
//                 }
//             }
//         }

//         // Direction = 1 => en haut à droite 
//         // Direction = 2 => en bas à droite 
//         // Direction = 3 => en bas à gauche 
//         // Direction = 4 => en haut à gauche 
//         public CaseEtat? GetCaseEtatAtNCasesDiagonally(int N, int direction)
//         {
//             if (N > 2 || N < 0) {
//                 throw "Invalid index";
//             } else if (direction < 1 || direction > 4) {
//                 throw "Invalid direction";
//             }
//         }

//         public static int CoordinatesToIndex(int x, int y)
//         {
//             return DIMENSION * y + x;
//         }

//         public static (int, int) IndexToCoordinates(int index)
//         {
//             return (index % DIMENSION, Math.Floor(index / DIMENSION));
//         }
//     }
// }