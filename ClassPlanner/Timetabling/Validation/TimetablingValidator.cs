using ClassPlanner.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ClassPlanner.Timetabling.Validation;

public class TimetablingValidator
{
    public static TimetableValidationResult Validate(Timetable timetable)
    {
        TimetableValidationResult validationResult = new();

        ValidateTeacherConflicts(timetable, validationResult);

        ValidateClassroomConflicts(timetable, validationResult);

        ValidateSubjectPeriodRequirements(timetable, validationResult);

        return validationResult;
    }

    private static void ValidateTeacherConflicts(Timetable timetable, TimetableValidationResult validationResult)
    {
        Dictionary<long, List<(DayOfWeek day, long period)>> teacherSchedules = [];

        foreach (ClassSchedule classSchedule in timetable.ClassSchedules)
        {
            foreach (SubjectSchedule subjectSchedule in classSchedule.SubjectSchedules)
            {
                if (subjectSchedule.Teacher is null)
                    continue;

                long teacherId = subjectSchedule.Teacher.TeacherId;
                (DayOfWeek Day, long PeriodId) key = (subjectSchedule.Day, subjectSchedule.Period);

                if (!teacherSchedules.TryGetValue(teacherId, out List<(DayOfWeek day, long period)>? value))
                {
                    value = [];
                    teacherSchedules[teacherId] = value;
                }

                if (value.Contains(key))
                {
                    validationResult.AddError($"Conflito de horário detectado: O professor '{subjectSchedule.Teacher.Name}' está alocado em mais de uma turma no dia {key.Day} e período {key.PeriodId}.");
                }
                else
                {
                    value.Add(key);
                }
            }
        }
    }

    private static void ValidateClassroomConflicts(Timetable timetable, TimetableValidationResult validationResult)
    {
        foreach (ClassSchedule classSchedule in timetable.ClassSchedules)
        {
            Dictionary<(DayOfWeek day, long period), List<string>> scheduleByTime = [];

            foreach (SubjectSchedule subjectSchedule in classSchedule.SubjectSchedules)
            {
                (DayOfWeek Day, long PeriodId) key = (subjectSchedule.Day, subjectSchedule.Period);

                if (!scheduleByTime.TryGetValue(key, out List<string>? value))
                {
                    value = [];
                    scheduleByTime[key] = value;
                }

                value.Add(subjectSchedule.Subject.Name);

                if (value.Count > 1)
                {
                    validationResult.AddError($"Conflito de horário detectado: A turma '{classSchedule.Classroom.Name}' tem múltiplas disciplinas alocadas no dia {key.Day}  e período  {key.PeriodId}.");
                }
            }
        }
    }

    private static void ValidateSubjectPeriodRequirements(Timetable timetable, TimetableValidationResult validationResult)
    {
        foreach (ClassSchedule classSchedule in timetable.ClassSchedules)
        {
            foreach (Data.Subject subject in classSchedule.Classroom.Subjects)
            {
                int scheduledPeriods = classSchedule.SubjectSchedules.Where(s => s.Subject.SubjectId == subject.SubjectId)
                                                                     .Count();

                if (scheduledPeriods != subject.PeriodsPerWeek)
                {
                    validationResult.AddError($"Requisito de períodos não atendido: A disciplina '{subject.Name}' da turma '{classSchedule.Classroom.Name}' requer {subject.PeriodsPerWeek} períodos por semana, mas foram alocados {scheduledPeriods}.");
                }
            }
        }
    }
}
