using ClassPlanner.Extensions;
using ClassPlanner.Models;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using WinUI.TableView;

namespace ClassPlanner.Controls;

public sealed partial class ScheduleView : Control
{
    public ScheduleView()
    {
        DefaultStyleKey = typeof(ScheduleView);
    }
    private ObservableCollection<WeeklyPeriod> Periods { get; } = [];
    private TableView TableViewControl = null!;

    protected override void OnApplyTemplate()
    {
        TableViewControl = this.GetTemplateChild<TableView>(nameof(TableViewControl));
        TableViewControl.ItemsSource = Periods;
    }

    public ClassSchedule ClassSchedule
    {
        get => (ClassSchedule)GetValue(ClassScheduleProperty);
        set => SetValue(ClassScheduleProperty, value);
    }

    public static readonly DependencyProperty ClassScheduleProperty = DependencyProperty.Register("ClassSchedule", typeof(ClassSchedule), typeof(ScheduleView), new PropertyMetadata(null, OnClassScheduleChanged));

    private static void OnClassScheduleChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
    {
        if (sender is ScheduleView sv && e.NewValue is ClassSchedule classSchedule)
        {
            sv.Periods.Clear();
            foreach (WeeklyPeriod item in GeneratePeriods(classSchedule))
            {
                sv.Periods.Add(item);
            }
        }
    }

    private static List<WeeklyPeriod> GeneratePeriods(ClassSchedule classSchedule)
    {
        List<WeeklyPeriod> weeklyPeriods = new(classSchedule.PeriodsPerDay);
        weeklyPeriods.AddRange(Enumerable.Range(0, classSchedule.PeriodsPerDay).
                      Select((_, index) => new WeeklyPeriod()
                      {
                          PeriodIndex = index + 1
                      }));

        foreach (SubjectSchedule subjectSchedule in classSchedule.SubjectSchedules)
        {
            WeeklyPeriod weeklyPeriod = weeklyPeriods[subjectSchedule.Period % classSchedule.PeriodsPerDay];

            switch (subjectSchedule.Day)
            {
                case DayOfWeek.Monday:
                    weeklyPeriod.Monday = subjectSchedule.Subject;
                    break;
                case DayOfWeek.Tuesday:
                    weeklyPeriod.Tuesday = subjectSchedule.Subject;
                    break;
                case DayOfWeek.Wednesday:
                    weeklyPeriod.Wednesday = subjectSchedule.Subject;
                    break;
                case DayOfWeek.Thursday:
                    weeklyPeriod.Thursday = subjectSchedule.Subject;
                    break;
                case DayOfWeek.Friday:
                    weeklyPeriod.Friday = subjectSchedule.Subject;
                    break;
            }
        }

        return weeklyPeriods;
    }
}