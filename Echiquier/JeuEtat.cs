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

        private List<CaseEtat> _mangerObligatoires = new List<CaseEtat>();

        private bool _joueurEnCours = false; // Commence par blanc

        public JeuEtat()
        {
            _cases = new CaseEtat[DIMENSION * DIMENSION];

            for (int i = 0; i < _cases.Length; i++)
            {
                _cases[i] = new CaseEtat(
                    i % DIMENSION,
                    i / DIMENSION,
                    (i % 2 == 0) ^ (i / DIMENSION % 2 == 0),
                    PieceInitiale(i)
                );
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
        public void Interaction(CaseEtat caseCliquee)
        {
            // If no source is selected, select the current case if it belongs to the current player
            CaseEtat? source = GetCaseSelectionnee();
            if (source == null)
            {
                if (caseCliquee.Piece != null && caseCliquee.Piece.Couleur == _joueurEnCours)
                {
                    caseCliquee.Selectionne(_joueurEnCours);
                }
                return;
            }

            // Si on a une source et une destination de définie
            CaseEtat? ennemiAManger = MouvementManger(source, caseCliquee);
            if (ennemiAManger != null)
            {
                source.Mouvement(caseCliquee);
                ennemiAManger.MangePiece();
                ChangeJoueur();
            }
            else if (MouvementSimple(source, caseCliquee))
            {
                source.Mouvement(caseCliquee);
                ChangeJoueur();

                int gagne = Gagne(caseCliquee);
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
                caseCliquee.Deselectionne();
            }
        }

        private CaseEtat? GetCaseSelectionnee()
        {
            // Find the selected source case
            for (int i = 0; i < _cases.Length; i++)
            {
                if (_cases[i].Selection)
                {
                    return _cases[i];
                }
            }

            return null;
        }

        private void ChangeJoueur()
        {
            _joueurEnCours = !_joueurEnCours;
        }


        public CaseEtat? MouvementManger(CaseEtat source, CaseEtat target)
        {
            // Verifie si la destination est vide
            bool destinationIsEmpty = target.Piece == null;

            if (!destinationIsEmpty)
            {
                return null;
            }

            // Calcule la position des pieces entre source et target en fonction de la direction
            //Direction 1
            if (target.X == source.X + 2 && target.Y == source.Y - 2)
            {
                int positionCaseEntreX = source.X + 1;
                int positionCaseEntreY = source.Y - 1;
                int indexCaseEntre = positionCaseEntreY * DIMENSION + positionCaseEntreX;
                CaseEtat entreCaseEtat = _cases[indexCaseEntre];
                bool estEnnemi = entreCaseEtat.Piece != null && source.Piece != null && entreCaseEtat.Piece.Couleur != source.Piece.Couleur;
                return estEnnemi ? entreCaseEtat : null;
            }

            //Direction 2
            if (target.X == source.X + 2 && target.Y == source.Y + 2)
            {
                int positionCaseEntreX = source.X + 1;
                int positionCaseEntreY = source.Y + 1;
                int indexCaseEntre = positionCaseEntreY * DIMENSION + positionCaseEntreX;
                CaseEtat entreCaseEtat = _cases[indexCaseEntre];
                bool estEnnemi = entreCaseEtat.Piece != null && source.Piece != null && entreCaseEtat.Piece.Couleur != source.Piece.Couleur;
                return estEnnemi ? entreCaseEtat : null;
            }
            //Direction 3
            if (target.X == source.X - 2 && target.Y == source.Y - 2)
            {
                int positionCaseEntreX = source.X - 1;
                int positionCaseEntreY = source.Y - 1;
                int indexCaseEntre = positionCaseEntreY * DIMENSION + positionCaseEntreX;
                CaseEtat entreCaseEtat = _cases[indexCaseEntre];
                bool estEnnemi = entreCaseEtat.Piece != null && source.Piece != null && entreCaseEtat.Piece.Couleur != source.Piece.Couleur;
                return estEnnemi ? entreCaseEtat : null;
            }
            //Direction 4   
            if (target.X == source.X - 2 && target.Y == source.Y + 2)
            {
                int positionCaseEntreX = source.X - 1;
                int positionCaseEntreY = source.Y + 1;
                int indexCaseEntre = positionCaseEntreY * DIMENSION + positionCaseEntreX;
                CaseEtat entreCaseEtat = _cases[indexCaseEntre];
                bool estEnnemi = entreCaseEtat.Piece != null && source.Piece != null && entreCaseEtat.Piece.Couleur != source.Piece.Couleur;
                return estEnnemi ? entreCaseEtat : null;
            }

            return null; // No valid capture or regular move found
        }



        public bool MouvementSimple(CaseEtat source, CaseEtat target)
        {
            // On ne peut se déplacer que sur une case libre
            bool DestinationEstLibre = target.Piece == null;
            if (!DestinationEstLibre) {
                return false;
            }

            if (source.X == 0)
            {
                return (source.Piece.Couleur == true && target.Y == source.Y + 1 && target.X == source.X + 1)
                     || (source.Piece.Couleur == false && target.Y == source.Y - 1 && target.X == source.X + 1);
            }
            if (source.X == DIMENSION - 1)
            {
                return (source.Piece.Couleur == true && target.Y == source.Y + 1 && target.X == source.X - 1)
                     || (source.Piece.Couleur == false && target.Y == source.Y - 1 && target.X == source.X - 1);
            }
            return (source.Piece.Couleur == true && target.Y == source.Y + 1 && target.X == source.X - 1)
                || (source.Piece.Couleur == true && target.Y == source.Y + 1 && target.X == source.X + 1)
                || (source.Piece.Couleur == false && target.Y == source.Y - 1 && target.X == source.X - 1)
                || (source.Piece.Couleur == false && target.Y == source.Y - 1 && target.X == source.X + 1);
        }

        public List<CaseEtat> CalculeMangerObligatoires() 
        {
            return new List<CaseEtat>();
        }

        public int Gagne(CaseEtat source)
        {
            if (source.Piece.Couleur == false && source.Y == 0 )
             {
                 return 1;

             }
            if (source.Piece.Couleur == true && source.Y == 9)
            {
                return 2;

            }
            else {
                return 0;
            }
        }
    }
}
