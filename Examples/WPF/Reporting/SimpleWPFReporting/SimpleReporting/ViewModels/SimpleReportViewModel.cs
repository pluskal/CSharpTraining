using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using SimpleReporting.Annotations;
using SimpleReporting.Commands;
using SimpleReporting.Interfaces;
using SimpleReporting.Models;

namespace SimpleReporting.ViewModels
{
    public class SimpleReportViewModel : INotifyPropertyChanged, IPrintableDataContext
    {
        private bool _isPrinting;
        private Person _personDetail;

        public bool IsPrinting
        {
            get => _isPrinting;
            set
            {
                if (value == _isPrinting) return;
                _isPrinting = value;
                OnPropertyChanged();
            }
        }

        public SimpleReportViewModel()
        {
            this.PersonDetail = new Person()
            {
                Name = "Jan",
                Surname = "Novak",
                Email = "jan.novak@gmail.com",
                Phone = "+420 721 123 456"
            };
            this.PrintPdfCommand = new PrintPdfCommand(this);

            this.Persons = new List<Person>()
            {
                this.PersonDetail,
                new Person()
                {
                    Name = "Frantisek",
                    Surname = "Voprsalek",
                    Email = "frantisek.voprsalek@gmail.com",
                    Phone = "+420 605 654 321"
                }
            };
        }

        public IEnumerable<Person> Persons { get; set; }

        public Person PersonDetail
        {
            get => _personDetail;
            set
            {
                if (Equals(value, _personDetail)) return;
                _personDetail = value;
                OnPropertyChanged();
            }
        }

        public ICommand PrintPdfCommand { get; }
        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}