namespace AdventureWorks.EmployeeManager.Presentation.ViewModels
{
    public static class ViewModelLocator
    {
        public static MainWindowViewModel MainWindowViewModel => App.Container.GetInstance<MainWindowViewModel>();
    }
}