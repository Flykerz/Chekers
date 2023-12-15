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

        // Dictionnaire indiquant quelles cases ont des manger obligatoires (si un pion peut manger, il doit le faire)
        // NB: Seules les cases qui ont des manger obligatoires sont présentes (afin d'économiser de la mémoire
        private Dictionary<CaseEtat, List<CaseEtat>> _mangerObligatoires = new Dictionary<CaseEtat, List<CaseEtat>>();

        private bool _joueurEnCours = false; // Commence par blanc

        // Indique si un pion est en train de manger plusieurs fois d'affilée
        private CaseEtat? _caseEnTrainDeManger = null;

        public JeuEtat()
        {
            _cases = new CaseEtat[DIMENSION * DIMENSION];

            for (int i = 0; i < _cases.Length; i++)
            {
                _cases[i] = new CaseEtat(
                    i % DIMENSION,                              // X
                    i / DIMENSION,                              // Y
                    (i % 2 == 0) ^ (i / DIMENSION % 2 == 0),    // Couleur
                    PieceInitiale(i)                            // Piece
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
            CaseEtat? source = GetCaseSelectionnee();

            // Si on n'a pas de source...
            if (source == null)
            {
                // ... on s'arrête si la case ciblée est vide, ou appartient à l'autre joueur ...
                if (caseCliquee.Piece == null || caseCliquee.Piece.Couleur != _joueurEnCours)
                {
                    return;
                }
                // ... ou bien si on a des manger obligatoires autre que sur la case cliquée
                if (_mangerObligatoires.Count > 0 && !_mangerObligatoires.ContainsKey(caseCliquee))
                {
                    return;
                }

                // Sinon, on peut sélectionner la case, et on s'arrête
                caseCliquee.Selectionne(_joueurEnCours);
                MetAJourDestinationsValides(caseCliquee);
                return;
            }

            // Si on a une source et qu'on re-clique sur la case sélectionnée, on la déselectionne
            // Seule exception : si on est en train de manger plusieurs fois d'affilée, la déselection est désactivée
            if (source == caseCliquee && _caseEnTrainDeManger == null)
            {
                source.Deselectionne();
                MetAJourDestinationsValides(null);
                return;
            }

            // Si on a une source et des mangers obligatoires, on est obligé de choisir parmi eux
            if (_mangerObligatoires.Count > 0 && _mangerObligatoires.ContainsKey(source) && !_mangerObligatoires[source].Contains(caseCliquee))
            {
                return;
            }

            // Si on a une source et une destination de définie, soit on mange, soit on fait un déplacement simple
            CaseEtat? ennemiAManger = MouvementManger(source, caseCliquee);
            if (ennemiAManger != null)
            {
                source.Mouvement(caseCliquee);
                ennemiAManger.MangePiece();

                // Une fois qu'on a mangé, soit on a encore des possibilités de jeu et on continue, soit on change de joueur
                CalculeMangerObligatoires();
                if (_mangerObligatoires.Count > 0 && _mangerObligatoires.ContainsKey(caseCliquee))
                {
                    _caseEnTrainDeManger = caseCliquee;
                    caseCliquee.Selectionne(_joueurEnCours);
                    MetAJourDestinationsValides(caseCliquee);
                } 
                else
                {
                    _caseEnTrainDeManger = null;
                    ChangeJoueur();
                    CalculeMangerObligatoires();
                    MetAJourDestinationsValides(null);
                }
            }
            else if (MouvementSimple(source, caseCliquee))
            {
                source.Mouvement(caseCliquee);
                ChangeJoueur();
                CalculeMangerObligatoires();
                MetAJourDestinationsValides(null);
            }

            // Gestion de la potentielle victoire d'un camp
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

        private CaseEtat? GetCaseSelectionnee()
        {
            // Essaie de trouver la seule case sélectionnée (normalement, elle est unique)
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
            if (target.Piece != null)
            {
                return null;
            }

            // Vérifie si on peut manger l'ennemi dans la direction en haut à droite
            if (target.X == source.X + 2 && target.Y == source.Y - 2)
            {
                int positionCaseEntreX = source.X + 1;
                int positionCaseEntreY = source.Y - 1;
                int indexCaseEntre = positionCaseEntreY * DIMENSION + positionCaseEntreX;
                CaseEtat entreCaseEtat = _cases[indexCaseEntre];
                bool estEnnemi = entreCaseEtat.Piece != null && source.Piece != null && entreCaseEtat.Piece.Couleur != source.Piece.Couleur;
                return estEnnemi ? entreCaseEtat : null;
            }

            // Vérifie si on peut manger l'ennemi dans la direction en bas à droite
            if (target.X == source.X + 2 && target.Y == source.Y + 2)
            {
                int positionCaseEntreX = source.X + 1;
                int positionCaseEntreY = source.Y + 1;
                int indexCaseEntre = positionCaseEntreY * DIMENSION + positionCaseEntreX;
                CaseEtat entreCaseEtat = _cases[indexCaseEntre];
                bool estEnnemi = entreCaseEtat.Piece != null && source.Piece != null && entreCaseEtat.Piece.Couleur != source.Piece.Couleur;
                return estEnnemi ? entreCaseEtat : null;
            }

            // Vérifie si on peut manger l'ennemi dans la direction en haut à gauche
            if (target.X == source.X - 2 && target.Y == source.Y - 2)
            {
                int positionCaseEntreX = source.X - 1;
                int positionCaseEntreY = source.Y - 1;
                int indexCaseEntre = positionCaseEntreY * DIMENSION + positionCaseEntreX;
                CaseEtat entreCaseEtat = _cases[indexCaseEntre];
                bool estEnnemi = entreCaseEtat.Piece != null && source.Piece != null && entreCaseEtat.Piece.Couleur != source.Piece.Couleur;
                return estEnnemi ? entreCaseEtat : null;
            }

            // Vérifie si on peut manger l'ennemi dans la direction en bas à gauche
            if (target.X == source.X - 2 && target.Y == source.Y + 2)
            {
                int positionCaseEntreX = source.X - 1;
                int positionCaseEntreY = source.Y + 1;
                int indexCaseEntre = positionCaseEntreY * DIMENSION + positionCaseEntreX;
                CaseEtat entreCaseEtat = _cases[indexCaseEntre];
                bool estEnnemi = entreCaseEtat.Piece != null && source.Piece != null && entreCaseEtat.Piece.Couleur != source.Piece.Couleur;
                return estEnnemi ? entreCaseEtat : null;
            }

            return null;
        }



        public bool MouvementSimple(CaseEtat source, CaseEtat target)
        {
            // On ne peut se déplacer que sur une case libre, et que si la source est bien une pièce
            if (target.Piece != null || source.Piece == null) {
                return false;
            }

            // Pour la première colonne, seuls les mouvements vers la droite sont possibles
            if (source.X == 0)
            {
                return (source.Piece.Couleur == true && target.Y == source.Y + 1 && target.X == source.X + 1)
                     || (source.Piece.Couleur == false && target.Y == source.Y - 1 && target.X == source.X + 1);
            }
            // Et pour la dernière colonne, seuls ceux vers la gauche
            if (source.X == DIMENSION - 1)
            {
                return (source.Piece.Couleur == true && target.Y == source.Y + 1 && target.X == source.X - 1)
                     || (source.Piece.Couleur == false && target.Y == source.Y - 1 && target.X == source.X - 1);
            }

            // Sinon, on vérifie toutes les directions
            return (source.Piece.Couleur == true && target.Y == source.Y + 1 && target.X == source.X - 1)
                || (source.Piece.Couleur == true && target.Y == source.Y + 1 && target.X == source.X + 1)
                || (source.Piece.Couleur == false && target.Y == source.Y - 1 && target.X == source.X - 1)
                || (source.Piece.Couleur == false && target.Y == source.Y - 1 && target.X == source.X + 1);
        }

        private void MetAJourDestinationsValides(CaseEtat? source)
        {
            for (int i = 0; i < _cases.Length; i++)
            {
                CaseEtat caseEtat = _cases[i];
                // Si on a une source et que la case est valide par rapport à la source, on la colore
                if (source != null && _mangerObligatoires.ContainsKey(source) && _mangerObligatoires[source].Contains(caseEtat))
                {
                    caseEtat.EstDestinationValide = true;
                }
                // Si on n'a pas de source et qu'on a des mangers obligatoires, on les colore
                else if (source == null && _mangerObligatoires.Count > 0 && _mangerObligatoires.ContainsKey(caseEtat))
                {
                    caseEtat.EstDestinationValide = true;
                }
                // Sinon, on en profite pour décolorer les cases qui étaient potentiellement colorées auparavant
                else
                {
                    caseEtat.EstDestinationValide = false;
                }
            }
        }

        private void CalculeMangerObligatoires() 
        {
            // Remise à zéro du dictionnaire des possibiltés
            _mangerObligatoires = new Dictionary<CaseEtat, List<CaseEtat>>();

            // On parcourt toutes les cases et on ajoute les possibilités au dictionnaire, seulement si il y en a
            for (int i = 0; i < _cases.Length; i++)
            {
                CaseEtat caseEtat = _cases[i];
                List<CaseEtat> mangerObligatoires = CalculeMangerObligatoirePourCase(caseEtat);
                if (mangerObligatoires.Count > 0)
                {
                    _mangerObligatoires.Add(caseEtat, mangerObligatoires);
                }
            }
        }

        private List<CaseEtat> CalculeMangerObligatoirePourCase(CaseEtat caseEtat)
        {
            // Les cases vides et ennemies n'ont pas de manger possible, on évite donc des calculs inutiles
            bool caseEstVide = caseEtat.Piece == null;
            bool caseEstEnnemie = caseEtat.Piece != null && caseEtat.Piece.Couleur != _joueurEnCours;
            if (caseEstVide || caseEstEnnemie)
            {
                return new List<CaseEtat>();
            }

            List<CaseEtat> result = new List<CaseEtat>();

            // Test de manger en haut à droite
            CaseEtat? targetSiManger = _cases.ToList().Find(c => c.X == caseEtat.X + 2 && c.Y == caseEtat.Y - 2);
            CaseEtat? ennemiSiManger = _cases.ToList().Find(c => c.X == caseEtat.X + 1 && c.Y == caseEtat.Y - 1);
            if (targetSiManger != null && targetSiManger.Piece == null && ennemiSiManger != null && ennemiSiManger.Piece != null && ennemiSiManger.Piece.Couleur != _joueurEnCours)
            {
                result.Add(targetSiManger);
            }

            // Test de manger en bas à droite
            targetSiManger = _cases.ToList().Find(c => c.X == caseEtat.X + 2 && c.Y == caseEtat.Y + 2);
            ennemiSiManger = _cases.ToList().Find(c => c.X == caseEtat.X + 1 && c.Y == caseEtat.Y + 1);
            if (targetSiManger != null && targetSiManger.Piece == null && ennemiSiManger != null && ennemiSiManger.Piece != null && ennemiSiManger.Piece.Couleur != _joueurEnCours)
            {
                result.Add(targetSiManger);
            }

            // Test de manger en bas à gauche
            targetSiManger = _cases.ToList().Find(c => c.X == caseEtat.X - 2 && c.Y == caseEtat.Y + 2);
            ennemiSiManger = _cases.ToList().Find(c => c.X == caseEtat.X - 1 && c.Y == caseEtat.Y + 1);
            if (targetSiManger != null && targetSiManger.Piece == null && ennemiSiManger != null && ennemiSiManger.Piece != null && ennemiSiManger.Piece.Couleur != _joueurEnCours)
            {
                result.Add(targetSiManger);
            }

            // Test de manger en haut à gauche
            targetSiManger = _cases.ToList().Find(c => c.X == caseEtat.X - 2 && c.Y == caseEtat.Y - 2);
            ennemiSiManger = _cases.ToList().Find(c => c.X == caseEtat.X - 1 && c.Y == caseEtat.Y - 1);
            if (targetSiManger != null && targetSiManger.Piece == null && ennemiSiManger != null && ennemiSiManger.Piece != null && ennemiSiManger.Piece.Couleur != _joueurEnCours)
            {
                result.Add(targetSiManger);
            }

            return result;
        }

        private int Gagne(CaseEtat source)
        {
            // Un pion blanc gagne la partie s'il atteint le haut du plateau
            if (source.Piece != null && source.Piece.Couleur == false && source.Y == 0 )
             {
                 return 1;
             }
            // Un pion noir gagne la partie s'il atteint le bas du plateau
            if (source.Piece != null && source.Piece.Couleur == true && source.Y == 9)
            {
                return 2;
            }
            // Sinon, la partie continue
            else {
                return 0;
            }
        }
    }
}
