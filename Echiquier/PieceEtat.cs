using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Echiquier
{
    public class PieceEtat
    {
        private readonly IPieceType _type;
        private readonly       bool _couleur; // False = Blanc / True = Noir

        public PieceEtat(IPieceType type, bool couleur)
        {
            _type    = type;
            _couleur = couleur;
        }

        public IPieceType Type
        {
            get { return _type; }
        }

        public bool Couleur
        {
            get { return _couleur; }
        }
    }
}
