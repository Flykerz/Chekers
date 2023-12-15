using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Echiquier
{
    public interface IPieceType
    {
        public string Nom { get; }

        public bool MouvementValide(int source, int target);
    }

    public class PionType : IPieceType
    {
        public string Nom
        {
            get { return "Pion"; }
        }

        public bool MouvementValide(int source, int target)
        {
            int sx = source % JeuEtat.DIMENSION;
            int sy = source / JeuEtat.DIMENSION;
            int tx = target % JeuEtat.DIMENSION;
            int ty = target / JeuEtat.DIMENSION;

            int dx = Math.Abs(sx - tx);
            int dy = Math.Abs(sy - ty);

            //if (CaseEtat(target) == null)
            //{

                return dx == 2 && dy ==2;
            //}


        }
    }
}