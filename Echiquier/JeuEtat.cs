using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Echiquier
{
    public class JeuEtat
    {
        public const int DIMENSION = 10;

        private readonly CaseEtat[] _cases;

        private bool joueurEnCours = false; // Commence par blanc

        public JeuEtat()
        {
            _cases = new CaseEtat[DIMENSION * DIMENSION];

            for (int i = 0; i < _cases.Length; i++)
            {
                _cases[i] = new CaseEtat(i, (i % 2 == 0) ^ (i / DIMENSION % 2 == 0), PieceInitiale(i));
            }
        }

        private PieceEtat? PieceInitiale(int pos)
        {
            switch (pos)
            {
                    case 1: return new PieceEtat(new PionType(), true);
                    case 3: return new PieceEtat(new PionType(), true);
                    case 5: return new PieceEtat(new PionType(), true);
                    case 7: return new PieceEtat(new PionType(), true);
                    case 9: return new PieceEtat(new PionType(), true);
                    case 10: return new PieceEtat(new PionType(), true);
                    case 12: return new PieceEtat(new PionType(), true);
                    case 14: return new PieceEtat(new PionType(), true);
                    case 16: return new PieceEtat(new PionType(), true);
                    case 18: return new PieceEtat(new PionType(), true);
                    case 21: return new PieceEtat(new PionType(), true);
                    case 23: return new PieceEtat(new PionType(), true);
                    case 25: return new PieceEtat(new PionType(), true);
                    case 27: return new PieceEtat(new PionType(), true);
                    case 29: return new PieceEtat(new PionType(), true);
                    case 30: return new PieceEtat(new PionType(), true);
                    case 32: return new PieceEtat(new PionType(), true);
                    case 34: return new PieceEtat(new PionType(), true);
                    case 36: return new PieceEtat(new PionType(), true);
                    case 38: return new PieceEtat(new PionType(), true);

                    case 61: return new PieceEtat(new PionType(), false);
                    case 63: return new PieceEtat(new PionType(), false);
                    case 65: return new PieceEtat(new PionType(), false);
                    case 67: return new PieceEtat(new PionType(), false);
                    case 69: return new PieceEtat(new PionType(), false);
                    case 70: return new PieceEtat(new PionType(), false);
                    case 72: return new PieceEtat(new PionType(), false);
                    case 74: return new PieceEtat(new PionType(), false);
                    case 76: return new PieceEtat(new PionType(), false);
                    case 78: return new PieceEtat(new PionType(), false);
                    case 81: return new PieceEtat(new PionType(), false);
                    case 83: return new PieceEtat(new PionType(), false);
                    case 85: return new PieceEtat(new PionType(), false);
                    case 87: return new PieceEtat(new PionType(), false);
                    case 89: return new PieceEtat(new PionType(), false);
                    case 90: return new PieceEtat(new PionType(), false);  
                    case 92: return new PieceEtat(new PionType(), false);
                    case 94: return new PieceEtat(new PionType(), false);
                    case 96: return new PieceEtat(new PionType(), false);
                    case 98: return new PieceEtat(new PionType(), false);

                default: return null;
            }
        }

        public CaseEtat[] Cases
        {
            get { return _cases; }
        }

        // Déclenché lors du clic sur une case.
        public void Interaction(CaseEtat c)
        {
            CaseEtat source = null;
            bool mange = false;

            // Find the selected source case
            for (int i = 0; i < _cases.Length; i++)
            {
                if (_cases[i].Selection)
                {
                    source = _cases[i];
                    break;
                }
            }

            // If no source is selected, select the current case if it belongs to the current player
            if (source == null)
            {
                if (c.Piece != null && c.Piece.Couleur == joueurEnCours)
                {
                    c.Selectionne(joueurEnCours);
                }
            }
            else
            {
                if (MouvementManger(source, c))
                {
                    int positionCaseEntre = (source.Position + c.Position) / 2;
                    _cases[positionCaseEntre].MangePiece();
                    source.Mouvement(c);
                    mange = true;

                    
                    
                        c.Selectionne(joueurEnCours); // Si le pion a Manger, On lui laisse la possibilité de rejouer derrière
                   
                    mange = false;
                }
                else if (MouvementSimple(source, c) && mange == false)
                {
                    source.Mouvement(c);
                    joueurEnCours = !joueurEnCours;

                    int gagne = Gagne(c);
                    if (gagne == 1)
                    {
                        MessageBox.Show("Les Blancs ont gagnés");
                    }
                    else if (gagne == 2)
                    {
                        MessageBox.Show("Les Noirs ont gagnés");
                    }
                }
                else
                {
                    c.Deselectionne();
                }
            }
        }


        public bool MouvementManger(CaseEtat source, CaseEtat target)
        {
            // Verifie si la destination est vide
            bool destinationIsEmpty = target.Piece == null;

            if (!destinationIsEmpty)
            {
                return false; 
            }

            // Calcule la position des pieces entre source et target
            int positionCaseEntre = (source.Position + target.Position) / 2;

            // Verifie si il y a une piece sur la position entre source et target
            bool isPieceBetween = _cases[positionCaseEntre].Piece != null;

            if (isPieceBetween && _cases[positionCaseEntre].Piece.Couleur != source.Couleur)
            {
                // Verifie si la position de target est diagonale de 2 cases
                if (Math.Abs(target.Position - source.Position) == 2 * JeuEtat.DIMENSION + 2 ||
                    Math.Abs(target.Position - source.Position) == 2 * JeuEtat.DIMENSION - 2)
                {
                    // Check if the position behind the enemy piece is empty
                    int positionBehindEnemy = (positionCaseEntre + source.Position) / 2;
                    bool isEmptyBehindEnemy = _cases[positionBehindEnemy].Piece == null;

                    if (isEmptyBehindEnemy)
                    {
                        // Remove the captured piece from the board
                        _cases[positionCaseEntre].MangePiece();
                        return true; // Return true if it's an opponent's piece and the target is two spaces ahead diagonally with an empty space behind the enemy
                    }
                }
            }

            return false; // No valid capture or regular move found
        }





        public bool MouvementSimple(CaseEtat source, CaseEtat target)
        {
            // On ne peut se déplacer que sur une case libre
            bool DestinationEstLibre = target.Piece == null;
            if (!DestinationEstLibre) {
                return false;
            }

            if (source.Position % DIMENSION == 0)
            {
                return (source.Piece.Couleur == true && target.Position == source.Position + DIMENSION + 1)
                    || (source.Piece.Couleur == false && target.Position == source.Position - DIMENSION + 1);

            }
            if (source.Position % DIMENSION == DIMENSION - 1)
            {
                return (source.Piece.Couleur == true && target.Position == source.Position + DIMENSION - 1)
                    || (source.Piece.Couleur == false && target.Position == source.Position - DIMENSION - 1);

            }
            return (source.Piece.Couleur == true && target.Position == source.Position + DIMENSION - 1)
                    || (source.Piece.Couleur == true && target.Position == source.Position + DIMENSION + 1)
                    || (source.Piece.Couleur == false && target.Position == source.Position - DIMENSION - 1)
                    || (source.Piece.Couleur == false && target.Position == source.Position - DIMENSION + 1);
        }

        public int Gagne(CaseEtat source)
        {

            if (source.Piece.Couleur == false && source.Position < DIMENSION )
            {
                return 1;

            }
            if (source.Piece.Couleur == true && source.Position > DIMENSION * (DIMENSION - 1))
            {
                return 2;

            }
            else { return  0; }


        }
    }
}
