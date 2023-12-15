using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Echiquier
{
    // En Java, on aurait écrit Etat implements INotifyPropertyChanged.
    // INotifyPropertyChanged indique à WPF que l'objet notifie les modifications sur ses propriétés.
    public abstract class Etat : INotifyPropertyChanged
    {
        // Déclaration d'un évènement (mot-clé event).
        // PropertyChangedEventHandler correspond au type de méthode qui s'abonne à l'évènement.
        // ? indique que PropertyChanged peut prendre une valeur nulle
        // Implémente l'interface INotifyPropertyChanged.
        public event PropertyChangedEventHandler? PropertyChanged;

        // protected = accessible aux classes filles.
        protected void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
