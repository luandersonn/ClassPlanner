using ClassPlanner.ViewModels;
using Google.OrTools.Sat;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml.Controls;

namespace ClassPlanner.Views;

public sealed partial class GenerateTimetablingViewPage : Page
{
    public GenerateTimetablingViewPage()
    {
        InitializeComponent();

        ViewModel = App.Current.ServiceProvider.GetRequiredService<GenerateTimetablingViewModel>();
    }

    public GenerateTimetablingViewModel ViewModel { get; }


    public string TimetableResultTitle(CpSolverStatus status)
    {
        return status switch
        {
            CpSolverStatus.Optimal => "O modelo foi resolvido",
            CpSolverStatus.Feasible => "O modelo foi resolvido parcialmente",
            CpSolverStatus.Infeasible => "O modelo n�o foi resolvido",
            CpSolverStatus.ModelInvalid => "O modelo � inv�lido",
            CpSolverStatus.Unknown => "Erro desconhecido",
            _ => "Unknown Timetabling Status"
        };
    }

    public string TimetableResultMessage(CpSolverStatus status)
    {
        return status switch
        {
            CpSolverStatus.Optimal => "A melhor solu��o para o modelo foi encontrada",
            CpSolverStatus.Feasible => "Uma solu��o para o modelo foi encontrada mas n�o � a melhor poss�vel",
            CpSolverStatus.Infeasible => "O modelo n�o tem solu��es poss�veis",
            CpSolverStatus.ModelInvalid => "O modelo foi construido de forma inv�lida",
            CpSolverStatus.Unknown => "Ocorreu um erro desconhecido",
            _ => "Unknown Timetabling Status"
        };
    }

    public InfoBarSeverity TimetableResultSeverity(CpSolverStatus status)
    {
        return status switch
        {
            CpSolverStatus.Optimal => InfoBarSeverity.Success,
            CpSolverStatus.Feasible => InfoBarSeverity.Warning,
            CpSolverStatus.Infeasible => InfoBarSeverity.Error,
            CpSolverStatus.ModelInvalid => InfoBarSeverity.Error,
            CpSolverStatus.Unknown => InfoBarSeverity.Error,
            _ => InfoBarSeverity.Error
        };
    }
}
