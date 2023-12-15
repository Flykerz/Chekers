using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Echiquier
{
    public class CaseEtat : Etat
    {
        private readonly  int _x;
        private readonly  int _y;
        private readonly bool _couleur; // False = Blanc / True = Noir

        private PieceEtat? _piece; // La case peut porter une pièce (peut être à null).
        private       bool _selection;

        public CaseEtat(int x, int y, bool couleur, PieceEtat? piece)
        {
            _x = x;
            _y = y;
            _couleur  = couleur;
            _piece    = piece;
        }

        public bool Couleur
        {
            get { return _couleur; }
        }
        
        public int X
        {
            get { return _x; }
        }

        public int Y
        {
            get { return _y; }
        }

        public PieceEtat? Piece
        {
            get { return _piece; }
            private set 
            { 
                _piece = value;
                OnPropertyChanged(nameof(Piece));
            }
        }

        public bool Selection
        {
            get { return _selection; }
            private set
            {
                _selection = value;
                OnPropertyChanged(nameof(Selection));
            }
        }

        public void Selectionne(bool joueurEnCours)
        {
            if (_piece != null && joueurEnCours == _piece.Couleur)
            {
                Selection = true;
            }
        }

        public void Deselectionne()
        {
            if (_piece != null)
            {
                Selection = false;
            }
        }
        public void MangePiece()
        {
            Piece = null;
        }

        public void Mouvement(CaseEtat cible)
        {
            if (_piece != null) 
            {
                cible.Piece = _piece;
                Piece = null;
                Selection = false;
            }
        }
    }
}
