using ClassPlanner.Timetabling.Validation;
using ClassPlanner.ViewModels;
using Google.OrTools.Sat;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;

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
            CpSolverStatus.Feasible => "Uma solução para o modelo foi encontrada mas pode não ser a melhor possível",
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


public partial class ValidationItemTemplateSelector : DataTemplateSelector
{
    public DataTemplate? ValidationTemplate { get; set; }
    public DataTemplate? ErrorTemplate { get; set; }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0046:Converter em expressão condicional", Justification = "<Pendente>")]
    protected override DataTemplate? SelectTemplateCore(object item) => item switch
    {
        TimetableValidationResult => ValidationTemplate,
        string => ErrorTemplate,
        _ => throw new NotSupportedException("Item not supported"),
    };
}
