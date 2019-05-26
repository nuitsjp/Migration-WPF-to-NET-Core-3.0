using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Linq;
using System.Runtime.CompilerServices;
using AdventureWorks.EmployeeManager.Presentation.Annotations;
using AdventureWorks.EmployeeManager.Services;
using PropertyChanged;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;

namespace AdventureWorks.EmployeeManager.Presentation.ViewModels
{
    [AddINotifyPropertyChangedInterface]
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        private readonly IHumanResourcesService _humanResourcesService;

        public IEnumerable<Gender> Genders { get; private set; }

        public IEnumerable<MaritalStatus> MaritalStatuses { get; private set; }

        public ObservableCollection<ManagedEmployeeViewModel> ManagedEmployees { get; } = new ObservableCollection<ManagedEmployeeViewModel>();

        public bool IsReadOnly { get; set; } = true;

        public ReactiveCommand InitializeCommand { get; } = new ReactiveCommand();

        public ReactiveCommand EditCommand { get; }

        public ReactiveCommand SaveCommand { get; }

        public MainWindowViewModel(IHumanResourcesService humanResourcesService)
        {
            _humanResourcesService = humanResourcesService;

            InitializeCommand.Subscribe(OnInitialize);

            EditCommand = this.ObserveProperty(x => x.IsReadOnly).ToReactiveCommand();
            EditCommand.Subscribe(() => IsReadOnly = false);

            SaveCommand = this.ObserveProperty(x => x.IsReadOnly).Select(x => !x).ToReactiveCommand();
            SaveCommand.Subscribe(OnSave);
        }

        private void OnInitialize()
        {
            Genders = _humanResourcesService.GetGenders().OrderBy(x => x.Code);
            MaritalStatuses = _humanResourcesService.GetMaritalStatuses().OrderBy(x => x.Code);

            LoadManagedEmployees();
        }

        private void LoadManagedEmployees()
        {
            var managedEmployees = _humanResourcesService.GetManagedEmployees();
            foreach (var managedEmployee in managedEmployees)
            {
                ManagedEmployees.Add(new ManagedEmployeeViewModel(managedEmployee));
            }
        }

        private void OnSave()
        {
            var updatedEmployees = 
                ManagedEmployees
                    .Where(x => x.EditStatus == EditStatus.Modified)
                    .Select(x => x.Commit())
                    .ToList();
            var newEmployees =
                ManagedEmployees
                    .Where(x => x.EditStatus == EditStatus.New)
                    .Select(x => x.Commit())
                    .ToList();
            _humanResourcesService.ModifyManagedEmployees(updatedEmployees, newEmployees);

            ManagedEmployees.Clear();
            LoadManagedEmployees();
            IsReadOnly = true;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        // ReSharper disable once UnusedMember.Global
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
