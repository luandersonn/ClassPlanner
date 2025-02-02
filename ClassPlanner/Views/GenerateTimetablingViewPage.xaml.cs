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
            CpSolverStatus.Infeasible => "O modelo não foi resolvido",
            CpSolverStatus.ModelInvalid => "O modelo é inválido",
            CpSolverStatus.Unknown => "Erro desconhecido",
            _ => "Unknown Timetabling Status"
        };
    }

    public string TimetableResultMessage(CpSolverStatus status)
    {
        return status switch
        {
            CpSolverStatus.Optimal => "A melhor solução para o modelo foi encontrada",
            CpSolverStatus.Feasible => "Uma solução para o modelo foi encontrada mas não é a melhor possível",
            CpSolverStatus.Infeasible => "O modelo não tem soluções possíveis",
            CpSolverStatus.ModelInvalid => "O modelo foi construido de forma inválida",
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
